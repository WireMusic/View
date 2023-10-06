using System.Runtime.InteropServices;

using Stage.UIModule;

namespace Stage.Core
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    unsafe struct GLFW
    {
        public Library Lib;

        public delegate* unmanaged[Cdecl]<int> Init;
        public delegate* unmanaged[Cdecl]<void> Terminate;
        public delegate* unmanaged[Cdecl]<int, int, string, nint, nint, nint> CreateWindow;
        public delegate* unmanaged[Cdecl]<nint, void> DestroyWindow;
        public delegate* unmanaged[Cdecl]<nint, bool> WindowShouldClose;
        public delegate* unmanaged[Cdecl]<void> PollEvents;
        public delegate* unmanaged[Cdecl]<nint, void> SwapBuffers;
        public delegate* unmanaged[Cdecl]<nint, void> MakeContextCurrent;
        public delegate* unmanaged[Cdecl]<int, void> SwapInterval;
        public delegate* unmanaged[Cdecl]<nint, string, void> SetClipboardString;
        public delegate* unmanaged[Cdecl]<nint, string> GetClipboardString;
        public delegate* unmanaged[Cdecl]<delegate* unmanaged[Cdecl]<int, string, void>, delegate* unmanaged[Cdecl]<int, string, void>> SetErrorCallback;
        public delegate* unmanaged[Cdecl]<int, nint> CreateStandardCursor;
        public delegate* unmanaged[Cdecl]<nint, int> GetError;
        public delegate* unmanaged[Cdecl]<nint, nint, nint> SetWindowFocusCallback;
        public delegate* unmanaged[Cdecl]<nint, nint, nint> SetCursorEnterCallback;
        public delegate* unmanaged[Cdecl]<nint, nint, nint> SetCursorPosCallback;
        public delegate* unmanaged[Cdecl]<nint, nint, nint> SetMouseButtonCallback;
        public delegate* unmanaged[Cdecl]<nint, nint, nint> SetScrollCallback;
        public delegate* unmanaged[Cdecl]<nint, nint, nint> SetKeyCallback;
        public delegate* unmanaged[Cdecl]<nint, nint, nint> SetCharCallback;
        public delegate* unmanaged[Cdecl]<nint, nint, nint> SetMonitorCallback;
        public delegate* unmanaged[Cdecl]<nint, nint> GetWin32Window;
		public delegate* unmanaged[Cdecl]<nint, int, int> GetKey;
		public delegate* unmanaged[Cdecl]<nint, int, int> GetMouseButton;
		public delegate* unmanaged[Cdecl]<nint, out double, out double, void> GetCursorPos;
		public delegate* unmanaged[Cdecl]<int, int, void> WindowHint;
        public delegate* unmanaged[Cdecl]<nint, int, int, void> SetWindowPos;
        public delegate* unmanaged[Cdecl]<nint, out int, out int, void> GetWindowSize;

        // glfw end (code gen)
    }

    unsafe struct Glad
    {
        public Library Lib;

        public delegate* unmanaged[Cdecl]<delegate* unmanaged[Cdecl]<char*, int>, int> LoadGLLoader;

        // glad end (code gen)
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    unsafe struct ImGui
    {
        public Library Lib;

		public delegate* unmanaged[Cdecl]<string, bool*, int, bool> Begin;
		public delegate* unmanaged[Cdecl]<void> End;
		public delegate* unmanaged[Cdecl]<string, nint, Vector2, Vector2, Vector2, Vector4, Vector4, bool> ImageButton;
		public delegate* unmanaged[Cdecl]<string, void> Text;
		public delegate* unmanaged[Cdecl]<int, float, void> PushStyleVar_Float;
		public delegate* unmanaged[Cdecl]<int, Vector2, void> PushStyleVar_Vec2;
		public delegate* unmanaged[Cdecl]<int, void> PopStyleVar;
		public delegate* unmanaged[Cdecl]<nint> GetMainViewport;
        public delegate* unmanaged[Cdecl]<Vector2, int, Vector2, void> SetNextWindowPos;
        public delegate* unmanaged[Cdecl]<Vector2, int, void> SetNextWindowSize;
        public delegate* unmanaged[Cdecl]<uint, void> SetNextWindowViewport;
		public delegate* unmanaged[Cdecl]<string, uint> GetID;
		public delegate* unmanaged[Cdecl]<uint, Vector2, int, nint, uint> DockSpace;
		public delegate* unmanaged[Cdecl]<bool> BeginMenuBar;
		public delegate* unmanaged[Cdecl]<void> EndMenuBar;
		public delegate* unmanaged[Cdecl]<string, bool, bool> BeginMenu;
		public delegate* unmanaged[Cdecl]<void> EndMenu;
		public delegate* unmanaged[Cdecl]<string, string, bool, bool, bool> MenuItem_Bool;
		public delegate* unmanaged[Cdecl]<float, float, void> SameLine;
		public delegate* unmanaged[Cdecl]<string, Vector2, bool> Button;
		public delegate* unmanaged[Cdecl]<string, Vector2, int, bool> ButtonEx;
		public delegate* unmanaged[Cdecl]<string, bool*, bool> Checkbox;
		public delegate* unmanaged[Cdecl]<int, uint, void> PushStyleColor_U32;
		public delegate* unmanaged[Cdecl]<int, Vector4, void> PushStyleColor_Vec4;
		public delegate* unmanaged[Cdecl]<int, void> PopStyleColor;
		public delegate* unmanaged[Cdecl]<string, int, int, Vector2*, float, bool> BeginTable;
		public delegate* unmanaged[Cdecl]<out Vector2, string, nint, bool, float, void> CalcTextSize;
		public delegate* unmanaged[Cdecl]<void> EndTable;
		public delegate* unmanaged[Cdecl]<string, int, float, uint, void> TableSetupColumn;
		public delegate* unmanaged[Cdecl]<out Vector2, void> GetWindowSize;
		public delegate* unmanaged[Cdecl]<int, float, void> TableNextRow;
		public delegate* unmanaged[Cdecl]<int, uint, int, void> TableSetBgColor;
		public delegate* unmanaged[Cdecl]<Vector4, uint> GetColorU32_Vec4;
		public delegate* unmanaged[Cdecl]<int, bool> TableSetColumnIndex;
		public delegate* unmanaged[Cdecl]<float> GetScrollY;
		public delegate* unmanaged[Cdecl]<float> GetScrollMaxY;
		public delegate* unmanaged[Cdecl]<string, int, void> OpenPopup;
		public delegate* unmanaged[Cdecl]<string, bool*, int, bool> BeginPopupModal;
		public delegate* unmanaged[Cdecl]<void> EndPopup;
		public delegate* unmanaged[Cdecl]<void> CloseCurrentPopup;
		public delegate* unmanaged[Cdecl]<out Vector2, nint, void> ImGuiViewport_GetCenter;
		public delegate* unmanaged[Cdecl]<nint> GetWindowViewport;
		public delegate* unmanaged[Cdecl]<float, void> SetCursorPosX;
		public delegate* unmanaged[Cdecl]<out Vector2, void> GetContentRegionAvail;
		public delegate* unmanaged[Cdecl]<float, void> SetCursorPosY;
		public delegate* unmanaged[Cdecl]<int, string, bool, void> Columns;
		public delegate* unmanaged[Cdecl]<int, float, void> SetColumnWidth;
		public delegate* unmanaged[Cdecl]<void> NextColumn;
		public delegate* unmanaged[Cdecl]<string, byte*, ulong, int, InputTextCallback, void*, bool> InputText;
		public delegate* unmanaged[Cdecl]<float, void> PushItemWidth;
		public delegate* unmanaged[Cdecl]<void> PopItemWidth;
		public delegate* unmanaged[Cdecl]<bool*, void> ShowDemoWindow;
		public delegate* unmanaged[Cdecl]<string, string, byte*, ulong, int, InputTextCallback, void*, bool> InputTextWithHint;
		public delegate* unmanaged[Cdecl]<void> PopFont;
		public delegate* unmanaged[Cdecl]<string, float*, float, float, float, string, int, bool> DragFloat;
		public delegate* unmanaged[Cdecl]<nint, Vector2*, Vector2*, bool, void> ImDrawList_PushClipRect;
		public delegate* unmanaged[Cdecl]<nint, void> ImDrawList_PushClipRectFullScreen;
		public delegate* unmanaged[Cdecl]<nint, void> ImDrawList_PopClipRect;
		public delegate* unmanaged[Cdecl]<nint, nint, void> ImDrawList_PushTextureID;
		public delegate* unmanaged[Cdecl]<nint, void> ImDrawList_PopTextureID;
		public delegate* unmanaged[Cdecl]<out Vector2, nint, void> ImDrawList_GetClipRectMin;
		public delegate* unmanaged[Cdecl]<out Vector2, nint, void> ImDrawList_GetClipRectMax;
		public delegate* unmanaged[Cdecl]<nint, Vector2*, Vector2*, uint, float, void> ImDrawList_AddLine;
		public delegate* unmanaged[Cdecl]<nint, Vector2*, Vector2*, uint, float, int, float, void> ImDrawList_AddRect;
		public delegate* unmanaged[Cdecl]<nint, Vector2*, Vector2*, uint, float, int, void> ImDrawList_AddRectFilled;
		public delegate* unmanaged[Cdecl]<nint, Vector2*, Vector2*, uint, uint, uint, uint, void> ImDrawList_AddRectFilledMultiColor;
		public delegate* unmanaged[Cdecl]<nint, Vector2*, Vector2*, Vector2*, Vector2*, uint, void> ImDrawList_AddQuad;
		public delegate* unmanaged[Cdecl]<nint> GetWindowDrawList;
		public delegate* unmanaged[Cdecl]<out Vector2, void> GetCursorScreenPos;
		public delegate* unmanaged[Cdecl]<string, void> PushID_Str;
		public delegate* unmanaged[Cdecl]<void> PopID;
        public delegate* unmanaged[Cdecl]<out Vector2, void> GetCursorPos;

        // imgui end (code gen)
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    unsafe struct GL
    {
        public delegate* unmanaged[Cdecl]<int, string> GetString;
        public delegate* unmanaged[Cdecl]<int, void> Clear;
        public delegate* unmanaged[Cdecl]<float, float, float, float, void> ClearColour;
        public delegate* unmanaged[Cdecl]<int, uint*, void> GenBuffers;
        public delegate* unmanaged[Cdecl]<int, uint, void> BindBuffer;
        public delegate* unmanaged[Cdecl]<int, nint, void*, int, void> BufferData;
        public delegate* unmanaged[Cdecl]<int, void> Begin;
        public delegate* unmanaged[Cdecl]<void> End;
        public delegate* unmanaged[Cdecl]<float, float, void> Vertex2f;
		public delegate* unmanaged[Cdecl]<uint, uint, void> BindTexture;
		public delegate* unmanaged[Cdecl]<int, uint, void> BlendColor;
		public delegate* unmanaged[Cdecl]<int, uint*, void> GenTextures;
		public delegate* unmanaged[Cdecl]<int, int, int, uint, uint, int, int, int, nint, void> TexImage2D;
		public delegate* unmanaged[Cdecl]<uint, int, int, void> TextureParameteri;
		public delegate* unmanaged[Cdecl]<int, int, void> PixelStorei;
		public delegate* unmanaged[Cdecl]<uint, uint, uint, uint, int, void> FramebufferTexture2D;
		public delegate* unmanaged[Cdecl]<uint, int, int, int, int, bool, void> TexImage2DMultisample;
		public delegate* unmanaged[Cdecl]<int, uint*, void> DeleteFramebuffers;
		public delegate* unmanaged[Cdecl]<int, uint*, void> DeleteTextures;
		public delegate* unmanaged[Cdecl]<int, int, int, int, void> Scissor;
		public delegate* unmanaged[Cdecl]<uint, int, uint, nint, void> DrawElements;

        // gl end (code gen)
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    unsafe struct Helper
    {
        public Library Lib;

        public delegate* unmanaged[Cdecl]<int, void> StbiSetFlipVerticallyOnLoad;
        public delegate* unmanaged[Cdecl]<string, out int, out int, out int, int, byte*> StbiLoad;
        public delegate* unmanaged[Cdecl]<nint, void> StbiFree;

        public delegate* unmanaged[Cdecl]<nint, uint> ImGuiGetImViewportID;
        public delegate* unmanaged[Cdecl]<nint, Vector2*> ImGuiGetImViewportPos;
        public delegate* unmanaged[Cdecl]<nint, Vector2*> ImGuiGetImViewportSize;
        public delegate* unmanaged[Cdecl]<float, void> ImGuiSetMinWinSizeX;
        public delegate* unmanaged[Cdecl]<float> ImGuiGetMinWinSizeX;
        public delegate* unmanaged[Cdecl]<void> ImGuiScrollToBottom;
        public delegate* unmanaged[Cdecl]<float, void> ImGuiAlignText;
        public delegate* unmanaged[Cdecl]<Vector2*> ImGuiIOGetDisplaySize;
        public delegate* unmanaged[Cdecl]<bool, float, byte*, int, bool> ImGuiPushFont;

        public delegate* unmanaged[Cdecl]<nint, bool> CenterWindow;

        public delegate* unmanaged[Cdecl]<nint, void> ImGuiSetCurrentWindow;
        public delegate* unmanaged[Cdecl]<
            delegate* unmanaged[Cdecl]<nint, Vector2*, Vector2*, int, ulong, Vector4*, nint, uint, uint, void>,
            void> ImGuiSetDrawCallback;
        public delegate* unmanaged[Cdecl]<nint, void> ImGuiForceInstallCallbacks;
        public delegate* unmanaged[Cdecl]<nint, void> ImGuiRestoreCallbacks;

        public delegate* unmanaged[Cdecl]<nint, out Vector2, void> ImDrawVert_GetPos;
        public delegate* unmanaged[Cdecl]<nint, ref Vector2, void> ImDrawVert_SetPos;
        public delegate* unmanaged[Cdecl]<nint, out Vector2, void> ImDrawVert_GetUv;
        public delegate* unmanaged[Cdecl]<nint, ref Vector2, void> ImDrawVert_SetUv;
        public delegate* unmanaged[Cdecl]<nint, out uint, void> ImDrawVert_GetCol;
        public delegate* unmanaged[Cdecl]<nint, uint, void> ImDrawVert_SetCol;

        public delegate* unmanaged[Cdecl]<nint, out Vector4, void> ImDrawCmd_GetClipRect;
        public delegate* unmanaged[Cdecl]<nint, ref Vector4, void> ImDrawCmd_SetClipRect;
        public delegate* unmanaged[Cdecl]<nint, out nint, void> ImDrawCmd_GetTextureID;
        public delegate* unmanaged[Cdecl]<nint, nint, void> ImDrawCmd_SetTextureID;
        public delegate* unmanaged[Cdecl]<nint, out uint, void> ImDrawCmd_GetVtxOffset;
        public delegate* unmanaged[Cdecl]<nint, uint, void> ImDrawCmd_SetVtxOffset;
        public delegate* unmanaged[Cdecl]<nint, out uint, void> ImDrawCmd_GetIdxOffset;
        public delegate* unmanaged[Cdecl]<nint, uint, void> ImDrawCmd_SetIdxOffset;
        public delegate* unmanaged[Cdecl]<nint, out uint, void> ImDrawCmd_GetElemCount;
        public delegate* unmanaged[Cdecl]<nint, uint, void> ImDrawCmd_SetElemCount;
        public delegate* unmanaged[Cdecl]<nint, out delegate* unmanaged[Cdecl]<DrawList*, DrawCommand*, void>, void> ImDrawCmd_GetUserCallback;
        public delegate* unmanaged[Cdecl]<nint, delegate* unmanaged[Cdecl]<DrawList*, DrawCommand*, void>, void> ImDrawCmd_SetUserCallback;
        public delegate* unmanaged[Cdecl]<nint, void*> ImDrawCmd_GetUserCallbackData;
        public delegate* unmanaged[Cdecl]<nint, void*, void> ImDrawCmd_SetUserCallbackData;

        public delegate* unmanaged[Cdecl]<nint, out int, void**> DrawList_GetCmdBuffer;
        public delegate* unmanaged[Cdecl]<nint, void**, int, void> DrawList_SetCmdBuffer;
    }

    internal class Externs
    {
        internal static GLFW _glfw;
        internal static Glad _glad;
        internal static GL _gl;
        internal static ImGui _imgui;
        internal static Helper _helper;
    }
}

















