using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Projects.Scripts.UI
{
    public class TutorialOverlayPresenter : MonoBehaviour
    {
        [Header("Root")]
        [SerializeField] private GameObject rootObject;

        [Header("Content")]
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private RectTransform tapIndicator;
        [SerializeField, Min(0f)] private float pulseDuration = 0.8f;
        [SerializeField] private Vector3 pulseScale = new(1.08f, 1.08f, 1f);

        private Tween _tapTween;

        private void Awake()
        {
            HideImmediate();
        }

        private void OnDisable()
        {
            KillTween();
        }

        public void Show(string message, bool showTapIndicator)
        {
            if (!ValidateReferences())
            {
                return;
            }

            rootObject.SetActive(true);
            messageText.text = message;
            SetupIndicator(showTapIndicator);
        }

        public void Hide()
        {
            KillTween();

            if (rootObject != null)
            {
                rootObject.SetActive(false);
            }
        }

        public void HideImmediate()
        {
            KillTween();

            if (tapIndicator != null)
            {
                tapIndicator.localScale = Vector3.one;
                tapIndicator.gameObject.SetActive(false);
            }

            if (rootObject != null)
            {
                rootObject.SetActive(false);
            }
        }

        private void SetupIndicator(bool showTapIndicator)
        {
            if (tapIndicator == null)
            {
                return;
            }

            tapIndicator.gameObject.SetActive(showTapIndicator);
            if (!showTapIndicator)
            {
                return;
            }

            tapIndicator.localScale = Vector3.one;
            _tapTween = tapIndicator.DOScale(pulseScale, pulseDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable);
        }

        private void KillTween()
        {
            _tapTween?.Kill();
            _tapTween = null;
        }

        private bool ValidateReferences()
        {
            if (rootObject == null || messageText == null)
            {
                Debug.LogWarning($"{nameof(TutorialOverlayPresenter)} is missing UI references.", this);
                return false;
            }

            return true;
        }
    }
}
