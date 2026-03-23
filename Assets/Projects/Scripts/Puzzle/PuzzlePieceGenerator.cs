using System;
using System.Collections.Generic;
using UnityEngine;

namespace Projects.Scripts.Puzzle
{
    public enum SlotLayoutDirection
    {
        Horizontal,
        Vertical
    }

    /// <summary>
    /// パズルピースを生成し、各スロットを同一形状のスタックとして保持する。
    /// ピースは時間経過で1枚ずつ補充され、最大枚数まで積み上がる。
    /// </summary>
    public class PuzzlePieceGenerator : MonoBehaviour
    {
        private const int MaxSupportedOrderInLayer = 20;

        [Header("Piece Settings")]
        [Tooltip("形状プール（ScriptableObject）")]
        [SerializeField] private ShapePool shapePool;

        [Tooltip("ピースのプレハブ（PuzzlePieceコンポーネント付き）")]
        [SerializeField] private PuzzlePiece piecePrefab;

        [Tooltip("配置先のグリッドビュー")]
        [SerializeField] private PuzzleGridView gridView;

        [Header("Slot Settings")]
        [Tooltip("スロットの配置間隔")]
        [SerializeField] private float slotSpacing = 3f;

        [Tooltip("1段あたりに並べるスロット数。超えた分は次の段に折り返す")]
        [SerializeField, Min(1)] private int slotsPerLine = 4;

        [Tooltip("段間の配置間隔")]
        [SerializeField] private float lineSpacing = 3f;

        [Tooltip("スロットを配置する基準位置からのオフセット（ローカル座標）")]
        [SerializeField] private Vector3 slotAreaOffset = new(0f, -5f, 0f);

        [Tooltip("スロットの並び方向")]
        [SerializeField] private SlotLayoutDirection slotDirection = SlotLayoutDirection.Horizontal;

        [Header("Refill Settings")]
        [Tooltip("各スロットの初期枚数")]
        [SerializeField, Range(1, MaxSupportedOrderInLayer)] private int initialPiecesPerSlot = 5;

        [Tooltip("各スロットの最大枚数")]
        [SerializeField, Range(1, MaxSupportedOrderInLayer)] private int maxPiecesPerSlot = MaxSupportedOrderInLayer;

        [Tooltip("同じスロット内で重ね表示するときの1枚ごとのオフセット")]
        [SerializeField] private Vector3 stackPieceOffset = new(0.04f, 0.04f, 0f);

        private PuzzlePiece[] _slots;
        private Vector3[] _slotLocalPositions;
        private PuzzlePieceSlotState[] _slotStates;
        private readonly Dictionary<PuzzlePiece, int> _pieceToSlotIndex = new();

        public event Action OnAllPiecesPlaced;
        public event Action OnPiecesGenerated;
        public event Action<PuzzlePiece> OnPiecePlacedOnGrid;
        public event Action<float> OnTraySubmitted;

        public IReadOnlyList<PuzzlePiece> Slots => _slots;

        public int RemainingPieceCount
        {
            get
            {
                if (_slotStates == null) return 0;

                var count = 0;
                foreach (var state in _slotStates)
                {
                    count += state?.Pieces.Count ?? 0;
                }

                return count;
            }
        }

        public float CurrentOccupancy
        {
            get
            {
                if (gridView == null || gridView.Grid == null) return 0f;
                return gridView.Grid.GetOccupancy();
            }
        }

        private PuzzlePieceSlotLayout SlotLayout => new(
            transform,
            slotAreaOffset,
            slotSpacing,
            slotsPerLine,
            lineSpacing,
            slotDirection
        );

        private PuzzlePieceFactory PieceFactory => new(transform, piecePrefab, gridView, HandlePiecePlaced);

        private void Start()
        {
            GenerateAllPieces();
        }

        private void Update()
        {
            ProcessTimedRefill();
        }

