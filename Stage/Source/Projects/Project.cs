using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Stage.Projects
{
    public class Project
    {
        public static Project? Current;

        public string Name = string.Empty;
        public string Description = string.Empty;
        public string Version = string.Empty;
        public string Artist = string.Empty;
        public string Genre = string.Empty;

        // key, genre, etc

        public Project()
        {
        }

        public Project(string path)
        {
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
