using System;
using System.Runtime.InteropServices;
// make this binding dependent on WPF, but easier to use

// http://www.codeproject.com/Articles/339290/PInvoke-pointer-safety-Replacing-IntPtr-with-unsaf

namespace DeezerPlayerLib.Engine
{      
    public delegate void ConnectOnEventCb(Connect connect, ConnectEvent connectEvent);
    public delegate void PlayerOnEventCb(Player player, PlayerEvent playerEvent);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    unsafe public delegate void libcConnectOnEventCb(CONNECT* libcConnect, CONNECT_EVENT* libcConnectEvent, IntPtr userdata);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    unsafe public delegate bool libcAppCrashDelegate();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    unsafe public delegate void libcPlayerOnEventCb(PLAYER* libcPlayer, PLAYER_EVENT* libcPlayerEvent, IntPtr userdata);    

    unsafe public struct CONNECT_EVENT { };

    unsafe public struct UTF8STRING { };

    unsafe public struct CONNECT { };

    unsafe public struct PLAYER_EVENT { };

    unsafe public struct PLAYER { };
}
