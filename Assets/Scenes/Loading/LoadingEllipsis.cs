using System.Collections;
using UnityEngine;
using TMPro;

public class LoadingEllipsis : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI loading;
    [SerializeField] private float dotConcatInterval = 0.5f;

    private void Start() {
        StartCoroutine(Ellipsis("LOADING"));
    }

    private IEnumerator Ellipsis(string prefix) {
        while(true) {
            loading.SetText(prefix);
            yield return new WaitForSeconds(dotConcatInterval);
            loading.SetText(prefix + ".");
            yield return new WaitForSeconds(dotConcatInterval);
            loading.SetText(prefix + "..");
            yield return new WaitForSeconds(dotConcatInterval);
            loading.SetText(prefix + "...");
            yield return new WaitForSeconds(dotConcatInterval);
        }
    }
}