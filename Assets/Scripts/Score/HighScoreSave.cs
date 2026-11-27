using UnityEngine;
using TMPro;

public class HighScoreSave : MonoBehaviour
{
    [SerializeField] private TMP_Text highScoreText;

    private const string HighScoreKey = "HighScore";

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
        Debug.Log($"High score saved: {score}");
    }

    private void UpdateHighScoreText()
    {
        if (highScoreText != null)
        {
            highScoreText.text = "High Score: " + GetHighScore();
        }
    }
}
