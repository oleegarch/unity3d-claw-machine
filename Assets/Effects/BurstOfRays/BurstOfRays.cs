using UnityEngine;

public class BurstOfRays : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    private RectTransform rt;

    private void Awake() {
        rt = GetComponent<RectTransform>();
    }

    private void Update() {
        float rotation = (Time.time * speed) % 360f;
        rt.localRotation = Quaternion.Euler(0f, 0f, rotation);
    }
}