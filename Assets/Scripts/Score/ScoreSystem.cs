using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

public class ScoreSystem : MonoBehaviour
{
    [SerializeField] private TMP_Text remainingTimeText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private int scoreMultiplier = 10;

    private int totalScore;

    private void OnEnable()
    {
        RoundManager.RoundEnded += AddRemainingTimeScore;
    }

    private void OnDisable()
    {
        RoundManager.RoundEnded -= AddRemainingTimeScore;
    }

    private void Start()
    {
        UpdateScoreText();
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

        totalScore += remainingTime * scoreMultiplier;
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
