using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Projects.Scripts.Control;
using UnityEngine;

namespace Projects.Scripts.InteractiveObjects
{
    public enum RackState
    {
        Empty,
        Packing,
        Packed,
        Washing,
        Washed,
        Sorting,
    }

    public class Rack : MonoBehaviour, IInputHandler, IInteractionHintTarget
    {
        [Header("Sprites")]
        [Tooltip("空のラックのスプライト")]
        [SerializeField] private Sprite emptySprite;

        [Tooltip("皿が入ったラックのスプライト")]
        [SerializeField] private Sprite filledSprite;

        [Header("Feedback")]
        [SerializeField, Min(0f)] private float emptyClickPunchScale = 0.12f;
        [SerializeField, Min(0f)] private float emptyClickDuration = 0.2f;

        private SpriteRenderer _spriteRenderer;
        private RackState _state = RackState.Empty;
        private RackPlacementData _placementData;
        private Vector3 _defaultPosition;
        private Vector3 _defaultScale;
        private Tween _activeTween;
        private Tween _feedbackTween;

        public RackState State => _state;
        public RackPlacementData PlacementData => _placementData;
        public bool ShouldShowInteractionHint => _state == RackState.Empty || _state == RackState.Washed;
        public SpriteRenderer HintSpriteRenderer => _spriteRenderer;

        public event Action<Rack> OnClicked;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _defaultPosition = transform.position;
            _defaultScale = transform.localScale;
            ApplySprite();
        }

        private void OnDisable()
        {
            KillActiveTween();
            KillFeedbackTween();
        }

        public void SetState(RackState newState)
        {
            _state = newState;
            ApplySprite();
        }

        public void SavePlacementData(RackPlacementData data)
        {
            _placementData = data;
        }

        public void ClearPlacementData()
        {
            _placementData = null;
        }

        public void ResetToDefaultTransform()
        {
            KillActiveTween();
            transform.position = _defaultPosition;
            transform.localScale = _defaultScale;
        }

        public async UniTask AnimateIntoWasherAsync(Transform targetTransform, float duration, float targetScale)
        {
            if (targetTransform == null || duration <= 0f)
            {
                gameObject.SetActive(false);
                return;
            }

            KillActiveTween();

            var completionSource = new UniTaskCompletionSource();
            _activeTween = DOTween.Sequence()
                .SetLink(gameObject, LinkBehaviour.KillOnDisable)
                .Append(transform.DOMove(targetTransform.position, duration).SetEase(Ease.InQuad))
                .Join(transform.DOScale(Vector3.one * targetScale, duration).SetEase(Ease.InQuad))
                .OnComplete(() => completionSource.TrySetResult())
                .OnKill(() => completionSource.TrySetResult());

            await completionSource.Task;

            ResetToDefaultTransform();
            gameObject.SetActive(false);
            _activeTween = null;
        }

        public async UniTask AnimateOutOfWasherAsync(Transform sourceTransform, float duration, float sourceScale)
        {
            gameObject.SetActive(true);

            if (sourceTransform == null || duration <= 0f)
            {
                transform.position = _defaultPosition;
                transform.localScale = _defaultScale;
                return;
            }

            KillActiveTween();

            transform.position = sourceTransform.position;
            transform.localScale = Vector3.one * sourceScale;

            var completionSource = new UniTaskCompletionSource();
            _activeTween = DOTween.Sequence()
                .SetLink(gameObject, LinkBehaviour.KillOnDisable)
                .Append(transform.DOMove(_defaultPosition, duration).SetEase(Ease.OutQuad))
                .Join(transform.DOScale(_defaultScale, duration).SetEase(Ease.OutQuad))
                .OnComplete(() => completionSource.TrySetResult())
                .OnKill(() => completionSource.TrySetResult());

            await completionSource.Task;

            ResetToDefaultTransform();
            _activeTween = null;
        }

        private void ApplySprite()
        {
            if (_spriteRenderer == null) return;

            _spriteRenderer.sprite = _state switch
            {
                RackState.Empty => emptySprite,
                RackState.Packing => emptySprite,
                _ => filledSprite,
            };
        }

        public void OnInputBegin(Vector2 pos)
        {
            if (_state == RackState.Empty)
            {
                PlayEmptyClickFeedback();
            }

            OnClicked?.Invoke(this);
        }

        private void KillActiveTween()
        {
            _activeTween?.Kill();
            _activeTween = null;
        }

        private void PlayEmptyClickFeedback()
        {
            if (emptyClickDuration <= 0f || emptyClickPunchScale <= 0f)
            {
                return;
            }

            KillFeedbackTween();
            transform.localScale = _defaultScale;
            _feedbackTween = transform.DOPunchScale(Vector3.one * emptyClickPunchScale, emptyClickDuration, 1, 0.5f)
                .SetEase(Ease.OutQuad)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable)
                .OnComplete(() =>
                {
                    transform.localScale = _defaultScale;
                    _feedbackTween = null;
                })
                .OnKill(() =>
                {
                    transform.localScale = _defaultScale;
                    _feedbackTween = null;
                });
        }

        private void KillFeedbackTween()
        {
            _feedbackTween?.Kill();
            _feedbackTween = null;
        }

        public void OnInputDrag(Vector2 pos) { }
        public void OnInputEnd(Vector2 pos) { }
    }
}
