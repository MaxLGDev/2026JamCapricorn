using UnityEngine;

// ゲームのイベントに合わせて効果音・BGMを鳴らす（名前は AudioManager の soundName と一致させる）
public class GameSfx : MonoBehaviour
{
    [Header("BGM")]
    [SerializeField] private string bgm = "BGM";

    [Header("効果音")]
    [SerializeField] private string correct = "Correct";
    [SerializeField] private string wrong = "Wrong";
    [SerializeField] private string bonus = "Bonus";
    [SerializeField] private string clear = "Clear";
    [SerializeField] private string fail = "Fail";

    private void OnEnable()
    {
        RoundManager.RoundStarted += OnRoundStarted;
        RoundManager.RoundEnded += OnRoundEnded;
        DropZone.OnDropJudged += OnDropJudged;
        DropZone.OnBonusCollected += OnBonus;
    }

    private void OnDisable()
    {
        RoundManager.RoundStarted -= OnRoundStarted;
        RoundManager.RoundEnded -= OnRoundEnded;
        DropZone.OnDropJudged -= OnDropJudged;
        DropZone.OnBonusCollected -= OnBonus;
    }

    private void OnRoundStarted(int round, RecipeData recipe) => PlayMusic(bgm);

    private void OnRoundEnded(bool won) => Play(won ? clear : fail);

    private void OnDropJudged(bool isCorrect, bool isTool) => Play(isCorrect ? correct : wrong);

    private void OnBonus(float seconds) => Play(bonus);

    private static void Play(string soundName)
    {
        if (AudioManager.Instance != null && !string.IsNullOrEmpty(soundName))
            AudioManager.Instance.PlaySFX(soundName);
    }

    private static void PlayMusic(string soundName)
    {
        if (AudioManager.Instance != null && !string.IsNullOrEmpty(soundName))
            AudioManager.Instance.PlayMusic(soundName);
    }
}
