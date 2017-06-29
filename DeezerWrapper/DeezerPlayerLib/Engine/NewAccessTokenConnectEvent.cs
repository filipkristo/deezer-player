// make this binding dependent on WPF, but easier to use

// http://www.codeproject.com/Articles/339290/PInvoke-pointer-safety-Replacing-IntPtr-with-unsaf

using DeezerPlayerLib.Enum;

namespace DeezerPlayerLib.Engine
{
    public class NewAccessTokenConnectEvent : ConnectEvent
    {
        string accessToken;

        public NewAccessTokenConnectEvent(string accessToken)
        : base(CONNECT_EVENT_TYPE.USER_ACCESS_TOKEN_OK)
        {
            this.accessToken = accessToken;
        }

        public string GetAccessToken()
        {
            return accessToken;
        }
    }
}
