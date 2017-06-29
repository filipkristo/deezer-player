using System;
using System.Runtime.InteropServices;
// make this binding dependent on WPF, but easier to use

// http://www.codeproject.com/Articles/339290/PInvoke-pointer-safety-Replacing-IntPtr-with-unsaf

namespace DeezerPlayerLib.Engine
{
    // trick from http://stackoverflow.com/questions/1573724/cpu-architecture-independent-p-invoke-can-the-dllname-or-path-be-dynamic
    // but actually SetDllDirectory works better (for pthread.dll)
    public static class NativeMethods
    {
        // call this to load this class
        public static void LoadClass()
        {
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetDllDirectory(string lpPathName);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern IntPtr LoadLibrary(string lpFileName);

        static NativeMethods()
        {
            string arch;
            string basePath = System.IO.Path.GetDirectoryName(typeof(NativeMethods).Assembly.Location);

            if (IntPtr.Size == 4)
                arch = "i386";
            else
                arch = "x86_64";

            System.Diagnostics.Debug.WriteLine("using arch: " + arch);

            SetDllDirectory(System.IO.Path.Combine(basePath, arch));
#if false // can be used to debug library loading
        IntPtr hExe = LoadLibrary("libdeezer.x86.dll");

        if (hExe == IntPtr.Zero)
        {
            Win32Exception ex = new Win32Exception(Marshal.GetLastWin32Error());
            System.Console.WriteLine("exception:" + ex);
            throw ex;
        }
#endif
        }

    }
}
