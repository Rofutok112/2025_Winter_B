using UnityEngine;

namespace Projects.Scripts.Common
{
    /// <summary>
    /// グリッドのローカル/ワールド座標変換を扱う幾何情報。
    /// </summary>
    public readonly struct GridGeometry
    {
        private readonly Transform _transform;
        private readonly int _gridSize;
        private readonly Vector2 _cellLocalSize;

        public GridGeometry(Transform transform, int gridSize, Vector2 cellLocalSize)
        {
            _transform = transform;
            _gridSize = gridSize;
            _cellLocalSize = cellLocalSize;
        }

        public int GridSize => _gridSize;
        public Vector2 CellLocalSize => _cellLocalSize;

        public Vector2 CellWorldSize => new(
            _transform != null ? _transform.TransformVector(Vector3.right * _cellLocalSize.x).magnitude : _cellLocalSize.x,
            _transform != null ? _transform.TransformVector(Vector3.up * _cellLocalSize.y).magnitude : _cellLocalSize.y
        );

        public Vector2 GridLocalSize => new(_gridSize * _cellLocalSize.x, _gridSize * _cellLocalSize.y);

        public Vector2 GridWorldSize => new(_gridSize * CellWorldSize.x, _gridSize * CellWorldSize.y);

        public Vector2 GridMinLocal => new(-GridLocalSize.x / 2f, -GridLocalSize.y / 2f);

        public Vector2 GridCellCenterOffsetLocal => new(
            GridMinLocal.x + _cellLocalSize.x / 2f,
            GridMinLocal.y + _cellLocalSize.y / 2f
        );

        public Vector2 GridToLocalPosition(Vector2Int gridPos)
        {
            var offset = GridCellCenterOffsetLocal;
            return new Vector2(
                offset.x + gridPos.x * _cellLocalSize.x,
                offset.y + gridPos.y * _cellLocalSize.y
            );
        }

        public Vector2 GridToWorldPosition(Vector2Int gridPos)
        {
            var localPosition = GridToLocalPosition(gridPos);
            return _transform != null
                ? _transform.TransformPoint(localPosition)
                : localPosition;
        }

        public Vector2Int WorldToGridPosition(Vector2 worldPos)
        {
            var localPos = _transform != null
                ? (Vector2)_transform.InverseTransformPoint(worldPos)
                : worldPos;
            var gridMin = GridMinLocal;

            var gx = Mathf.FloorToInt((localPos.x - gridMin.x) / _cellLocalSize.x);
            var gy = Mathf.FloorToInt((localPos.y - gridMin.y) / _cellLocalSize.y);
            return new Vector2Int(gx, gy);
        }
    }
}
