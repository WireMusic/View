using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Stage.Core
{
    internal class Library : IDisposable
    {
        [DllImport("kernel32.dll")]
        private static extern nint LoadLibrary(string path);

        [DllImport("kernel32.dll")]
        private static extern nint GetProcAddress(nint libraryPtr, string name);

        [DllImport("kernel32.dll")]
        private static extern void FreeLibrary(nint libraryPtr);

        [DllImport("kernel32.dll")]
        private static extern int GetLastError();

        private nint m_Ptr;
        private string m_Name;

        private static List<string> ToDelete;

        public Library(string name)
        {
            m_Ptr = LoadLibrary(name);
            m_Name = name;
            int err = GetLastError();

            if (m_Ptr == nint.Zero)
                throw new ArgumentException($"Could not load library ({err}).");
        }

        public Library(string name, byte[] data)
        {
            if (File.Exists($"{name}.dll"))
                File.Delete($"{name}.dll");
            FileStream dll = new FileStream($"{name}.dll", FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite, 0, FileOptions.None);
            dll.Write(data);
            dll.Close();

            ToDelete.Add($"{name}.dll");
            
            m_Ptr = LoadLibrary($"{name}.dll");
            m_Name = name;
            int err = GetLastError();

            if (m_Ptr == nint.Zero)
                throw new ArgumentException($"Could not load library ({err}).");
        }

        static Library()
        {
            ToDelete = new List<string>();
            Cleanup();
        }

        public nint Load(string name)
        {
            nint address = GetProcAddress(m_Ptr, name);

            int err = GetLastError();

            if (address == nint.Zero)
                throw new Exception();

            return address;
        }

        public void Dispose()
        {
            FreeLibrary(m_Ptr);
        }

        public static void Cleanup()
        {
            foreach (string file in ToDelete)
            {
                if (File.Exists(file))
                    File.Delete(file);
            }
        }
    }
}
