using UnityEngine;

public class LoaderRotator : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 50f;
    [SerializeField] private Transform[] icons;

    private void FixedUpdate() {
        float zRotate = Time.fixedDeltaTime * rotateSpeed;
        transform.Rotate(0.0f, 0.0f, zRotate);
        foreach(Transform icon in icons) {
            icon.Rotate(0.0f, 0.0f, -zRotate);
        }
    }
}