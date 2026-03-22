using UnityEngine;

namespace Projects.Scripts.Sorting
{
    /// <summary>
    /// 選別画面のドロップ先。皿の種類ごとに1つ配置される。
    /// </summary>
    public class SortingTarget : MonoBehaviour
    {
        private string _dishTypeKey;
        private SpriteRenderer _spriteRenderer;

        public string DishTypeKey => _dishTypeKey;

        public void Initialize(string dishTypeKey, Sprite sprite, float alpha)
        {
            _dishTypeKey = dishTypeKey;

            _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_spriteRenderer == null)
                _spriteRenderer = gameObject.AddComponent<SpriteRenderer>();

            _spriteRenderer.sprite = sprite;
            _spriteRenderer.color = new Color(1f, 1f, 1f, alpha);
            _spriteRenderer.sortingOrder = 0;

            EnsureCollider();
        }

        private void EnsureCollider()
        {
            if (GetComponent<Collider2D>() != null) return;

            var col = gameObject.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
        }
    }
}
