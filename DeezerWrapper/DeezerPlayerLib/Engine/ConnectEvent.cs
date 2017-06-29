using DeezerPlayerLib.Enum;
using System;
using System.Runtime.InteropServices;
// make this binding dependent on WPF, but easier to use

// http://www.codeproject.com/Articles/339290/PInvoke-pointer-safety-Replacing-IntPtr-with-unsaf

namespace DeezerPlayerLib.Engine
{
    public class ConnectEvent
    {
        public CONNECT_EVENT_TYPE eventType;

        /* two design strategies:
     * - we could keep a reference to CONNECT_EVENT* with dz_object_retain and call method on the fly
     * - we extract all info in constructor and have pure managed object
     * 
     * here we keep the second option, because we have to have a managed object anyway, and it's 
     * a lot fewer unsafe method to expose, even though it's making a lot of calls in the constructor..
     */
        public unsafe static ConnectEvent newFromLibcEvent(CONNECT_EVENT* libcConnectEventHndl)
        {
            CONNECT_EVENT_TYPE eventType;
            unsafe
            {
                eventType = dz_connect_event_get_type(libcConnectEventHndl);
            }
            switch (eventType)
            {
                case CONNECT_EVENT_TYPE.USER_ACCESS_TOKEN_OK:
                    string accessToken;
                    unsafe
                    {
                        IntPtr libcAccessTokenString = dz_connect_event_get_access_token(libcConnectEventHndl);
                        accessToken = Marshal.PtrToStringAnsi(libcAccessTokenString);
                    }
                    return new NewAccessTokenConnectEvent(accessToken);
                default:
                    return new ConnectEvent(eventType);
            }
        }

        public ConnectEvent(CONNECT_EVENT_TYPE eventType)
        {
            this.eventType = eventType;
        }

        public CONNECT_EVENT_TYPE GetEventType()
        {
            return eventType;
        }

        [DllImport("libdeezer.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe CONNECT_EVENT_TYPE dz_connect_event_get_type(CONNECT_EVENT* dzConnectEvent);

        [DllImport("libdeezer.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe IntPtr dz_connect_event_get_access_token(CONNECT_EVENT* dzConnectEvent);
    }
}
