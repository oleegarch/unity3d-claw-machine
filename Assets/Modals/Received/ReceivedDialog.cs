using UnityEngine;
using TMPro;
using Grabby;
using Grabby.UI;

public class ReceivedDialog : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreTitle;
    [SerializeField] private TextMeshProUGUI coinsTitle;
    private Modal modal;
    
    private void Awake() {
        modal = GetComponent<Modal>();
    }

    public void SetUp(int score, int coins) {
        scoreTitle.SetText($"{score} ОПЫТА");
        coinsTitle.SetText($"{coins} МОНЕТОК");
    }
}