// dear imgui: Renderer Backend for OpenGL2 (legacy OpenGL, fixed pipeline)
// This needs to be used along with a Platform Backend (e.g. GLFW, SDL, Win32, custom..)

// Implemented features:
//  [X] Renderer: User texture binding. Use 'GLuint' OpenGL texture identifier as void*/ImTextureID. Read the FAQ about ImTextureID!
//  [X] Renderer: Multi-viewport support (multiple windows). Enable with 'io.ConfigFlags |= ImGuiConfigFlags_ViewportsEnable'.

// You can use unmodified imgui_impl_* files in your project. See examples/ folder for examples of using this.
// Prefer including the entire imgui/ repository into your project (either as a copy or as a submodule), and only build the backends you need.
// If you are new to Dear ImGui, read documentation from the docs/ folder + read the top of imgui.cpp.
// Read online: https://github.com/ocornut/imgui/tree/master/docs

// **DO NOT USE THIS CODE IF YOUR CODE/ENGINE IS USING MODERN OPENGL (SHADERS, VBO, VAO, etc.)**
// **Prefer using the code in imgui_impl_opengl3.cpp**
// This code is mostly provided as a reference to learn how ImGui integration works, because it is shorter to read.
// If your code is using GL3+ context or any semi modern OpenGL calls, using this is likely to make everything more
// complicated, will require your code to reset every single OpenGL attributes to their initial state, and might
// confuse your GPU driver.
// The GL2 code is unable to reset attributes or even call e.g. "glUseProgram(0)" because they don't exist in that API.

// CHANGELOG
// (minor and older changes stripped away, please see git history for details)
//  2023-XX-XX: Platform: Added support for multiple windows via the ImGuiPlatformIO interface.
//  2022-10-11: Using 'nullptr' instead of 'NULL' as per our switch to C++11.
//  2021-12-08: OpenGL: Fixed mishandling of the the ImDrawCmd::IdxOffset field! This is an old bug but it never had an effect until some internal rendering changes in 1.86.
//  2021-06-29: Reorganized backend to pull data from a single structure to facilitate usage with multiple-contexts (all g_XXXX access changed to bd->XXXX).
//  2021-05-19: OpenGL: Replaced direct access to ImDrawCmd::TextureId with a call to ImDrawCmd::GetTexID(). (will become a requirement)
//  2021-01-03: OpenGL: Backup, setup and restore GL_SHADE_MODEL state, disable GL_STENCIL_TEST and disable GL_NORMAL_ARRAY client state to increase compatibility with legacy OpenGL applications.
//  2020-01-23: OpenGL: Backup, setup and restore GL_TEXTURE_ENV to increase compatibility with legacy OpenGL applications.
//  2019-04-30: OpenGL: Added support for special ImDrawCallback_ResetRenderState callback to reset render state.
//  2019-02-11: OpenGL: Projecting clipping rectangles correctly using draw_data->FramebufferScale to allow multi-viewports for retina display.
//  2018-11-30: Misc: Setting up io.BackendRendererName so it can be displayed in the About Window.
//  2018-08-03: OpenGL: Disabling/restoring GL_LIGHTING and GL_COLOR_MATERIAL to increase compatibility with legacy OpenGL applications.
//  2018-06-08: Misc: Extracted imgui_impl_opengl2.cpp/.h away from the old combined GLFW/SDL+OpenGL2 examples.
//  2018-06-08: OpenGL: Use draw_data->DisplayPos and draw_data->DisplaySize to setup projection matrix and clipping rectangle.
//  2018-02-16: Misc: Obsoleted the io.RenderDrawListsFn callback and exposed ImGui_ImplOpenGL2_RenderDrawData() in the .h file so you can call it yourself.
//  2017-09-01: OpenGL: Save and restore current polygon mode.
//  2016-09-10: OpenGL: Uploading font texture as RGBA32 to increase compatibility with users shaders (not ideal).
//  2016-09-05: OpenGL: Fixed save and restore of current scissor rectangle.

#include "imgui.h"
#ifndef IMGUI_DISABLE
#include "imgui_impl_opengl2.h"
#include "GLFW/glfw3.h"
#include <stdint.h>     // intptr_t

// Clang/GCC warnings with -Weverything
#if defined(__clang__)
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Wunused-macros"                      // warning: macro is not used
#pragma clang diagnostic ignored "-Wnonportable-system-include-path"
#endif

