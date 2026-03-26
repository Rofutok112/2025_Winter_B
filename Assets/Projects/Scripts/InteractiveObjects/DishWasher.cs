using System;
using Cysharp.Threading.Tasks;
using Projects.Scripts.BackGround;
using Projects.Scripts.Control;
using UnityEngine;

namespace Projects.Scripts.InteractiveObjects
{
    public enum DishWasherState
    {
        Idle,
        Running,
        ReadyToUnload,
    }

    public class DishWasher : MonoBehaviour, IInputHandler, IInteractionHintTarget
    {
        [Header("Settings")]
        [SerializeField, Min(1f)] private float washDuration = 10f;

        [Header("References")]
        [SerializeField] private RackManager rackManager;
        [SerializeField] private WasherAnim washerAnim;
        [SerializeField] private Transform rackAnimationTarget;

        [Header("Animation")]
        [SerializeField, Min(0f)] private float rackMoveDuration = 0.35f;
        [SerializeField, Min(0.1f)] private float rackMoveScale = 0.75f;

        private DishWasherState _state = DishWasherState.Idle;
        private Rack _currentRack;
        private float _completedWashElapsedSeconds;
        private float _currentWashElapsedSeconds;
        private float _totalRunningSeconds;
        private SpriteRenderer _spriteRenderer;
        private int _washRunVersion;
        private bool _isRackTransitioning;

        public DishWasherState State => _state;
        public Rack CurrentRack => _currentRack;
        public float TotalRunningSeconds => _totalRunningSeconds;
        public bool ShouldShowInteractionHint => HasRackReadyToWash() || HasRackReadyToUnload();
        public SpriteRenderer HintSpriteRenderer => _spriteRenderer;
        public event Action WashStarted;
        public event Action<float> WashProgressChanged;
        public event Action<float> WashCompleted;
        public event Action<Rack> RackUnloaded;
        public event Action<DishWasherState> StateChanged;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void OnInputBegin(Vector2 pos)
        {
            if (_isRackTransitioning)
            {
                return;
            }

            switch (_state)
            {
                case DishWasherState.Idle:
                    TryStartWashing().Forget();
                    break;
                case DishWasherState.Running:
                    break;
                case DishWasherState.ReadyToUnload:
                    TakeOutRack().Forget();
                    break;
            }
        }

        /// <summary>
        /// Packed状態のラックを探して洗浄を開始する
        /// </summary>
        private async UniTaskVoid TryStartWashing()
        {
            var rack = rackManager.FindPackedRack();
            if (rack == null) return;

            _isRackTransitioning = true;
            _currentRack = rack;
            rack.SetState(RackState.Washing);
            await rack.AnimateIntoWasherAsync(GetRackAnimationTarget(), rackMoveDuration, rackMoveScale);

            SetState(DishWasherState.Running);
            _isRackTransitioning = false;
            RunWashTimer().Forget();
        }

        /// <summary>
        /// 洗浄完了後にラックを取り出す
        /// </summary>
        private async UniTaskVoid TakeOutRack()
        {
            if (_currentRack == null) return;

            _isRackTransitioning = true;
            var rack = _currentRack;
            rack.SetState(RackState.Washed);
            await rack.AnimateOutOfWasherAsync(GetRackAnimationTarget(), rackMoveDuration, rackMoveScale);
            _currentRack = null;
            _completedWashElapsedSeconds = 0f;

            SetState(DishWasherState.Idle);
            RackUnloaded?.Invoke(rack);
            _isRackTransitioning = false;
        }

        private async UniTaskVoid RunWashTimer()
        {
            var washRunVersion = ++_washRunVersion;
            WashStarted?.Invoke();
            washerAnim?.StartVibration();

            var remaining = washDuration;
            var elapsed = 0f;
            _currentWashElapsedSeconds = 0f;

            while (remaining > 0f && washRunVersion == _washRunVersion && _state == DishWasherState.Running)
            {
                await UniTask.Yield();
                elapsed += Time.deltaTime;
                remaining -= Time.deltaTime;
                _currentWashElapsedSeconds = Mathf.Min(elapsed, washDuration);
                var normalized = Mathf.Clamp01(remaining / washDuration);
                WashProgressChanged?.Invoke(normalized);
            }

            if (washRunVersion != _washRunVersion || _state != DishWasherState.Running)
            {
                return;
            }

            washerAnim?.StopVibration();

            WashProgressChanged?.Invoke(0f);
            _completedWashElapsedSeconds = Mathf.Min(elapsed, washDuration);
            _totalRunningSeconds += _completedWashElapsedSeconds;
            _currentWashElapsedSeconds = 0f;
            WashCompleted?.Invoke(_completedWashElapsedSeconds);
            SetState(DishWasherState.ReadyToUnload);
        }

        public void StopForGameEnd()
        {
            if (_state != DishWasherState.Running)
            {
                return;
            }

            _washRunVersion++;
            washerAnim?.StopVibration();
            _totalRunningSeconds += Mathf.Min(_currentWashElapsedSeconds, washDuration);
            _currentWashElapsedSeconds = 0f;
            _completedWashElapsedSeconds = 0f;
            WashProgressChanged?.Invoke(1f);

            if (_currentRack != null)
            {
                _currentRack.gameObject.SetActive(true);
                _currentRack.ResetToDefaultTransform();
                _currentRack.SetState(RackState.Packed);
                _currentRack = null;
            }

            SetState(DishWasherState.Idle);
        }

        private void SetState(DishWasherState newState)
        {
            _state = newState;
            StateChanged?.Invoke(newState);
        }

        private bool HasRackReadyToWash()
        {
            return _state == DishWasherState.Idle && rackManager != null && rackManager.FindPackedRack() != null;
        }

        private bool HasRackReadyToUnload()
        {
            return _state == DishWasherState.ReadyToUnload && _currentRack != null;
        }

        public void OnInputDrag(Vector2 pos) { }
        public void OnInputEnd(Vector2 pos) { }

        private Transform GetRackAnimationTarget()
        {
            return rackAnimationTarget != null ? rackAnimationTarget : transform;
        }
    }
}
