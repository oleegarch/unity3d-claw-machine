using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Grabby;


public class CollectionPageCardPreview : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private RawImage preview;
    [SerializeField] private CollectionPageCard card;

    private GameObject prefab;
    private Camera cameraPreviewer;

    public void OnBeginDrag(PointerEventData eventData) {
        card.rotate = false;
    }
    public void OnEndDrag(PointerEventData eventData) {
        card.rotate = true;
    }
    
    public void OnDrag(PointerEventData eventData) {
        if(prefab == null || cameraPreviewer == null) {
            return;
        }
        if(Vector3.Dot(prefab.transform.up, Vector3.up) >= 0f) {
            prefab.transform.Rotate(prefab.transform.up, -Vector3.Dot(eventData.delta, cameraPreviewer.transform.right), Space.World);
        }
        else {
            prefab.transform.Rotate(prefab.transform.up, Vector3.Dot(eventData.delta, cameraPreviewer.transform.right), Space.World);
        }
        prefab.transform.Rotate(cameraPreviewer.transform.right, Vector3.Dot(eventData.delta, cameraPreviewer.transform.up), Space.World);
    }

    public void SetUp(Load load, GameObject prefabPreview, Camera cameraForLoad) {
        preview.texture = load.renderTexture;
        prefab = prefabPreview;
        cameraPreviewer = cameraForLoad;
    }
}