using DeezerPlayerLib.Enum;
using System;
using System.Runtime.InteropServices;
using System.Text;
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

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

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

        public static IntPtr NativeUtf8FromString(string managedString)
        {
            var len = Encoding.UTF8.GetByteCount(managedString);
            var buffer = new byte[len + 1];
            Encoding.UTF8.GetBytes(managedString, 0, managedString.Length, buffer, 0);
            var nativeUtf8 = Marshal.AllocHGlobal(buffer.Length);
            Marshal.Copy(buffer, 0, nativeUtf8, buffer.Length);
            return nativeUtf8;
        }

        public static string StringFromNativeUtf8(IntPtr nativeUtf8)
        {
            if (nativeUtf8 == IntPtr.Zero)
                return null;

            var len = 0;
            while (Marshal.ReadByte(nativeUtf8, len) != 0) ++len;
            var buffer = new byte[len];
            Marshal.Copy(nativeUtf8, buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer);
        }

    }
}
