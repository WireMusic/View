using System;
using System.Collections.Generic;

using Stage.Core;

namespace Stage.UIModule
{
    // TODO
    internal struct DrawList
    {
        private nint m_Ptr;

        public unsafe List<DrawCommand> CmdBuffer
        {
            get
            {
                List<DrawCommand> result = new List<DrawCommand>();
                void** ptrs = _helper.DrawList_GetCmdBuffer(m_Ptr, out int size);

                for (int i = 0; i < size; i++)
                {
                    result.Add(new DrawCommand(new nint(ptrs[i])));
                }

                return result;
            }
            set
            {
                DrawCommand[] arr = (DrawCommand[])typeof(List<DrawCommand>).GetField("_items", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(value);

                if (arr == null)
                    throw new Exception();

                fixed (void** ptr = new void*[arr.Length])
                {
                    for (int i = 0; i < arr.Length; i++)
                    {
                        ptr[i] = (void*)arr[i].m_Ptr;
                    }

                    _helper.DrawList_SetCmdBuffer(m_Ptr, ptr, arr.Length);
                }
            }
        }

        // TODO
        public List<ushort> IdxBuffer
        {
            get
            {
                return new List<ushort>();
            }
            set
            {

            }
        }

        // TODO
        public List<DrawVert> VtxBuffer
        {
            get
            {
                return new List<DrawVert>();
            }
            set
            {

            }
        }

        // TODO
        public DrawListFlags Flags
        {
            get
            {
                return 0;
            }
            set
            {

            }
        }

        public DrawList(nint ptr)
        {
            m_Ptr = ptr;
        }

        public unsafe void PushClipRect(Vector2 clip_rect_min, Vector2 clip_rect_max, bool intersect_with_current_clip_rect = false)
        {
            _imgui.ImDrawList_PushClipRect(m_Ptr, &clip_rect_min, &clip_rect_max, intersect_with_current_clip_rect);
        }

        public unsafe void PushClipRectFullScreen()
        {
            _imgui.ImDrawList_PushClipRectFullScreen(m_Ptr);
        }

        public unsafe void PopClipRect()
        {
            _imgui.ImDrawList_PopClipRect(m_Ptr);
        }

        public unsafe void PushTextureID(nint textureID)
        {
            _imgui.ImDrawList_PushTextureID(m_Ptr, textureID);
        }

        public unsafe void PopTextureID()
        {
            _imgui.ImDrawList_PopTextureID(m_Ptr);
        }

        public unsafe Vector2 GetClipRectMin()
        {
            _imgui.ImDrawList_GetClipRectMin(out Vector2 val, m_Ptr);
            return val;
        }

        public unsafe Vector2 GetClipRectMax()
        {
            _imgui.ImDrawList_GetClipRectMax(out Vector2 val, m_Ptr);
            return val;
        }

        public unsafe void AddLine(Vector2 point1, Vector2 point2, Vector4 colour, float thickness = 1.0f)
        {
            _imgui.ImDrawList_AddLine(m_Ptr, &point1, &point2, _imgui.GetColorU32_Vec4(&colour), thickness);
        }

        public unsafe void AddRect(Vector2 min, Vector2 max, Vector4 colour, float rounding = 0.0f, DrawFlags flags = 0, float thickness = 1.0f)
        {
            _imgui.ImDrawList_AddRect(m_Ptr, &min, &max, _imgui.GetColorU32_Vec4(&colour), rounding, (int)flags, thickness);
        }

        public unsafe void AddRectFilled(Vector2 min, Vector2 max, Vector4 colour, float rounding = 0.0f, DrawFlags flags = 0)
        {
            _imgui.ImDrawList_AddRectFilled(m_Ptr, &min, &max, _imgui.GetColorU32_Vec4(&colour), rounding, (int)flags);
        }

        public unsafe void AddRectFilledMultiColor(Vector2 min, Vector2 max, Vector4 colourTL, Vector4 colourTR, Vector4 colourBR, Vector4 colourBL)
        {
            _imgui.ImDrawList_AddRectFilledMultiColor(m_Ptr, &min, &max, _imgui.GetColorU32_Vec4(&colourTL), _imgui.GetColorU32_Vec4(&colourTR), _imgui.GetColorU32_Vec4(&colourBR), _imgui.GetColorU32_Vec4(&colourBL));
        }

        public unsafe void AddQuad(Vector2 point1, Vector2 point2, Vector2 point3, Vector2 point4, Vector4 colour)
        {
            _imgui.ImDrawList_AddQuad(m_Ptr, &point1, &point2, &point3, &point4, _imgui.GetColorU32_Vec4(&colour));
        }

        public void AddQuadFilled()
        {

        }

        public void AddTriangle()
        {

        }

        public void AddTriangleFilled()
        {

        }

        public void AddCircle()
        {

        }

        public void AddCircleFilled()
        {

        }

        public void AddNgon()
        {

        }

        public void AddNgonFilled()
        {

        }

        public unsafe void AddText(Vector2 pos, string text, Vector4 colour)
        {
            _imgui.ImDrawList_AddText_Vec2(m_Ptr, &pos, UI.GetColourU32(colour), text, null);
        }

        public unsafe void AddText(float textSize, bool bold, Vector2 pos, string text, Vector4 colour, float wrapWidth = 0.0f, Vector4? cpu_fine_clip_rect = null)
        {
            Vector4* cpuPtr = null;
            if (cpu_fine_clip_rect.HasValue)
            {
                Vector4 val = cpu_fine_clip_rect.Value;
                cpuPtr = &val;
            }

            byte[] data = bold ? Application.Instance.Specification.BoldFontData : Application.Instance.Specification.DefaultFontData;
            fixed (byte* ptr = data) 
            {
                _imgui.ImDrawList_AddText_Font(m_Ptr, bold, textSize, ptr, data.Length, &pos, UI.GetColourU32(colour), text, null, wrapWidth, cpuPtr);
            }
        }

        public void AddPolyline()
        {

        }

        public void AddConvexPolyFilled()
        {

        }

        public void AddBezierCubic()
        {

        }

        public void AddBezierQuadratic()
        {

        }

        public void AddImage()
        {

        }

        public void AddImageQuad()
        {

        }

        public void AddImageRounded()
        {

        }

        public void PathClear()
        {

        }

        public void PathLineTo()
        {

        }

        public void PathLineToMergeDuplicate()
        {

        }

        public void PathFillConvex()
        {

        }

        public void PathStroke()
        {

        }

        public void PathArcTo()
        {

        }

        public void PathArcToFast()
        {

        }

        public void PathBezierCubicCurveTo()
        {

        }

        public void PathBezierQuadraticCurveTo()
        {

        }

        public void PathRect()
        {

        }

        public void AddCallback()
        {

        }

        public void AddDrawCmd()
        {

        }

        public void CloneOutput()
        {

        }

        public void ChannelsSplit()
        {

        }

        public void ChannelsMerge()
        {

        }

        public void ChannelsSetCurrent()
        {

        }

        public void PrimReserve()
        {

        }

        public void PrimUnreserve()
        {

        }

        public void PrimRect()
        {

        }

        public void PrimRectUV()
        {

        }

        public void PrimQuadUV()
        {

        }

        public void PrimWriteVtx()
        {

        }

        public void PrimWriteIdx()
        {

        }

        public void PrimVtx()
        {

        }
    }

