global using static Stage.Core.Externs;

using Stage.Audio;
using Stage.ImGui;
using Stage.Utils;
using Stage.UIModule;
using Stage.Renderer;

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Stage.Core
{
    public struct ApplicationSpecification
    {
        public string Name;
        public string[] CommandLineArgs;
        public bool VSync;
        public string ThemePath;
        public bool MaximiseWindow;

        // Fonts
        public float DefaultFontSize;
        public float[] DefaultFontSizes;
        public byte[] DefaultFontData;
        public float[] BoldFontSizes;
        public byte[] BoldFontData;
        /// <summary>
        /// Load a font with size if not in arrays DefaultFontSizes or BoldFontSizes.
        /// If true, it will load the font with the size, else it will throw <see cref="System.ArgumentException"/>.
        /// </summary>
        public bool LoadFontOnNewSize;

        public byte[] GLFW;
        public byte[] Glad;
        public byte[] cimgui;
        public byte[] OtherImplementations;
        public string OtherImplementationsName;
    }

    public class Application
    {
        public static Application Instance { get; private set; }

        public ApplicationSpecification Specification { get; private set; }

        private List<ILayer> m_Layers;

        private Window m_Window;
        public Window Window => m_Window;
        private bool m_Running = true;
        private List<float> _imguiTimes = new List<float>();

        private List<Action> m_MainThreadQueue = new List<Action>();

        [DllImport("ImGui.impl.dll")]
        private static extern void ImGuiInit(nint win);

        public Application(ApplicationSpecification specification)
        {
            if (Instance != null)
                throw new Exception();

            Instance = this;

            Specification = specification;
            m_Layers = new List<ILayer>();

            AudioEngine.Init();

            m_Window = new Window(new WindowProperties { Width = 1280, Height = 720, Title = specification.Name, Maximised = specification.MaximiseWindow });
            m_Window.VSync = specification.VSync;

            ImGuiImpl.InitFor(
                m_Window, 
                specification.DefaultFontData, 
                specification.BoldFontData, 
                specification.DefaultFontSize,
                specification.DefaultFontSizes, 
                specification.BoldFontSizes,
                specification.LoadFontOnNewSize
            );

            Console.WriteLine(Unsafe.SizeOf<ImGui>() + Unsafe.SizeOf<GLFW>() + Unsafe.SizeOf<Glad>() + Unsafe.SizeOf<Helper>() + Unsafe.SizeOf<GL>());
        }

        bool open = true;

        public unsafe void Run()
        {
            while (m_Running && !m_Window.ShouldClose)
            {
                _gl.ClearColour(0.8f, 0.2f, 0.3f, 1.0f);
                _gl.Clear(0x00004000 | 0x00000100);

                ExecuteMainThreadQueue();

                foreach (var layer in m_Layers)
                    layer.OnUpdate();

                // ImGui
                if (!Window.Frozen)
                {
                    ImGuiImpl.ImGuiBegin();

                    bool* p_open = (bool*)Unsafe.AsPointer(ref open);

                    foreach (var layer in m_Layers)
                        layer.OnUI();

                    ImGuiImpl.ImGuiEnd();
                }

                m_Window.Update();
            }
        }

        public void AddToMainThreadQueue(Action action)
        {
            m_MainThreadQueue.Add(action);
        }

        private void ExecuteMainThreadQueue()
        {
            foreach (var action in m_MainThreadQueue)
            {
                action();
            }

            m_MainThreadQueue.Clear();
        }

        public void Close()
        {
            m_Running = false;
        }

        public void AddLayer<T>() where T : ILayer, new()
        {
            AddLayer<T>(new T());
        }

        public void AddLayer<T>(T layer) where T : ILayer
        {
            m_Layers.Add(layer);

            layer.OnAttach();
        }

        public void Shutdown()
        {
            ImGuiImpl.ImGuiShutdown();
            AudioEngine.Shutdown();

            m_Window.Dispose();
        }
    }
}
