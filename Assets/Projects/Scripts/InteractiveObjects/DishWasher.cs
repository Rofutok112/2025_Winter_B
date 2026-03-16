using System;
using Cysharp.Threading.Tasks;
using Projects.Scripts.Audio;
using Projects.Scripts.BackGround;
using Projects.Scripts.Control;
using Projects.Scripts.Puzzle;
using UnityEngine;

namespace Projects.Scripts.InteractiveObjects
{
    /// <summary>
    /// 食洗機のGameObject
    /// </summary>
    public class DishWasher : MonoBehaviour, IInputHandler
    {
        private const float WashingTime = 10.0f;

        [SerializeField] private WasherAnim washerAnim;
        [SerializeField] private PuzzlePieceGenerator puzzlePieceGenerator;
        [SerializeField] private AudioClip washerStartClip;
        [SerializeField] private AudioClip washingNoiseClip;
        [SerializeField] private AudioClip washingCompleteClip;

        private bool _isRunning;
        private float _currentNormalizedRemainingTime;

        public event Action<float> OnWashProgressChanged;
        public event Action<bool> OnWashStateChanged;

        public float WashDuration => WashingTime;
        public bool IsRunning => _isRunning;
        public float CurrentNormalizedRemainingTime => _currentNormalizedRemainingTime;

        private void Start()
        {
            AudioManager.Register("WashingNoise", washingNoiseClip);
            AudioManager.Register("WashingStart", washerStartClip);
            AudioManager.Register("WashingComplete", washingCompleteClip);
        }

        public void OnInputBegin(Vector2 pos)
        {
            if (_isRunning) return;

            var score = puzzlePieceGenerator != null ? puzzlePieceGenerator.CurrentOccupancy : 0f;
            PuzzleScoreStore.SaveScore(score);
            puzzlePieceGenerator?.ResetPuzzle();
            WasherTimer().Forget();
        }

        private async UniTaskVoid WasherTimer()
        {
            _isRunning = true;
            _currentNormalizedRemainingTime = 1f;
            OnWashStateChanged?.Invoke(true);
            OnWashProgressChanged?.Invoke(_currentNormalizedRemainingTime);
            washerAnim?.StartVibration();
            AudioManager.PlayOneShot("WashingStart", volume: 0.7f);
            AudioManager.Play("WashingNoise", volume: 0.2f, loop: true);

            var currentTime = WashingTime;
            while (currentTime >= 0)
            {
                await UniTask.Yield();
                currentTime -= Time.deltaTime;
                _currentNormalizedRemainingTime = Mathf.Clamp01(currentTime / WashingTime);
                OnWashProgressChanged?.Invoke(_currentNormalizedRemainingTime);
            }

            washerAnim?.StopVibration();
            AudioManager.Stop("WashingNoise");
            AudioManager.PlayOneShot("WashingComplete", volume: 0.7f);
            _isRunning = false;
            _currentNormalizedRemainingTime = 0f;
            OnWashProgressChanged?.Invoke(_currentNormalizedRemainingTime);
            OnWashStateChanged?.Invoke(false);
            Debug.Log("洗浄完了！");
        }

        public void OnInputDrag(Vector2 pos) { }

        public void OnInputEnd(Vector2 pos) { }
    }
}
