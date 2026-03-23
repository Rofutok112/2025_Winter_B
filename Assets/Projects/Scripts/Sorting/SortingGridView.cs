using System.Collections.Generic;
using Projects.Scripts.Common;
using Projects.Scripts.Puzzle;
using UnityEngine;

// SlotLayoutDirectionはPuzzlePieceGenerator内で定義済み

namespace Projects.Scripts.Sorting
{
    /// <summary>
    /// 選別画面用のグリッド表示。
    /// グリッドセルの描画と、Shape Poolに基づくドロップターゲットの配置を担当する。
    /// </summary>
    public class SortingGridView : BaseGridView
    {
        [Header("Target Settings")]
        [Tooltip("ターゲットのプレハブ")]
        [SerializeField] private SortingTarget targetPrefab;

        [Tooltip("形状プール（パズルと共有）")]
        [SerializeField] private ShapePool shapePool;

        [Tooltip("ターゲットを配置する基準位置からのオフセット（ローカル座標）")]
        [SerializeField] private Vector3 targetAreaOffset = new(0f, -5f, 0f);

        [Tooltip("ターゲット間の間隔")]
        [SerializeField] private float targetSpacing = 2f;

        [Tooltip("1段あたりに並べるターゲット数。超えた分は次の段に折り返す")]
        [SerializeField, Min(1)] private int targetsPerLine = 4;

        [Tooltip("段間の配置間隔")]
        [SerializeField] private float lineSpacing = 3f;

        [Tooltip("ターゲットの並び方向")]
        [SerializeField] private SlotLayoutDirection targetDirection = SlotLayoutDirection.Horizontal;

        [Tooltip("ターゲットの半透明度")]
        [SerializeField, Range(0f, 1f)] private float targetAlpha = 0.3f;

        [Tooltip("ターゲットの判定半径")]
        [SerializeField, Min(0.1f)] private float targetRadius = 1.5f;

        private readonly List<SortingTarget> _activeTargets = new();

        public IReadOnlyList<SortingTarget> ActiveTargets => _activeTargets;
        public float TargetRadius => targetRadius;

        protected override void Awake()
        {
            base.Awake();
            CreateTargets();
        }

        private SortingTargetFactory CreateTargetFactory()
        {
            return new SortingTargetFactory(
                transform,
                targetPrefab,
                targetAreaOffset,
                targetSpacing,
                targetsPerLine,
                lineSpacing,
                targetDirection,
                targetAlpha,
                CellLocalSize
            );
        }

        /// <summary>
        /// Shape Poolからターゲットを事前生成する
        /// </summary>
        private void CreateTargets()
        {
            _activeTargets.Clear();

            if (shapePool == null || shapePool.Shapes == null || shapePool.Shapes.Count == 0 || targetPrefab == null) return;

            _activeTargets.AddRange(CreateTargetFactory().CreateTargets(shapePool.Shapes));
        }

        protected override string GetCellObjectPrefix()
        {
            return "SortCell";
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            DrawGridBoundsGizmo();

            if (_activeTargets is { Count: > 0 })
            {
                Gizmos.color = new Color(0.2f, 0.8f, 0.3f, 0.5f);
                foreach (var target in _activeTargets)
                {
                    if (target == null) continue;
                    Gizmos.DrawWireSphere(target.transform.position, targetRadius);
                    Gizmos.DrawSphere(target.transform.position, 0.1f);
                }
                return;
            }

            var count = shapePool != null && shapePool.Shapes != null ? shapePool.Shapes.Count : 1;
            var factory = CreateTargetFactory();
            Gizmos.color = new Color(0.8f, 0.6f, 0.2f, 0.5f);
            for (var i = 0; i < count; i++)
            {
                var pos = factory.GetPreviewPosition(i, count);
                Gizmos.DrawWireSphere(pos, targetRadius);
                Gizmos.DrawSphere(pos, 0.1f);
            }
        }
#endif
    }
}
