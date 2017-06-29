// make this binding dependent on WPF, but easier to use

// http://www.codeproject.com/Articles/339290/PInvoke-pointer-safety-Replacing-IntPtr-with-unsaf

namespace DeezerPlayerLib.Enum
{
    public enum CONNECT_EVENT_TYPE
    {
        UNKNOWN,                           /**< Connect event has not been set yet, not a valid value. */
        USER_OFFLINE_AVAILABLE,            /**< User logged in, and credentials from offline store are loaded. */

        USER_ACCESS_TOKEN_OK,              /**< (Not available) dz_connect_login_with_email() ok, and access_token is available */
        USER_ACCESS_TOKEN_FAILED,          /**< (Not available) dz_connect_login_with_email() failed */

        USER_LOGIN_OK,                     /**< Login with access_token ok, infos from user available. */
        USER_LOGIN_FAIL_NETWORK_ERROR,     /**< Login with access_token failed because of network condition. */
        USER_LOGIN_FAIL_BAD_CREDENTIALS,   /**< Login with access_token failed because of bad credentials. */
        USER_LOGIN_FAIL_USER_INFO,         /**< Login with access_token failed because of other problem. */
        USER_LOGIN_FAIL_OFFLINE_MODE,      /**< Login with access_token failed because we are in forced offline mode. */

        USER_NEW_OPTIONS,                  /**< User options have just changed. */

        ADVERTISEMENT_START,               /**< A new advertisement needs to be displayed. */
        ADVERTISEMENT_STOP,                /**< An advertisement needs to be stopped. */
    };
}
