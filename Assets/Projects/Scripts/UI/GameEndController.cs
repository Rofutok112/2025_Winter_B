using System.Collections;
using Projects.Scripts.Control;
using Projects.Scripts.InteractiveObjects;
using Projects.Scripts.Sorting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Projects.Scripts.UI
{
    public class GameEndController : MonoBehaviour
    {
        [SerializeField] private string titleSceneName = "Title";
        [SerializeField, Min(0f)] private float resultShowDelaySeconds = 0.5f;
        [SerializeField] private GameScoreManager gameScoreManager;
        [SerializeField] private InputManager inputManager;
        [SerializeField] private SortingManager sortingManager;
        [SerializeField] private RackManager rackManager;
        [SerializeField] private DishWasher dishWasher;
        [SerializeField] private GameResultPresenter resultPresenter;
        private bool _isEnding;

        private void OnEnable()
        {
            if (gameScoreManager != null)
            {
                gameScoreManager.TimeUp += HandleTimeUp;
            }
        }

        private void OnDisable()
        {
            if (gameScoreManager != null)
            {
                gameScoreManager.TimeUp -= HandleTimeUp;
            }
        }

        private void HandleTimeUp()
        {
            if (_isEnding)
            {
                return;
            }

            _isEnding = true;
            StartCoroutine(EndGameSequence());
        }

        private IEnumerator EndGameSequence()
        {
            if (!ValidateReferences())
            {
                yield break;
            }

            inputManager?.SetInputEnabled(false);
            rackManager?.CompleteActivePuzzleForGameEnd();
            dishWasher?.StopForGameEnd();
            sortingManager?.ForceCloseSorting();
            gameScoreManager?.PauseTimer();

            if (resultShowDelaySeconds > 0f)
            {
                yield return new WaitForSecondsRealtime(resultShowDelaySeconds);
            }

            resultPresenter.Show(
                gameScoreManager.BuildResultSummary(),
                RetryCurrentScene,
                LoadTitleScene);
        }

        private void RetryCurrentScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void LoadTitleScene()
        {
            if (!string.IsNullOrWhiteSpace(titleSceneName) && Application.CanStreamedLevelBeLoaded(titleSceneName))
            {
                SceneManager.LoadScene(titleSceneName);
                return;
            }

            if (SceneManager.sceneCountInBuildSettings > 0)
            {
                var activeScene = SceneManager.GetActiveScene();
                if (activeScene.buildIndex != 0)
                {
                    SceneManager.LoadScene(0);
                    return;
                }
            }

            Debug.LogWarning($"{nameof(GameEndController)} could not resolve a title scene to load.");
        }
        private bool ValidateReferences()
        {
            if (gameScoreManager == null ||
                inputManager == null ||
                sortingManager == null ||
                rackManager == null ||
                dishWasher == null ||
                resultPresenter == null)
            {
                Debug.LogWarning($"{nameof(GameEndController)} is missing scene references.", this);
                return false;
            }

            return true;
        }
    }
}
