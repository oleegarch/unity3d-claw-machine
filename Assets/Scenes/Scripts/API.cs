using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Grabby;

namespace Grabby
{
    public struct APIError
    {
        public string message;
    }
    public class APIException : Exception
    {
        public APIException(string json) : base(ErrorToMessage(JsonUtility.FromJson<APIError>(json).message)) {
            Debug.LogError($"APIException {json}");
        }

        public static string ErrorToMessage(string errorString) {
            switch(errorString) {
                case "tries_not_enough": {
                    return "Недостаточно попыток!";
                }
                default: {
                    return errorString;
                }
            }
        }
    }
    public struct APIGameStartedResponse
    {
        public int leftTries;
    }
    public struct APIGameEndedResponse
    {
        public string[] simulatedWonLoads;
        public GameScene endScene;
        public int newScore;
        public int newCoins;
        public UserGame userGame;
    }
    [Serializable]
    public struct APIMachineControlStateRequest
    {
        public MachineControlState[] value;
        public GameScene startScene;
        public GameScene endScene;
        public string[] wonLoads;
    }

    internal static class API
    {
        private static string accessToken;

        async internal static Task<string> Call(string method, WWWForm form) {
            string query = $"?authId={Authenticator.authId}";
            string url = Config.API_URL + method + query;

            UnityWebRequest request = UnityWebRequest.Post(url, form);
            request.SendWebRequest();

            while(!request.isDone) {
                await Task.Yield();
            }

            if(request.responseCode != 200) {
                throw new APIException(request.downloadHandler.text);
            }

            return request.downloadHandler.text;
        }
        async internal static Task<string> GetUserAccessToken(AuthorizationPlatform authPlatform, string code) {
            WWWForm form = new WWWForm();
            form.AddField("platform", authPlatform.ToString());
            form.AddField("payload", code);

            string accessToken = await Call("/users/getAccessToken", form);
            return accessToken;
        }
        async internal static Task<string> CallWithAccess(string method, WWWForm form) {
            if(string.IsNullOrEmpty(accessToken)) {
                accessToken = await Authenticator.SignIn();
            }

            form.AddField("accessToken", accessToken);

            return await Call(method, form);
        }
        async internal static Task<User> GetLocalUser() {
            WWWForm form = new WWWForm();

            string json = await CallWithAccess("/users/getLocalUser", form);
            User result = JsonUtility.FromJson<User>(json);

            return result;
        }
        async internal static Task<UserTraining> SaveTraining(string trainingType) {
            WWWForm form = new WWWForm();

            form.AddField("trainingType", trainingType);

            string json = await CallWithAccess("/users/saveTraining", form);
            UserTraining result = JsonUtility.FromJson<UserTraining>(json);

            return result;
        }
        async internal static Task<Settings> ChangeSettings(Settings settings) {
            WWWForm form = new WWWForm();

            form.AddField("settings", JsonUtility.ToJson(settings));

            string json = await CallWithAccess("/users/changeSettings", form);
            Settings result = JsonUtility.FromJson<Settings>(json);

            return result;
        }
        async internal static Task<UserGame> GameReload(string gameType) {
            WWWForm form = new WWWForm();

            form.AddField("gameType", gameType);

            string json = await CallWithAccess("/users/game/reload", form);
            UserGame result = JsonUtility.FromJson<UserGame>(json);

            return result;
        }
        async internal static Task<APIGameStartedResponse> GameStarted(
            string gameType,
            GameScene scene
        ) {
            WWWForm form = new WWWForm();

            form.AddField("gameType", gameType);
            form.AddField("gameScene", JsonUtility.ToJson(scene));

            string json = await CallWithAccess("/users/game/started", form);
            APIGameStartedResponse result = JsonUtility.FromJson<APIGameStartedResponse>(json);

            return result;
        }
        async internal static Task<APIGameEndedResponse> GameEnded(
            string gameType,
            List<MachineControlState> controlState,
            GameScene gameScene,
            string[] wonLoads
        ) {
            WWWForm form = new WWWForm();

            APIMachineControlStateRequest request;
            request.value = controlState.ToArray();
            request.startScene = new GameScene();
            request.endScene = gameScene;
            request.wonLoads = wonLoads;

            form.AddField("gameType", gameType);
            form.AddField("controlState", JsonUtility.ToJson(request));

            string json = await CallWithAccess("/users/game/ended", form);
            APIGameEndedResponse result = JsonUtility.FromJson<APIGameEndedResponse>(json);

            return result;
        }
    }
}