using TMPro;
using UnityEngine;

namespace Projects.Scripts.Sorting
{
    /// <summary>
    /// 選別画面のドロップ先。Shape（形状）ごとに1つ配置される。
    /// </summary>
    public class SortingTarget : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private TextMeshPro label;

        private string _shapeKey;

        public string ShapeKey => _shapeKey;

        public void Initialize(string shapeKey, string dishTypeName, Sprite sprite, float alpha, int shapeWidth, int shapeHeight, Vector2 cellSize)
        {
            _shapeKey = shapeKey;
            SortingTargetVisualBuilder.Build(
                spriteRenderer,
                label,
                shapeKey,
                dishTypeName,
                sprite,
                alpha,
                shapeWidth,
                shapeHeight,
                cellSize
            );
        }
    }
}
