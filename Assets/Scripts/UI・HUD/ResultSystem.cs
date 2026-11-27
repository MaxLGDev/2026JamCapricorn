using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResultSystem : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private Image resultImage;

    [SerializeField] private ScoreSystem scoreSystem;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private GameObject clearText;
    

    private void OnEnable()
    {
        RoundManager.RoundStarted += OnRoundStarted;
        RoundManager.RoundEnded += OnRoundEnded;
    }

    private void OnDisable()
    {
        RoundManager.RoundStarted -= OnRoundStarted;
        RoundManager.RoundEnded -= OnRoundEnded;
    }

    private void OnRoundStarted(int round, RecipeData recipe)
    {
        resultPanel.SetActive(false);
        clearText.SetActive(false);
    }
    
    private void OnRoundEnded(bool won)
    {
        if (won)
        {
            clearText.SetActive(true);
            return;
        }

        float score = scoreSystem.TotalScore;
        scoreText.text = "Score: " + score.ToString("0");
        resultPanel.SetActive(true);
    }
}
