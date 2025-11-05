using System;
using UnityEngine;
using Grabby;
using Grabby.UI;

public class WinCollider : MonoBehaviour
{
    [SerializeField] private Loads loads;
    [SerializeField] private ModalOpener caughtLoadOpener;
    [SerializeField] private ModalOpener receivedOpener;
    [SerializeField] private MachineController machine;
    [SerializeField] private string toysLayerName = "Toy";
    private int toysLayer;
    private int newScore;
    private int newCoins;

    private void Awake() {
        toysLayer = LayerMask.NameToLayer(toysLayerName);
    }
    private void OnTriggerEnter(Collider other) {
        if(other.transform.gameObject.layer == toysLayer) {
            LoadManager loadManager = other.attachedRigidbody.transform.gameObject.GetComponent<LoadManager>();
            loadManager.isCaught = true;
            machine.wonLoads.Add(loadManager.index, loadManager.load);
            Destroy(other.attachedRigidbody.transform.gameObject);
            ShowCaughtLoadDialog(loadManager.load.name); // for game test
            Debug.Log($"Выиграли! {loadManager.index} {loadManager.load.name}");
        }
    }

    public CaughtLoadDialog ShowCaughtLoadDialog(Load load) {
        Modal caughtLoadModal = caughtLoadOpener.Open();
        CaughtLoadDialog caughtLoadDialog = caughtLoadModal.gameObject.GetComponent<CaughtLoadDialog>();
        caughtLoadDialog.SetLoad(load);
        caughtLoadDialog.SetOnEnded(OnEndedReceivedDialog);
        return caughtLoadDialog;
    }
    public CaughtLoadDialog ShowCaughtLoadDialog(string loadName) {
        return ShowCaughtLoadDialog(Array.Find(loads.items, load => load.name == loadName));
    }

    public ReceivedDialog ShowReceivedDialog(int score, int coins) {
        Modal receivedModal = receivedOpener.Open();
        ReceivedDialog receivedDialog = receivedModal.gameObject.GetComponent<ReceivedDialog>();
        receivedDialog.SetUp(score, coins);
        return receivedDialog;
    }
    public void SetData(int score, int coins) {
        newScore = score;
        newCoins = coins;
    }
    public void OnEndedReceivedDialog() {
        ShowReceivedDialog(newScore, newCoins);
        newScore = 0;
        newCoins = 0;
    }
}