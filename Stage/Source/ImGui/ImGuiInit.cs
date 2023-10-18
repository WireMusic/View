using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Stage.Core;
using Stage.Rendering;
using static Stage.Rendering.GLConstants;

namespace Stage.ImGui
{
    internal static unsafe class ImGuiImpl
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private delegate void InitDelegate(nint window, byte* fontData, int dataSize, byte* boldFontData, int boldFontDataSize, float fontSize, float* defaultFontSizes, int dfsSize, float* boldFontSizes, int bfsSize, bool loadFontSizes);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private delegate void ImGuiMethodDelegate();

        private static InitDelegate Init;
        private static ImGuiMethodDelegate Shutdown;
        private static ImGuiMethodDelegate Begin;
        private static ImGuiMethodDelegate End;

        internal static nint DrawingWindow { get; set; } = Core.Application.Instance.Window.Handle;

        internal static bool HasBegun = false;

        public static unsafe void InitFor(Window window, byte[] fontData, byte[] boldFontData, float defaultFontSize, float[] defaultFontSizes, float[] boldFontSizes, bool loadFontOnNewSize)
        {
            if (Init == null)
                Init = Marshal.GetDelegateForFunctionPointer<InitDelegate>(_helper.Lib.Load("ImGuiInit"));
            if (Shutdown == null)
                Shutdown = Marshal.GetDelegateForFunctionPointer<ImGuiMethodDelegate>(_helper.Lib.Load("ImGuiShutdown"));
            if (Begin == null)
                Begin = Marshal.GetDelegateForFunctionPointer<ImGuiMethodDelegate>(_helper.Lib.Load("ImGuiBegin"));
            if (End == null)
                End = Marshal.GetDelegateForFunctionPointer<ImGuiMethodDelegate>(_helper.Lib.Load("ImGuiEnd"));

            fixed (byte* data = fontData, boldData = boldFontData)
            {
                fixed (float* dfsPtr = defaultFontSizes, bfsPtr = boldFontSizes) 
                {
                    Init(
                        window.Handle,
                        data, 
                        fontData.Length, 
                        boldData, 
                        boldFontData.Length, 
                        defaultFontSize, 
                        dfsPtr, 
                        defaultFontSizes.Length,
                        bfsPtr,
                        boldFontSizes.Length,
                        loadFontOnNewSize
                    );
                }
            }

            _helper.ImGuiSetDrawCallback(&RenderDrawData);
        }

        public static void ImGuiBegin() 
        {
            Begin();
            HasBegun = true;
        }

        public static void ImGuiEnd()
        {
            End();
            HasBegun = false;
        }

        public static void ImGuiShutdown() => Shutdown();

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static unsafe void RenderDrawData(nint pcmd, Vector2* clip_off, Vector2* clip_scale, int fb_height,
            ulong sizeofImDrawIdx, Vector4* pcmd_ClipRect, nint pcmd_getTexID, uint pcmd_elemCount, uint pcmd_IdxOffset)
        {
            Vector2 clipMin = new Vector2((pcmd_ClipRect->X - clip_off->X) * clip_scale->X, (pcmd_ClipRect->Y - clip_off->Y) * clip_scale->Y);
            Vector2 clipMax = new Vector2((pcmd_ClipRect->Z - clip_off->X) * clip_scale->X, (pcmd_ClipRect->W - clip_off->Y) * clip_scale->Y); ;
            if (clipMax.X <= clipMin.X || clipMax.Y <= clipMin.Y)
                return;

            _gl.Scissor((int)clipMin.X, (int)((float)fb_height - clipMax.Y), (int)(clipMax.X - clipMin.X), (int)(clipMax.Y - clipMin.Y));

            _gl.BindTexture(GL_TEXTURE_2D, (uint)(long)pcmd_getTexID);
            _gl.DrawElements(GL_TRIANGLES, (int)pcmd_elemCount, sizeofImDrawIdx == 2 ? GL_UNSIGNED_SHORT : GL_UNSIGNED_INT, (nint)(pcmd_IdxOffset * sizeofImDrawIdx));
        }
    }
}
