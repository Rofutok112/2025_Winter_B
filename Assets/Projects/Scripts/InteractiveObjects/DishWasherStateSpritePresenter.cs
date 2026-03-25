using DG.Tweening;
using UnityEngine;

namespace Projects.Scripts.InteractiveObjects
{
    public class DishWasherStateSpritePresenter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private DishWasher dishWasher;
        [SerializeField] private SpriteRenderer primaryRenderer;
        [SerializeField] private SpriteRenderer transitionRenderer;

        [Header("Sprites")]
        [SerializeField] private Sprite idleSprite;
        [SerializeField] private Sprite runningSprite;
        [SerializeField] private Sprite readyToUnloadSprite;

        [Header("Animation")]
        [SerializeField, Min(0f)] private float fadeDuration = 0.25f;

        private Tween _primaryFadeTween;
        private Tween _transitionFadeTween;

        private void Awake()
        {
            ApplyStateInstant(dishWasher != null ? dishWasher.State : DishWasherState.Idle);
        }

        private void OnEnable()
        {
            if (!ValidateReferences())
            {
                return;
            }

            dishWasher.StateChanged += HandleStateChanged;
            ApplyStateInstant(dishWasher.State);
        }

        private void OnDisable()
        {
            if (dishWasher != null)
            {
                dishWasher.StateChanged -= HandleStateChanged;
            }

            KillTweens();
        }

        private void HandleStateChanged(DishWasherState state)
        {
            var nextSprite = GetSprite(state);
            if (nextSprite == null)
            {
                return;
            }

            if (primaryRenderer.sprite == null || fadeDuration <= 0f)
            {
                ApplyStateInstant(state);
                return;
            }

            KillTweens();

            transitionRenderer.gameObject.SetActive(true);
            transitionRenderer.sprite = primaryRenderer.sprite;
            transitionRenderer.color = GetColorWithAlpha(transitionRenderer.color, 1f);

            primaryRenderer.sprite = nextSprite;
            primaryRenderer.color = GetColorWithAlpha(primaryRenderer.color, 0f);

            _primaryFadeTween = primaryRenderer.DOFade(1f, fadeDuration)
                .SetEase(Ease.OutQuad)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable);

            _transitionFadeTween = transitionRenderer.DOFade(0f, fadeDuration)
                .SetEase(Ease.OutQuad)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable)
                .OnComplete(() =>
                {
                    transitionRenderer.sprite = null;
                    transitionRenderer.gameObject.SetActive(false);
                });
        }

        private void ApplyStateInstant(DishWasherState state)
        {
            var sprite = GetSprite(state);
            primaryRenderer.sprite = sprite;
            primaryRenderer.color = GetColorWithAlpha(primaryRenderer.color, 1f);

            transitionRenderer.sprite = null;
            transitionRenderer.color = GetColorWithAlpha(transitionRenderer.color, 0f);
            transitionRenderer.gameObject.SetActive(false);
        }

        private Sprite GetSprite(DishWasherState state)
        {
            return state switch
            {
                DishWasherState.Idle => idleSprite,
                DishWasherState.Running => runningSprite,
                DishWasherState.ReadyToUnload => readyToUnloadSprite,
                _ => idleSprite,
            };
        }

        private bool ValidateReferences()
        {
            if (dishWasher == null || primaryRenderer == null || transitionRenderer == null)
            {
                Debug.LogWarning($"{nameof(DishWasherStateSpritePresenter)} is missing references.", this);
                return false;
            }

            return true;
        }

        private void KillTweens()
        {
            _primaryFadeTween?.Kill();
            _primaryFadeTween = null;

            _transitionFadeTween?.Kill();
            _transitionFadeTween = null;
        }

        private static Color GetColorWithAlpha(Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }
    }
}
