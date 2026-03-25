using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Projects.Scripts.UI
{
    public class UguiButtonPressAnimation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [Header("References")]
        [SerializeField] private Button button;
        [SerializeField] private RectTransform targetTransform;
        [SerializeField] private Graphic accentGraphic;

        [Header("Press")]
        [SerializeField, Min(0.01f)] private float pressDuration = 0.08f;
        [SerializeField] private Vector3 pressedScale = new(0.94f, 0.94f, 1f);

        [Header("Click")]
        [SerializeField, Min(0.01f)] private float popDuration = 0.16f;
        [SerializeField] private Vector3 overshootScale = new(1.08f, 1.08f, 1f);
        [SerializeField, Range(0f, 20f)] private float twistAngle = 6f;
        [SerializeField, Min(0f)] private float accentFlashDuration = 0.14f;
        [SerializeField] private Color accentFlashColor = new(1f, 0.96f, 0.72f, 1f);

        private Vector3 _baseScale = Vector3.one;
        private float _baseRotationZ;
        private Color _baseAccentColor = Color.white;
        private Tween _scaleTween;
        private Tween _rotationTween;
        private Tween _accentTween;
        private bool _isPointerDown;

        private void Awake()
        {
            if (button == null)
            {
                button = GetComponent<Button>();
            }

            if (targetTransform == null)
            {
                targetTransform = transform as RectTransform;
            }

            if (button != null && accentGraphic == null)
            {
                accentGraphic = button.targetGraphic;
            }

            CacheBaseState();
        }

        private void OnEnable()
        {
            if (button != null)
            {
                button.onClick.AddListener(HandleClick);
            }

            CacheBaseState();
            ResetVisuals();
        }

        private void OnDisable()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(HandleClick);
            }

            KillTweens();
            _isPointerDown = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!IsInteractable()) return;

            _isPointerDown = true;
            AnimateScale(pressedScale, pressDuration, Ease.OutCubic);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_isPointerDown) return;

            _isPointerDown = false;
            AnimateScale(Vector3.one, pressDuration, Ease.OutCubic);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_isPointerDown) return;

            _isPointerDown = false;
            AnimateScale(Vector3.one, pressDuration, Ease.OutCubic);
        }

        private void HandleClick()
        {
            _isPointerDown = false;

            AnimateScale(Vector3.one, 0.01f, Ease.Linear);

            _scaleTween = DOTween.Sequence()
                .Append(targetTransform.DOScale(overshootScale, popDuration).SetEase(Ease.OutBack))
                .Append(targetTransform.DOScale(Vector3.one, popDuration * 0.9f).SetEase(Ease.OutElastic))
                .SetTarget(this);

            var direction = Random.value < 0.5f ? -1f : 1f;
            _rotationTween?.Kill();
            _rotationTween = DOTween.Sequence()
                .Append(targetTransform.DOLocalRotate(new Vector3(0f, 0f, direction * twistAngle), popDuration * 0.5f).SetEase(Ease.OutQuad))
                .Append(targetTransform.DOLocalRotate(Vector3.forward * _baseRotationZ, popDuration).SetEase(Ease.OutBack))
                .SetTarget(this);

            if (accentGraphic != null && accentFlashDuration > 0f)
            {
                _accentTween?.Kill();
                _accentTween = DOTween.Sequence()
                    .Append(accentGraphic.DOColor(accentFlashColor, accentFlashDuration).SetEase(Ease.OutQuad))
                    .Append(accentGraphic.DOColor(_baseAccentColor, accentFlashDuration * 1.2f).SetEase(Ease.InOutQuad))
                    .SetTarget(this);
            }
        }

        private void AnimateScale(Vector3 normalizedScale, float duration, Ease ease)
        {
            if (targetTransform == null) return;

            _scaleTween?.Kill();
            _scaleTween = targetTransform
                .DOScale(ScaleRelativeToBase(normalizedScale), duration)
                .SetEase(ease)
                .SetTarget(this);
        }

        private Vector3 ScaleRelativeToBase(Vector3 normalizedScale)
        {
            return new Vector3(
                _baseScale.x * normalizedScale.x,
                _baseScale.y * normalizedScale.y,
                _baseScale.z * normalizedScale.z
            );
        }

        private bool IsInteractable()
        {
            return button == null || button.IsInteractable();
        }

        private void CacheBaseState()
        {
            if (targetTransform != null)
            {
                _baseScale = targetTransform.localScale;
                _baseRotationZ = targetTransform.localEulerAngles.z;
            }

            if (accentGraphic != null)
            {
                _baseAccentColor = accentGraphic.color;
            }
        }

        private void ResetVisuals()
        {
            if (targetTransform != null)
            {
                targetTransform.localScale = _baseScale;
                targetTransform.localRotation = Quaternion.Euler(0f, 0f, _baseRotationZ);
            }

            if (accentGraphic != null)
            {
                accentGraphic.color = _baseAccentColor;
            }
        }

        private void KillTweens()
        {
            _scaleTween?.Kill();
            _rotationTween?.Kill();
            _accentTween?.Kill();
        }
    }
}
