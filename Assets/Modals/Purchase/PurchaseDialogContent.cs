using UnityEngine;
using UnityEngine.UI;

public class PurchaseDialogContent : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;

    public void EnableScrolling() {
        scrollRect.enabled = true;
    }
    public void DisableScrolling() {
        scrollRect.enabled = false;
    }
}