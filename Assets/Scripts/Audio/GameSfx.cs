using UnityEngine;

// ゲームのイベントに合わせて効果音・BGMを鳴らす
// 名前は AudioManager の Sound の soundName と一致させる
public class GameSfx : MonoBehaviour
{
    [Header("BGM")]
    [SerializeField] private string bgm = "MainBGM";

    [Header("効果音")]
    [SerializeField] private string pick = "Pick";          // 食材をつかんだ
    [SerializeField] private string correct = "Correct";    // 正解ドロップ・ボーナス
    [SerializeField] private string wrong = "Drop";         // 不正解ドロップ
    [SerializeField] private string clear = "Correct";      // ラウンドクリア
    [SerializeField] private float clearPitch = 1.3f;
    [SerializeField] private string fail = "Fail";          // ゲームオーバー

    private void Start() => PlayMusic(bgm); // タイトル画面からBGM

    private void OnEnable()
    {
        RoundManager.RoundStarted += OnRoundStarted;
        RoundManager.RoundEnded += OnRoundEnded;
        DropZone.OnDropJudged += OnDropJudged;
        DragAndDrop.DragStarted += OnDragStarted;
    }

    private void OnDisable()
    {
        RoundManager.RoundStarted -= OnRoundStarted;
        RoundManager.RoundEnded -= OnRoundEnded;
        DropZone.OnDropJudged -= OnDropJudged;
        DragAndDrop.DragStarted -= OnDragStarted;
    }

    private void OnRoundStarted(int round, RecipeData recipe) => PlayMusic(bgm); // 同じ曲なら何もしない

    private void OnRoundEnded(bool won)
    {
        if (won) Play(clear, clearPitch);
        else Play(fail);
    }

    // ボーナスも isCorrect = true で届く
    private void OnDropJudged(bool isCorrect, bool isTool) => Play(isCorrect ? correct : wrong);

    private void OnDragStarted() => Play(pick);

    // ボタンの OnClick からも呼べる（例：GameSfx.PlayButton("ButtonClick")）
    public void PlayButton(string soundName) => Play(soundName);

    private static void Play(string soundName, float pitch = 1f)
    {
        if (AudioManager.Instance != null && !string.IsNullOrEmpty(soundName))
            AudioManager.Instance.PlaySFX(soundName, pitch);
    }

    private static void PlayMusic(string soundName)
    {
        if (AudioManager.Instance != null && !string.IsNullOrEmpty(soundName))
            AudioManager.Instance.PlayMusic(soundName);
    }
}
