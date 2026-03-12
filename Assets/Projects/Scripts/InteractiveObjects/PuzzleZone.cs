using Projects.Scripts.Control;
using UnityEngine;

namespace Projects.Scripts.InteractiveObjects
{
    public class PuzzleZone : MonoBehaviour, IInputHandler
    {
        [SerializeField] private GameObject puzzleWindow;
        
        public void OnInputBegin(Vector2 pos)
        {
            puzzleWindow.SetActive(false);
        }

        public void OnInputDrag(Vector2 pos) { }

        public void OnInputEnd(Vector2 pos) { }
    }
}