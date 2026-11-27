using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

public class ScoreSystem : MonoBehaviour
{
    [SerializeField] private TMP_Text remainingTimeText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField, Min(1)] private int baseScoreMultiplier = 10;
    [SerializeField, Min(1)] private int roundsPerMultiplierIncrease = 3;
    [SerializeField, Min(0)] private int multiplierIncrease = 10;

    private int totalScore;
    private int currentRound = 1;

    private void OnEnable()
    {
        RoundManager.RoundStarted += UpdateCurrentRound;
        RoundManager.RoundEnded += AddRemainingTimeScore;
    }

    private void OnDisable()
    {
        RoundManager.RoundStarted -= UpdateCurrentRound;
        RoundManager.RoundEnded -= AddRemainingTimeScore;
    }

    private void Start()
    {
        UpdateScoreText();
    }

    private void UpdateCurrentRound(int round, RecipeData recipe)
    {
        currentRound = round;
    }

    private void AddRemainingTimeScore(bool won)
    {
        if (remainingTimeText == null)
        {
            return;
        }

        Match match = Regex.Match(remainingTimeText.text, @"-?\d+");
        if (!int.TryParse(match.Value, out int remainingTime))
        {
            return;
        }

        int multiplierLevel = (currentRound - 1) / Mathf.Max(1, roundsPerMultiplierIncrease);
        int currentMultiplier = baseScoreMultiplier + multiplierLevel * multiplierIncrease;
        totalScore += remainingTime * currentMultiplier;
        HighScoreSave.SaveHighScore(totalScore);
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + totalScore.ToString();
        }
    }
}