        private void InitializeSlots(int slotCount)
        {
            _slots = new PuzzlePiece[slotCount];
            _slotLocalPositions = new Vector3[slotCount];
            _slotStates = new PuzzlePieceSlotState[slotCount];

            var layout = SlotLayout;
            for (var i = 0; i < slotCount; i++)
            {
                _slotLocalPositions[i] = layout.GetSlotLocalPosition(i, slotCount);
                _slotStates[i] = new PuzzlePieceSlotState();
            }
        }

        public void GenerateAllPieces()
        {
            var availableShapes = GetAvailableShapes();
            if (availableShapes.Count == 0)
            {
                Debug.LogWarning("[PuzzlePieceGenerator] 形状プールが空です。インスペクターで形状を設定してください。");
                return;
            }

            if (piecePrefab == null)
            {
                Debug.LogWarning("[PuzzlePieceGenerator] ピースプレハブが設定されていません。");
                return;
            }

            if (_slotStates == null || _slotStates.Length != availableShapes.Count)
            {
                InitializeSlots(availableShapes.Count);
            }

            ClearAllPieces();
            var initialCount = Mathf.Clamp(initialPiecesPerSlot, 1, Mathf.Min(maxPiecesPerSlot, MaxSupportedOrderInLayer));

            for (var i = 0; i < availableShapes.Count; i++)
            {
                var state = _slotStates[i];
                state.shape = availableShapes[i];
                PuzzlePieceRefillScheduler.Reset(ref state.refillState);

                if (state.shape == null)
                {
                    Debug.LogWarning($"[PuzzlePieceGenerator] スロット{i}に適切な形状が見つかりませんでした。");
                    continue;
                }

                RefillSlot(i, initialCount, availableShapes.Count);
            }

            OnPiecesGenerated?.Invoke();
        }

        private void ProcessTimedRefill()
        {
            if (_slotStates == null) return;

            var generatedAny = false;
            for (var i = 0; i < _slotStates.Length; i++)
            {
                var state = _slotStates[i];
                if (state == null || state.shape == null) continue;

                if (state.Pieces.Count >= maxPiecesPerSlot)
                {
                    PuzzlePieceRefillScheduler.MarkFull(ref state.refillState);
                    continue;
                }

                PuzzlePieceRefillScheduler.EnsureScheduled(ref state.refillState, state.shape, Time.time);

                while (PuzzlePieceRefillScheduler.ShouldRefill(state.refillState, state.Pieces.Count, maxPiecesPerSlot, Time.time))
                {
                    RefillSlot(i, 1, _slotStates.Length);
                    PuzzlePieceRefillScheduler.Advance(ref state.refillState, state.shape);
                    generatedAny = true;
                }

                if (state.Pieces.Count >= maxPiecesPerSlot)
                {
                    PuzzlePieceRefillScheduler.MarkFull(ref state.refillState);
                }
            }

            if (generatedAny)
            {
                OnPiecesGenerated?.Invoke();
            }
        }

        private List<PuzzlePieceShape> GetAvailableShapes()
        {
            var uniqueShapes = new List<PuzzlePieceShape>();
            if (shapePool == null || shapePool.Shapes == null) return uniqueShapes;

            foreach (var shape in shapePool.Shapes)
            {
                if (shape == null || uniqueShapes.Contains(shape)) continue;
                uniqueShapes.Add(shape);
            }

            return uniqueShapes;
        }

        private void RefillSlot(int slotIndex, int amount, int slotCount)
        {
            var state = _slotStates[slotIndex];
            if (state?.shape == null) return;

            var spawnCount = Mathf.Min(amount, maxPiecesPerSlot - state.Pieces.Count);
            for (var i = 0; i < spawnCount; i++)
            {
                var piece = PieceFactory.Create(state.shape, GetSlotWorldPosition(slotIndex, slotCount));
                state.Pieces.Add(piece);
                _pieceToSlotIndex[piece] = slotIndex;
            }

            RefreshSlotVisuals(slotIndex, slotCount);
        }

