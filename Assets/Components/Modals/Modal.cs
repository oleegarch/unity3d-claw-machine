using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Grabby.UI
{
    [Serializable]
    public class ModalEvent : UnityEvent<bool> {}

    public class Modal : MonoBehaviour
    {
        [SerializeField] private GameObject canvasObject;
        [SerializeField] private Animator animator;

        [SerializeField] private bool openOnAwake = false;

        [NonSerialized] public bool opened = false;

        public ModalEvent onStateChanged = new ModalEvent();
        public UnityEvent onOpen = new UnityEvent();
        public UnityEvent onClose = new UnityEvent();
        public UnityEvent onOpened = new UnityEvent();
        public UnityEvent onClosed = new UnityEvent();

        private void Awake() {
            if(openOnAwake) {
                Open();
            }
        }

        public void Open() {
            opened = true;
            canvasObject.SetActive(opened);
            animator.SetBool("isOn", opened);
            onStateChanged.Invoke(opened);
            onOpen.Invoke();
        }

        public void Close() {
            opened = false;
            animator.SetBool("isOn", opened);
            onStateChanged.Invoke(opened);
            onClose.Invoke();
        }
        public void CloseThrough(float secs = 3f) {
            StartCoroutine(CloseThroughCoroutine(secs));
        }
        private IEnumerator CloseThroughCoroutine(float secs = 3f) {
            yield return new WaitForSeconds(secs);
            Close();
        }

        public void OnOpened() {
            onOpened.Invoke();
        }
        public void OnClosed() {
            canvasObject.SetActive(opened);
            onClosed.Invoke();
        }
    }
}