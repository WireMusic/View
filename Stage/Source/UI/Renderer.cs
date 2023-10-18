using System;

using Stage.Core;

namespace Stage.UIModule
{
    public class Renderer
    {
        public static Renderer Current => new Renderer(UI.GetWindowDrawList());

        private DrawList _drawList;

        private Renderer()
        {
            throw new InvalidOperationException();
        }

        private Renderer(DrawList drawList)
        {
            _drawList = drawList;
        }

        public void PushClipRect(Vector2 clip_rect_min, Vector2 clip_rect_max, bool intersect_with_current_clip_rect = false)
            => _drawList.PushClipRect(clip_rect_min, clip_rect_max, intersect_with_current_clip_rect);
        public void PushClipRectFullScreen() => _drawList.PushClipRectFullScreen();
        public void PopClipRect() => _drawList.PopClipRect();
        public void PushTextureID(nint textureID) => _drawList.PushTextureID(textureID);
        public void PopTextureID() => _drawList.PopTextureID();
        public Vector2 GetClipRectMin() => _drawList.GetClipRectMin();
        public Vector2 GetClipRectMax() => _drawList.GetClipRectMax();

        public void DrawLine(Vector2 point1, Vector2 point2, Vector4 colour, float thickness = 1.0f)
            => _drawList.AddLine(point1, point2, colour, thickness);
        public void DrawRect(Vector2 min, Vector2 max, Vector4 colour, float rounding = 0.0f, DrawFlags flags = 0, float thickness = 1.0f)
            => _drawList.AddRect(min, max, colour, rounding, flags, thickness);
        public void DrawFilledRect(Vector2 min, Vector2 max, Vector4 colour, float rounding = 0.0f, DrawFlags flags = 0)
            => _drawList.AddRectFilled(min, max, colour, rounding, flags);
        public void DrawMultiColorFilledRect(Vector2 min, Vector2 max, Vector4 colourTL, Vector4 colourTR, Vector4 colourBR, Vector4 colourBL)
            => _drawList.AddRectFilledMultiColor(min, max, colourTL, colourTR, colourBR, colourBL);
        public void DrawQuad(Vector2 point1, Vector2 point2, Vector2 point3, Vector2 point4, Vector4 colour)
            => _drawList.AddQuad(point1, point2, point3, point4, colour);

        public void DrawText(string text, Vector2 pos, Vector4 colour) => _drawList.AddText(pos, text, colour);
        public void DrawText(string text, Vector2 pos, Vector4 colour, float textSize, bool bold, float wrapWidth = 0.0f)
            => _drawList.AddText(textSize, bold, pos, text, colour, wrapWidth);
        
    }
}
