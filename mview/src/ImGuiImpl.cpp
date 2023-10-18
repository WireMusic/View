//#include "imgui.h"
//#include "imgui_internal.h"
#include "imgui_impl_glfw.h"
#include "imgui_impl_opengl2.h"

#include "cimgui.h"

#include "mview.h"
#include "GLFW/glfw3.h"

#include <iostream>
#include <vector>
#include <map>

static void SetStyle();

static std::map<float, ImFont*> DefaultFonts;
static std::map<float, ImFont*> BoldFonts;
static bool LoadNewSizes = false;

MV_EXPORT void ImGuiInit(GLFWwindow* window, void* fontData, int dataSize, void* boldFontData, int boldFontDataSize, float fontSize, float* defaultFontSizes, int dfsSize, float* boldFontSizes, int bfsSize, bool loadFontSizes)
{
    igCreateContext(nullptr);
    ImGuiIO& io = *igGetIO();
    io.ConfigFlags |= ImGuiConfigFlags_NavEnableKeyboard;
    io.ConfigFlags |= ImGuiConfigFlags_DockingEnable;
    io.ConfigFlags |= ImGuiConfigFlags_ViewportsEnable;

    LoadNewSizes = loadFontSizes;

    DefaultFonts[fontSize] = io.FontDefault = ImFontAtlas_AddFontFromMemoryTTF(io.Fonts, fontData, dataSize, fontSize, nullptr, nullptr);

    for (int i = 0; i < dfsSize; i++)
    {
        float size = defaultFontSizes[i];
        if (size == fontSize)
            continue;
        if (DefaultFonts.count(size))
            continue;

        DefaultFonts[size] = ImFontAtlas_AddFontFromMemoryTTF(io.Fonts, fontData, dataSize, size, nullptr, nullptr);
    }

    BoldFonts[fontSize] = ImFontAtlas_AddFontFromMemoryTTF(io.Fonts, boldFontData, boldFontDataSize, fontSize, nullptr, nullptr);

    for (int i = 0; i < bfsSize; i++)
    {
        float size = boldFontSizes[i];
        if (size == fontSize)
            continue;
        if (BoldFonts.count(size))
            continue;

        BoldFonts[size] = ImFontAtlas_AddFontFromMemoryTTF(io.Fonts, boldFontData, boldFontDataSize, size, nullptr, nullptr);
    }

    igStyleColorsDark(nullptr);

    ImGuiStyle& style = *igGetStyle();
    if (io.ConfigFlags & ImGuiConfigFlags_ViewportsEnable)
    {
        style.WindowRounding = 0.0f;
		style.Colors[ImGuiCol_WindowBg].w = 1.0f;
    }

    SetStyle();

    ImGui_ImplGlfw_InitForOpenGL(window, true);
    ImGui_ImplOpenGL2_Init();
}

