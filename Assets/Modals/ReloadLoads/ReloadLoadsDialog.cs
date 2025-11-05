using System;
using UnityEngine;
using Grabby;
using Grabby.UI;

public class ReloadLoadsDialog : MonoBehaviour
{
    [SerializeField] private ModalOpener negativeCardOpener;

    async public void Reload() {
        // string gameType = Store.settings.liveGame ? "live" : "test";

        // try {
        //     User user = Store.user;
        //     user.game = await API.GameReload(gameType);
        //     Store.user = user;
        //     LoadsController.currentController.Generate();
        //     GameSceneSetUp.currentGameScene.ShowTries();
        // } catch(Exception e) {
        //     Modal modal = negativeCardOpener.OpenAndClose();
        //     modal.GetComponent<NegativeCard>().SetDescription($"Не удалось обновить игрушки в автомате! {e.Message}");
        // }
    }
}