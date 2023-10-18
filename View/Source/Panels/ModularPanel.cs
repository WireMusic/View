using System;
using System.Text;
using Stage.Core;
using Stage.UIModule;
using View.Nodes;

namespace View.Panels
{
    public class ModularPanel : IPanel
    {
        private bool _open = true;
        public bool Open => _open;

        private Vector2 _scrolling = Vector2.Zero;
        private bool _enableContextMenu = true;
        private float _scroll = 1.0f;

        private NodeManager NodeManager;

        public ModularPanel()
        {
            NodeManager = new NodeManager();
        }

        public void OnUI()
        {
            if (_open)
            {
                UI.Begin("Modular", ref _open);

                Vector2 initialCanvasPos = UI.GetCursorPos();

                Renderer renderer = Renderer.Current;

                Vector2 canvasMin = UI.GetCurrentCursorPos();
                Vector2 canvasSize = UI.GetContentRegionAvailable();
                Vector2 canvasMax = canvasMin + canvasSize;

                if (canvasSize.X < 50.0f)
                    canvasSize.X = 50.0f;
                if (canvasSize.Y < 50.0f)
                    canvasSize.Y = 50.0f;

                renderer.DrawFilledRect(canvasMin, canvasMax, new Vector4(new Vector3(50.0f / 255.0f), 1.0f));

                UI.InvisibleButton("canvas", canvasSize, ButtonFlags.MouseButtonLeft | ButtonFlags.MouseButtonRight);
                bool isHovered = UI.IsItemHovered();
                bool isActive = UI.IsItemActive();
                Vector2 origin = new Vector2(canvasMin.X + _scrolling.X, canvasMin.Y + _scrolling.Y);
                Vector2 mousePosInCanvas = new Vector2(UI.GetMousePos().X - origin.X, UI.GetMousePos().Y - origin.Y);

                float mouseThresholdForPan = _enableContextMenu ? -1.0f : 0.0f;
                if (isActive && UI.IsMouseDragging(MouseButton.Button1, mouseThresholdForPan))
                {
                    _scrolling += UI.GetMouseDelta();
                }

                renderer.PushClipRect(canvasMin, canvasMax);
                float gridStep = 64.0f;

                for (float x = _scrolling.X % gridStep; x < canvasSize.X; x += gridStep)
                    renderer.DrawLine(new Vector2(canvasMin.X + x, canvasMin.Y), new Vector2(canvasMin.X + x, canvasMax.Y), new Vector4(new Vector3(200.0f / 255.0f), 1.0f));
                for (float y = _scrolling.Y % gridStep; y < canvasSize.Y; y += gridStep)
                    renderer.DrawLine(new Vector2(canvasMin.X, canvasMin.Y + y), new Vector2(canvasMax.X, canvasMin.Y + y), new Vector4(new Vector3(200.0f / 255.0f), 1.0f));

                UI.SetCursorPos(canvasMin);

                int i = 0;
                foreach (INode node in NodeManager.Nodes)
                {
                    DrawNode(node, _scrolling, canvasMin, canvasMax, initialCanvasPos, i++);
                }

                // Draw canvas outline last
                renderer.DrawRect(canvasMin, canvasMax, new Vector4(1.0f));

                renderer.PopClipRect();

                UI.End();
            }
        }

        Dictionary<string, bool> activeLasts = new Dictionary<string, bool>();

        private void DrawNode(INode node, Vector2 scrolling, Vector2 canvasMin, Vector2 canvasMax, Vector2 initialCanvasPos, int index)
        {
            Renderer renderer = Renderer.Current;

            StringBuilder builder = new StringBuilder("headerButton_", 16 + node.ToString()!.Length);
            builder.Append(node.ToString());
            builder.AppendFormat("_{0}", index);

            if (!activeLasts.ContainsKey(builder.ToString()))
                activeLasts.Add(builder.ToString(), false);

            bool activeLast = activeLasts[builder.ToString()];

            UI.PushFont(true, 33.0f);
            float headerHeight = UI.CalcTextSize("|").Y + 16.0f;
            UI.PopFont();

            Vector2 min = canvasMin + node.GetPosition();
            Vector2 max = min + node.GetSize();

            UI.SetCursorPos(initialCanvasPos + node.GetPosition());

            UI.PushID(builder.ToString());
            UI.InvisibleButton(builder.ToString(), new Vector2(max.X, min.Y + headerHeight) - min);

            bool hovered = UI.IsItemHovered();
            bool active = (hovered && UI.IsMouseDown(MouseButton.Left)) || (activeLast && UI.IsMouseDragging(MouseButton.Left));

            //if (!active)
            //    Console.WriteLine(active);

            if (active && UI.IsMouseDragging(MouseButton.Left))
            {
                node.MoveBy(UI.GetMouseDelta());
            }

            // Draw full node
            renderer.DrawFilledRect(min, max, new Vector4(new Vector3(0.265f), 1.0f), 10.0f);
            // Draw header
            renderer.DrawFilledRect(min, new Vector2(max.X, min.Y + headerHeight), new Vector4(new Vector3(0.32f + (hovered ? 0.05f : 0.0f)), 1.0f), 10.0f, DrawFlags.RoundCornersTop);
            // Draw outline
            renderer.DrawRect(min, max, new Vector4(1.0f), 10.0f);
            // Draw name
            renderer.DrawText(node.ToString(), min + 8.0f, new Vector4(1.0f), 33.0f, true);

            activeLasts[builder.ToString()] = active;

            UI.PopID();
        }
    }
}
