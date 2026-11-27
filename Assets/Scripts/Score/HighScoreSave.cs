using UnityEngine;
using TMPro;
using System;
using System.Globalization;

public class HighScoreSave : MonoBehaviour
{
    [SerializeField] private TMP_Text highScoreText;

    private const string HighScoreKey = "HighScore";

    private static event Action<float> HighScoreChanged;

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

    public static float GetHighScore()
    {
        return PlayerPrefs.GetFloat(HighScoreKey, 0f);
    }

    public static void SaveHighScore(float score)
    {
        if (score <= GetHighScore())
        {
            return;
        }

        PlayerPrefs.SetFloat(HighScoreKey, score);
        PlayerPrefs.Save();
        HighScoreChanged?.Invoke(score);
        Debug.Log($"High score saved: {score}");
    }

    private void UpdateHighScoreText()
    {
        if (highScoreText != null)
        {
            highScoreText.text = "High Score: " + GetHighScore().ToString("0.##", CultureInfo.InvariantCulture);
        }
    }

    private void OnHighScoreChanged(float score)
    {
        if (highScoreText != null)
        {
            highScoreText.text = "High Score: " + score.ToString("0.##", CultureInfo.InvariantCulture);
        }
    }
}
