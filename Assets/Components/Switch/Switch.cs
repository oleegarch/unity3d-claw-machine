using UnityEngine;
using UnityEngine.UI;

namespace Grabby.UI
{
    public class Switch : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private UnityEngine.UI.Toggle toggle;

        public bool isOn;

        private void OnEnable() {
            changeAnimatorState();
        }

        private void Update() {
            if(toggle.isOn != isOn) {
                changeAnimatorState();
            }
        }

        private void changeAnimatorState() {
            isOn = toggle.isOn;
            animator.SetBool("isOn", isOn);
        }

        public void Toggle() {
            isOn = toggle.isOn = !toggle.isOn;
            changeAnimatorState();
        }
    }
}