MV_EXPORT void ImGuiShutdown()
{
    ImGui_ImplOpenGL2_Shutdown();
    ImGui_ImplGlfw_Shutdown();
    igDestroyContext(igGetCurrentContext());

    /*ImGuiContext* ctx = igGetCurrentContext();
    igSetCurrentContext(ctx);
    //igShutdown();

    ImGuiContext& g = *igGetCurrentContext();
    if (g.IO.Fonts && g.FontAtlasOwnedByContext)
    {
        g.IO.Fonts->Locked = false;
        //IM_DELETE(g.IO.Fonts);
    }
    g.IO.Fonts = NULL;
    
    g.DrawListSharedData.TempBuffer.Capacity = 0;
    g.DrawListSharedData.TempBuffer.Data = nullptr;
    g.DrawListSharedData.TempBuffer.Size = 0;

    if (!g.Initialized)
        return;

    if (g.SettingsLoaded && g.IO.IniFilename != NULL)
        igSaveIniSettingsToDisk(g.IO.IniFilename);

    igDestroyPlatformWindows();

    igDockContextShutdown(&g);

    igCallContextHooks(&g, ImGuiContextHookType_Shutdown);

    //g.Windows.clear_delete();
    g.WindowsFocusOrder.Capacity = 0;
    g.WindowsFocusOrder.Data = nullptr;
    g.WindowsFocusOrder.Size = 0;
    g.WindowsTempSortBuffer.Capacity = 0;
    g.WindowsTempSortBuffer.Data = nullptr;
    g.WindowsTempSortBuffer.Size = 0;
    g.CurrentWindow = NULL;
    g.CurrentWindowStack.Capacity = 0;
    g.CurrentWindowStack.Data = 0;
    g.CurrentWindowStack.Size = 0;
    ImGuiStorage_Clear(&g.WindowsById);
    g.NavWindow = NULL;
    g.HoveredWindow = g.HoveredWindowUnderMovingWindow = NULL;
    g.ActiveIdWindow = g.ActiveIdPreviousFrameWindow = NULL;
    g.MovingWindow = NULL;

    ImGuiKeyRoutingTable_Clear(&g.KeysRoutingTable);

    g.ColorStack.clear();
    g.StyleVarStack.clear();
    //g.FontStack.clear();
    g.OpenPopupStack.clear();
    g.BeginPopupStack.clear();
    g.NavTreeNodeStack.clear();

    g.CurrentViewport = g.MouseViewport = g.MouseLastHoveredViewport = NULL;
    g.Viewports.clear_delete();

    //g.TabBars.Clear();
    g.CurrentTabBarStack.clear();
    g.ShrinkWidthBuffer.clear();

    g.ClipperTempData.clear_destruct();

    //g.Tables.Clear();
    //g.TablesTempData.clear_destruct();
    g.DrawChannelsTempMergeBuffer.clear();

    g.ClipboardHandlerData.clear();
    g.MenusIdSubmittedThisFrame.clear();
    //g.InputTextState.ClearFreeMemory();
    ImGuiInputTextState_ClearFreeMemory(&g.InputTextState);
    //g.InputTextDeactivatedState.ClearFreeMemory();
    ImGuiInputTextDeactivatedState_ClearFreeMemory(&g.InputTextDeactivatedState);

    g.SettingsWindows.clear();
    g.SettingsHandlers.clear();

    if (g.LogFile)
    {
#ifndef IMGUI_DISABLE_TTY_FUNCTIONS
        if (g.LogFile != stdout)
#endif
            igImFileClose(g.LogFile);
        g.LogFile = NULL;
    }
    g.LogBuffer.clear();
    g.DebugLogBuf.clear();
    g.DebugLogIndex.clear();

    g.Initialized = false;

    //
    igSetCurrentContext(nullptr);
    if (ctx)
    {
        igMemFree(ctx);
    }*/
}

MV_EXPORT void ImGuiIO_GetMousePos(ImVec2* outValue)
{
    *outValue = igGetIO()->MousePos;
}

MV_EXPORT void ImGuiIO_GetMouseDelta(ImVec2* outValue)
{
    *outValue = igGetIO()->MouseDelta;
}

MV_EXPORT float ImGuiStyle_GetAlpha()
{
    return igGetCurrentContext()->Style.Alpha;
}

MV_EXPORT void DrawList_AddText_Vec2(ImDrawList* ptr, ImVec2* pos, ImU32 col, const char* text_begin, const char* text_end)
{
    ImDrawList_AddText_Vec2(ptr, pos, col, text_begin, NULL);
}

MV_EXPORT void DrawList_AddText_Font(ImDrawList* ptr, bool bold, float fontSize, void* fontData, int dataSize, ImVec2* pos, ImU32 col, const char* text_begin, const char* text_end, float wrap_width, ImVec4* cpu_fine_clip_rect)
{
    bool mapContainsSize = bold ? BoldFonts.count(fontSize) : DefaultFonts.count(fontSize);
    ImFont* font = nullptr;

    if (LoadNewSizes)
    {
        if (mapContainsSize)
        {
            font = bold ? BoldFonts.at(fontSize) : DefaultFonts.at(fontSize);
        }
        else
        {
            if (bold)
            {
                font = BoldFonts[fontSize] = ImFontAtlas_AddFontFromMemoryTTF(igGetIO()->Fonts, fontData, dataSize, fontSize, nullptr, nullptr);
            }
            else
            {
                font = DefaultFonts[fontSize] = ImFontAtlas_AddFontFromMemoryTTF(igGetIO()->Fonts, fontData, dataSize, fontSize, nullptr, nullptr);
            }
        }
    }
    else
    {
        if (mapContainsSize)
        {
            font = bold ? BoldFonts.at(fontSize) : DefaultFonts.at(fontSize);
        }
    }

    if (font)
    {
        ImDrawList_AddText_FontPtr(ptr, font, fontSize, pos, col, text_begin, text_end, wrap_width, const_cast<const ImVec4**>(&cpu_fine_clip_rect));
    }
    else
    {
        DrawList_AddText_Vec2(ptr, pos, col, text_begin, text_end);
    }
}

