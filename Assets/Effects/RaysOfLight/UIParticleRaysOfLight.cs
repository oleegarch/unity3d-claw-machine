using UnityEngine;
using Coffee.UIExtensions;

public class UIParticleRaysOfLight : MonoBehaviour
{
    private UIParticle confetti;

    private void Awake() {
        confetti = GetComponent<UIParticle>();
    }

    public void StartWarpDrive() {
        confetti.Play();
    }
    public void StopWarpDrive() {
        confetti.Stop();
    }
}