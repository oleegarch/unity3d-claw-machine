using System.Collections;
using UnityEngine;
using Coffee.UIExtensions;

public class UIParticleBoomConfetti : MonoBehaviour
{
    private UIParticle confetti;
    private RectTransform rt;
    private Coroutine coroutine;

    private void Awake() {
        confetti = GetComponent<UIParticle>();
        rt = GetComponent<RectTransform>();
    }

    private IEnumerator Boom() {
        while(true) {
            rt.anchoredPosition = new Vector2(Random.Range(-300.0f, 300.0f), Random.Range(-700.0f, 700.0f));
            confetti.Play();
            yield return new WaitForSeconds(confetti.particles[0].main.duration);
        }
    }

    public void StartBoom() {
        coroutine = StartCoroutine(Boom());
    }
    public void StopBoom() {
        StopCoroutine(coroutine);
    }
}