using Projects.Scripts.InteractiveObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Projects.Scripts.UI
{
    public class PuzzleUI : MonoBehaviour
    {
        [Header("UGUI Buttons")]
        [Tooltip("確定ボタン")]
        [SerializeField] private Button confirmButton;

        [Header("Puzzle Components")]
        [SerializeField] private GameObject puzzleWindow;
        [SerializeField] private DishWasher dishWasher;

        [Header("Puzzle Window UI")]
        [SerializeField] private GameObject puzzleUI;

        [Header("Washer Timer UI")]
        [SerializeField] private Image washerTimerFillImage;
        [SerializeField, Range(0f, 1f)] private float warningThreshold = 0.3f;
        [SerializeField] private Color normalTimerColor = new(0.27f, 0.78f, 0.98f, 1f);
        [SerializeField] private Color warningTimerColor = new(1f, 0.25f, 0.25f, 1f);
        [SerializeField, Min(0.05f)] private float warningColorPulseDuration = 0.35f;

        private void Awake()
        {
            confirmButton.onClick.AddListener(OnConfirmButtonClicked);

            UpdateWasherTimerVisual(0f);
        }

        private void OnEnable()
        {
            if (dishWasher == null) return;

            dishWasher.OnWashProgressChanged += HandleWashProgressChanged;
            dishWasher.OnWashStateChanged += HandleWashStateChanged;
            HandleWashProgressChanged(dishWasher.CurrentNormalizedRemainingTime);
            HandleWashStateChanged(dishWasher.IsRunning);
        }

        private void OnDisable()
        {
            if (dishWasher != null)
            {
                dishWasher.OnWashProgressChanged -= HandleWashProgressChanged;
                dishWasher.OnWashStateChanged -= HandleWashStateChanged;
            }
        }

        private void OnConfirmButtonClicked()
        {
            puzzleWindow.SetActive(false);
            //puzzleUI.SetActive(false);
        }

        private void HandleWashProgressChanged(float normalizedRemainingTime)
        {
            UpdateWasherTimerVisual(normalizedRemainingTime);
        }

        private void HandleWashStateChanged(bool isRunning)
        {
            if (!isRunning)
            {
                UpdateWasherTimerVisual(0f);
            }
        }

        private void Update()
        {
            if (dishWasher == null || !dishWasher.IsRunning) return;
            if (dishWasher.CurrentNormalizedRemainingTime > warningThreshold) return;

            UpdateWasherTimerVisual(dishWasher.CurrentNormalizedRemainingTime);
        }

        private void UpdateWasherTimerVisual(float normalizedRemainingTime)
        {
            if (washerTimerFillImage == null) return;

            var clampedValue = Mathf.Clamp01(normalizedRemainingTime);
            washerTimerFillImage.fillAmount = clampedValue;

            if (clampedValue > warningThreshold || warningThreshold <= 0f)
            {
                washerTimerFillImage.color = normalTimerColor;
                return;
            }

            var warningProgress = 1f - (clampedValue / warningThreshold);
            var baseWarningColor = Color.Lerp(normalTimerColor, warningTimerColor, warningProgress);
            var pulseT = Mathf.PingPong(Time.unscaledTime / warningColorPulseDuration, 1f);
            washerTimerFillImage.color = Color.Lerp(baseWarningColor, warningTimerColor, pulseT);
        }
    }
}
