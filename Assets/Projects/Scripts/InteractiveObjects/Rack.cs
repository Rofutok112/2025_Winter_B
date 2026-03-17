using System;
using Projects.Scripts.Control;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

namespace Projects.Scripts.InteractiveObjects
{
    public class Rack : MonoBehaviour, IInputHandler
    {
        [SerializeField] private GameObject puzzleWindow;
        [SerializeField] private GameObject puzzleUI;
        
        [SerializeField] private Vector3 openPositionOffset = new(0f, 0.5f, 0f);
        [SerializeField] private float openAnimationDuration = 0.5f;

        private Vector3 _puzzleWindowDefaultPosition;
        private Tween _puzzleWindowTween;
        private CancellationTokenSource _openAnimationCts;

        private void Awake()
        {
            if (puzzleWindow != null)
            {
                _puzzleWindowDefaultPosition = puzzleWindow.transform.position;
            }
        }

        private void OnDisable()
        {
            _puzzleWindowTween?.Kill();
            _openAnimationCts?.Cancel();
            _openAnimationCts?.Dispose();
            _openAnimationCts = null;
        }

        public void OnInputBegin(Vector2 pos)
        {
            if (puzzleWindow == null || puzzleUI == null) return;

            _openAnimationCts?.Cancel();
            _openAnimationCts?.Dispose();
            _openAnimationCts = new CancellationTokenSource();
            OpenPuzzleWindowAsync(_openAnimationCts.Token).Forget();
        }

        private async UniTaskVoid OpenPuzzleWindowAsync(CancellationToken cancellationToken)
        {
            _puzzleWindowTween?.Kill();

            puzzleWindow.transform.position = _puzzleWindowDefaultPosition - openPositionOffset;
            puzzleWindow.SetActive(true);
            puzzleUI.SetActive(true);

            try
            {
                _puzzleWindowTween = puzzleWindow.transform
                    .DOMove(_puzzleWindowDefaultPosition, openAnimationDuration)
                    .SetEase(Ease.OutBack);

                await _puzzleWindowTween.AsyncWaitForCompletion().AsUniTask().AttachExternalCancellation(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                _puzzleWindowTween?.Kill();
            }
            finally
            {
                _puzzleWindowTween = null;

                if (_openAnimationCts != null && _openAnimationCts.Token == cancellationToken)
                {
                    _openAnimationCts.Dispose();
                    _openAnimationCts = null;
                }
            }
        }

        public void OnInputDrag(Vector2 pos) { }

        public void OnInputEnd(Vector2 pos) { }
    }
}