// Include OpenGL header (without an OpenGL loader) requires a bit of fiddling
#if defined(_WIN32) && !defined(APIENTRY)
#define APIENTRY __stdcall                  // It is customary to use APIENTRY for OpenGL function pointer declarations on all platforms.  Additionally, the Windows OpenGL header needs APIENTRY.
#endif
#if defined(_WIN32) && !defined(WINGDIAPI)
#define WINGDIAPI __declspec(dllimport)     // Some Windows OpenGL headers need this
#endif
#if defined(__APPLE__)
#define GL_SILENCE_DEPRECATION
#include <OpenGL/gl.h>
#else
#include <GL/gl.h>
#endif

static void (*import_glEnable)(GLenum);
static void (*import_glBlendFunc)(GLenum, GLenum);
static void (*import_glDisable)(GLenum);
static void (*import_glEnableClientState)(GLenum);
static void (*import_glDisableClientState)(GLenum);
static void (*import_glPolygonMode)(GLenum, GLenum);
static void (*import_glShadeModel)(GLenum);
static void (*import_glTexEnvi)(GLenum, GLenum, GLint);
static void (*import_glViewport)(GLint, GLint, GLsizei, GLsizei);
static void (*import_glMatrixMode)(GLenum);
static void (*import_glPushMatrix)(void);
static void (*import_glPopMatrix)(void);
static void (*import_glLoadIdentity)(void);
static void (*import_glOrtho)(GLdouble, GLdouble, GLdouble, GLdouble, GLdouble, GLdouble);
static void (*import_glGetIntegerv)(GLenum, GLint*);
static void (*import_glGetTexEnviv)(GLenum, GLenum, GLint*);
static void (*import_glPushAttrib)(GLbitfield);
static void (*import_glPopAttrib)(void);
static void (*import_glVertexPointer)(GLint, GLenum, GLsizei, const GLvoid*);
static void (*import_glTexCoordPointer)(GLint, GLenum, GLsizei, const GLvoid*);
static void (*import_glColorPointer)(GLint, GLenum, GLsizei, const GLvoid*);
static void (*import_glScissor)(GLint, GLint, GLsizei, GLsizei);
static void (*import_glBindTexture)(GLenum, GLuint);
static void (*import_glDrawElements)(GLenum, GLsizei, GLenum, const GLvoid*);
static void (*import_glGenTextures)(GLsizei, GLuint*);
static void (*import_glTexParameteri)(GLenum, GLenum, GLint);
static void (*import_glPixelStorei)(GLenum, GLint);
static void (*import_glTexImage2D)(GLenum, GLint, GLint, GLsizei, GLsizei, GLint, GLenum, GLenum, const GLvoid*);
static void (*import_glDeleteTextures)(GLsizei, const GLuint*);
static void (*import_glClearColor)(GLclampf, GLclampf, GLclampf, GLclampf);
static void (*import_glClear)(GLbitfield);

#define IMPORT_GL_FUNC(name, ...) import_##name = (void (*)(__VA_ARGS__))(glfwGetProcAddress(#name));

struct ImGui_ImplOpenGL2_Data
{
    GLuint       FontTexture;

    ImGui_ImplOpenGL2_Data() { memset((void*)this, 0, sizeof(*this)); }
};

// Backend data stored in io.BackendRendererUserData to allow support for multiple Dear ImGui contexts
// It is STRONGLY preferred that you use docking branch with multi-viewports (== single Dear ImGui context + multiple windows) instead of multiple Dear ImGui contexts.
static ImGui_ImplOpenGL2_Data* ImGui_ImplOpenGL2_GetBackendData()
{
    return igGetCurrentContext() ? (ImGui_ImplOpenGL2_Data*)igGetIO().BackendRendererUserData : nullptr;
}

// Forward Declarations
static void ImGui_ImplOpenGL2_InitPlatformInterface();
static void ImGui_ImplOpenGL2_ShutdownPlatformInterface();