    public enum DrawFlags
    {
        None = 0,
        Closed = 1 << 0,
        RoundCornersTopLeft = 1 << 4,
        RoundCornersTopRight = 1 << 5,
        RoundCornersBottomLeft = 1 << 6,
        RoundCornersBottomRight = 1 << 7,
        RoundCornersNone = 1 << 8,
        RoundCornersTop = RoundCornersTopLeft | RoundCornersTopRight,
        RoundCornersBottom = RoundCornersBottomLeft | RoundCornersBottomRight,
        RoundCornersLeft = RoundCornersBottomLeft | RoundCornersTopLeft,
        RoundCornersRight = RoundCornersBottomRight | RoundCornersTopRight,
        RoundCornersAll = RoundCornersTopLeft | RoundCornersTopRight | RoundCornersBottomLeft | RoundCornersBottomRight,
    }

    public enum DrawListFlags
    {
        None = 0,
        AntiAliasedLines = 1 << 0,
        AntiAliasedLinesUseTex = 1 << 1,
        AntiAliasedFill = 1 << 2,
        AllowVtxOffset = 1 << 3,
    }

    internal unsafe struct DrawVert
    {
        private nint m_Ptr;

        public Vector2 Pos
        {
            get
            {
                _imgui.ImDrawVert_GetPos(m_Ptr, out Vector2 result);
                return result;
            }
            set
            {
                _imgui.ImDrawVert_SetPos(m_Ptr, ref value);
            }
        }

