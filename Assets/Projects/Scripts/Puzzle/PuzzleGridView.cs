using UnityEngine;

namespace Projects.Scripts.Puzzle
{
    /// <summary>
    /// PuzzleGridの表示を管理するMonoBehaviour。
    /// グリッドのセルをSpriteRendererで表示し、占有状態に応じて色を変える。
    /// </summary>
    public class PuzzleGridView : MonoBehaviour
    {
        [Header("Grid Settings")]
        [Tooltip("グリッドの一辺のセル数")]
        [SerializeField] private int gridSize = 8;

        [Tooltip("1セルのワールド空間サイズ")]
        [SerializeField] private float cellSize = 1f;

        [Header("Cell Visuals")]
        [Tooltip("セルに使用するスプライト（正方形推奨）")]
        [SerializeField] private Sprite cellSprite;

        [Tooltip("空のセルの色")]
        [SerializeField] private Color emptyColor = new(0.9f, 0.9f, 0.9f, 1f);

        [Tooltip("埋まっているセルの色")]
        [SerializeField] private Color occupiedColor = new(0.3f, 0.6f, 1f, 1f);

        [Tooltip("配置プレビュー（配置可能）の色")]
        [SerializeField] private Color previewValidColor = new(0.3f, 1f, 0.3f, 0.5f);

        [Tooltip("配置プレビュー（配置不可能）の色")]
        [SerializeField] private Color previewInvalidColor = new(1f, 0.3f, 0.3f, 0.5f);

        private SpriteRenderer[,] _cellRenderers;

        /// <summary>
        /// パズルグリッドのデータへのアクセス
        /// </summary>
        public PuzzleGrid Grid { get; private set; }

        /// <summary>
        /// 1セルのワールド空間サイズ
        /// </summary>
        public float CellSize => cellSize;

        /// <summary>
        /// グリッド全体のワールド空間サイズ
        /// </summary>
        public float GridWorldSize => gridSize * cellSize;

        private void Awake()
        {
            Grid = new PuzzleGrid(gridSize);
            Grid.OnGridChanged += RefreshView;
            CreateGridVisuals();
        }

        private void OnDestroy()
        {
            if (Grid != null)
                Grid.OnGridChanged -= RefreshView;
        }

        /// <summary>
        /// グリッドのセルSpriteRendererを生成する
        /// </summary>
        private void CreateGridVisuals()
        {
            _cellRenderers = new SpriteRenderer[gridSize, gridSize];

            // グリッドの左下を基準とするオフセット
            var gridOffset = new Vector2(
                -(gridSize * cellSize) / 2f + cellSize / 2f,
                -(gridSize * cellSize) / 2f + cellSize / 2f
            );

            for (var y = 0; y < gridSize; y++)
            {
                for (var x = 0; x < gridSize; x++)
                {
                    var cellObj = new GameObject($"Cell_{x}_{y}");
                    cellObj.transform.SetParent(transform, false);
                    cellObj.transform.localPosition = new Vector3(
                        gridOffset.x + x * cellSize,
                        gridOffset.y + y * cellSize,
                        0
                    );

                    // スプライトのサイズをcellSizeに合わせる
                    var sr = cellObj.AddComponent<SpriteRenderer>();
                    sr.sprite = cellSprite;
                    sr.color = emptyColor;

                    if (cellSprite != null)
                    {
                        var spriteSize = cellSprite.bounds.size;
                        var scale = cellSize / Mathf.Max(spriteSize.x, spriteSize.y);
                        // 少し隙間を作る（グリッド線の代わり）
                        cellObj.transform.localScale = Vector3.one * (scale * 1f);
                    }

                    _cellRenderers[x, y] = sr;
                }
            }
        }

        /// <summary>
        /// グリッドの表示を現在のデータに基づいて更新する
        /// </summary>
        public void RefreshView()
        {
            for (var y = 0; y < gridSize; y++)
            {
                for (var x = 0; x < gridSize; x++)
                {
                    _cellRenderers[x, y].color = Grid.IsOccupied(x, y) ? occupiedColor : emptyColor;
                }
            }
        }

        /// <summary>
        /// ピースの配置プレビューを表示する
        /// </summary>
        public void ShowPreview(PuzzlePieceShape shape, Vector2Int origin)
        {
            // まず通常表示に戻す
            RefreshView();

            var canPlace = Grid.CanPlace(shape, origin);
            var previewColor = canPlace ? previewValidColor : previewInvalidColor;

            var filledCells = shape.GetFilledCells();
            foreach (var cell in filledCells)
            {
                var gx = origin.x + cell.x;
                var gy = origin.y + cell.y;
                if (Grid.IsInBounds(gx, gy))
                {
                    _cellRenderers[gx, gy].color = previewColor;
                }
            }
        }

        /// <summary>
        /// プレビューをクリアして通常表示に戻す
        /// </summary>
        public void ClearPreview()
        {
            RefreshView();
        }

        /// <summary>
        /// ワールド座標をグリッド座標に変換する
        /// </summary>
        public Vector2Int WorldToGridPosition(Vector2 worldPos)
        {
            // グリッドのワールド空間でのオフセットを考慮
            var localPos = (Vector2)transform.InverseTransformPoint(worldPos);
            var gridOffset = new Vector2(
                -(gridSize * cellSize) / 2f,
                -(gridSize * cellSize) / 2f
            );

            var gx = Mathf.FloorToInt((localPos.x - gridOffset.x) / cellSize);
            var gy = Mathf.FloorToInt((localPos.y - gridOffset.y) / cellSize);

            return new Vector2Int(gx, gy);
        }

        /// <summary>
        /// グリッド座標をワールド座標（セル中心）に変換する
        /// </summary>
        public Vector2 GridToWorldPosition(Vector2Int gridPos)
        {
            var gridOffset = new Vector2(
                -(gridSize * cellSize) / 2f + cellSize / 2f,
                -(gridSize * cellSize) / 2f + cellSize / 2f
            );

            var localPos = new Vector3(
                gridOffset.x + gridPos.x * cellSize,
                gridOffset.y + gridPos.y * cellSize,
                0
            );

            return transform.TransformPoint(localPos);
        }
    }
}
