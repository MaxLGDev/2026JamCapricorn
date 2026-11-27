using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using System.Globalization;

public class ScoreSystem : MonoBehaviour
{
    [SerializeField] private TMP_Text remainingTimeText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private string scorePrefix = ""; // 画像に「SCORE」がある場合は空のまま
    [SerializeField, Min(1)] private int baseScoreMultiplier = 10;
    [SerializeField, Min(1)] private int roundsPerMultiplierIncrease = 3;
    [SerializeField, Min(0)] private int multiplierIncrease = 10;
    [SerializeField, Min(0)] private float correctToolScore = 10f;
    [SerializeField, Min(0)] private float incorrectToolPenalty = 10f;

    private float totalScore;
    public float TotalScore => totalScore;
    
    private int currentRound = 1;

    private void OnEnable()
    {
        RoundManager.RoundStarted += UpdateCurrentRound;
        RoundManager.RoundEnded += AddRemainingTimeScore;
        DropZone.OnDropJudged += UpdateToolScore;
    }

    private void OnDisable()
    {
        RoundManager.RoundStarted -= UpdateCurrentRound;
        RoundManager.RoundEnded -= AddRemainingTimeScore;
        DropZone.OnDropJudged -= UpdateToolScore;
    }

    private void Start()
    {
        UpdateScoreText();
    }

    private void UpdateToolScore(bool isCorrect, bool isTool)
    {
        if (!isTool)
            return;

        totalScore += isCorrect ? correctToolScore : -incorrectToolPenalty;
        HighScoreSave.SaveHighScore(totalScore);
        UpdateScoreText();
    }

    private void UpdateCurrentRound(int round, RecipeData recipe)
    {
        currentRound = round;

        if (round == 1)
        {
            totalScore = 0;
            UpdateScoreText();
        }
    }

    private void AddRemainingTimeScore(bool won)
    {
        if (!won)
            return;
        
        if (remainingTimeText == null)
        {
            return;
        }

        Match match = Regex.Match(remainingTimeText.text, @"-?\d+(?:[\.,]\d+)?");
        string remainingTimeTextValue = match.Value.Replace(',', '.');
        if (!float.TryParse(remainingTimeTextValue, NumberStyles.Float, CultureInfo.InvariantCulture, out float remainingTime))
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
            scoreText.text = scorePrefix + totalScore.ToString("0", CultureInfo.InvariantCulture);
        }
    }
}
