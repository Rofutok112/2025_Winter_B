using Projects.Scripts.UI;
using UnityEngine;

namespace Projects.Scripts.Sorting
{
    public class SortingScoreBinder : MonoBehaviour
    {
        [SerializeField] private SortingManager sortingManager;
        [SerializeField] private GameScoreManager gameScoreManager;

        private void OnEnable()
        {
            if (!ValidateReferences())
            {
                return;
            }

            sortingManager.SortingScoreConfirmed += gameScoreManager.AddSortedPiecePoints;
        }

        private void OnDisable()
        {
            if (sortingManager == null || gameScoreManager == null)
            {
                return;
            }

            sortingManager.SortingScoreConfirmed -= gameScoreManager.AddSortedPiecePoints;
        }

        private bool ValidateReferences()
        {
            if (sortingManager == null || gameScoreManager == null)
            {
                Debug.LogWarning($"{nameof(SortingScoreBinder)} is missing references.", this);
                return false;
            }

            return true;
        }
    }
}
