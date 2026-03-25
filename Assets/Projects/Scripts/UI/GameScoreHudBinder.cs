using UnityEngine;

namespace Projects.Scripts.UI
{
    public class GameScoreHudBinder : MonoBehaviour
    {
        [SerializeField] private GameScoreManager gameScoreManager;
        [SerializeField] private GameHudTextPresenter hudPresenter;

        private void OnEnable()
        {
            if (!ValidateReferences())
            {
                return;
            }

            gameScoreManager.ScoreChanged += hudPresenter.RefreshScore;
            gameScoreManager.TimeChanged += hudPresenter.RefreshTime;
            hudPresenter.RefreshScore(gameScoreManager.CurrentScore);
            hudPresenter.RefreshTime(gameScoreManager.RemainingTimeSeconds);
        }

        private void OnDisable()
        {
            if (gameScoreManager == null || hudPresenter == null)
            {
                return;
            }

            gameScoreManager.ScoreChanged -= hudPresenter.RefreshScore;
            gameScoreManager.TimeChanged -= hudPresenter.RefreshTime;
        }

        private bool ValidateReferences()
        {
            if (gameScoreManager == null || hudPresenter == null)
            {
                Debug.LogWarning($"{nameof(GameScoreHudBinder)} is missing references.", this);
                return false;
            }

            return true;
        }
    }
}