MV_EXPORT bool PushFont(bool bold, float fontSize, void* fontData, int dataSize)
{
    bool mapContainsSize = bold ? BoldFonts.count(fontSize) : DefaultFonts.count(fontSize);

    if (LoadNewSizes)
    {
        if (mapContainsSize)
        {
            igPushFont(bold ? BoldFonts.at(fontSize) : DefaultFonts.at(fontSize));
            return true;
        }
        else
        {
            if (bold)
            {
                BoldFonts[fontSize] = ImFontAtlas_AddFontFromMemoryTTF(igGetIO()->Fonts, fontData, dataSize, fontSize, nullptr, nullptr);
            }
            else
            {
                DefaultFonts[fontSize] = ImFontAtlas_AddFontFromMemoryTTF(igGetIO()->Fonts, fontData, dataSize, fontSize, nullptr, nullptr);
            }

            return true;
        }
    }
    else
    {
        if (mapContainsSize)
        {
            igPushFont(bold ? BoldFonts.at(fontSize) : DefaultFonts.at(fontSize));
            return true;
        }
    }
    return false;
}

MV_EXPORT void** DrawList_GetCmdBuffer(ImDrawList* drawList, int* outSize)
{
    std::vector<void*> ptrs;
    
    for (int i = 0; i < drawList->CmdBuffer.Size; i++)
    {
        ptrs.push_back(&drawList->CmdBuffer.Data[i]);
    }

    *outSize = (int)ptrs.size();

    return ptrs.data();
}

MV_EXPORT void DrawList_SetCmdBuffer(ImDrawList* drawList, void** ptr, int size)
{
    ImVector_ImDrawCmd vector;
    vector.Data = (ImDrawCmd*)*ptr;
    vector.Size = size;

    drawList->CmdBuffer = vector;
}

#define DrawCmd(type, name, ret, ptr) MV_EXPORT ret ImDrawCmd_Get##name(ImDrawCmd* cmd, type out) \
    { \
        ptr##out = cmd->name; \
    } \
    MV_EXPORT void ImDrawCmd_Set##name(ImDrawCmd* cmd, type in) \
    { \
        cmd->name = ptr##in; \
    }

DrawCmd(ImVec4*, ClipRect, void, *);
DrawCmd(void*, TextureId, void, );
DrawCmd(uint32_t, VtxOffset, void, );
DrawCmd(uint32_t, IdxOffset, void, );
DrawCmd(uint32_t, ElemCount, void, );
DrawCmd(ImDrawCallback, UserCallback, void, );

MV_EXPORT void* ImDrawCmd_GetUserCallbackData(ImDrawCmd* cmd)
{
    return cmd->UserCallbackData;
}

MV_EXPORT void ImDrawCmd_SetUserCallbackData(ImDrawCmd* cmd, void* in)
{
    cmd->UserCallbackData = in;
}

#define DrawVert(type, name, ret, ptr) MV_EXPORT ret ImDrawVert_Get##name(ImDrawVert* vert, type out) \
    { \
        ptr##out = vert->name; \
    } \
    MV_EXPORT ret ImDrawVert_Set##name(ImDrawVert* vert, type in) \
    { \
        vert->name = ptr##in; \
    }

DrawVert(ImVec2*, pos, void, *);
DrawVert(ImVec2*, uv, void, *);
DrawVert(uint32_t, col, void, );

MV_EXPORT bool BeginTable(const char* id, int columns, ImGuiTableFlags flags, ImVec2* size, float inner)
{
    //std::cout << id << std::endl;
    //std::cout << flags << std::endl;
    //std::cout << size->x << ", " << size->y << std::endl;
    //std::cout << inner << std::endl;

    bool val = igBeginTable(id, columns, flags, size, inner);
    //std::cout << val << std::endl;
    return val;
}

MV_EXPORT void DrawList_PushClipRect(ImDrawList* drawList, ImVec2* min, ImVec2* max, bool intersect)
{
    ImDrawList_PushClipRect(drawList, min, max, intersect);
}

