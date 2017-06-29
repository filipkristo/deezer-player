using DeezerPlayerLib.Enum;
using System.Runtime.InteropServices;
// make this binding dependent on WPF, but easier to use

// http://www.codeproject.com/Articles/339290/PInvoke-pointer-safety-Replacing-IntPtr-with-unsaf

namespace DeezerPlayerLib.Engine
{
    public class PlayerEvent
    {
        public PLAYER_EVENT_TYPE eventType;

        /* two design strategies:
     * - we could keep a reference to PLAYER_EVENT* with dz_object_retain and call method on the fly
     * - we extract all info in constructor and have pure managed object
     * 
     * here we keep the second option, because we have to have a managed object anyway, and it's 
     * a lot fewer unsafe method to expose, even though it's making a lot of calls in the constructor..
     */
        public unsafe static PlayerEvent newFromLibcEvent(PLAYER_EVENT* libcPlayerEventHndl)
        {
            PLAYER_EVENT_TYPE eventType;
            unsafe
            {
                eventType = dz_player_event_get_type(libcPlayerEventHndl);
            }
            switch (eventType)
            {
                default:
                    return new PlayerEvent(eventType);
            }
        }

        public PlayerEvent(PLAYER_EVENT_TYPE eventType)
        {
            this.eventType = eventType;
        }

        public PLAYER_EVENT_TYPE GetEventType()
        {
            return eventType;
        }

        [DllImport("libdeezer.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe PLAYER_EVENT_TYPE dz_player_event_get_type(PLAYER_EVENT* dzPlayerEvent);
    }
}
