using System;
using Projects.Scripts.Control;
using UnityEngine;

namespace Projects.Scripts.Sorting
{
    /// <summary>
    /// 選別画面でD&Dできる皿。
    /// </summary>
    public class SortingDish : MonoBehaviour, IInputHandler
    {
        private string _dishTypeKey;
        private SpriteRenderer _spriteRenderer;
        private Vector2 _dragOffset;
        private Vector2 _spawnPosition;
        private Action<SortingDish> _onSortedCallback;

        public string DishTypeKey => _dishTypeKey;

        public void Initialize(string dishTypeKey, Sprite sprite, Action<SortingDish> onSorted)
        {
            _dishTypeKey = dishTypeKey;
            _onSortedCallback = onSorted;

            _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_spriteRenderer == null)
                _spriteRenderer = gameObject.AddComponent<SpriteRenderer>();

            _spriteRenderer.sprite = sprite;
            _spriteRenderer.sortingOrder = 10;

            _spawnPosition = transform.position;

            if (GetComponent<Collider2D>() == null)
            {
                var col = gameObject.AddComponent<BoxCollider2D>();
                col.isTrigger = false;
            }
        }

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
            var target = FindTargetAtPosition(pos + _dragOffset);

            if (target != null && target.DishTypeKey == _dishTypeKey)
            {
                _onSortedCallback?.Invoke(this);
                Destroy(gameObject);
            }
            else
            {
                transform.position = _spawnPosition;
            }
        }

        private static SortingTarget FindTargetAtPosition(Vector2 worldPos)
        {
            var hit = Physics2D.OverlapPoint(worldPos);
            if (hit == null) return null;
            return hit.GetComponent<SortingTarget>();
        }
    }
}