MV_EXPORT void DrawList_PushClipRectFullScreen(ImDrawList* drawList)
{
    ImDrawList_PushClipRectFullScreen(drawList);
}

MV_EXPORT void DrawList_PopClipRect(ImDrawList* drawList)
{
    ImDrawList_PopClipRect(drawList);
}

MV_EXPORT void DrawList_PushTextureID(ImDrawList* drawList, ImTextureID id)
{
    ImDrawList_PushTextureID(drawList, id);
}

MV_EXPORT void DrawList_PopTextureID(ImDrawList* self)
{
    ImDrawList_PopTextureID(self);
}

MV_EXPORT void DrawList_GetClipRectMin(ImVec2* out, ImDrawList* self)
{
    ImDrawList_GetClipRectMin(out, self);
}

MV_EXPORT void DrawList_GetClipRectMax(ImVec2* out, ImDrawList* self)
{
    ImDrawList_GetClipRectMax(out, self);
}

MV_EXPORT void DrawList_AddLine(ImDrawList* self, ImVec2* p1, ImVec2* p2, ImU32 col, float thickness)
{
    ImDrawList_AddLine(self, p1, p2, col, thickness);
}

MV_EXPORT void DrawList_AddRect(ImDrawList* self, ImVec2* min, ImVec2* max, ImU32 col, float rounding, ImDrawFlags flags, float thickness)
{
    ImDrawList_AddRect(self, min, max, col, rounding, flags, thickness);
}

MV_EXPORT void DrawList_AddRectFilled(ImDrawList* self, ImVec2* min, ImVec2* max, ImU32 col, float rounding, ImDrawFlags flags)
{
    ImDrawList_AddRectFilled(self, min, max, col, rounding, flags);
}

MV_EXPORT void DrawList_AddRectFilledMultiColor(ImDrawList* self, ImVec2* min, ImVec2* max, ImU32 tl, ImU32 tr, ImU32 br, ImU32 bl)
{
    ImDrawList_AddRectFilledMultiColor(self, min, max, tl, tr, br, bl);
}

MV_EXPORT void DrawList_AddQuad(ImDrawList* self, ImVec2* p1, ImVec2* p2, ImVec2* p3, ImVec2* p4, ImU32 col, float thickness)
{
    ImDrawList_AddQuad(self, p1, p2, p3, p4, col, thickness);
}

template<typename T> static inline T ImMax(T lhs, T rhs) { return lhs >= rhs ? lhs : rhs; }
template<typename T> static inline T ImLerp(T a, T b, float t) { return (T)(a + (b - a) * t); }

MV_EXPORT void ScrollToBottom()
{
    float center_y_ratio = 1.0f;

    ImGuiContext& g = *igGetCurrentContext();
    ImGuiWindow* window = g.CurrentWindow;
    float spacing_y = ImMax(window->WindowPadding.y, g.Style.ItemSpacing.y);
    float target_pos_y = ImLerp(window->DC.CursorPosPrevLine.y - spacing_y, window->DC.CursorPosPrevLine.y + window->DC.PrevLineSize.y + spacing_y, center_y_ratio) + 100.0f;
    
    igSetScrollFromPosY_WindowPtr(window, target_pos_y - window->Pos.y, center_y_ratio);

    window->ScrollTargetEdgeSnapDist.y = ImMax(0.0f, window->WindowPadding.y - spacing_y);
}

MV_EXPORT void AlignText(float offset)
{
    ImGuiWindow* window = igGetCurrentWindow();
    if (window->SkipItems)
        return;

    ImGuiContext& g = *igGetCurrentContext();
    window->DC.CurrLineSize.y = ImMax(window->DC.CurrLineSize.y, g.FontSize + g.Style.FramePadding.y * 2);
    window->DC.CurrLineTextBaseOffset = ImMax(window->DC.CurrLineTextBaseOffset, g.Style.FramePadding.y) + offset;
}

MV_EXPORT void ImGuiBegin()
{
    ImGui_ImplOpenGL2_NewFrame();
    ImGui_ImplGlfw_NewFrame();
    igNewFrame();
}

