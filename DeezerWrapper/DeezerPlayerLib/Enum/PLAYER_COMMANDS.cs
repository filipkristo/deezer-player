// make this binding dependent on WPF, but easier to use

// http://www.codeproject.com/Articles/339290/PInvoke-pointer-safety-Replacing-IntPtr-with-unsaf

namespace DeezerPlayerLib.Enum
{
    public enum PLAYER_COMMANDS
    {
        UNKNOWN,           /**< Player command has not been set yet, not a valid value. */
        START_TRACKLIST,   /**< A new tracklist was loaded and a track played. */
        JUMP_IN_TRACKLIST, /**< The user jump into a new song in the current tracklist. */
        NEXT,              /**< Next button. */
        PREV,              /**< Prev button. */
        DISLIKE,           /**< Dislike button. */
        NATURAL_END,       /**< Natural end. */
        RESUMED_AFTER_ADS, /**< Reload after playing an ads. */
    }
}
