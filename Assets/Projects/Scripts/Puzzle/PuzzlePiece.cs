using Projects.Scripts.Control;
using UnityEngine;

namespace Projects.Scripts.Puzzle
{
    public class PuzzlePiece : MonoBehaviour, IInputHandler
    {
        private Vector2 _dragOffset;
        
        public void OnInputBegin(Vector2 pos)
        {
            _dragOffset = (Vector2)transform.position - pos;
        }

        public void OnInputDrag(Vector2 pos)
        {
            transform.position = pos + _dragOffset;
        }

        public void OnInputEnd(Vector2 pos)
        {
            
        }
    }
}