using UnityEngine;
using TMPro;
using System.Globalization;
using UnityEngine.UI;

public class ResultSystem : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private Image resultImage;
    [SerializeField] private Sprite gameOverSprite;
    [SerializeField] private Sprite gameClearSprite;
    [SerializeField] private int ClearLine;

    private void Start()
    {
        if (resultImage != null)
        {
            resultImage.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (scoreText == null || resultImage == null ||
            !int.TryParse(scoreText.text, NumberStyles.Integer, CultureInfo.CurrentCulture, out int score))
        {
            return;
        }

        resultImage.sprite = score <= ClearLine ? gameOverSprite : gameClearSprite;
        resultImage.gameObject.SetActive(true);

        if (resultText != null)
        {
            resultText.text = score <= ClearLine ? "Game Over" : "Game Clear";
        }

    }
}
