using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Stage.Projects
{
    public class Project
    {
        public delegate void ProjectEventHandler(Project? project);

        public static Project? Current;
        public static event ProjectEventHandler ProjectChanged = project => {};

        public string Name = string.Empty;
        public string Description = string.Empty;
        public string Version = string.Empty;
        public string Artist = string.Empty;
        public string Genre = string.Empty;

        public string[] Items = new string[0];

        // key, genre, etc

        public Project()
        {
        }

        public void Load()
        {
            Current = this;
            ProjectChanged(this);
        }

        public static void Close()
        {
            Current = null;
            ProjectChanged(null);
        }

        public static Project Create(string path, string name, string desc, string version, string artist, string genre)
        {
            Project project = new Project();

            project.Name = name;
            project.Description = desc;
            project.Version = version;
            project.Artist = artist;
            project.Genre = genre;

            FileStream fs = File.Create(path);
            JsonSerializer.Serialize(fs, project, ProjectSourceGenerationContext.Default.Project);
            fs.Close();

            return project;
        }

        public static Project Open(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            Project project = JsonSerializer.Deserialize<Project>(fs, ProjectSourceGenerationContext.Default.Project) ?? throw new Exception("Could not load project!");
            fs.Close();
            return project;
        }
    }

    [JsonSourceGenerationOptions(WriteIndented = true, IncludeFields = true)]
    [JsonSerializable(typeof(Project))]
    partial class ProjectSourceGenerationContext : JsonSerializerContext
    {
    }
}