        private void RefreshSlotVisuals(int slotIndex, int slotCount)
        {
            var state = _slotStates[slotIndex];
            if (state == null) return;

            for (var i = 0; i < state.Pieces.Count; i++)
            {
                var piece = state.Pieces[i];
                if (piece == null) continue;

                var position = GetSlotWorldPosition(slotIndex, slotCount) + stackPieceOffset * i;
                var isTopPiece = i == state.Pieces.Count - 1;
                piece.ConfigureStackPresentation(position, i, isTopPiece);
            }

            _slots[slotIndex] = state.Pieces.Count > 0 ? state.Pieces[^1] : null;
        }

        private void HandlePiecePlaced(PuzzlePiece piece)
        {
            OnPiecePlacedOnGrid?.Invoke(piece);

            if (!_pieceToSlotIndex.TryGetValue(piece, out var slotIndex))
            {
                return;
            }

            var state = _slotStates[slotIndex];
            _pieceToSlotIndex.Remove(piece);
            state.Pieces.Remove(piece);

            if (state.Pieces.Count < maxPiecesPerSlot)
            {
                PuzzlePieceRefillScheduler.EnsureScheduled(ref state.refillState, state.shape, Time.time);
            }

            RefreshSlotVisuals(slotIndex, _slotStates.Length);

            if (RemainingPieceCount == 0)
            {
                OnAllPiecesPlaced?.Invoke();
            }
        }

        private void CleanupPlacedPieces()
        {
            var pieces = GetComponentsInChildren<PuzzlePiece>();
            foreach (var piece in pieces)
            {
                if (piece != null && piece.IsPlaced)
                {
                    Destroy(piece.gameObject);
                }
            }
        }

        public void ClearAllPieces()
        {
            if (_slotStates == null) return;

            foreach (var state in _slotStates)
            {
                if (state == null) continue;

                foreach (var piece in state.Pieces)
                {
                    if (piece != null)
                    {
                        Destroy(piece.gameObject);
                    }
                }

                state.Pieces.Clear();
                PuzzlePieceRefillScheduler.Reset(ref state.refillState);
            }

            _pieceToSlotIndex.Clear();

            if (_slots == null) return;
            for (var i = 0; i < _slots.Length; i++)
            {
                _slots[i] = null;
            }
        }

        public void ResetAndRegenerate()
        {
            ResetPuzzle();
        }

        public void ResetPuzzle()
        {
            if (gridView != null && gridView.Grid != null)
            {
                gridView.Grid.Clear();
            }

            CleanupPlacedPieces();
            ClearAllPieces();
            GenerateAllPieces();
        }

        private Vector3 GetSlotWorldPosition(int slotIndex, int slotCount)
        {
            if (_slotLocalPositions == null || slotIndex < 0 || slotIndex >= _slotLocalPositions.Length)
            {
                return transform.position;
            }

            return SlotLayout.GetSlotWorldPosition(slotIndex, slotCount);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            maxPiecesPerSlot = Mathf.Clamp(maxPiecesPerSlot, 1, MaxSupportedOrderInLayer);
            initialPiecesPerSlot = Mathf.Clamp(initialPiecesPerSlot, 1, maxPiecesPerSlot);
            slotsPerLine = Mathf.Max(1, slotsPerLine);
        }

        private void OnDrawGizmosSelected()
        {
            var slotCount = GetAvailableShapes().Count;
            if (slotCount <= 0) return;

            var layout = SlotLayout;
            Gizmos.color = new Color(0.2f, 0.8f, 0.3f, 0.5f);

            for (var i = 0; i < slotCount; i++)
            {
                var pos = layout.GetSlotWorldPosition(i, slotCount);
                Gizmos.DrawSphere(pos, 0.1f);

                var topPos = pos + stackPieceOffset * Mathf.Max(initialPiecesPerSlot - 1, 0);
                Gizmos.DrawLine(pos, topPos);
            }
        }
#endif
    }
}
