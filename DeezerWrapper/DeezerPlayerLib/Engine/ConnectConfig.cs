using System.Runtime.InteropServices;
// make this binding dependent on WPF, but easier to use

// http://www.codeproject.com/Articles/339290/PInvoke-pointer-safety-Replacing-IntPtr-with-unsaf

namespace DeezerPlayerLib.Engine
{    
    // to be in sync with dz_connect_configuration
    [StructLayout(LayoutKind.Sequential)]
    public class ConnectConfig
    {
        public string ccAppId;

        public string product_id;
        public string product_build_id;
        public string anonymousblob;        

        public string ccUserProfilePath;        
        public ConnectOnEventCb ccConnectEventCb;
    }
}
