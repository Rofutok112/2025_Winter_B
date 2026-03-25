namespace Projects.Scripts.UI
{
    public readonly struct GameResultSummary
    {
        public GameResultSummary(int pieceScore, float washerRunningSeconds, float utilizationRatio, float finalScore)
        {
            PieceScore = pieceScore;
            WasherRunningSeconds = washerRunningSeconds;
            UtilizationRatio = utilizationRatio;
            FinalScore = finalScore;
        }

        public int PieceScore { get; }
        public float WasherRunningSeconds { get; }
        public float UtilizationRatio { get; }
        public float FinalScore { get; }
    }
}
