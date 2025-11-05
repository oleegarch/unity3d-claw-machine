using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Grabby;

#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif

namespace Grabby
{
    public enum AuthorizationPlatform
    {
        GooglePlayGames
    }
    public struct Authorization
    {
        public object id;
        public string platform;
    }
    public class AuthenticateException : Exception
    {
        public AuthenticateException(string text) : base(text) {
            Debug.LogError($"AuthenticateException message={text}");
        }
    }

    internal static class Authenticator
    {
        internal static string authId;
        internal static string accessToken;

#if UNITY_ANDROID
        async internal static Task<string> SignIn() {
            // if(Application.isEditor) {
            //     authId = "";
            //     return "";
            // }

            if(!string.IsNullOrEmpty(accessToken)) {
                return accessToken;
            }

            SignInStatus status = await Authenticate();

            if(status != SignInStatus.Success) {
                throw new AuthenticateException($"Ошибка авторизации в Google Play Games!");
            }

            authId = PlayGamesPlatform.Instance.GetUserId();

            string code = await RequestServerSideAccess();

            if(code.IndexOf("error:") == 0) {
                throw new AuthenticateException(code);
            }

            if(code == null) {
                throw new AuthenticateException("Ошибка при запросе серверного ключа в Google Play Games!");
            }

            accessToken = await API.GetUserAccessToken(AuthorizationPlatform.GooglePlayGames, code);
            return accessToken;
        }
        internal static Task<SignInStatus> Authenticate() {
            TaskCompletionSource<SignInStatus> tcs = new TaskCompletionSource<SignInStatus>();

            PlayGamesPlatform.Instance.Authenticate(status => tcs.TrySetResult(status));

            return tcs.Task;
        }
        internal static Task<string> RequestServerSideAccess(bool forceRefreshToken = false) {
            TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();

            PlayGamesPlatform.Instance.RequestServerSideAccess(forceRefreshToken, code => tcs.TrySetResult(code));

            return tcs.Task;
        }
#else
        async internal static Task<string> SignIn() {
            return "";
        }
#endif
    }
}