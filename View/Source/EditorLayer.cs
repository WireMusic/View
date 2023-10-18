using Stage.Core;
using Stage.Projects;
using Stage.Rendering;
using Stage.UIModule;

using View.Utils;
using View.Panels;

using System;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace View
{
    public class EditorLayer : Layer
    {
        private Menubar Menu;

        private Window? CreateProjectWindow;

        private ProjectBrowserPanel ProjectBrowser;
        private ModularPanel ModularPanel;

        private bool _openProject = false;
        private int _frames = 0;

        public static readonly string DefaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        private string _createProjectName = string.Empty;
        private string _createProjectDesc = string.Empty;
        private string _createProjectVersion = string.Empty;
        private string _createProjectArtist = string.Empty;
        private string _createProjectGenre = string.Empty;
        private string _createProjectPath = DefaultPath;
        private bool _pathChanged = false;

        private bool _createProject = false;

        private Project[] _recentProjects = new Project[0];

        public EditorLayer()
            : base("EditorLayer")
        {

            Menu = new Menubar();
            Menu file = new Menu("File");
            Menu help = new Menu("Help");
            file.Items = new List<MenuPart>
            {
                new Menu("Project")
                {
                    Items = new List<MenuPart>
                    {
                        new MenuItem("Open Project...", "Ctrl+Shift+O")
                        {
                            Callback = () => _openProject = true
                        },
                        new MenuItem("Close Project", "Ctrl+Shift+W")
                        {
                            Callback = Project.Close
                        }
                    }
                },
                new MenuItem("Close", "Alt+F4")
                {
                    Callback = Application.Instance.Close
                }
            };
            Menu.AddMenu(file);

            ProjectBrowser = new ProjectBrowserPanel();
            ModularPanel = new ModularPanel();
        }

        public override void OnAttach()
        {
            if (Project.Current == null)
            {
                _openProject = true;
            }

            if (File.Exists("data/recents.json"))
            {
                FileStream fs = new FileStream("data/recents.json", FileMode.Open, FileAccess.Read);

                _recentProjects = GetProjects(fs);

                fs.Close();
            }
        }

        public override void OnUpdate()
        {
            if (_frames < 6)
            {
                _frames++;
                return;
            }

            if (_openProject && UI.BeginModal("Open a Project", new Vector2(900, 494), freezeOtherWindows: true))
            {
                if (!_createProject)
                {
                    UI.PushFont(true, 45.0f);
                    UI.Text("Open a Project");
                    UI.PopFont();

                    UI.SameLine();

                    UI.SetCursorPosX(UI.GetWindowSize().X - (UI.CalcTextSize("Create Project").X + 8.0f) - 16.0f);

                    if (UI.Button("Create Project", new Vector2(0, 50.0f)))
                    {
                        _createProject = true;
                    }

                    Vector2 max = UI.GetCurrentCursorPos() + new Vector2(UI.GetContentRegionAvailable().X, 58.0f * _recentProjects.Length);

                    Renderer.Current.DrawFilledRect(UI.GetCurrentCursorPos(), max, new Vector4(0.35f), 5.0f);

                    for (int i = 0; i < _recentProjects.Length; i++)
                    {
                        bool last = i == _recentProjects.Length - 1;
                        DrawProject(project: _recentProjects[i], count: _recentProjects.Length, index: i, isLast: last);
                    }
                }
                else
                {
                    float itemWidthTwo = UI.GetContentRegionAvailable().X / 2.0f - 4.0f;

                    UI.SetCursorPosY(20.0f);
                    UI.PushStyleVar(StyleVar.ItemSpacing, new Vector2(8.0f, 20.0f));

                    UI.PushItemWidth(itemWidthTwo);
                    UI.PushFont(true, 22.0f);
                    UI.Text("Name");
                    UI.SameLine(UI.GetContentRegionAvailable().X - itemWidthTwo + 8.0f);
                    UI.Text("Description");
                    UI.PopFont();
                    UI.PopItemWidth();

                    UI.PushItemWidth(itemWidthTwo);
                    UI.PushStyleVar(StyleVar.FramePadding, new Vector2(8.0f, 18.0f));
                    UI.InputText("##Name", ref _createProjectName, "Project Name");
                    UI.SameLine();
                    UI.InputText("##Description", ref _createProjectDesc, "Project Description");
                    UI.PopStyleVar();
                    UI.PopItemWidth();

                    float itemWidthThree = UI.GetContentRegionAvailable().X / 3.0f - 5.0f;

                    UI.PushItemWidth(itemWidthThree);
                    UI.PushFont(true, 22.0f);
                    UI.Text("Version");
                    UI.SameLine(UI.GetContentRegionAvailable().X - itemWidthThree * 2.0f + 8.0f);
                    UI.Text("Artist");
                    UI.SameLine(UI.GetContentRegionAvailable().X - itemWidthThree + 8.0f);
                    UI.Text("Genre");
                    UI.PopFont();
                    UI.PopItemWidth();

                    UI.PushItemWidth(itemWidthThree);
                    UI.PushStyleVar(StyleVar.FramePadding, new Vector2(8.0f, 18.0f));
                    UI.InputText("##Version", ref _createProjectVersion, "Project Version");
                    UI.SameLine();
                    UI.InputText("##Artist", ref _createProjectArtist, "Artist");
                    UI.SameLine();
                    UI.InputText("##Genre", ref _createProjectGenre, "Genre");
                    UI.PopStyleVar();
                    UI.PopItemWidth();

                    if (_createProjectName != string.Empty && !_pathChanged)
                        _createProjectPath = Path.Combine(DefaultPath, _createProjectName);

                    UI.PushFont(true, 22.0f);
                    UI.Text("Path");
                    UI.PopFont();

                    UI.PushStyleVar(StyleVar.ItemSpacing, new Vector2(4.0f, 0.0f));
                    UI.PushItemWidth(UI.GetContentRegionAvailable().X - UI.CalcTextSize("...").X - 20.0f);
                    UI.PushStyleVar(StyleVar.FramePadding, new Vector2(8.0f, 18.0f));
                    if (UI.InputText("##path", ref _createProjectPath, "Project Path"))
                        _pathChanged = true;
                    UI.PopItemWidth();
                    UI.PopStyleVar();
                    UI.SameLine();
                    if (UI.Button("...", new Vector2(0, 54.0f)))
                    {
                        _createProjectPath = UI.OpenFolder(DefaultPath, CreateProjectWindow);
                        if (_createProjectPath == null)
                            throw new Exception();
                        _pathChanged = true;
                    }
                    UI.PopStyleVar();

                    UI.PopStyleVar();

                    UI.SetCursorPosY(UI.GetWindowSize().Y - 50.0f - 8.0f);

                    if (UI.Button("Back", new Vector2(0, 50.0f)))
                        _createProject = false;

                    UI.SameLine();

                    UI.SetCursorPosX(UI.GetWindowSize().X - (UI.CalcTextSize("Create Project").X + 8.0f) - 16.0f);
                    if (UI.Button("Create Project", new Vector2(0, 50.0f)))
                    {
                        Project.Current = Project.Create(_createProjectPath, _createProjectName, _createProjectDesc, _createProjectVersion, _createProjectArtist, _createProjectGenre);
                    }
                }

                UI.EndModal();
            }
            else
            {
                _openProject = false;
                _createProject = false;
            }
        }

        private Vector2 _lastCursorPos = new Vector2(-1.0f);
        private Vector2? _avail = null;
        private List<bool> _opened = new List<bool>();

        public void DrawProject(Project project, int count, int index, bool isLast)
        {
            //Project project = Project.Open(path);

            UI.PushID(project.Name + index.ToString());

            DrawFlags flags = DrawFlags.None;
            if (index == 0)
                flags |= DrawFlags.RoundCornersTop;
            if (isLast)
                flags |= DrawFlags.RoundCornersBottom;

            //UI.SetCursorPosY(68.0f + index * 58.0f);

            //DrawList drawList = UI.GetWindowDrawList();
            //Vector2 min = UI.GetCurrentCursorPos();
            //Vector2 max = new Vector2(min.X + UI.GetContentRegionAvailable().X, min.Y + 58.0f);
            Vector2 avail = _avail ?? UI.GetContentRegionAvailable();
            _avail = _avail ?? avail;
            //drawList.AddRectFilled(min, max, Vector4.Zero, 5.0f, flags);

            if (index == 0)
            {
                UI.PushStyleVar(StyleVar.ItemSpacing, new Vector2(0.0f));
                UI.PushStyleColour(StyleColour.Button, new Vector4(0.0f));
                for (int i = 0; i < count; i++)
                {
                    _opened.Add(UI.Button($"##{project.Name}", new Vector2(avail.X, 58.0f)));
                }
                UI.PopStyleColour();
                UI.PopStyleVar();
            }
            _lastCursorPos = UI.GetCurrentCursorPos();

            UI.Columns(2, false);
            UI.SetColumnWidth(0, avail.X / 2.0f);
            UI.SetColumnWidth(1, avail.X / 2.0f);

            UI.SetCursorPosX(16.0f);
            if (index != 0)
                UI.SetCursorPosY(68.0f + (58.0f * index));
            else
                UI.SetCursorPosY(68.0f);

            UI.PushFont(true, 22.0f);
            UI.Text(project.Name);
            UI.PopFont();

            UI.SetCursorPosX(16.0f);

            UI.Text(project.Description);

            UI.NextColumn();

            if (index != 0)
                UI.SetCursorPosY(68.0f + (58.0f * index));
            else
                UI.SetCursorPosY(68.0f);

            UI.PushFont(true, 22.0f);
            UI.Text(project.Artist);
            UI.PopFont();

            UI.Text(project.Version);

            UI.Columns(1, false);

            UI.SetCursorPosY(68.0f + (58.0f * index));
            UI.SetCursorPosX(UI.GetContentRegionAvailable().X - UI.CalcTextSize("Open").X - 16.0f);
            if (_opened[index])
            {
                UI.CloseCurrentModal();
                _openProject = false;
                _createProject = false;

                project.Load();
            }

            if (!isLast)
            {
                UI.SetCursorPosX(_lastCursorPos.X);
                UI.SetCursorPosY(_lastCursorPos.Y);
            }

            if (isLast)
            {
                _opened.Clear();
            }

            UI.PopID();
        }

        public override void OnUI()
        {
            UI.CreateDockspace(Menu);
            //CP.OnUI();
            ProjectBrowser.OnUI();
            ModularPanel.OnUI();
        }

        private Project[] GetProjects(FileStream fs)
        {
            ProjectArray arr = JsonSerializer.Deserialize(fs, ProjectArraySourceGenerationContext.Default.ProjectArray);

            Project[] projects = new Project[arr.Projects.Length];

            int i = 0;
            foreach (string path in arr.Projects)
            {
                projects[i++] = Project.Open(path);
            }

            return projects;
        }
    }

    struct ProjectArray
    {
        public string[] Projects;
    }

    [JsonSourceGenerationOptions(WriteIndented = true, IncludeFields = true)]
    [JsonSerializable(typeof(ProjectArray))]
    partial class ProjectArraySourceGenerationContext : JsonSerializerContext
    {
    }
}