// Functions
bool    ImGui_ImplOpenGL2_Init()
{
    ImGuiIO& io = igGetIO();
    IM_ASSERT(io.BackendRendererUserData == nullptr && "Already initialized a renderer backend!");

    IMPORT_GL_FUNC(glEnable, GLenum);
    IMPORT_GL_FUNC(glBlendFunc, GLenum, GLenum);
    IMPORT_GL_FUNC(glDisable, GLenum);
    IMPORT_GL_FUNC(glEnableClientState, GLenum);
    IMPORT_GL_FUNC(glDisableClientState, GLenum);
    IMPORT_GL_FUNC(glPolygonMode, GLenum, GLenum);
    IMPORT_GL_FUNC(glShadeModel, GLenum);
    IMPORT_GL_FUNC(glTexEnvi, GLenum, GLenum, GLint);
    IMPORT_GL_FUNC(glViewport, GLint, GLint, GLsizei, GLsizei);
    IMPORT_GL_FUNC(glMatrixMode, GLenum);
    IMPORT_GL_FUNC(glPushMatrix, void);
    IMPORT_GL_FUNC(glPopMatrix, void);
    IMPORT_GL_FUNC(glLoadIdentity, void);
    IMPORT_GL_FUNC(glOrtho, GLdouble, GLdouble, GLdouble, GLdouble, GLdouble, GLdouble);
    IMPORT_GL_FUNC(glGetIntegerv, GLenum, GLint*);
    IMPORT_GL_FUNC(glGetTexEnviv, GLenum, GLenum, GLint*);
    IMPORT_GL_FUNC(glPushAttrib, GLbitfield);
    IMPORT_GL_FUNC(glPopAttrib, void);
    IMPORT_GL_FUNC(glVertexPointer, GLint, GLenum, GLsizei, const GLvoid*);
    IMPORT_GL_FUNC(glTexCoordPointer, GLint, GLenum, GLsizei, const GLvoid*);
    IMPORT_GL_FUNC(glColorPointer, GLint, GLenum, GLsizei, const GLvoid*);
    IMPORT_GL_FUNC(glScissor, GLint, GLint, GLsizei, GLsizei);
    IMPORT_GL_FUNC(glBindTexture, GLenum, GLuint);
    IMPORT_GL_FUNC(glDrawElements, GLenum, GLsizei, GLenum, const GLvoid*);
    IMPORT_GL_FUNC(glGenTextures, GLsizei, GLuint*);
    IMPORT_GL_FUNC(glTexParameteri, GLenum, GLenum, GLint);
    IMPORT_GL_FUNC(glPixelStorei, GLenum, GLint);
    IMPORT_GL_FUNC(glTexImage2D, GLenum, GLint, GLint, GLsizei, GLsizei, GLint, GLenum, GLenum, const GLvoid*);
    IMPORT_GL_FUNC(glDeleteTextures, GLsizei, const GLuint*);
    IMPORT_GL_FUNC(glClearColor, GLclampf, GLclampf, GLclampf, GLclampf);
    IMPORT_GL_FUNC(glClear, GLbitfield);

    // Setup backend capabilities flags
    ImGui_ImplOpenGL2_Data* bd = IM_NEW(ImGui_ImplOpenGL2_Data)();
    io.BackendRendererUserData = (void*)bd;
    io.BackendRendererName = "imgui_impl_opengl2";
    io.BackendFlags |= ImGuiBackendFlags_RendererHasViewports;    // We can create multi-viewports on the Renderer side (optional)

    if (io.ConfigFlags & ImGuiConfigFlags_ViewportsEnable)
        ImGui_ImplOpenGL2_InitPlatformInterface();

    return true;
}

void    ImGui_ImplOpenGL2_Shutdown()
{
    ImGui_ImplOpenGL2_Data* bd = ImGui_ImplOpenGL2_GetBackendData();
    IM_ASSERT(bd != nullptr && "No renderer backend to shutdown, or already shutdown?");
    ImGuiIO& io = igGetIO();

    ImGui_ImplOpenGL2_ShutdownPlatformInterface();
    ImGui_ImplOpenGL2_DestroyDeviceObjects();
    io.BackendRendererName = nullptr;
    io.BackendRendererUserData = nullptr;
    io.BackendFlags &= ~ImGuiBackendFlags_RendererHasViewports;
    IM_DELETE(bd);
}

void    ImGui_ImplOpenGL2_NewFrame()
{
    ImGui_ImplOpenGL2_Data* bd = ImGui_ImplOpenGL2_GetBackendData();
    IM_ASSERT(bd != nullptr && "Did you call ImGui_ImplOpenGL2_Init()?");

    if (!bd->FontTexture)
        ImGui_ImplOpenGL2_CreateDeviceObjects();
}