        public Vector2 Uv
        {
            get
            {
                _imgui.ImDrawVert_GetUv(m_Ptr, out Vector2 result);
                return result;
            }
            set
            {
                _imgui.ImDrawVert_SetUv(m_Ptr, ref value);
            }
        }

        public uint Col
        {
            get
            {
                _imgui.ImDrawVert_GetCol(m_Ptr, out uint result);
                return result;
            }
            set
            {
                _imgui.ImDrawVert_SetCol(m_Ptr, value);
            }
        }

        public DrawVert(nint ptr)
        {
            m_Ptr = ptr;
        }
    }

    internal unsafe struct DrawCommand
    {
        internal nint m_Ptr;

        public Vector4 ClipRect
        {
            get
            {
                _imgui.ImDrawCmd_GetClipRect(m_Ptr, out Vector4 val);
                return val;
            }
            set
            {
                _imgui.ImDrawCmd_SetClipRect(m_Ptr, ref value);
            }
        }

        public nint TextureID
        {
            get
            {
                _imgui.ImDrawCmd_GetTextureID(m_Ptr, out nint result);
                return result;
            }
            set
            {
                _imgui.ImDrawCmd_SetTextureID(m_Ptr, value);
            }
        }

        public uint VtxOffset
        {
            get
            {
                _imgui.ImDrawCmd_GetVtxOffset(m_Ptr, out uint result);
                return result;
            }
            set
            {
                _imgui.ImDrawCmd_SetVtxOffset(m_Ptr, value);
            }
        }

        public uint IdxOffset
        {
            get
            {
                _imgui.ImDrawCmd_GetIdxOffset(m_Ptr, out uint result);
                return result;
            }
            set
            {
                _imgui.ImDrawCmd_SetIdxOffset(m_Ptr, value);
            }
        }

        public uint ElemCount
        {
            get
            {
                _imgui.ImDrawCmd_GetElemCount(m_Ptr, out uint result);
                return result;
            }
            set
            {
                _imgui.ImDrawCmd_SetElemCount(m_Ptr, value);
            }
        }

        public delegate* unmanaged[Cdecl]<DrawList*, DrawCommand*, void> UserCallback
        {
            get
            {
                _imgui.ImDrawCmd_GetUserCallback(m_Ptr, out delegate* unmanaged[Cdecl]<DrawList*, DrawCommand*, void> result);
                return result;
            }
            set
            {
                _imgui.ImDrawCmd_SetUserCallback(m_Ptr, value);
            }
        }

        public void* UserCallbackData
        {
            get
            {
                return _imgui.ImDrawCmd_GetUserCallbackData(m_Ptr);
            }
            set
            {
                _imgui.ImDrawCmd_SetUserCallbackData(m_Ptr, value);
            }
        }

        public DrawCommand(nint ptr)
        {
            m_Ptr = ptr;
        }
    }
}
