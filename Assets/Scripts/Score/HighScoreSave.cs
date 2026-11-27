using UnityEngine;
using TMPro;
using System;

public class HighScoreSave : MonoBehaviour
{
    [SerializeField] private TMP_Text highScoreText;

    private const string HighScoreKey = "HighScore";

    private static event Action<int> HighScoreChanged;

    private void OnEnable()
    {
        HighScoreChanged += OnHighScoreChanged;
    }

    private void OnDisable()
    {
        HighScoreChanged -= OnHighScoreChanged;
    }

    private void Start()
    {
        UpdateHighScoreText();
    }

    public static int GetHighScore()
    {
        return PlayerPrefs.GetInt(HighScoreKey, 0);
    }

    public static void SaveHighScore(int score)
    {
        if (score <= GetHighScore())
        {
            return;
        }

        PlayerPrefs.SetInt(HighScoreKey, score);
        PlayerPrefs.Save();
        HighScoreChanged?.Invoke(score);
        Debug.Log($"High score saved: {score}");
    }

    private void UpdateHighScoreText()
    {
        if (highScoreText != null)
        {
            highScoreText.text = "High Score: " + GetHighScore();
        }
    }

    private void OnHighScoreChanged(int score)
    {
        if (highScoreText != null)
        {
            highScoreText.text = "High Score: " + score;
        }
    }
}
