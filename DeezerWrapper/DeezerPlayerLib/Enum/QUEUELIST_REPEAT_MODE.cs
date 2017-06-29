// make this binding dependent on WPF, but easier to use

// http://www.codeproject.com/Articles/339290/PInvoke-pointer-safety-Replacing-IntPtr-with-unsaf

namespace DeezerPlayerLib.Enum
{
    public enum QUEUELIST_REPEAT_MODE
    {
        DZ_QUEUELIST_REPEAT_MODE_OFF,          /**< Play the loaded content starting from the given track index in the queuelist. */
        DZ_QUEUELIST_REPEAT_MODE_ONE,          /**< Automatically play the current track forever. */
        DZ_QUEUELIST_REPEAT_MODE_ALL,          /**< Automatically play the entire queuelist forever with a natural order. */
    }
}
