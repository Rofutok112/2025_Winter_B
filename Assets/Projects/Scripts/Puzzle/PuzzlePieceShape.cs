using System;
using UnityEngine;

namespace Projects.Scripts.Puzzle
{
    /// <summary>
    /// パズルピースの形状を定義するScriptableObject。
    /// Inspectorから形状を設定できる。
    /// </summary>
    [CreateAssetMenu(fileName = "NewPuzzlePieceShape", menuName = "Puzzle/Piece Shape")]
    public class PuzzlePieceShape : ScriptableObject
    {
        [Tooltip("形状の幅")]
        [SerializeField] private int width = 3;

        [Tooltip("形状の高さ")]
        [SerializeField] private int height = 3;

        [Tooltip("セルの占有状態（左下起点、行優先で width*height の要素数）")]
        [SerializeField] private bool[] cells;

        public int Width => width;
        public int Height => height;

        /// <summary>
        /// 指定座標のセルが埋まっているかどうかを返す。
        /// ローカル座標 (0,0) が左下。
        /// </summary>
        public bool GetCell(int x, int y)
        {
            if (x < 0 || x >= width || y < 0 || y >= height) return false;
            var index = y * width + x;
            if (index < 0 || index >= cells.Length) return false;
            return cells[index];
        }

        /// <summary>
        /// 形状に含まれるすべてのセルのローカル座標を返す。
        /// </summary>
        public Vector2Int[] GetFilledCells()
        {
            var count = 0;
            for (var i = 0; i < cells.Length; i++)
            {
                if (cells[i]) count++;
            }

            var result = new Vector2Int[count];
            var idx = 0;
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    if (GetCell(x, y))
                    {
                        result[idx++] = new Vector2Int(x, y);
                    }
                }
            }
            return result;
        }

        private void OnValidate()
        {
            if (width < 1) width = 1;
            if (height < 1) height = 1;

            var requiredLength = width * height;
            if (cells == null || cells.Length != requiredLength)
            {
                Array.Resize(ref cells, requiredLength);
            }
        }
    }
}
