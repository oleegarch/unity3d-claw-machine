using UnityEngine;
using Coffee.UIExtensions;

public class UIParticleFallingConfetti : MonoBehaviour
{
    private UIParticle confetti;

    private void Awake() {
        confetti = GetComponent<UIParticle>();
    }

    public void StartFalling() {
        confetti.Play();
    }
    public void StopFalling() {
        confetti.Stop();
    }
}