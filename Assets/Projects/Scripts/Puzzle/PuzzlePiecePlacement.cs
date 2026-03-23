using Projects.Scripts.Common;
using UnityEngine;

namespace Projects.Scripts.Puzzle
{
    internal readonly struct PuzzlePiecePlacement
    {
        private readonly PuzzlePieceShape _shape;
        private readonly GridGeometry _geometry;

        public PuzzlePiecePlacement(PuzzlePieceShape shape, GridGeometry geometry)
        {
            _shape = shape;
            _geometry = geometry;
        }

        public Vector2 CellWorldSize => _geometry.CellWorldSize;

        public Vector2 CalculateCenter()
        {
            var filledCells = _shape != null ? _shape.GetFilledCells() : null;
            if (filledCells == null || filledCells.Length == 0) return Vector2.zero;

            var sum = Vector2.zero;
            foreach (var cell in filledCells)
            {
                sum += new Vector2(cell.x * CellWorldSize.x, cell.y * CellWorldSize.y);
            }

            return sum / filledCells.Length;
        }

        public Vector2Int GetGridOrigin(Vector2 pieceWorldPosition)
        {
            var center = CalculateCenter();
            var originWorldPos = new Vector2(
                pieceWorldPosition.x - center.x,
                pieceWorldPosition.y - center.y
            );
            return _geometry.WorldToGridPosition(originWorldPos);
        }

        public Vector2 GetPieceWorldPosition(Vector2Int gridOrigin)
        {
            return _geometry.GridToWorldPosition(gridOrigin) + CalculateCenter();
        }
    }
}
