using System.Collections;
using TMPro;
using UnityEngine;

// タイトル画面：スタートボタン → ロゴとボタンを隠す → カウントダウン → ゲーム開始
public class StartScreen : MonoBehaviour
{
    [SerializeField] private RoundManager roundManager;
    [SerializeField] private GameObject startPanel;      // 黒いパネル（全体）
    [SerializeField] private GameObject[] hideOnStart;   // ロゴ・スタートボタン
    [SerializeField] private TMP_Text countdownText;     // パネルの上のカウントダウン
    [SerializeField] private int countFrom = 3;
    [SerializeField] private string startWord = "START!";
    [SerializeField] private string clickSound = "ButtonClick";

    private bool started;

    private void Start()
    {
        startPanel.SetActive(true);
        if (countdownText != null)
            countdownText.gameObject.SetActive(false);
    }

    // スタートボタンの OnClick に登録する
    public void OnStartPressed()
    {
        if (started)
            return;

        started = true;
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(clickSound);
        StartCoroutine(Countdown());
    }

    private IEnumerator Countdown()
    {
        foreach (GameObject go in hideOnStart)
        {
            if (go != null)
                go.SetActive(false);
        }

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);

            for (int i = countFrom; i > 0; i--)
            {
                countdownText.text = i.ToString();
                yield return new WaitForSecondsRealtime(1f);
            }

            countdownText.text = startWord;
            yield return new WaitForSecondsRealtime(0.5f);
        }

        startPanel.SetActive(false);
        roundManager.StartRound();
    }
}