static void ImGui_ImplOpenGL2_SetupRenderState(ImDrawData* draw_data, int fb_width, int fb_height)
{
    // Setup render state: alpha-blending enabled, no face culling, no depth testing, scissor enabled, vertex/texcoord/color pointers, polygon fill.
    import_glEnable(GL_BLEND);
    import_glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
    //glBlendFuncSeparate(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA, GL_ONE, GL_ONE_MINUS_SRC_ALPHA); // In order to composite our output buffer we need to preserve alpha
    import_glDisable(GL_CULL_FACE);
    import_glDisable(GL_DEPTH_TEST);
    import_glDisable(GL_STENCIL_TEST);
    import_glDisable(GL_LIGHTING);
    import_glDisable(GL_COLOR_MATERIAL);
    import_glEnable(GL_SCISSOR_TEST);
    import_glEnableClientState(GL_VERTEX_ARRAY);
    import_glEnableClientState(GL_TEXTURE_COORD_ARRAY);
    import_glEnableClientState(GL_COLOR_ARRAY);
    import_glDisableClientState(GL_NORMAL_ARRAY);
    import_glEnable(GL_TEXTURE_2D);
    import_glPolygonMode(GL_FRONT_AND_BACK, GL_FILL);
    import_glShadeModel(GL_SMOOTH);
    import_glTexEnvi(GL_TEXTURE_ENV, GL_TEXTURE_ENV_MODE, GL_MODULATE);

    // If you are using this code with non-legacy OpenGL header/contexts (which you should not, prefer using imgui_impl_opengl3.cpp!!),
    // you may need to backup/reset/restore other state, e.g. for current shader using the commented lines below.
    // (DO NOT MODIFY THIS FILE! Add the code in your calling function)
    //   GLint last_program;
    //   glGetIntegerv(GL_CURRENT_PROGRAM, &last_program);
    //   glUseProgram(0);
    //   ImGui_ImplOpenGL2_RenderDrawData(...);
    //   glUseProgram(last_program)
    // There are potentially many more states you could need to clear/setup that we can't access from default headers.
    // e.g. glBindBuffer(GL_ARRAY_BUFFER, 0), glDisable(GL_TEXTURE_CUBE_MAP).

    // Setup viewport, orthographic projection matrix
    // Our visible imgui space lies from draw_data->DisplayPos (top left) to draw_data->DisplayPos+data_data->DisplaySize (bottom right). DisplayPos is (0,0) for single viewport apps.
    import_glViewport(0, 0, (GLsizei)fb_width, (GLsizei)fb_height);
    import_glMatrixMode(GL_PROJECTION);
    import_glPushMatrix();
    import_glLoadIdentity();
    import_glOrtho(draw_data->DisplayPos.x, draw_data->DisplayPos.x + draw_data->DisplaySize.x, draw_data->DisplayPos.y + draw_data->DisplaySize.y, draw_data->DisplayPos.y, -1.0f, +1.0f);
    import_glMatrixMode(GL_MODELVIEW);
    import_glPushMatrix();
    import_glLoadIdentity();
}

