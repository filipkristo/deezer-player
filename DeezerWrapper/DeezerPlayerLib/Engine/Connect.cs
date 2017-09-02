using DeezerPlayerLib.Enum;
using System;
using System.Collections;
using System.Runtime.InteropServices;
// make this binding dependent on WPF, but easier to use

// http://www.codeproject.com/Articles/339290/PInvoke-pointer-safety-Replacing-IntPtr-with-unsaf

namespace DeezerPlayerLib.Engine
{
    unsafe public class Connect : IDisposable
    {        
        static Hashtable refKeeper = new Hashtable();

        internal unsafe CONNECT* libcConnectHndl;
        internal ConnectConfig connectConfig;

        private libcOnCacheEvent onCache;

        public unsafe Connect(ConnectConfig cc)
        {
            NativeMethods.LoadClass();
            //ConsoleHelper.AllocConsole();
            // attach a console to parent process (launch from cmd.exe)
            //ConsoleHelper.AttachConsole(-1);

            CONNECT_CONFIG libcCc = new CONNECT_CONFIG();
            connectConfig = cc;
            IntPtr intptr = new IntPtr(this.GetHashCode());

            refKeeper[intptr] = this;

            libcCc.ccAppId = cc.ccAppId;
            libcCc.ccAnonymousBlob = cc.anonymousblob;
            //libcCc.ccAppSecret = cc.ccAppSecret;
            libcCc.ccProductBuildId = cc.product_build_id;
            libcCc.ccProductId = cc.product_id;
            libcCc.ccUserProfilePath = UTF8Marshaler.GetInstance(null).MarshalManagedToNative(cc.ccUserProfilePath);
            libcCc.ccConnectEventCb = delegate (CONNECT* libcConnect, CONNECT_EVENT* libcConnectEvent, IntPtr userdata)
            {
                Connect connect = (Connect)refKeeper[userdata];
                ConnectEvent connectEvent = ConnectEvent.newFromLibcEvent(libcConnectEvent);                

                connect.connectConfig.ccConnectEventCb.Invoke(connect, connectEvent);
            };

            libcConnectHndl = dz_connect_new(libcCc);

            UTF8Marshaler.GetInstance(null).CleanUpNativeData(libcCc.ccUserProfilePath);            
        }

        public ERRORS Start()
        {
            ERRORS ret;
            ret = dz_connect_activate(libcConnectHndl, new IntPtr(this.GetHashCode()));
            return ret;
        }

        public string GetDeviceId()
        {
            string deviceId = null;
            var libcDeviceId = dz_connect_get_device_id(libcConnectHndl);

            if (libcDeviceId != null)
                deviceId = Marshal.PtrToStringAnsi(libcDeviceId);

            return deviceId;
        }

        public string GetSdkVersion()
        {
            string version = null;
            var libcDeviceId = dz_connect_get_build_id();

            if (libcDeviceId != null)
                version = Marshal.PtrToStringAnsi(libcDeviceId);

            return version;
        }

        public ERRORS SetAccessToken(string accessToken)
        {
            ERRORS ret;
            ret = dz_connect_set_access_token(libcConnectHndl, IntPtr.Zero, IntPtr.Zero, accessToken);
            return ret;
        }

        public ERRORS SetSmartCache(string path, uint quotaKb)
        {
            ERRORS ret;
            ret = dz_connect_cache_path_set(libcConnectHndl, IntPtr.Zero, IntPtr.Zero, path);
            ret = dz_connect_smartcache_quota_set(libcConnectHndl, IntPtr.Zero, IntPtr.Zero, quotaKb);
            return ret;
        }

        public ERRORS ConnectOfflineMode()
        {
            ERRORS ret;
            ret = dz_connect_offline_mode(libcConnectHndl, IntPtr.Zero, IntPtr.Zero, false);
            return ret;
        }

        public void SmartCacheEvent()
        {
            //onCache = new libcOnCacheEvent((libcConnect, libcConnectEvent, userdata) =>
            //{
            //    Console.WriteLine(userdata);
            //});

            //dz_connect_cache_eventcb_set(libcConnectHndl, IntPtr.Zero, IntPtr.Zero, onCache);
        }

        [DllImport("libdeezer.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe CONNECT* dz_connect_new(
            [In, MarshalAs(UnmanagedType.LPStruct)]
            CONNECT_CONFIG lpcc);

        [DllImport("libdeezer.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe IntPtr dz_connect_get_device_id(
            CONNECT* dzConnect);

        [DllImport("libdeezer.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe ERRORS dz_connect_activate(
            CONNECT* dzConnect, IntPtr userdata);

        [DllImport("libdeezer.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe ERRORS dz_connect_set_access_token(
            CONNECT* dzConnect, IntPtr cb, IntPtr userdata, string access_token);

        [DllImport("libdeezer.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe ERRORS dz_connect_cache_path_set(
            CONNECT* dzConnect, IntPtr cb, IntPtr userdata,
            [MarshalAs(UnmanagedType.CustomMarshaler,
              MarshalTypeRef=typeof(UTF8Marshaler))]
              string local_path);

        [DllImport("libdeezer.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe ERRORS dz_connect_cache_eventcb_set(
            CONNECT* dzConnect, IntPtr cb, IntPtr userdata, libcOnCacheEvent onCache);

        [DllImport("libdeezer.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe ERRORS dz_connect_smartcache_quota_set(
            CONNECT* dzConnect, IntPtr cb, IntPtr userdata,
              uint quota_kB);

        [DllImport("libdeezer.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe ulong dz_connect_cache_current_size(
            CONNECT* dzConnect);

        [DllImport("libdeezer.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe ERRORS dz_connect_cache_flush(CONNECT* dzConnect, IntPtr cb, IntPtr userData);

        [DllImport("libdeezer.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe ERRORS dz_connect_offline_mode(CONNECT* dzConnect, IntPtr cb, IntPtr userData, bool offlineMode);

        [DllImport("libdeezer.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe ERRORS dz_player_deactivate(
            CONNECT* dzConnect, IntPtr cb, IntPtr userdata);

        [DllImport("libdeezer.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe IntPtr dz_connect_get_build_id();

        public void Dispose()
        {
            dz_player_deactivate(libcConnectHndl, IntPtr.Zero, IntPtr.Zero);
        }
    }
}
