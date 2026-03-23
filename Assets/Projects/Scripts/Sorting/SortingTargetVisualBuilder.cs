using TMPro;
using UnityEngine;

namespace Projects.Scripts.Sorting
{
    internal static class SortingTargetVisualBuilder
    {
        public static void Build(
            SpriteRenderer spriteRenderer,
            TextMeshPro label,
            string shapeKey,
            string dishTypeName,
            Sprite sprite,
            float alpha,
            int shapeWidth,
            int shapeHeight,
            Vector2 cellSize)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = sprite;
                spriteRenderer.color = new Color(1f, 1f, 1f, alpha);

                if (sprite != null)
                {
                    var spriteSize = sprite.bounds.size;
                    var targetWidth = shapeWidth * cellSize.x;
                    var targetHeight = shapeHeight * cellSize.y;
                    spriteRenderer.transform.localScale = new Vector3(
                        targetWidth / spriteSize.x,
                        targetHeight / spriteSize.y,
                        1f
                    );
                }
            }

            if (label != null)
            {
                label.text = dishTypeName ?? shapeKey;
            }
        }
    }
}
