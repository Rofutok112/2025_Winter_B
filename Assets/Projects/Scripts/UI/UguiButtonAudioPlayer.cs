using Projects.Scripts.Audio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Projects.Scripts.UI
{
    public class UguiButtonAudioPlayer : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private Button button;
        [SerializeField] private string pointerDownSeKey;
        [SerializeField] private string clickSeKey = "ButtonClick";
        [SerializeField, Range(0f, 1f)] private float pointerDownVolume = 0.45f;
        [SerializeField, Range(0f, 1f)] private float clickVolume = 0.6f;

        private void Awake()
        {
            if (button == null)
            {
                button = GetComponent<Button>();
            }
        }

        private void OnEnable()
        {
            if (button != null)
            {
                button.onClick.AddListener(HandleClick);
            }
        }

        private void OnDisable()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(HandleClick);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!IsInteractable() || string.IsNullOrWhiteSpace(pointerDownSeKey))
            {
                return;
            }

            AudioManager.PlayOneShot(pointerDownSeKey, pointerDownVolume);
        }

        private void HandleClick()
        {
            if (string.IsNullOrWhiteSpace(clickSeKey))
            {
                return;
            }

            AudioManager.PlayOneShot(clickSeKey, clickVolume);
        }

        private bool IsInteractable()
        {
            return button == null || button.IsInteractable();
        }
    }
}
