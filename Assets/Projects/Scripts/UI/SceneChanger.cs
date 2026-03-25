using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Projects.Scripts.UI
{
    public class SceneChanger : MonoBehaviour
    {
        [SerializeField] private string sceneNameToLoad;
        [Header("Loading Screen")]
        [SerializeField] private GameObject loadingScreenRoot;
        [SerializeField] private CanvasGroup loadingScreenCanvasGroup;
        [SerializeField] private Selectable triggerSelectable;
        [SerializeField, Min(0f)] private float loadingFadeDuration = 0.2f;
        [SerializeField, Min(0f)] private float minimumLoadingScreenDuration = 1.5f;

        private bool _isLoading;

        public void ChangeScene()
        {
            if (_isLoading) return;
            ChangeSceneAsync().Forget();
        }

        private async UniTaskVoid ChangeSceneAsync()
        {
            if (string.IsNullOrWhiteSpace(sceneNameToLoad))
            {
                Debug.LogWarning($"{nameof(SceneChanger)} has no target scene name.", this);
                return;
            }

            _isLoading = true;

            if (triggerSelectable != null)
            {
                triggerSelectable.interactable = false;
            }

            await ShowLoadingScreenAsync();

            var loadOperation = SceneManager.LoadSceneAsync(sceneNameToLoad);
            if (loadOperation == null)
            {
                Debug.LogWarning($"{nameof(SceneChanger)} failed to start loading scene: {sceneNameToLoad}", this);
                _isLoading = false;
                if (triggerSelectable != null)
                {
                    triggerSelectable.interactable = true;
                }
                return;
            }

            loadOperation.allowSceneActivation = false;

            if (minimumLoadingScreenDuration > 0f)
            {
                await UniTask.Delay(System.TimeSpan.FromSeconds(minimumLoadingScreenDuration));
            }

            await UniTask.WaitUntil(() => loadOperation.progress >= 0.9f);
            loadOperation.allowSceneActivation = true;
        }

        private async UniTask ShowLoadingScreenAsync()
        {
            if (loadingScreenRoot == null && loadingScreenCanvasGroup == null)
            {
                return;
            }

            if (loadingScreenCanvasGroup != null)
            {
                loadingScreenCanvasGroup.gameObject.SetActive(true);
            }

            if (loadingScreenRoot != null)
            {
                loadingScreenRoot.SetActive(true);
            }

            if (loadingScreenCanvasGroup == null)
            {
                return;
            }

            loadingScreenCanvasGroup.alpha = 0f;

            if (loadingFadeDuration <= 0f)
            {
                loadingScreenCanvasGroup.alpha = 1f;
                return;
            }

            var completionSource = new UniTaskCompletionSource();
            var tween = loadingScreenCanvasGroup
                .DOFade(1f, loadingFadeDuration)
                .SetEase(Ease.OutCubic);

            tween.OnComplete(() => completionSource.TrySetResult());
            tween.OnKill(() => completionSource.TrySetResult());

            await completionSource.Task;
        }
    }
}
