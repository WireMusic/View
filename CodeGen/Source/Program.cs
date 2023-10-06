using System.CommandLine;

namespace Stage.CodeGen
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            Option<string> nameOption = new Option<string>(
                name: "--name", description: "Sets the name of the command.");

            Option<string> argsOption = new Option<string>(
                name: "--args", description: "Set arguments of the function.");

            Option<bool> noPrefix = new Option<bool>(
                name: "--no-prefix", () => true, description: "Should function have ImGui prefix.") { IsRequired = false };
            noPrefix.AddAlias("-np");

            Command glfwCommand = new Command(
                name: "glfw", description: "Adds GLFW functions to the code.");
            Command gladCommand = new Command(
                name: "glad", description: "Adds Glad functions to the code.");
            Command glCommand = new Command(
                name: "gl", description: "Adds OpenGL functions to the code.");
            Command imguiCommand = new Command(
                name: "imgui", description: "Adds ImGui functions to the code.");

            glfwCommand.AddOption(nameOption);
            glfwCommand.AddOption(argsOption);
            gladCommand.AddOption(nameOption);
            gladCommand.AddOption(argsOption);
            glCommand.AddOption(nameOption);
            glCommand.AddOption(argsOption);
            imguiCommand.AddOption(nameOption);
            imguiCommand.AddOption(argsOption);
            imguiCommand.AddOption(noPrefix);

            glfwCommand.SetHandler(GLFW, nameOption, argsOption);
            gladCommand.SetHandler(Glad, nameOption, argsOption);
            imguiCommand.SetHandler(ImGui, nameOption, argsOption, noPrefix);
            glCommand.SetHandler(OpenGL, nameOption, argsOption);

            RootCommand rootCommand = new RootCommand("Generate code for the project.");
            rootCommand.AddCommand(glfwCommand);
            rootCommand.AddCommand(gladCommand);
            rootCommand.AddCommand(glCommand);
            rootCommand.AddCommand(imguiCommand);

            return await rootCommand.InvokeAsync(args);
        }

        static void GLFW(string name, string arguments)
        {
            FileStream globals = new FileStream("Source/Core/Externs.cs", FileMode.Open, FileAccess.ReadWrite);
            FileStream window = new FileStream("Source/Renderer/Window.cs", FileMode.Open, FileAccess.ReadWrite);

            StreamReader globalsReader = new StreamReader(globals);
            StreamReader windowReader = new StreamReader(window);
            
            string globalsContents = globalsReader.ReadToEnd();

            int glfwStructStart = -1;

            int i = 0;
            foreach (string line in globalsContents.Split(Environment.NewLine))
            {
                if (line.Contains("unsafe struct GLFW"))
                {
                    glfwStructStart = i;
                    break;
                }
                i++;
            }

            if (glfwStructStart == -1)
                return;

            string[] fileFromStruct = globalsContents.Split(Environment.NewLine)[glfwStructStart..];

            int glfwStructEnd = -1;
            i = glfwStructStart;
            foreach (string line in fileFromStruct)
            {
                if (line.Contains("glfw end"))
                {
                    glfwStructEnd = i;
                    break;
                }
                i++;
            }

            List<string> allLines = new List<string>(globalsContents.Split(Environment.NewLine));

            string template = "\t\tpublic delegate* unmanaged[Cdecl]<ARGS> NAME;";

            string args = string.Join(", ", arguments.Split(" "));

            allLines.Insert(glfwStructEnd, template.Replace("ARGS", args).Replace("NAME", name));
            
            globalsReader.Close();
            globals.Close();

            File.Delete("Source/Core/Externs.cs");

            globals = File.Create("Source/Core/Externs.cs");
            StreamWriter sw = new StreamWriter(globals);
            
            foreach (string line in allLines)
            {
                sw.WriteLine(line);
                Console.WriteLine($"Writing line: {line}");
            }

            sw.Close();

            string windowContents = windowReader.ReadToEnd();

            int initStart = -1;

            i = 0;
            foreach (string line in windowContents.Split(Environment.NewLine))
            {
                if (line.Contains("private static unsafe void Init()"))
                {
                    initStart = i;
                    break;
                }
            }

            if (initStart == -1)
                return;

            string[] fileFromInit = windowContents.Split(Environment.NewLine)[initStart..];

            int initEnd = -1;
            i = initStart;
            foreach (string line in fileFromInit)
            {
                if (line.Contains("glad"))
                {
                    initEnd = i;
                    break;
                }
                i++;
            }

            allLines = new List<string>(windowContents.Split(Environment.NewLine));
            template = "\t\t\t_glfw.NAME = (delegate* unmanaged[Cdecl]<ARGS>)_glfw.Lib.Load(\"glfwNAME\");";
            allLines.Insert(initEnd - 1, template.Replace("NAME", name).Replace("ARGS", args));

            windowReader.Close();
            window.Close();

            File.Delete("Source/Renderer/Window.cs");
            window = File.Create("Source/Renderer/Window.cs");

            sw = new StreamWriter(window);
            foreach (string line in allLines)
            {
                if (line != Environment.NewLine)
                    sw.WriteLine(line);
                sw.WriteLine();
                Console.WriteLine($"Writing line: {line}");
            }

            sw.Close();
            window.Close();
        }

        static void Glad(string name, string arguments)
        {
            FileStream globals = new FileStream("Source/Core/Externs.cs", FileMode.Open, FileAccess.ReadWrite);
            FileStream window = new FileStream("Source/Renderer/Window.cs", FileMode.Open, FileAccess.ReadWrite);

            StreamReader globalsReader = new StreamReader(globals);
            StreamReader windowReader = new StreamReader(window);

            string globalsContents = globalsReader.ReadToEnd();

            int gladStructStart = -1;

            int i = 0;
            foreach (string line in globalsContents.Split(Environment.NewLine))
            {
                if (line.Contains("unsafe struct Glad"))
                {
                    gladStructStart = i;
                    break;
                }
                i++;
            }

            if (gladStructStart == -1)
                return;

            string[] fileFromStruct = globalsContents.Split(Environment.NewLine)[gladStructStart..];

            int gladStructEnd = -1;
            i = gladStructStart;
            foreach (string line in fileFromStruct)
            {
                if (line.Contains("glad end"))
                {
                    gladStructEnd = i;
                    break;
                }
                i++;
            }

            List<string> allLines = new List<string>(globalsContents.Split(Environment.NewLine));

            string template = "\t\tpublic delegate* unmanaged[Cdecl]<ARGS> NAME;\n";

            string args = string.Join(", ", arguments.Split(" "));

            allLines.Insert(gladStructEnd, template.Replace("ARGS", args).Replace("NAME", name));

            globalsReader.Close();
            globals.Close();

            File.Delete("Source/Core/Externs.cs");

            globals = File.Create("Source/Core/Externs.cs");
            StreamWriter sw = new StreamWriter(globals);

            foreach (string line in allLines)
            {
                sw.WriteLine(line);
                Console.WriteLine($"Writing line: {line}");
            }

            sw.Close();

            string windowContents = windowReader.ReadToEnd();

            int initStart = -1;

            i = 0;
            foreach (string line in windowContents.Split(Environment.NewLine))
            {
                if (line.Contains("private static unsafe void Init()"))
                {
                    initStart = i;
                    break;
                }
            }

            if (initStart == -1)
                return;

            string[] fileFromInit = windowContents.Split(Environment.NewLine)[initStart..];

            int initEnd = -1;
            i = initStart;
            foreach (string line in fileFromInit)
            {
                if (line.Contains("imgui"))
                {
                    initEnd = i;
                    break;
                }
                i++;
            }

            allLines = new List<string>(windowContents.Split(Environment.NewLine));
            template = "\t\t\t_glad.NAME = (delegate* unmanaged[Cdecl]<ARGS>)_glad.Lib.Load(\"gladNAME\");";
            allLines.Insert(initEnd - 1, template.Replace("NAME", name).Replace("ARGS", args));

            windowReader.Close();
            window.Close();

            File.Delete("Source/Renderer/Window.cs");
            window = File.Create("Source/Renderer/Window.cs");

            sw = new StreamWriter(window);
            foreach (string line in allLines)
            {
                sw.WriteLine(line);
                Console.WriteLine($"Writing line: {line}");
            }

            sw.Close();
            window.Close();
        }

        static void ImGui(string name, string arguments, bool noPrefix)
        {
            FileStream globals = new FileStream("Source/Core/Externs.cs", FileMode.Open, FileAccess.ReadWrite);
            FileStream window = new FileStream("Source/Renderer/Window.cs", FileMode.Open, FileAccess.ReadWrite);

            StreamReader globalsReader = new StreamReader(globals);
            StreamReader windowReader = new StreamReader(window);

            string globalsContents = globalsReader.ReadToEnd();

            int imguiStructStart = -1;

            int i = 0;
            foreach (string line in globalsContents.Split(Environment.NewLine))
            {
                if (line.Contains("unsafe struct ImGui"))
                {
                    imguiStructStart = i;
                    break;
                }
                i++;
            }

            if (imguiStructStart == -1)
                return;

            string[] fileFromStruct = globalsContents.Split(Environment.NewLine)[imguiStructStart..];

            int imguiStructEnd = -1;
            i = imguiStructStart;
            foreach (string line in fileFromStruct)
            {
                if (line.Contains("imgui end"))
                {
                    imguiStructEnd = i;
                    break;
                }
                i++;
            }

            List<string> allLines = new List<string>(globalsContents.Split(Environment.NewLine));

            string template = "\t\tpublic delegate* unmanaged[Cdecl]<ARGS> NAME;\n";

            string args = string.Join(", ", arguments.Split(" "));

            allLines.Insert(imguiStructEnd, template.Replace("ARGS", args).Replace("NAME", name));

            globalsReader.Close();
            globals.Close();

            File.Delete("Source/Core/Externs.cs");

            globals = File.Create("Source/Core/Externs.cs");
            StreamWriter sw = new StreamWriter(globals);

            foreach (string line in allLines)
            {
                sw.WriteLine(line);
                Console.WriteLine($"Writing line: {line}");
            }

            sw.Close();

            string windowContents = windowReader.ReadToEnd();

            int initStart = -1;

            i = 0;
            foreach (string line in windowContents.Split(Environment.NewLine))
            {
                if (line.Contains("private static unsafe void Init()"))
                {
                    initStart = i;
                    break;
                }
            }

            if (initStart == -1)
                return;

            string[] fileFromInit = windowContents.Split(Environment.NewLine)[initStart..];

            int initEnd = -1;
            i = initStart;
            foreach (string line in fileFromInit)
            {
                if (line.Contains("imgui end"))
                {
                    initEnd = i;
                    break;
                }
                i++;
            }

            allLines = new List<string>(windowContents.Split(Environment.NewLine));
            if (!noPrefix)
                template = "\t\t\t_imgui.NAME = (delegate* unmanaged[Cdecl]<ARGS>)_imgui.Lib.Load(\"igNAME\");";
            else
                template = "\t\t\t_imgui.NAME = (delegate* unmanaged[Cdecl]<ARGS>)_imgui.Lib.Load(\"NAME\");";
            allLines.Insert(initEnd - 1, template.Replace("NAME", name).Replace("ARGS", args));

            windowReader.Close();
            window.Close();

            File.Delete("Source/Renderer/Window.cs");
            window = File.Create("Source/Renderer/Window.cs");

            sw = new StreamWriter(window);
            foreach (string line in allLines)
            {
                sw.WriteLine(line);
                Console.WriteLine($"Writing line: {line}");
            }

            sw.Close();
            window.Close();
        }

        static void OpenGL(string name, string arguments)
        {
            FileStream globals = new FileStream("Source/Core/Externs.cs", FileMode.Open, FileAccess.ReadWrite);
            FileStream window = new FileStream("Source/Renderer/Window.cs", FileMode.Open, FileAccess.ReadWrite);

            StreamReader globalsReader = new StreamReader(globals);
            StreamReader windowReader = new StreamReader(window);

            string globalsContents = globalsReader.ReadToEnd();

            int glStructStart = -1;

            int i = 0;
            foreach (string line in globalsContents.Split(Environment.NewLine))
            {
                if (line.Contains("unsafe struct GL"))
                {
                    glStructStart = i;
                    break;
                }
                i++;
            }

            if (glStructStart == -1)
                return;

            string[] fileFromStruct = globalsContents.Split(Environment.NewLine)[glStructStart..];

            int glStructEnd = -1;
            i = glStructStart;
            foreach (string line in fileFromStruct)
            {
                if (line.Contains("gl end"))
                {
                    glStructEnd = i;
                    break;
                }
                i++;
            }

            List<string> allLines = new List<string>(globalsContents.Split(Environment.NewLine));

            string template = "\t\tpublic delegate* unmanaged[Cdecl]<ARGS> NAME;\n";

            string args = string.Join(", ", arguments.Split(" "));

            allLines.Insert(glStructEnd, template.Replace("ARGS", args).Replace("NAME", name));

            globalsReader.Close();
            globals.Close();

            File.Delete("Source/Core/Externs.cs");

            globals = File.Create("Source/Core/Externs.cs");
            StreamWriter sw = new StreamWriter(globals);

            foreach (string line in allLines)
            {
                sw.WriteLine(line);
                Console.WriteLine($"Writing line: {line}");
            }

            sw.Close();

            string windowContents = windowReader.ReadToEnd();

            int initStart = -1;

            i = 0;
            foreach (string line in windowContents.Split(Environment.NewLine))
            {
                if (line.Contains("internal static unsafe void GLInit()"))
                {
                    initStart = i;
                    break;
                }
            }

            if (initStart == -1)
                return;

            string[] fileFromInit = windowContents.Split(Environment.NewLine)[initStart..];

            int initEnd = -1;
            i = initStart;
            foreach (string line in fileFromInit)
            {
                if (line.Contains("gl end"))
                {
                    initEnd = i;
                    break;
                }
                i++;
            }

            allLines = new List<string>(windowContents.Split(Environment.NewLine));
            template = "\t\t\t_gl.NAME = (delegate* unmanaged[Cdecl]<ARGS>)glfwGetProcAddress(\"glNAME\");";
            allLines.Insert(initEnd - 1, template.Replace("NAME", name).Replace("ARGS", args));

            windowReader.Close();
            window.Close();

            File.Delete("Source/Renderer/Window.cs");
            window = File.Create("Source/Renderer/Window.cs");

            sw = new StreamWriter(window);
            foreach (string line in allLines)
            {
                sw.WriteLine(line);
                Console.WriteLine($"Writing line: {line}");
            }

            sw.Close();
            window.Close();
        }
    }
}
