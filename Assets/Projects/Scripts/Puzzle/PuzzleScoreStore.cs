using UnityEngine;

namespace Projects.Scripts.Puzzle
{
    /// <summary>
    /// パズルのスコアを永続化する
    /// </summary>
    public static class PuzzleScoreStore
    {
        private const string LatestScoreKey = "Puzzle.LatestScore";
        private const string BestScoreKey = "Puzzle.BestScore";

        public static float LatestScore => PlayerPrefs.GetFloat(LatestScoreKey, 0f);
        public static float BestScore => PlayerPrefs.GetFloat(BestScoreKey, 0f);

        public static void SaveScore(float score)
        {
            var normalizedScore = Mathf.Clamp01(score);

            PlayerPrefs.SetFloat(LatestScoreKey, normalizedScore);

            if (normalizedScore > BestScore)
            {
                PlayerPrefs.SetFloat(BestScoreKey, normalizedScore);
            }

            PlayerPrefs.Save();
        }
    }
}
