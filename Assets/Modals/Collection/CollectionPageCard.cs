using UnityEngine;
using Grabby;

public class CollectionPageCard : MonoBehaviour
{
    [SerializeField] private Camera cameraForLoad;
    [SerializeField] private CollectionPageCardPreview preview;

    public bool rotate = false;
    private GameObject currentPrefabPreview;

    private void OnEnable() {
        rotate = true;
    }
    private void OnDisable() {
        rotate = false;
    }
    private void FixedUpdate() {
        if(rotate && currentPrefabPreview != null) {
            currentPrefabPreview.transform.Rotate(0.0f, Time.fixedDeltaTime * 10f, 0.0f);
        }
    }

    public void SetUp(Load load) {
        currentPrefabPreview = Instantiate(load.prefabPreview, transform);
        cameraForLoad.targetTexture = load.renderTexture;
        preview.SetUp(load, currentPrefabPreview, cameraForLoad);
    }
}