using UnityEngine;
using Grabby.UI;

namespace Grabby.UI
{
    public class ModalOpener : MonoBehaviour
    {
        [SerializeField] private GameObject modal;

        private GameObject currentModalObject;
        private Modal currentModal;
        
        public Modal Open() {
            if(currentModalObject == null) {
                currentModalObject = Instantiate(modal);
                currentModal = currentModalObject.GetComponent<Modal>();
            }
            currentModal.Open();
            return currentModal;
        }
        public void JustOpen() {
            Open();
        }
        public Modal Close() {
            if(currentModalObject == null) return null;
            currentModal.Close();
            return currentModal;
        }
        public void JustClose() {
            Close();
        }
        public Modal Toggle(bool isOn) {
            if(isOn) {
                return Open();
            }
            else {
                return Close();
            }
        }
        public void JustToggle(bool isOn) {
            Toggle(isOn);
        }
        public Modal OpenAndClose(float secs = 3f) {
            Modal modal = Open();
            modal.CloseThrough(secs);
            return modal;
        }
        public void JustOpenAndClose(float secs = 3f) {
            OpenAndClose(secs);
        }
    }
}