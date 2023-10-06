using System;
using System.Text;
using System.Runtime.InteropServices;

using Stage.Core;
using Stage.ImGui;
using Stage.UIModule;
using static Stage.Renderer.GLConstants;

namespace Stage.Renderer
{
    public struct WindowProperties
    {
        public string Title = "Window";
        public uint Width = 1280;
        public uint Height = 720;
        public bool Maximised = false;

        public WindowProperties(string title, uint width, uint height, bool maximised)
        {
            Title = title;
            Width = width;
            Height = height;
            Maximised = maximised;
        }
    }

    public class Window : IDisposable
    {
        internal delegate nint GetProcAddress([MarshalAs(UnmanagedType.LPStr)] string functionName);

        internal static GetProcAddress glfwGetProcAddress = null;

        private static bool s_Init = false;
        private static ushort s_WindowCount = 0;

        private nint m_WindowPtr;
        private GraphicsContext m_Context;
        private WindowProperties m_WindowProperties;

        private bool m_FrozeOtherWindows;
        private bool m_SecondaryWindow;

        private static bool s_Frozen = false;
        public static bool Frozen => s_Frozen;

        public unsafe bool ShouldClose => _glfw.WindowShouldClose(m_WindowPtr);

        public nint Handle => m_WindowPtr;
        public unsafe nint NativeHandle => _glfw.GetWin32Window(Handle);

        //public Vector2 Size => new Vector2(m_WindowProperties.Width, m_WindowProperties.Height);

        public unsafe Vector2 Size
        {
            get
            {
                _glfw.GetWindowSize(m_WindowPtr, out int width, out int height);
                return new Vector2(width, height);
            }
        }

        private bool m_VSync = false;
        public unsafe bool VSync
        {
            get
            {
                return m_VSync;
            }
            set
            {
                if (value)
                    _glfw.SwapInterval(1);
                else
                    _glfw.SwapInterval(0);

                m_VSync = value;
            }
        }

        public unsafe Window(WindowProperties props, bool secondaryWindow = false, bool freezeOtherWindows = false)
        {
            if (!s_Init)
            {
                Init();
                _glfw.Init();
            }

            if (props.Maximised)
            {
                _glfw.WindowHint(0x00020008, 1);
            }
            else
            {
                _glfw.WindowHint(0x00020008, 0);
            }

            if (secondaryWindow)
            {
                // temp: TODO: add more to WindowProperties
                _glfw.WindowHint(0x00020003, 0);
            }

            _glfw.WindowHint(0x00022002, 3);
            _glfw.WindowHint(0x00022003, 3);
            _glfw.WindowHint(0x00022008, 0x00032001);
            _glfw.WindowHint(0x00022006, 1);

            nint share = secondaryWindow ? Application.Instance.Window.Handle : nint.Zero;

            m_WindowPtr = _glfw.CreateWindow((int)props.Width, (int)props.Height, props.Title, nint.Zero, share);

            m_SecondaryWindow = secondaryWindow;
            
            if (secondaryWindow)
            {
                _helper.CenterWindow(m_WindowPtr);

#if WINDOWS
                SetWindowLongA(NativeHandle, -16, GetWindowLongA(NativeHandle, -16) & ~((int)0x01000000 | (int)0x20000000));
#endif
            }
            s_Frozen = freezeOtherWindows;
            m_FrozeOtherWindows = freezeOtherWindows;

            if (!s_Init)
            {
                s_Init = true;
            }

            m_Context = new GraphicsContext(m_WindowPtr, !secondaryWindow);
            m_WindowProperties = props;

            if (secondaryWindow)
            {
                m_Context.MakeCurrent();
                _helper.ImGuiSetCurrentWindow(m_WindowPtr);
                ImGuiImpl.DrawingWindow = m_WindowPtr;

                _helper.ImGuiForceInstallCallbacks(m_WindowPtr);
            }

            s_WindowCount++;
        }

