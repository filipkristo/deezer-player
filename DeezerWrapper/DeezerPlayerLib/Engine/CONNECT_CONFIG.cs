using System;
using System.Runtime.InteropServices;
// make this binding dependent on WPF, but easier to use

// http://www.codeproject.com/Articles/339290/PInvoke-pointer-safety-Replacing-IntPtr-with-unsaf

namespace DeezerPlayerLib.Engine
{
    [StructLayout(LayoutKind.Sequential)]
    public class CONNECT_CONFIG
    {
        public string ccAppId;

        public string ccProductId;
        public string ccProductBuildId;

        public IntPtr ccUserProfilePath;

        public libcConnectOnEventCb ccConnectEventCb;

        public string ccAnonymousBlob;

        public libcAppCrashDelegate ccAppCrashDelegate;

    }
}
