using UnityEngine;

namespace Projects.Scripts.Puzzle
{
    internal readonly struct PuzzlePieceSlotLayout
    {
        private readonly Transform _transform;
        private readonly Vector3 _slotAreaOffset;
        private readonly float _slotSpacing;
        private readonly int _slotsPerLine;
        private readonly float _lineSpacing;
        private readonly SlotLayoutDirection _slotDirection;

        public PuzzlePieceSlotLayout(
            Transform transform,
            Vector3 slotAreaOffset,
            float slotSpacing,
            int slotsPerLine,
            float lineSpacing,
            SlotLayoutDirection slotDirection)
        {
            _transform = transform;
            _slotAreaOffset = slotAreaOffset;
            _slotSpacing = slotSpacing;
            _slotsPerLine = slotsPerLine;
            _lineSpacing = lineSpacing;
            _slotDirection = slotDirection;
        }

        public Vector3 GetSlotLocalPosition(int slotIndex, int slotCount)
        {
            return _slotAreaOffset + CalculateSlotOffset(slotIndex, slotCount);
        }

        public Vector3 GetSlotWorldPosition(int slotIndex, int slotCount)
        {
            return _transform.TransformPoint(GetSlotLocalPosition(slotIndex, slotCount));
        }

        private Vector3 CalculateSlotOffset(int slotIndex, int slotCount)
        {
            if (slotCount <= 0) return Vector3.zero;

            var effectiveSlotsPerLine = Mathf.Max(1, _slotsPerLine);
            var lineIndex = slotIndex / effectiveSlotsPerLine;
            var indexInLine = slotIndex % effectiveSlotsPerLine;
            var lineWidth = (effectiveSlotsPerLine - 1) * _slotSpacing;
            var lineStart = -lineWidth / 2f;

            if (_slotDirection == SlotLayoutDirection.Horizontal)
            {
                return new Vector3(
                    lineStart + indexInLine * _slotSpacing,
                    -lineIndex * _lineSpacing,
                    0f
                );
            }

            return new Vector3(
                lineIndex * _lineSpacing,
                -lineStart - indexInLine * _slotSpacing,
                0f
            );
        }
    }
}
