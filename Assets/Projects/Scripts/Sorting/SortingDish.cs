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
        private string _shapeKey;
        private Vector2 _dragOffset;
        private Vector2 _spawnPosition;
        private Action<SortingDish> _onSortedCallback;
        private SortingDropResolver _dropResolver;

        public string ShapeKey => _shapeKey;

        public void Initialize(
            string shapeKey,
            Sprite sprite,
            int shapeWidth,
            int shapeHeight,
            Vector2 cellSize,
            SortingDropResolver dropResolver,
            Action<SortingDish> onSorted)
        {
            _shapeKey = shapeKey;
            _dropResolver = dropResolver;
            _onSortedCallback = onSorted;

            SortingDishVisualBuilder.Build(transform, sprite, shapeWidth, shapeHeight, cellSize);
            _spawnPosition = transform.position;
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
            var dropPos = pos + _dragOffset;
            var target = _dropResolver != null ? _dropResolver.FindClosestTarget(dropPos) : null;

            if (target != null && target.ShapeKey == _shapeKey)
            {
                _onSortedCallback?.Invoke(this);
                Destroy(gameObject);
            }
            else
            {
                transform.position = _spawnPosition;
            }
        }
    }
}
