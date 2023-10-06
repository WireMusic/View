using System;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;

using Stage.Utils;

namespace Stage.Renderer
{
    internal class GraphicsContext
    {
        private nint m_WindowHandle;
        private bool m_GladInit = false;

        private static bool s_WindowInit = false;

        public unsafe GraphicsContext(nint windowHandle, bool initContext = true)
        {
            m_WindowHandle = windowHandle;

            if (initContext)
            {
                _glfw.MakeContextCurrent(m_WindowHandle);

                nint glfwGetProcAddress = Marshal.GetFunctionPointerForDelegate(Window.glfwGetProcAddress);

                //nint glfwGetProcAddress = Marshal.GetFunctionPointerForDelegate<GetProcAddress>(method.CreateDelegate<GetProcAddress>());

                int status = _glad.LoadGLLoader((delegate* unmanaged[Cdecl]<char*, int>)glfwGetProcAddress);
                if (status != 1)
                {
                    Console.Error.WriteLine("Could not initialise glad!");
                    return;
                }

                m_GladInit = true;
            }

            if (!s_WindowInit)
            {
                Window.GLInit();
                s_WindowInit = true;
            }
        }

        public unsafe void MakeCurrent()
        {
            _glfw.MakeContextCurrent(m_WindowHandle);

            if (m_GladInit)
            {
                nint glfwGetProcAddress = Marshal.GetFunctionPointerForDelegate(Window.glfwGetProcAddress);

                int status = _glad.LoadGLLoader((delegate* unmanaged[Cdecl]<char*, int>)glfwGetProcAddress);
                if (status != 1)
                {
                    Console.Error.WriteLine("Could not initialise glad!");
                    return;
                }

                m_GladInit = true;
            }
        }

        public unsafe void SwapBuffers()
        {
            _glfw.SwapBuffers(m_WindowHandle);
        }
    }
}
