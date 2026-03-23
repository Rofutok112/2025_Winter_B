using System.Collections.Generic;
using UnityEngine.Serialization;

namespace Projects.Scripts.Puzzle
{
    internal sealed class PuzzlePieceSlotState
    {
        [FormerlySerializedAs("Shape")] public PuzzlePieceShape shape;
        public readonly List<PuzzlePiece> Pieces = new();
        public PuzzlePieceRefillState refillState;
    }
}
