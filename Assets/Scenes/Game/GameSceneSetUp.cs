using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Grabby;
using Grabby.UI;

public class GameSceneSetUp : MonoBehaviour
{
    public static GameSceneSetUp currentGameScene;

    [SerializeField] private TextMeshProUGUI triesTitle;
    [SerializeField] private ModalOpener negativeCardOpener;
    [SerializeField] private TrainingManager trainingManager;
    [SerializeField] private Toggle liveGameToggle;

    private void Awake()
    {
        trainingManager.ShowTraining();
    }

    // private void Awake() {
    //     currentGameScene = this;

    //     liveGameToggle.isOn = Store.settings.liveGame;

    //     if(Store.user.training.gameScene == false) {
    //         trainingManager.ShowTraining();
    //     }

    //     ShowTries();
    // }

    // async public void SaveTraining() {
    //     try {
    //         User user = Store.user;
    //         user.training = await API.SaveTraining("gameScene");
    //         Store.user = user;
    //     } catch(Exception e) {
    //         Modal modal = negativeCardOpener.OpenAndClose();
    //         modal.GetComponent<NegativeCard>().SetDescription($"Не удалось сохранить состояние пройденности обучения игры на сервере! {e.Message}");
    //     }
    // }

    // public void LiveGameToggleValueChanged() {
    //     Settings settings = Store.settings;
    //     settings.liveGame = liveGameToggle.isOn;
    //     Store.settings = settings;
    // }

    // public void ShowTries() {
    //     triesTitle.SetText($"У тебя есть {Store.currentGameTries} попыток");
    // }
}