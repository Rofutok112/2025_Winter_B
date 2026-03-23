using UnityEngine;

namespace Projects.Scripts.Sorting
{
    internal static class SortingDishVisualBuilder
    {
        public static SpriteRenderer Build(Transform root, Sprite sprite, int shapeWidth, int shapeHeight, Vector2 cellSize)
        {
            var spriteRenderer = root.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = root.gameObject.AddComponent<SpriteRenderer>();
            }

            spriteRenderer.sprite = sprite;
            spriteRenderer.sortingOrder = 10;

            if (sprite != null)
            {
                var spriteSize = sprite.bounds.size;
                var targetWidth = shapeWidth * cellSize.x;
                var targetHeight = shapeHeight * cellSize.y;
                root.localScale = new Vector3(
                    targetWidth / spriteSize.x,
                    targetHeight / spriteSize.y,
                    1f
                );
            }

            if (root.GetComponent<Collider2D>() == null)
            {
                root.gameObject.AddComponent<BoxCollider2D>();
            }

            return spriteRenderer;
        }
    }
}
