using System;
using UnityEngine;
using TMPro;
using Grabby;
using Grabby.UI;

public class MainSceneSetUp : MonoBehaviour
{
    [SerializeField] private ModalOpener negativeCardOpener;
    [SerializeField] private SceneChanger sceneChanger;
    [SerializeField] private TrainingManager trainingManager;
    [SerializeField] private TextMeshProUGUI counterScore;
    [SerializeField] private TextMeshProUGUI counterLoads;
    [SerializeField] private TextMeshProUGUI counterCoins;

    private void Awake() {
        counterScore.SetText($"{Store.user.score}");
        counterLoads.SetText($"{Store.user.loadsCount}");
        counterCoins.SetText($"{Store.user.coins}");

        if(Store.user.training.mainScene == false) {
            trainingManager.ShowTraining();
        }
    }

    public void GoToGame(bool isLive = true) {
        Settings settings = Store.settings;
        settings.liveGame = isLive;
        Store.settings = settings;
        sceneChanger.Change("Game");
    }

    async public void SaveTraining() {
        try {
            User user = Store.user;
            user.training = await API.SaveTraining("mainScene");
            Store.user = user;
        } catch(Exception e) {
            Modal modal = negativeCardOpener.OpenAndClose();
            modal.GetComponent<NegativeCard>().SetDescription($"Не удалось сохранить состояние пройденности обучения на сервере! {e.Message}");
        }
    }
}