// OpenGL2 Render function.
// Note that this implementation is little overcomplicated because we are saving/setting up/restoring every OpenGL state explicitly.
// This is in order to be able to run within an OpenGL engine that doesn't do so.
void ImGui_ImplOpenGL2_RenderDrawData(ImDrawData* draw_data)
{
    // Avoid rendering when minimized, scale coordinates for retina displays (screen coordinates != framebuffer coordinates)
    int fb_width = (int)(draw_data->DisplaySize.x * draw_data->FramebufferScale.x);
    int fb_height = (int)(draw_data->DisplaySize.y * draw_data->FramebufferScale.y);
    if (fb_width == 0 || fb_height == 0)
        return;

    // Backup GL state
    GLint last_texture; import_glGetIntegerv(GL_TEXTURE_BINDING_2D, &last_texture);
    GLint last_polygon_mode[2]; import_glGetIntegerv(GL_POLYGON_MODE, last_polygon_mode);
    GLint last_viewport[4]; import_glGetIntegerv(GL_VIEWPORT, last_viewport);
    GLint last_scissor_box[4]; import_glGetIntegerv(GL_SCISSOR_BOX, last_scissor_box);
    GLint last_shade_model; import_glGetIntegerv(GL_SHADE_MODEL, &last_shade_model);
    GLint last_tex_env_mode; import_glGetTexEnviv(GL_TEXTURE_ENV, GL_TEXTURE_ENV_MODE, &last_tex_env_mode);
    import_glPushAttrib(GL_ENABLE_BIT | GL_COLOR_BUFFER_BIT | GL_TRANSFORM_BIT);

    // Setup desired GL state
    ImGui_ImplOpenGL2_SetupRenderState(draw_data, fb_width, fb_height);

    // Will project scissor/clipping rectangles into framebuffer space
    ImVec2 clip_off = draw_data->DisplayPos;         // (0,0) unless using multi-viewports
    ImVec2 clip_scale = draw_data->FramebufferScale; // (1,1) unless using retina display which are often (2,2)

    // Render command lists
    for (int n = 0; n < draw_data->CmdListsCount; n++)
    {
        const ImDrawList* cmd_list = draw_data->CmdLists[n];
        const ImDrawVert* vtx_buffer = cmd_list->VtxBuffer.Data;
        const ImDrawIdx* idx_buffer = cmd_list->IdxBuffer.Data;
        import_glVertexPointer(2, GL_FLOAT, sizeof(ImDrawVert), (const GLvoid*)((const char*)vtx_buffer + IM_OFFSETOF(ImDrawVert, pos)));
        import_glTexCoordPointer(2, GL_FLOAT, sizeof(ImDrawVert), (const GLvoid*)((const char*)vtx_buffer + IM_OFFSETOF(ImDrawVert, uv)));
        import_glColorPointer(4, GL_UNSIGNED_BYTE, sizeof(ImDrawVert), (const GLvoid*)((const char*)vtx_buffer + IM_OFFSETOF(ImDrawVert, col)));

        for (int cmd_i = 0; cmd_i < cmd_list->CmdBuffer.Size; cmd_i++)
        {
            const ImDrawCmd* pcmd = &cmd_list->CmdBuffer[cmd_i];
            if (pcmd->UserCallback)
            {
                // User callback, registered via ImDrawList::AddCallback()
                // (ImDrawCallback_ResetRenderState is a special callback value used by the user to request the renderer to reset render state.)
                if (pcmd->UserCallback == ImDrawCallback_ResetRenderState)
                    ImGui_ImplOpenGL2_SetupRenderState(draw_data, fb_width, fb_height);
                else
                    pcmd->UserCallback(cmd_list, pcmd);
            }
            else
            {
                // Project scissor/clipping rectangles into framebuffer space
                ImVec2 clip_min((pcmd->ClipRect.x - clip_off.x) * clip_scale.x, (pcmd->ClipRect.y - clip_off.y) * clip_scale.y);
                ImVec2 clip_max((pcmd->ClipRect.z - clip_off.x) * clip_scale.x, (pcmd->ClipRect.w - clip_off.y) * clip_scale.y);
                if (clip_max.x <= clip_min.x || clip_max.y <= clip_min.y)
                    continue;

                // Apply scissor/clipping rectangle (Y is inverted in OpenGL)
                import_glScissor((int)clip_min.x, (int)((float)fb_height - clip_max.y), (int)(clip_max.x - clip_min.x), (int)(clip_max.y - clip_min.y));

                // Bind texture, Draw
                import_glBindTexture(GL_TEXTURE_2D, (GLuint)(intptr_t)pcmd->GetTexID());
                import_glDrawElements(GL_TRIANGLES, (GLsizei)pcmd->ElemCount, sizeof(ImDrawIdx) == 2 ? GL_UNSIGNED_SHORT : GL_UNSIGNED_INT, idx_buffer + pcmd->IdxOffset);
            }
        }
    }
    // Restore modified GL state
    import_glDisableClientState(GL_COLOR_ARRAY);
    import_glDisableClientState(GL_TEXTURE_COORD_ARRAY);
    import_glDisableClientState(GL_VERTEX_ARRAY);
    import_glBindTexture(GL_TEXTURE_2D, (GLuint)last_texture);
    import_glMatrixMode(GL_MODELVIEW);
    import_glPopMatrix();
    import_glMatrixMode(GL_PROJECTION);
    import_glPopMatrix();
    import_glPopAttrib();
    import_glPolygonMode(GL_FRONT, (GLenum)last_polygon_mode[0]); import_glPolygonMode(GL_BACK, (GLenum)last_polygon_mode[1]);
    import_glViewport(last_viewport[0], last_viewport[1], (GLsizei)last_viewport[2], (GLsizei)last_viewport[3]);
    import_glScissor(last_scissor_box[0], last_scissor_box[1], (GLsizei)last_scissor_box[2], (GLsizei)last_scissor_box[3]);
    import_glShadeModel(last_shade_model);
    import_glTexEnvi(GL_TEXTURE_ENV, GL_TEXTURE_ENV_MODE, last_tex_env_mode);
}