        private static unsafe void Init()
        {
            _glfw = new GLFW();

            if (Core.Application.Instance.Specification.GLFW == null)
                _glfw.Lib = new Library("glfw3.dll");
            else
                _glfw.Lib = new Library("glfw3", Core.Application.Instance.Specification.GLFW);

            glfwGetProcAddress = Marshal.GetDelegateForFunctionPointer<GetProcAddress>(_glfw.Lib.Load("glfwGetProcAddress"));
            _glfw.Init = (delegate* unmanaged[Cdecl]<int>)_glfw.Lib.Load("glfwInit");
            _glfw.Terminate = (delegate* unmanaged[Cdecl]<void>)_glfw.Lib.Load("glfwTerminate");
            _glfw.CreateWindow = (delegate* unmanaged[Cdecl]<int, int, string, nint, nint, nint>)_glfw.Lib.Load("glfwCreateWindow");
            _glfw.DestroyWindow = (delegate* unmanaged[Cdecl]<nint, void>)_glfw.Lib.Load("glfwDestroyWindow");
            _glfw.WindowShouldClose = (delegate* unmanaged[Cdecl]<nint, bool>)_glfw.Lib.Load("glfwWindowShouldClose");
            _glfw.PollEvents = (delegate* unmanaged[Cdecl]<void>)_glfw.Lib.Load("glfwPollEvents");
            _glfw.SwapBuffers = (delegate* unmanaged[Cdecl]<nint, void>)_glfw.Lib.Load("glfwSwapBuffers");
            _glfw.MakeContextCurrent = (delegate* unmanaged[Cdecl]<nint, void>)_glfw.Lib.Load("glfwMakeContextCurrent");
            _glfw.SwapInterval = (delegate* unmanaged[Cdecl]<int, void>)_glfw.Lib.Load("glfwSwapInterval");
            _glfw.SetClipboardString = (delegate* unmanaged[Cdecl]<nint, string, void>)_glfw.Lib.Load("glfwSetClipboardString");
            _glfw.GetClipboardString = (delegate* unmanaged[Cdecl]<nint, string>)_glfw.Lib.Load("glfwGetClipboardString");
            _glfw.SetErrorCallback = (delegate* unmanaged[Cdecl]<delegate* unmanaged[Cdecl]<int, string, void>, delegate* unmanaged[Cdecl]<int, string, void>>)_glfw.Lib.Load("glfwSetErrorCallback");
            _glfw.CreateStandardCursor = (delegate* unmanaged[Cdecl]<int, nint>)_glfw.Lib.Load("glfwCreateStandardCursor");
            _glfw.GetError = (delegate* unmanaged[Cdecl]<nint, int>)_glfw.Lib.Load("glfwGetError");
            _glfw.SetWindowFocusCallback = (delegate* unmanaged[Cdecl]<nint, nint, nint>)_glfw.Lib.Load("glfwSetWindowFocusCallback");
            _glfw.SetCursorEnterCallback = (delegate* unmanaged[Cdecl]<nint, nint, nint>)_glfw.Lib.Load("glfwSetCursorEnterCallback");
            _glfw.SetCursorPosCallback = (delegate* unmanaged[Cdecl]<nint, nint, nint>)_glfw.Lib.Load("glfwSetCursorPosCallback");
            _glfw.SetMouseButtonCallback = (delegate* unmanaged[Cdecl]<nint, nint, nint>)_glfw.Lib.Load("glfwSetMouseButtonCallback");
            _glfw.SetScrollCallback = (delegate* unmanaged[Cdecl]<nint, nint, nint>)_glfw.Lib.Load("glfwSetScrollCallback");
            _glfw.SetKeyCallback = (delegate* unmanaged[Cdecl]<nint, nint, nint>)_glfw.Lib.Load("glfwSetKeyCallback");
            _glfw.SetCharCallback = (delegate* unmanaged[Cdecl]<nint, nint, nint>)_glfw.Lib.Load("glfwSetCharCallback");
            _glfw.SetMonitorCallback = (delegate* unmanaged[Cdecl]<nint, nint, nint>)_glfw.Lib.Load("glfwSetMonitorCallback");
            _glfw.GetWin32Window = (delegate* unmanaged[Cdecl]<nint, nint>)_glfw.Lib.Load("glfwGetWin32Window");
            _glfw.GetKey = (delegate* unmanaged[Cdecl]<nint, int, int>)_glfw.Lib.Load("glfwGetKey");
            _glfw.GetMouseButton = (delegate* unmanaged[Cdecl]<nint, int, int>)_glfw.Lib.Load("glfwGetMouseButton");
            _glfw.GetCursorPos = (delegate* unmanaged[Cdecl]<nint, out double, out double, void>)_glfw.Lib.Load("glfwGetCursorPos");
            _glfw.WindowHint = (delegate* unmanaged[Cdecl]<int, int, void>)_glfw.Lib.Load("glfwWindowHint");
            _glfw.SetWindowPos = (delegate* unmanaged[Cdecl]<nint, int, int, void>)_glfw.Lib.Load("glfwSetWindowPos");
            _glfw.GetWindowSize = (delegate* unmanaged[Cdecl]<nint, out int, out int, void>)_glfw.Lib.Load("glfwGetWindowSize");

            _glad = new Glad();
            if (Core.Application.Instance.Specification.Glad == null)
                _glfw.Lib = new Library("glad.dll");
            else
                _glad.Lib = new Library("glad", Core.Application.Instance.Specification.Glad);
            _glad.LoadGLLoader = (delegate* unmanaged[Cdecl]<delegate* unmanaged[Cdecl]<char*, int>, int>)_glad.Lib.Load("gladLoadGLLoader");
            _imgui = new Core.ImGui();
            if (Core.Application.Instance.Specification.cimgui == null)
                _imgui.Lib = new Library("cimgui.dll");
            else
                _imgui.Lib = new Library("cimgui", Core.Application.Instance.Specification.cimgui);
            _imgui.Begin = (delegate* unmanaged[Cdecl]<string, bool*, int, bool>)_imgui.Lib.Load("igBegin");
            _imgui.End = (delegate* unmanaged[Cdecl]<void>)_imgui.Lib.Load("igEnd");
            _imgui.ImageButton = (delegate* unmanaged[Cdecl]<string, nint, Vector2, Vector2, Vector2, Vector4, Vector4, bool>)_imgui.Lib.Load("igImageButton");
            _imgui.Text = (delegate* unmanaged[Cdecl]<string, void>)_imgui.Lib.Load("igText");
            _imgui.PushStyleVar_Float = (delegate* unmanaged[Cdecl]<int, float, void>)_imgui.Lib.Load("igPushStyleVar_Float");
            _imgui.PushStyleVar_Vec2 = (delegate* unmanaged[Cdecl]<int, Vector2, void>)_imgui.Lib.Load("igPushStyleVar_Vec2");
            _imgui.PopStyleVar = (delegate* unmanaged[Cdecl]<int, void>)_imgui.Lib.Load("igPopStyleVar");
            _imgui.GetMainViewport = (delegate* unmanaged[Cdecl]<nint>)_imgui.Lib.Load("igGetMainViewport");
            _imgui.SetNextWindowPos = (delegate* unmanaged[Cdecl]<Vector2, int, Vector2, void>)_imgui.Lib.Load("igSetNextWindowPos");
            _imgui.SetNextWindowSize = (delegate* unmanaged[Cdecl]<Vector2, int, void>)_imgui.Lib.Load("igSetNextWindowSize");
            _imgui.SetNextWindowViewport = (delegate* unmanaged[Cdecl]<uint, void>)_imgui.Lib.Load("igSetNextWindowViewport");
            _imgui.GetID = (delegate* unmanaged[Cdecl]<string, uint>)_imgui.Lib.Load("igGetID_Str");
            _imgui.DockSpace = (delegate* unmanaged[Cdecl]<uint, Vector2, int, nint, uint>)_imgui.Lib.Load("igDockSpace");
            _imgui.BeginMenuBar = (delegate* unmanaged[Cdecl]<bool>)_imgui.Lib.Load("igBeginMenuBar");
            _imgui.EndMenuBar = (delegate* unmanaged[Cdecl]<void>)_imgui.Lib.Load("igEndMenuBar");
            _imgui.BeginMenu = (delegate* unmanaged[Cdecl]<string, bool, bool>)_imgui.Lib.Load("igBeginMenu");
            _imgui.EndMenu = (delegate* unmanaged[Cdecl]<void>)_imgui.Lib.Load("igEndMenu");
            _imgui.MenuItem_Bool = (delegate* unmanaged[Cdecl]<string, string, bool, bool, bool>)_imgui.Lib.Load("igMenuItem_Bool");
            _imgui.SameLine = (delegate* unmanaged[Cdecl]<float, float, void>)_imgui.Lib.Load("igSameLine");
            _imgui.Button = (delegate* unmanaged[Cdecl]<string, Vector2, bool>)_imgui.Lib.Load("igButton");
            _imgui.ButtonEx = (delegate* unmanaged[Cdecl]<string, Vector2, int, bool>)_imgui.Lib.Load("igButtonEx");
            _imgui.Checkbox = (delegate* unmanaged[Cdecl]<string, bool*, bool>)_imgui.Lib.Load("igCheckbox");
            _imgui.PushStyleColor_U32 = (delegate* unmanaged[Cdecl]<int, uint, void>)_imgui.Lib.Load("igPushStyleColor_U32");
            _imgui.PushStyleColor_Vec4 = (delegate* unmanaged[Cdecl]<int, Vector4, void>)_imgui.Lib.Load("igPushStyleColor_Vec4");
            _imgui.PopStyleColor = (delegate* unmanaged[Cdecl]<int, void>)_imgui.Lib.Load("igPopStyleColor");
            //_imgui.BeginTable = (delegate* unmanaged[Cdecl]<string, int, int, Vector2*, float, bool>)_imgui.Lib.Load("igBeginTable");
            _imgui.CalcTextSize = (delegate* unmanaged[Cdecl]<out Vector2, string, nint, bool, float, void>)_imgui.Lib.Load("igCalcTextSize");
            _imgui.EndTable = (delegate* unmanaged[Cdecl]<void>)_imgui.Lib.Load("igEndTable");
            _imgui.TableSetupColumn = (delegate* unmanaged[Cdecl]<string, int, float, uint, void>)_imgui.Lib.Load("igTableSetupColumn");
            _imgui.GetWindowSize = (delegate* unmanaged[Cdecl]<out Vector2, void>)_imgui.Lib.Load("igGetWindowSize");
            _imgui.TableNextRow = (delegate* unmanaged[Cdecl]<int, float, void>)_imgui.Lib.Load("igTableNextRow");
            _imgui.TableSetBgColor = (delegate* unmanaged[Cdecl]<int, uint, int, void>)_imgui.Lib.Load("igTableSetBgColor");
            _imgui.GetColorU32_Vec4 = (delegate* unmanaged[Cdecl]<Vector4, uint>)_imgui.Lib.Load("igGetColorU32_Vec4");
            _imgui.TableSetColumnIndex = (delegate* unmanaged[Cdecl]<int, bool>)_imgui.Lib.Load("igTableSetColumnIndex");
            _imgui.GetScrollY = (delegate* unmanaged[Cdecl]<float>)_imgui.Lib.Load("igGetScrollY");
            _imgui.GetScrollMaxY = (delegate* unmanaged[Cdecl]<float>)_imgui.Lib.Load("igGetScrollMaxY");
            _imgui.OpenPopup = (delegate* unmanaged[Cdecl]<string, int, void>)_imgui.Lib.Load("igOpenPopup_Str");
            _imgui.BeginPopupModal = (delegate* unmanaged[Cdecl]<string, bool*, int, bool>)_imgui.Lib.Load("igBeginPopupModal");
            _imgui.EndPopup = (delegate* unmanaged[Cdecl]<void>)_imgui.Lib.Load("igEndPopup");
            _imgui.CloseCurrentPopup = (delegate* unmanaged[Cdecl]<void>)_imgui.Lib.Load("igCloseCurrentPopup");
            _imgui.ImGuiViewport_GetCenter = (delegate* unmanaged[Cdecl]<out Vector2, nint, void>)_imgui.Lib.Load("ImGuiViewport_GetCenter");
            _imgui.GetWindowViewport = (delegate* unmanaged[Cdecl]<nint>)_imgui.Lib.Load("igGetWindowViewport");
			_imgui.SetCursorPosX = (delegate* unmanaged[Cdecl]<float, void>)_imgui.Lib.Load("igSetCursorPosX");
			_imgui.GetContentRegionAvail = (delegate* unmanaged[Cdecl]<out Vector2, void>)_imgui.Lib.Load("igGetContentRegionAvail");
			_imgui.SetCursorPosY = (delegate* unmanaged[Cdecl]<float, void>)_imgui.Lib.Load("igSetCursorPosY");
			_imgui.Columns = (delegate* unmanaged[Cdecl]<int, string, bool, void>)_imgui.Lib.Load("igColumns");
			_imgui.SetColumnWidth = (delegate* unmanaged[Cdecl]<int, float, void>)_imgui.Lib.Load("igSetColumnWidth");
			_imgui.NextColumn = (delegate* unmanaged[Cdecl]<void>)_imgui.Lib.Load("igNextColumn");
			_imgui.InputText = (delegate* unmanaged[Cdecl]<string, byte*, ulong, int, InputTextCallback, void*, bool>)_imgui.Lib.Load("igInputText");
			_imgui.PushItemWidth = (delegate* unmanaged[Cdecl]<float, void>)_imgui.Lib.Load("igPushItemWidth");
			_imgui.PopItemWidth = (delegate* unmanaged[Cdecl]<void>)_imgui.Lib.Load("igPopItemWidth");
			_imgui.ShowDemoWindow = (delegate* unmanaged[Cdecl]<bool*, void>)_imgui.Lib.Load("igShowDemoWindow");
			_imgui.InputTextWithHint = (delegate* unmanaged[Cdecl]<string, string, byte*, ulong, int, InputTextCallback, void*, bool>)_imgui.Lib.Load("igInputTextWithHint");
			_imgui.PopFont = (delegate* unmanaged[Cdecl]<void>)_imgui.Lib.Load("igPopFont");
			_imgui.DragFloat = (delegate* unmanaged[Cdecl]<string, float*, float, float, float, string, int, bool>)_imgui.Lib.Load("igDragFloat");
			_imgui.GetWindowDrawList = (delegate* unmanaged[Cdecl]<nint>)_imgui.Lib.Load("igGetWindowDrawList");
			_imgui.GetCursorScreenPos = (delegate* unmanaged[Cdecl]<out Vector2, void>)_imgui.Lib.Load("igGetCursorScreenPos");
			_imgui.PushID_Str = (delegate* unmanaged[Cdecl]<string, void>)_imgui.Lib.Load("igPushID_Str");
			_imgui.PopID = (delegate* unmanaged[Cdecl]<void>)_imgui.Lib.Load("igPopID");
            _imgui.GetCursorPos = (delegate* unmanaged[Cdecl]<out Vector2, void>)_imgui.Lib.Load("igGetCursorPos");

            // imgui end (for code gen)

            _helper = new Helper();
            if (Core.Application.Instance.Specification.OtherImplementations == null)
                _helper.Lib = new Library(Core.Application.Instance.Specification.OtherImplementationsName + ".dll");
            else
                _helper.Lib = new Library(Core.Application.Instance.Specification.OtherImplementationsName, 
                    Core.Application.Instance.Specification.OtherImplementations);
            _helper.StbiSetFlipVerticallyOnLoad = (delegate* unmanaged[Cdecl]<int, void>)_helper.Lib.Load("stbi_set_flip_vertically_on_load");
            _helper.StbiLoad = (delegate* unmanaged[Cdecl]<string, out int, out int, out int, int, byte*>)_helper.Lib.Load("stbi_load");
            _helper.StbiFree = (delegate* unmanaged[Cdecl]<nint, void>)_helper.Lib.Load("stbi_image_free");

            _helper.ImGuiGetImViewportID = (delegate* unmanaged[Cdecl]<nint, uint>)_helper.Lib.Load("GetImViewportID");
            _helper.ImGuiGetImViewportPos = (delegate* unmanaged[Cdecl]<nint, Vector2*>)_helper.Lib.Load("GetImViewportPos");
            _helper.ImGuiGetImViewportSize = (delegate* unmanaged[Cdecl]<nint, Vector2*>)_helper.Lib.Load("GetImViewportSize");
            _helper.ImGuiSetMinWinSizeX = (delegate* unmanaged[Cdecl]<float, void>)_helper.Lib.Load("SetMinWinSizeX");
            _helper.ImGuiGetMinWinSizeX = (delegate* unmanaged[Cdecl]<float>)_helper.Lib.Load("GetMinWinSizeX");
            _helper.ImGuiScrollToBottom = (delegate* unmanaged[Cdecl]<void>)_helper.Lib.Load("ScrollToBottom");
            _helper.ImGuiAlignText = (delegate* unmanaged[Cdecl]<float, void>)_helper.Lib.Load("AlignText");
            _helper.ImGuiIOGetDisplaySize = (delegate* unmanaged[Cdecl]<Vector2*>)_helper.Lib.Load("IOGetDisplaySize");
            _helper.ImGuiPushFont = (delegate* unmanaged[Cdecl]<bool, float, byte*, int, bool>)_helper.Lib.Load("PushFont");

            _helper.CenterWindow = (delegate* unmanaged[Cdecl]<nint, bool>)_helper.Lib.Load("CenterWindow");

            _helper.ImGuiSetCurrentWindow = (delegate* unmanaged[Cdecl]<nint, void>)_helper.Lib.Load("ImGui_ImplGlfw_SetCurrentWindow");
            _helper.ImGuiSetDrawCallback = (delegate* unmanaged[Cdecl]<delegate* unmanaged[Cdecl]<nint, Vector2*, Vector2*, int, ulong, Vector4*, nint, uint, uint, void>, void>)_helper.Lib.Load("ImGui_ImplOpenGL3_AddDrawCallback");
            _helper.ImGuiForceInstallCallbacks = (delegate* unmanaged[Cdecl]<nint, void>)_helper.Lib.Load("ImGui_ImplGlfw_InstallCallbacks_Force");
            _helper.ImGuiRestoreCallbacks = (delegate* unmanaged[Cdecl]<nint, void>)_helper.Lib.Load("ImGui_ImplGlfw_RestoreCallbacks");

            _helper.ImDrawVert_GetPos = (delegate* unmanaged[Cdecl]<nint, out Vector2, void>)_helper.Lib.Load("ImDrawVert_Getpos");
            _helper.ImDrawVert_SetPos = (delegate* unmanaged[Cdecl]<nint, ref Vector2, void>)_helper.Lib.Load("ImDrawVert_Setpos");
            _helper.ImDrawVert_GetUv = (delegate* unmanaged[Cdecl]<nint, out Vector2, void>)_helper.Lib.Load("ImDrawVert_Getuv");
            _helper.ImDrawVert_SetUv = (delegate* unmanaged[Cdecl]<nint, ref Vector2, void>)_helper.Lib.Load("ImDrawVert_Setuv");
            _helper.ImDrawVert_GetCol = (delegate* unmanaged[Cdecl]<nint, out uint, void>)_helper.Lib.Load("ImDrawVert_Getcol");
            _helper.ImDrawVert_SetCol = (delegate* unmanaged[Cdecl]<nint, uint, void>)_helper.Lib.Load("ImDrawVert_Setcol");

            _helper.ImDrawCmd_GetClipRect = (delegate* unmanaged[Cdecl]<nint, out Vector4, void>)_helper.Lib.Load("ImDrawCmd_GetClipRect");
            _helper.ImDrawCmd_SetClipRect = (delegate* unmanaged[Cdecl]<nint, ref Vector4, void>)_helper.Lib.Load("ImDrawCmd_SetClipRect");
            _helper.ImDrawCmd_GetTextureID = (delegate* unmanaged[Cdecl]<nint, out nint, void>)_helper.Lib.Load("ImDrawCmd_GetTextureId");
            _helper.ImDrawCmd_SetTextureID = (delegate* unmanaged[Cdecl]<nint, nint, void>)_helper.Lib.Load("ImDrawCmd_SetTextureId");
            _helper.ImDrawCmd_GetVtxOffset = (delegate* unmanaged[Cdecl]<nint, out uint, void>)_helper.Lib.Load("ImDrawCmd_GetVtxOffset");
            _helper.ImDrawCmd_SetVtxOffset = (delegate* unmanaged[Cdecl]<nint, uint, void>)_helper.Lib.Load("ImDrawCmd_SetVtxOffset");
            _helper.ImDrawCmd_GetIdxOffset = (delegate* unmanaged[Cdecl]<nint, out uint, void>)_helper.Lib.Load("ImDrawCmd_GetIdxOffset");
            _helper.ImDrawCmd_SetIdxOffset = (delegate* unmanaged[Cdecl]<nint, uint, void>)_helper.Lib.Load("ImDrawCmd_SetIdxOffset");
            _helper.ImDrawCmd_GetElemCount = (delegate* unmanaged[Cdecl]<nint, out uint, void>)_helper.Lib.Load("ImDrawCmd_GetElemCount");
            _helper.ImDrawCmd_SetElemCount = (delegate* unmanaged[Cdecl]<nint, uint, void>)_helper.Lib.Load("ImDrawCmd_SetElemCount");
            _helper.ImDrawCmd_GetUserCallback = (delegate* unmanaged[Cdecl]<nint, out delegate* unmanaged[Cdecl]<DrawList*, DrawCommand*, void>, void>)_helper.Lib.Load("ImDrawCmd_GetUserCallback");
            _helper.ImDrawCmd_SetUserCallback = (delegate* unmanaged[Cdecl]<nint, delegate* unmanaged[Cdecl]<DrawList*, DrawCommand*, void>, void>)_helper.Lib.Load("ImDrawCmd_SetUserCallback");
            _helper.ImDrawCmd_GetUserCallbackData = (delegate* unmanaged[Cdecl]<nint, void*>)_helper.Lib.Load("ImDrawCmd_GetUserCallbackData");
            _helper.ImDrawCmd_SetUserCallbackData = (delegate* unmanaged[Cdecl]<nint, void*, void>)_helper.Lib.Load("ImDrawCmd_SetUserCallbackData");

            _helper.DrawList_GetCmdBuffer = (delegate* unmanaged[Cdecl]<nint, out int, void**>)_helper.Lib.Load("DrawList_GetCmdBuffer");
            _helper.DrawList_SetCmdBuffer = (delegate* unmanaged[Cdecl]<nint, void**, int, void>)_helper.Lib.Load("DrawList_SetCmdBuffer");

            _imgui.ImDrawList_PushClipRect = (delegate* unmanaged[Cdecl]<nint, Vector2*, Vector2*, bool, void>)_helper.Lib.Load("DrawList_PushClipRect");
            _imgui.ImDrawList_PushClipRectFullScreen = (delegate* unmanaged[Cdecl]<nint, void>)_helper.Lib.Load("DrawList_PushClipRectFullScreen");
            _imgui.ImDrawList_PopClipRect = (delegate* unmanaged[Cdecl]<nint, void>)_helper.Lib.Load("DrawList_PopClipRect");
            _imgui.ImDrawList_PushTextureID = (delegate* unmanaged[Cdecl]<nint, nint, void>)_helper.Lib.Load("DrawList_PushTextureID");
            _imgui.ImDrawList_PopTextureID = (delegate* unmanaged[Cdecl]<nint, void>)_helper.Lib.Load("DrawList_PopTextureID");
            _imgui.ImDrawList_GetClipRectMin = (delegate* unmanaged[Cdecl]<out Vector2, nint, void>)_helper.Lib.Load("DrawList_GetClipRectMin");
            _imgui.ImDrawList_GetClipRectMax = (delegate* unmanaged[Cdecl]<out Vector2, nint, void>)_helper.Lib.Load("DrawList_GetClipRectMax");
            _imgui.ImDrawList_AddLine = (delegate* unmanaged[Cdecl]<nint, Vector2*, Vector2*, uint, float, void>)_helper.Lib.Load("DrawList_AddLine");
            _imgui.ImDrawList_AddRect = (delegate* unmanaged[Cdecl]<nint, Vector2*, Vector2*, uint, float, int, float, void>)_helper.Lib.Load("DrawList_AddRect");
            _imgui.ImDrawList_AddRectFilled = (delegate* unmanaged[Cdecl]<nint, Vector2*, Vector2*, uint, float, int, void>)_helper.Lib.Load("DrawList_AddRectFilled");
            _imgui.ImDrawList_AddRectFilledMultiColor = (delegate* unmanaged[Cdecl]<nint, Vector2*, Vector2*, uint, uint, uint, uint, void>)_helper.Lib.Load("DrawList_AddRectFilledMultiColor");
            _imgui.ImDrawList_AddQuad = (delegate* unmanaged[Cdecl]<nint, Vector2*, Vector2*, Vector2*, Vector2*, uint, void>)_helper.Lib.Load("DrawList_AddQuad");

            _imgui.BeginTable = (delegate* unmanaged[Cdecl]<string, int, int, Vector2*, float, bool>)_helper.Lib.Load("BeginTable");
        }

