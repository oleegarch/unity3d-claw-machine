using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Cinemachine;

public class BoardHandler : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private new Camera camera;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private OrbitControls orbitControls;
    [SerializeField] private MachineController machine;
    [SerializeField] private BoardController board;
    [SerializeField] private Animator standForButtonAnimator;
    [SerializeField] private Animator standForMoverAnimator;
    [SerializeField] private Transform handleStand;
    [SerializeField] private Transform handleRotator;
    [SerializeField] private MeshCollider buttonCollider;
    [SerializeField] private SphereCollider handleCollider;
    [SerializeField] private float maxHandleAngle = 45f;
    [SerializeField] private float handleSensitivity = 1f;
    [SerializeField] private float machineSensitivity = 0.5f;

    private Vector2 handleDraggingDelta;
    private Quaternion handleInitRotation;
    private RawImage cameraTexture;
    private CinemachinePOV cinemachinePOV;
    private RectTransform rectTransform;

    private void Awake() {
        handleInitRotation = handleRotator.rotation;
        cameraTexture = GetComponent<RawImage>();
        cinemachinePOV = virtualCamera.GetCinemachineComponent<CinemachinePOV>();
        rectTransform = GetComponent<RectTransform>();
    }
    private void Update() {
        if(cameraTexture == null && board.handleDragging == false) {
            handleRotator.rotation = Quaternion.Lerp(handleRotator.rotation, handleInitRotation, Time.deltaTime * 10f);
        }
        if(cameraTexture == null) {
            Quaternion rotation = handleStand.localRotation;
            handleStand.localRotation = Quaternion.Euler(rotation.eulerAngles.x, rotation.eulerAngles.y, cinemachinePOV.m_HorizontalAxis.Value);
        }
    }
    
    public void OnPointerClick(PointerEventData eventData) {
        Ray ray = CreateRay(eventData.position);
        RaycastHit hit;

        if(buttonCollider.Raycast(ray, out hit, 100)) {
            standForButtonAnimator.SetTrigger("press");
            machine.OnBoardPress();
        }
    }
    public void OnBeginDrag(PointerEventData eventData) {
        Ray ray = CreateRay(eventData.position);
        RaycastHit hit;

        if(handleCollider.Raycast(ray, out hit, 100)) {
            board.handleDragging = true;
            orbitControls.enabled = false;
            standForMoverAnimator.SetBool("hold", true);
        }
    }
    public void OnDrag(PointerEventData eventData) {
        if(board.handleDragging) {
            handleDraggingDelta -= eventData.delta;
            Vector2 rotatedDelta = VectorsUtils.RotateVector2(handleDraggingDelta, 90f - cinemachinePOV.m_HorizontalAxis.Value);
            float angleX = Mathf.Clamp(rotatedDelta.x * handleSensitivity, -maxHandleAngle, maxHandleAngle);
            float angleY = Mathf.Clamp(rotatedDelta.y * handleSensitivity, -maxHandleAngle, maxHandleAngle);
            handleRotator.localRotation = Quaternion.Euler(angleX, angleY, 0f);
            machine.MoveZ(angleX / maxHandleAngle * machineSensitivity);
            machine.MoveX(angleY / maxHandleAngle * machineSensitivity);
        }
    }
    public void OnEndDrag(PointerEventData eventData) {
        board.handleDragging = false;
        handleDraggingDelta = Vector2.zero;
        orbitControls.enabled = true;
        standForMoverAnimator.SetBool("hold", false);
        machine.Unmove();
    }

    private Ray CreateRay(Vector3 pointerPosition) {
        if(cameraTexture == null) return camera.ScreenPointToRay(pointerPosition);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, pointerPosition, null, out Vector2 localClick);
        localClick.x = (rectTransform.rect.xMin * -1) - (localClick.x * -1);
        localClick.y = (rectTransform.rect.yMin * -1) - (localClick.y * -1);
        Vector2 viewportClick = new Vector2(localClick.x / rectTransform.rect.size.x, localClick.y / rectTransform.rect.size.y);
        return camera.ViewportPointToRay(new Vector3(viewportClick.x, viewportClick.y, 0));
    }
}