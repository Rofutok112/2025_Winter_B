using UnityEngine;

namespace Projects.Scripts.Common
{
    /// <summary>
    /// グリッド表示と座標変換を扱う共通基底クラス。
    /// </summary>
    public abstract class BaseGridView : MonoBehaviour
    {
        protected const float LocalCellSize = 1f;

        [Header("Grid Settings")]
        [Tooltip("グリッドの一辺のセル数")]
        [SerializeField] private int gridSize = 8;

        [Header("Cell Visuals")]
        [Tooltip("セルに使用するスプライト（正方形推奨）")]
        [SerializeField] private Sprite cellSprite;

        [Tooltip("セルの色")]
        [SerializeField] private Color cellColor = new(0.9f, 0.9f, 0.9f, 1f);

        private SpriteRenderer[,] _cellRenderers;

        public GridGeometry Geometry => new(transform, gridSize, Vector2.one * LocalCellSize);
        public int GridSize => gridSize;
        public float CellSize => Geometry.CellWorldSize.x;
        public Vector2 CellLocalSize => Geometry.CellLocalSize;
        public Vector2 CellWorldSize => Geometry.CellWorldSize;
        public Sprite CellSprite => cellSprite;
        public Color CellColor => cellColor;
        public float GridWorldSize => Geometry.GridWorldSize.x;

        protected virtual void Awake()
        {
            CreateGridVisuals();
        }

        protected void RefreshGridView()
        {
            if (_cellRenderers == null) return;

            for (var y = 0; y < gridSize; y++)
            {
                for (var x = 0; x < gridSize; x++)
                {
                    _cellRenderers[x, y].color = cellColor;
                }
            }
        }

        public Vector2Int WorldToGridPosition(Vector2 worldPos)
        {
            return Geometry.WorldToGridPosition(worldPos);
        }

        public Vector2 GridToWorldPosition(Vector2Int gridPos)
        {
            return Geometry.GridToWorldPosition(gridPos);
        }

        private void CreateGridVisuals()
        {
            _cellRenderers = new SpriteRenderer[gridSize, gridSize];
            var geometry = Geometry;

            for (var y = 0; y < gridSize; y++)
            {
                for (var x = 0; x < gridSize; x++)
                {
                    var cellObj = new GameObject($"{GetCellObjectPrefix()}_{x}_{y}");
                    cellObj.transform.SetParent(transform, false);
                    var localPos = geometry.GridToLocalPosition(new Vector2Int(x, y));
                    cellObj.transform.localPosition = new Vector3(localPos.x, localPos.y, 0f);

                    var sr = cellObj.AddComponent<SpriteRenderer>();
                    sr.sprite = cellSprite;
                    sr.color = cellColor;

                    if (cellSprite != null)
                    {
                        var spriteSize = cellSprite.bounds.size;
                        var scale = LocalCellSize / Mathf.Max(spriteSize.x, spriteSize.y);
                        cellObj.transform.localScale = Vector3.one * scale;
                    }

                    _cellRenderers[x, y] = sr;
                }
            }
        }

        protected virtual string GetCellObjectPrefix()
        {
            return "Cell";
        }

#if UNITY_EDITOR
        protected void DrawGridBoundsGizmo()
        {
            if (gridSize <= 0) return;

            var previousMatrix = Gizmos.matrix;
            var previousColor = Gizmos.color;
            var geometry = Geometry;

            Gizmos.matrix = transform.localToWorldMatrix;

            var gridLocalSize = geometry.GridLocalSize;
            var halfSize = gridLocalSize / 2f;

            Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.9f);
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(gridLocalSize.x, gridLocalSize.y, 0.01f));

            Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.35f);
            for (var i = 1; i < gridSize; i++)
            {
                var x = -halfSize.x + i * geometry.CellLocalSize.x;
                var y = -halfSize.y + i * geometry.CellLocalSize.y;
                Gizmos.DrawLine(new Vector3(x, -halfSize.y, 0f), new Vector3(x, halfSize.y, 0f));
                Gizmos.DrawLine(new Vector3(-halfSize.x, y, 0f), new Vector3(halfSize.x, y, 0f));
            }

            Gizmos.matrix = previousMatrix;
            Gizmos.color = previousColor;
        }
#endif
    }
}
