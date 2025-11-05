using UnityEngine;
using UnityEngine.EventSystems;
using Grabby;

public class CaughtLoadPreview : MonoBehaviour, IDragHandler
{
    [SerializeField] private Transform previewer;
    [SerializeField] private Transform cameraPreviewer;
    private GameObject prefab;

    public void SetLoad(Load load) {
        prefab = Instantiate(load.prefabPreview, previewer);
    }

    public void OnDrag(PointerEventData eventData) {
        if(Vector3.Dot(prefab.transform.up, Vector3.up) >= 0f) {
            prefab.transform.Rotate(prefab.transform.up, -Vector3.Dot(eventData.delta, cameraPreviewer.transform.right), Space.World);
        }
        else {
            prefab.transform.Rotate(prefab.transform.up, Vector3.Dot(eventData.delta, cameraPreviewer.transform.right), Space.World);
        }
        prefab.transform.Rotate(cameraPreviewer.transform.right, Vector3.Dot(eventData.delta, cameraPreviewer.transform.up), Space.World);
    }

    public void ClearLoad() {
        Destroy(prefab);
    }
}