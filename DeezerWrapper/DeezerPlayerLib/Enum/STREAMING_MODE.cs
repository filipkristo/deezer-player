// make this binding dependent on WPF, but easier to use

// http://www.codeproject.com/Articles/339290/PInvoke-pointer-safety-Replacing-IntPtr-with-unsaf

namespace DeezerPlayerLib.Enum
{    
    public enum STREAMING_MODE
    {
        DZ_STREAMING_MODE_UNKNOWN,  /**< Mode is not known or audio ad is playing. */
        DZ_STREAMING_MODE_ONDEMAND, /**< On demand streaming mode. */
        DZ_STREAMING_MODE_RADIO,    /**< Radio streaming mode. */
    }
}
