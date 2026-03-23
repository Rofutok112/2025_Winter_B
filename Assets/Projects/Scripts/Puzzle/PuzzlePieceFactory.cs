using System;
using UnityEngine;

namespace Projects.Scripts.Puzzle
{
    internal sealed class PuzzlePieceFactory
    {
        private readonly Transform _parent;
        private readonly PuzzlePiece _piecePrefab;
        private readonly PuzzleGridView _gridView;
        private readonly Action<PuzzlePiece> _onPlaced;

        public PuzzlePieceFactory(
            Transform parent,
            PuzzlePiece piecePrefab,
            PuzzleGridView gridView,
            Action<PuzzlePiece> onPlaced)
        {
            _parent = parent;
            _piecePrefab = piecePrefab;
            _gridView = gridView;
            _onPlaced = onPlaced;
        }

        public PuzzlePiece Create(PuzzlePieceShape shape, Vector3 position)
        {
            var piece = UnityEngine.Object.Instantiate(_piecePrefab, position, Quaternion.identity, _parent);
            piece.Initialize(shape, _gridView, ChooseRandomSprite(shape), _onPlaced);
            return piece;
        }

        private static Sprite ChooseRandomSprite(PuzzlePieceShape shape)
        {
            if (shape == null) return null;

            var assignedSpriteCount = shape.GetAssignedDishSpriteCount();
            if (assignedSpriteCount <= 0)
            {
                return null;
            }

            var targetIndex = UnityEngine.Random.Range(0, assignedSpriteCount);
            var currentIndex = 0;
            for (var i = 0; i < shape.DishSprites.Count; i++)
            {
                var sprite = shape.GetSpriteAt(i);
                if (sprite == null) continue;

                if (currentIndex == targetIndex)
                {
                    return sprite;
                }

                currentIndex++;
            }

            return null;
        }
    }
}
