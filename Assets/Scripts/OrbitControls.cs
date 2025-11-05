using UnityEngine;
using UnityEngine.EventSystems;
using Cinemachine;

public class OrbitControls : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    private CinemachineFramingTransposer cinemachineFramingTransposer;
    private CinemachinePOV cinemachinePOV;
    private Vector2 turn;

    public new bool enabled = true;
    
    private void Awake() {
        cinemachineFramingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        cinemachineFramingTransposer.m_CameraDistance = 10f;
        cinemachinePOV = virtualCamera.GetCinemachineComponent<CinemachinePOV>();
        cinemachinePOV.m_VerticalAxis.m_InputAxisName = "";
        cinemachinePOV.m_HorizontalAxis.m_InputAxisName = "";
    }

    private void Update() {
        if(Input.GetAxis("Mouse ScrollWheel") != 0f) {
            Zoom(Input.GetAxis("Mouse ScrollWheel") * 10f);
        }
    }

    public void OnDrag(PointerEventData eventData) {
        if(enabled == false) {
            return;
        }
        if(eventData.button == PointerEventData.InputButton.Right) {
            Vector2 mouseDelta = eventData.delta;
            float absX = Mathf.Abs(mouseDelta.x);
            float absY = Mathf.Abs(mouseDelta.y);
            if(absX > absY) {
                float delta = mouseDelta.x > 0f ? 0.1f : -0.1f;
                Zoom(delta);
            }
            else if(absY > absX) {
                float delta = mouseDelta.y > 0f ? 0.1f : -0.1f;
                Zoom(delta);
            }
        }
        else {
            Drag(eventData.delta);
        }
    }
    public void OnEndDrag(PointerEventData eventData) {
        turn = Vector2.zero;
        Drag(turn);
    }

    private void Drag(Vector2 delta) {
        cinemachinePOV.m_VerticalAxis.m_InputAxisValue = delta.y;
        cinemachinePOV.m_HorizontalAxis.m_InputAxisValue = delta.x;
    }
    private void Zoom(float delta) {
        cinemachineFramingTransposer.m_CameraDistance += delta;
        cinemachineFramingTransposer.m_CameraDistance = Mathf.Clamp(cinemachineFramingTransposer.m_CameraDistance, 2.5f, 10f);
    }
}