        internal static unsafe void GLInit()
        {
            _gl = new GL();

            _gl.GetString = (delegate* unmanaged[Cdecl]<int, string>)glfwGetProcAddress("glGetString");
            _gl.Clear = (delegate* unmanaged[Cdecl]<int, void>)glfwGetProcAddress("glClear");
            _gl.ClearColour = (delegate* unmanaged[Cdecl]<float, float, float, float, void>)glfwGetProcAddress("glClearColor");
            _gl.GenBuffers = (delegate* unmanaged[Cdecl]<int, uint*, void>)glfwGetProcAddress("glGenBuffers");
            _gl.BindBuffer = (delegate* unmanaged[Cdecl]<int, uint, void>)glfwGetProcAddress("glBindBuffer");
            _gl.BufferData = (delegate* unmanaged[Cdecl]<int, nint, void*, int, void>)glfwGetProcAddress("glBufferData");
            _gl.Begin = (delegate* unmanaged[Cdecl]<int, void>)glfwGetProcAddress("glBegin");
            _gl.End = (delegate* unmanaged[Cdecl]<void>)glfwGetProcAddress("glEnd");
            _gl.Vertex2f = (delegate* unmanaged[Cdecl]<float, float, void>)glfwGetProcAddress("glVertex2f");
            _gl.BindTexture = (delegate* unmanaged[Cdecl]<uint, uint, void>)glfwGetProcAddress("glBindTexture");
            _gl.BlendColor = (delegate* unmanaged[Cdecl]<int, uint, void>)glfwGetProcAddress("glBlendColor");
            _gl.GenTextures = (delegate* unmanaged[Cdecl]<int, uint*, void>)glfwGetProcAddress("glGenTextures");
            _gl.TexImage2D = (delegate* unmanaged[Cdecl]<int, int, int, uint, uint, int, int, int, nint, void>)glfwGetProcAddress("glTexImage2D");
            _gl.TextureParameteri = (delegate* unmanaged[Cdecl]<uint, int, int, void>)glfwGetProcAddress("glTextureParameteri");
            _gl.PixelStorei = (delegate* unmanaged[Cdecl]<int, int, void>)glfwGetProcAddress("glPixelStorei");
			_gl.FramebufferTexture2D = (delegate* unmanaged[Cdecl]<uint, uint, uint, uint, int, void>)glfwGetProcAddress("glFramebufferTexture2D");
			_gl.TexImage2DMultisample = (delegate* unmanaged[Cdecl]<uint, int, int, int, int, bool, void>)glfwGetProcAddress("glTexImage2DMultisample");
			_gl.DeleteFramebuffers = (delegate* unmanaged[Cdecl]<int, uint*, void>)glfwGetProcAddress("glDeleteFramebuffers");
			_gl.DeleteTextures = (delegate* unmanaged[Cdecl]<int, uint*, void>)glfwGetProcAddress("glDeleteTextures");
			_gl.Scissor = (delegate* unmanaged[Cdecl]<int, int, int, int, void>)glfwGetProcAddress("glScissor");
			_gl.DrawElements = (delegate* unmanaged[Cdecl]<uint, int, uint, nint, void>)glfwGetProcAddress("glDrawElements");

            // gl end (for code gen)
        }

#if WINDOWS
        [DllImport("user32.dll")]
        private static extern nint SetParent(nint child, nint parent);

