using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Grabby.UI;

namespace Grabby.UI
{
    public class ModalPageClosure : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] private Modal modal;
        [SerializeField] private Animator animator;
        [SerializeField] private RectTransform backgroundRect;
        [SerializeField] private Image mask;

        [SerializeField] private float closeSpeed = 0.1f;

        private IEnumerator Move(Vector2 moveDirection, Color32 changeColor, bool close) {
            Vector2 startPosition = backgroundRect.anchoredPosition;
            Color startColor = mask.color;

            yield return CoroutineUtils.Lerp(closeSpeed, t => {
                backgroundRect.anchoredPosition = Vector2.Lerp(startPosition, moveDirection, t);
            });
            yield return CoroutineUtils.Lerp(closeSpeed, t => {
                mask.color = Color.Lerp(startColor, changeColor, t);
            });

            if(close) {
                modal.Close();
                modal.OnClosed();
            }

            animator.enabled = true;
        }

        public void OnDrag(PointerEventData eventData) {
            float canMoveTop = backgroundRect.anchoredPosition.y >= 0 ? 0 : Mathf.Abs(backgroundRect.anchoredPosition.y);
            backgroundRect.anchoredPosition += new Vector2(0f, eventData.delta.y >= 0 ? Mathf.Min(canMoveTop, eventData.delta.y) : eventData.delta.y);
            mask.color = new Color(0f,0f,0f, (1f - backgroundRect.anchoredPosition.y / -1631f) * (150f / 255f));
        }

        public void OnBeginDrag(PointerEventData eventData) {
            animator.enabled = false;
        }

        public void OnEndDrag(PointerEventData eventData) {
            float shift = Mathf.Abs(backgroundRect.anchoredPosition.y);

            if(shift > 100f) {
                StartCoroutine(Move(Vector2.down * 1631f, new Color32(0,0,0, 0), true));
            }

            else if(shift > 50f) {
                StartCoroutine(Move(Vector2.zero, new Color32(0,0,0, 100), false));
            }

            else {
                backgroundRect.anchoredPosition = Vector2.zero;
                animator.enabled = true;
            }
        }
    }
}