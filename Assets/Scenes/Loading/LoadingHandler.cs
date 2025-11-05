using System;
using UnityEngine;
using TMPro;
using Grabby;

public class LoadingHandler : MonoBehaviour
{
    [SerializeField] private GameObject buttonSignIn;
    [SerializeField] private TextMeshProUGUI loadingError;
    [SerializeField] private SceneChanger sceneChanger;

    private void Start() {
#if UNITY_SERVER
        sceneChanger.Change("Simulation");
        return;
#endif
        LoadUser();
    }

    async public void LoadUser() {
        try {
            Store.user = await API.GetLocalUser();
            Store.settings = Store.user.settings;
            sceneChanger.Change("Main");
        } catch(Exception e) {
            HandleError(e.Message);
        }
    }

    public void HandleError(string message) {
        buttonSignIn.SetActive(true);
        loadingError.gameObject.SetActive(true);
        loadingError.SetText(message);
    }

    public void SignIn() {
        buttonSignIn.SetActive(false);
        loadingError.gameObject.SetActive(false);
        loadingError.SetText("");
        LoadUser();
    }
}