        [DllImport("user32.dll")]
        private static extern int GetWindowLongA(nint window, int index);

        [DllImport("user32.dll")]
        private static extern int SetWindowLongA(nint window, int index, int value);
#endif

        public unsafe void Update()
        {
            if (!Frozen)
            {
                _glfw.PollEvents();
                m_Context.SwapBuffers();
            }
        }

        private unsafe void Update(bool ignoreFreeze)
        {
            if (ignoreFreeze)
            {
                _gl.Clear((int)GL_COLOR_BUFFER_BIT | (int)GL_DEPTH_BUFFER_BIT);

                _glfw.PollEvents();
                m_Context.SwapBuffers();
            }
        }

        public unsafe void Dispose()
        {
            _glfw.DestroyWindow(m_WindowPtr);

            s_WindowCount--;

            if (m_FrozeOtherWindows)
            {
                Core.Application.Instance.AddToMainThreadQueue(() => { s_Frozen = false; });
            }

            if (m_SecondaryWindow)
            {
                ImGuiImpl.DrawingWindow = Core.Application.Instance.Window.Handle;
                _helper.ImGuiSetCurrentWindow(Core.Application.Instance.Window.Handle);
                Core.Application.Instance.Window.m_Context.MakeCurrent();

                _helper.ImGuiRestoreCallbacks(Core.Application.Instance.Window.Handle);
                _helper.ImGuiForceInstallCallbacks(Core.Application.Instance.Window.Handle);
                //_helper.ImGuiForceInstallCallbacks(Core.Application.Instance.Window.Handle);
            }

            if (s_WindowCount == 0)
            {
                s_Init = false;
                _glfw.Terminate();

                _glfw.Lib.Dispose();
                _glad.Lib.Dispose();
                _imgui.Lib.Dispose();
                _helper.Lib.Dispose();

                Library.Cleanup();
            }
        }

        public static unsafe void UpdateSecondWindow(Window previous, Window second)
        {
            second.m_Context.MakeCurrent();

            second.Update(true);

            previous.m_Context.MakeCurrent();
        }

        public static unsafe void SecondWindowBeginUI(Window previous, Window second)
        {
            _gl.Clear(0x00004000 | 0x00000100);

            ImGuiImpl.ImGuiBegin();
        }

        public static unsafe void SecondWindowEndUI(Window previous, Window second)
        {
            second.m_Context.MakeCurrent();

            ImGuiImpl.ImGuiEnd();

            _glfw.SwapBuffers(second.Handle);
            _glfw.PollEvents();
        }
    }
 
}

































