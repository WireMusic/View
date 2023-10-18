using Stage.Core;
using Stage.Projects;
using Stage.UIModule;

namespace View.Panels
{
    public class ProjectBrowserPanel : IPanel
    {
        private bool _projectLoaded = false;

        private bool _open = true;
        public bool Open => _open;

        public ProjectBrowserPanel()
        {
            Project.ProjectChanged += project => _projectLoaded = project != null;
        }

        public void OnUI()
        {
            if (_open)
            {
                UI.Begin("Project Browser", ref _open);

                if (_projectLoaded)
                {

                }
                else
                {
                    var windowSize = UI.GetWindowSize();
                    var textSize = UI.CalcTextSize("No project opened.");

                    UI.SetCursorPos((windowSize - textSize) * 0.5f);
                    UI.PushStyleColour(StyleColour.Text, new Vector4(0.388f, 0.388f, 0.388f, 1.0f));
                    UI.Text("No project opened.");
                    UI.PopStyleColour();
                }

                UI.End();
            }
        }
    }
}
