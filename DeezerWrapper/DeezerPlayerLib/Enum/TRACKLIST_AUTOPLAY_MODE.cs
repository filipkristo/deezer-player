// make this binding dependent on WPF, but easier to use

// http://www.codeproject.com/Articles/339290/PInvoke-pointer-safety-Replacing-IntPtr-with-unsaf

namespace DeezerPlayerLib.Enum
{
    public enum TRACKLIST_AUTOPLAY_MODE
    {
        DZ_INDEX_IN_QUEUELIST_INVALID = -1,

        DZ_INDEX_IN_QUEUELIST_PREVIOUS = -2,

        DZ_INDEX_IN_QUEUELIST_CURRENT = -3,

        DZ_INDEX_IN_QUEUELIST_NEXT = -4,        
    };
}
