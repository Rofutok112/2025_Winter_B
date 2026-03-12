using Projects.Scripts.Puzzle;
using UnityEngine;
using UnityEngine.UI;

namespace Projects.Scripts.UI
{
    public class PuzzleUI : MonoBehaviour
    {
        [Header("UGUI Buttons")] 
        [Tooltip("戻るボタン")] 
        [SerializeField]
        private Button backButton;
        
        [Header("Puzzle Grid Field")] 
        [Tooltip("PuzzleGridView")] 
        [SerializeField]
        private PuzzleGridView puzzleGridView;

        private void Awake()
        {
            backButton.onClick.AddListener(OnBackButtonClicked);
        }

        private void OnBackButtonClicked()
        {
            var ratio = puzzleGridView.Grid.Clear();
            
            Debug.Log(ratio);
        }
    }
}