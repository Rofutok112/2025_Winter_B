using UnityEngine;

namespace Projects.Scripts.Puzzle
{
    internal struct PuzzlePieceRefillState
    {
        public float NextRefillTime;
    }

    internal static class PuzzlePieceRefillScheduler
    {
        public static void Reset(ref PuzzlePieceRefillState refillState)
        {
            refillState.NextRefillTime = -1f;
        }

        public static void MarkFull(ref PuzzlePieceRefillState refillState)
        {
            refillState.NextRefillTime = -1f;
        }

        public static void EnsureScheduled(ref PuzzlePieceRefillState refillState, PuzzlePieceShape shape, float currentTime)
        {
            if (refillState.NextRefillTime >= 0f) return;
            refillState.NextRefillTime = currentTime + GetRefillInterval(shape);
        }

        public static bool ShouldRefill(PuzzlePieceRefillState refillState, int currentCount, int maxCount, float currentTime)
        {
            return currentCount < maxCount && refillState.NextRefillTime >= 0f && currentTime >= refillState.NextRefillTime;
        }

        public static void Advance(ref PuzzlePieceRefillState refillState, PuzzlePieceShape shape)
        {
            refillState.NextRefillTime += GetRefillInterval(shape);
        }

        private static float GetRefillInterval(PuzzlePieceShape shape)
        {
            return shape != null ? shape.RefillIntervalSeconds : 0.1f;
        }
    }
}
