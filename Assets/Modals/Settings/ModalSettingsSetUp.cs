using System;
using UnityEngine;
using UnityEngine.UI;
using Grabby;
using Grabby.UI;

public class ModalSettingsSetUp : MonoBehaviour
{
    [SerializeField] private Toggle soundsToggle;
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Toggle vibrationToggle;
    [SerializeField] private Toggle leadersToggle;
    [SerializeField] private ModalOpener negativeCardOpener;

    private void Awake() {
        soundsToggle.isOn = Store.settings.sounds;
        soundsToggle.onValueChanged.AddListener(delegate {
            Settings settings = Store.settings;
            settings.sounds = soundsToggle.isOn;
            ChangeStoreSettings(settings);
        });
        musicToggle.isOn = Store.settings.music;
        musicToggle.onValueChanged.AddListener(delegate {
            Settings settings = Store.settings;
            settings.music = musicToggle.isOn;
            ChangeStoreSettings(settings);
        });
        vibrationToggle.isOn = Store.settings.vibration;
        vibrationToggle.onValueChanged.AddListener(delegate {
            Settings settings = Store.settings;
            settings.vibration = vibrationToggle.isOn;
            ChangeStoreSettings(settings);
        });
        leadersToggle.isOn = Store.settings.hiddenInLeaders;
        leadersToggle.onValueChanged.AddListener(delegate {
            Settings settings = Store.settings;
            settings.hiddenInLeaders = leadersToggle.isOn;
            ChangeStoreSettings(settings);
        });
    }

    async private void ChangeStoreSettings(Settings settings) {
        try {
            Store.settings = await API.ChangeSettings(settings);
        } catch(Exception e) {
            Modal modal = negativeCardOpener.OpenAndClose();
            modal.GetComponent<NegativeCard>().SetDescription($"Не удалось сохранить настройки на сервере! {e.Message}");
        }
    }
}