bool ImGui_ImplOpenGL2_CreateFontsTexture()
{
    // Build texture atlas
    ImGuiIO& io = igGetIO();
    ImGui_ImplOpenGL2_Data* bd = ImGui_ImplOpenGL2_GetBackendData();
    unsigned char* pixels;
    int width, height;
    ImFontAtlas_GetTexDataAsRGBA32(io.Fonts, &pixels, &width, &height, nullptr); // Load as RGBA 32-bit (75% of the memory is wasted, but default font is so small) because it is more likely to be compatible with user's existing shaders. If your ImTextureId represent a higher-level concept than just a GL texture id, consider calling GetTexDataAsAlpha8() instead to save on GPU memory.

    // Upload texture to graphics system
    // (Bilinear sampling is required by default. Set 'io.Fonts->Flags |= ImFontAtlasFlags_NoBakedLines' or 'style.AntiAliasedLinesUseTex = false' to allow point/nearest sampling)
    GLint last_texture;
    import_glGetIntegerv(GL_TEXTURE_BINDING_2D, &last_texture);
    import_glGenTextures(1, &bd->FontTexture);
    import_glBindTexture(GL_TEXTURE_2D, bd->FontTexture);
    import_glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
    import_glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
    import_glPixelStorei(GL_UNPACK_ROW_LENGTH, 0);
    import_glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, width, height, 0, GL_RGBA, GL_UNSIGNED_BYTE, pixels);

    // Store our identifier
    io.Fonts->SetTexID((ImTextureID)(intptr_t)bd->FontTexture);

    // Restore state
    import_glBindTexture(GL_TEXTURE_2D, last_texture);

    return true;
}

void ImGui_ImplOpenGL2_DestroyFontsTexture()
{
    ImGuiIO& io = igGetIO();
    ImGui_ImplOpenGL2_Data* bd = ImGui_ImplOpenGL2_GetBackendData();
    if (bd->FontTexture)
    {
        import_glDeleteTextures(1, &bd->FontTexture);
        ImFontAtlas_SetTexID(io.Fonts, 0);
        bd->FontTexture = 0;
    }
}

bool    ImGui_ImplOpenGL2_CreateDeviceObjects()
{
    return ImGui_ImplOpenGL2_CreateFontsTexture();
}

void    ImGui_ImplOpenGL2_DestroyDeviceObjects()
{
    ImGui_ImplOpenGL2_DestroyFontsTexture();
}


//--------------------------------------------------------------------------------------------------------
// MULTI-VIEWPORT / PLATFORM INTERFACE SUPPORT
// This is an _advanced_ and _optional_ feature, allowing the backend to create and handle multiple viewports simultaneously.
// If you are new to dear imgui or creating a new binding for dear imgui, it is recommended that you completely ignore this section first..
//--------------------------------------------------------------------------------------------------------

static void ImGui_ImplOpenGL2_RenderWindow(ImGuiViewport* viewport, void*)
{
    if (!(viewport->Flags & ImGuiViewportFlags_NoRendererClear))
    {
        ImVec4 clear_color = ImVec4(0.0f, 0.0f, 0.0f, 1.0f);
        import_glClearColor(clear_color.x, clear_color.y, clear_color.z, clear_color.w);
        import_glClear(GL_COLOR_BUFFER_BIT);
    }
    ImGui_ImplOpenGL2_RenderDrawData(viewport->DrawData);
}

static void ImGui_ImplOpenGL2_InitPlatformInterface()
{
    ImGuiPlatformIO& platform_io = igGetPlatformIO();
    platform_io.Renderer_RenderWindow = ImGui_ImplOpenGL2_RenderWindow;
}

static void ImGui_ImplOpenGL2_ShutdownPlatformInterface()
{
    igDestroyPlatformWindows();
}

//-----------------------------------------------------------------------------

#if defined(__clang__)
#pragma clang diagnostic pop
#endif

#endif // #ifndef IMGUI_DISABLE
