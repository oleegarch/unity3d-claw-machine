using UnityEngine;
using TMPro;

public class NegativeCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI description;

    public void SetDescription(string message) {
        description.SetText(message);
    }
}