MV_EXPORT void ImGuiEnd(GLFWwindow* window)
{
    ImGuiIO& io = *igGetIO();
    int width, height;
    glfwGetWindowSize(window, &width, &height);
    io.DisplaySize = ImVec2{ (float)width, (float)height };

    igRender();
    ImGui_ImplOpenGL2_RenderDrawData((ImDrawData*)igGetDrawData());

    if (io.ConfigFlags & ImGuiConfigFlags_ViewportsEnable)
    {
        GLFWwindow* backup_current_context = glfwGetCurrentContext();
        igUpdatePlatformWindows();
        igRenderPlatformWindowsDefault(nullptr, nullptr);
        glfwMakeContextCurrent(backup_current_context);
    }
}

MV_EXPORT ImGuiID GetImViewportID(ImGuiViewport* viewport)
{
    return viewport->ID;
}

MV_EXPORT ImVec2* GetImViewportPos(ImGuiViewport* viewport)
{
    return &viewport->Pos;
}

MV_EXPORT ImVec2* GetImViewportSize(ImGuiViewport* viewport)
{
    return &viewport->Size;
}

MV_EXPORT void SetMinWinSizeX(float size)
{
    igGetStyle()->WindowMinSize.x = size;
}

MV_EXPORT float GetMinWinSizeX()
{
    return igGetStyle()->WindowMinSize.x;
}

MV_EXPORT ImVec2* IOGetDisplaySize()
{
    return &igGetIO()->DisplaySize;
}

static void SetDarkThemeColours();

static void SetStyle()
{
    auto& style = *igGetStyle();
    style.PopupRounding = 5.0f;
    style.FrameRounding = 5.0f;
    
    style.FramePadding = ImVec2{ 8.0f, 8.0f };

    SetDarkThemeColours();
}

static void SetDarkThemeColours()
{
    auto& colours = (*igGetStyle()).Colors;
    colours[ImGuiCol_WindowBg] = ImVec4{ 0.1f, 0.105f, 0.11f, 1.0f };
    colours[ImGuiCol_PopupBg] = ImVec4{ 0.2f, 0.205f, 0.21f, 1.0f };

    // Headers
    colours[ImGuiCol_Header] = ImVec4{ 0.2f, 0.205f, 0.21f, 1.0f };
    colours[ImGuiCol_HeaderHovered] = ImVec4{ 0.3f, 0.305f, 0.31f, 1.0f };
    colours[ImGuiCol_HeaderActive] = ImVec4{ 0.15f, 0.1505f, 0.151f, 1.0f };

    // Buttons
    colours[ImGuiCol_Button] = ImVec4{ 0.2f, 0.205f, 0.21f, 1.0f };
    colours[ImGuiCol_ButtonHovered] = ImVec4{ 0.3f, 0.305f, 0.31f, 1.0f };
    colours[ImGuiCol_ButtonActive] = ImVec4{ 0.15f, 0.1505f, 0.151f, 1.0f };

    // Frame BG
    colours[ImGuiCol_FrameBg] = ImVec4{ 0.2f, 0.205f, 0.21f, 1.0f };
    colours[ImGuiCol_FrameBgHovered] = ImVec4{ 0.3f, 0.305f, 0.31f, 1.0f };
    colours[ImGuiCol_FrameBgActive] = ImVec4{ 0.15f, 0.1505f, 0.151f, 1.0f };

    // Tabs
    colours[ImGuiCol_Tab] = ImVec4{ 0.15f, 0.1505f, 0.151f, 1.0f };
    colours[ImGuiCol_TabHovered] = ImVec4{ 0.38f, 0.3805f, 0.381f, 1.0f };
    colours[ImGuiCol_TabActive] = ImVec4{ 0.28f, 0.2805f, 0.281f, 1.0f };
    colours[ImGuiCol_TabUnfocused] = ImVec4{ 0.15f, 0.1505f, 0.151f, 1.0f };
    colours[ImGuiCol_TabUnfocusedActive] = ImVec4{ 0.2f, 0.205f, 0.21f, 1.0f };

    // Title
    colours[ImGuiCol_TitleBg] = ImVec4{ 0.15f, 0.1505f, 0.151f, 1.0f };
    colours[ImGuiCol_TitleBgActive] = ImVec4{ 0.15f, 0.1505f, 0.151f, 1.0f };
    colours[ImGuiCol_TitleBgCollapsed] = ImVec4{ 0.15f, 0.1505f, 0.151f, 1.0f };
}
