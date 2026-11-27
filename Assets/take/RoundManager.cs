using UnityEngine;
using TMPro;
using System;
using System.Collections;

public enum RoundState
{
    Ready,
    Playing,
    Success,
    Fail
}

public class RoundManager : MonoBehaviour
{
    // ===== イベント（コードから購読する） =====
    public static event Action<int, RecipeData> RoundStarted;   // ラウンド番号, このラウンドのレシピ
    public static event Action<bool> RoundEnded;                // true = クリア, false = 失敗
    public static event Action<int, int> IngredientAdded;       // 現在の正解数, 必要数
    public static event Action<float> TimeChanged;              // 残り時間（秒）

    [Header("Round Settings")]
    [SerializeField] private int round = 1;
    [SerializeField] private float roundTime = 10f;
    [SerializeField] private float timerBeforeNextRound = 2f;
    [SerializeField] private bool autoStart = true; // OFF = StartScreen から StartRound() を呼ぶ

    [Header("Recipes（材料が少ない順）")]
    [SerializeField] private RecipeData[] recipes;

    [Header("Recipe Base（ベース材料のドロップを必須にするか）")]
    [SerializeField] private bool requireRecipeBase = true;

    [Header("Timer UI")]
    [SerializeField] private TMP_Text timerText;

    private RoundState currentState = RoundState.Ready;
    private Coroutine roundTimer;
    private int availableIngredients;
    private bool recipeBaseSelected;
    private bool recipeBaseCorrect;

    public int CurrentRound => round;
    public RoundState CurrentState => currentState;
    public float RemainingTime { get; private set; }

    // ラウンドに対応するレシピ（最後のレシピ以降はそれを使い続ける）
    public RecipeData CurrentRecipe =>
        recipes[Mathf.Min(round - 1, recipes.Length - 1)];

    public int RequiredNumber => CurrentRecipe.Ingredients.Length;

    private void Start()
    {
        if (recipes == null || recipes.Length == 0)
        {
            Debug.LogError("[Round] レシピが設定されていません", this);
            return;
        }

        if (autoStart)
            StartRound();
    }

    // ラウンド開始・リトライ
    public void StartRound()
    {
        if (currentState != RoundState.Ready && currentState != RoundState.Fail)
        {
            Debug.LogWarning("[Round] Cannot Start Round");
            return;
        }

        // 失敗後のリトライはラウンド1から
        if (currentState == RoundState.Fail)
            round = 1;

        StopRoundTimer();

        currentState = RoundState.Playing;
        RemainingTime = roundTime;
        availableIngredients = 0;
        // requireRecipeBase が ON なら生地（RecipeBase）のドロップが必要
        recipeBaseSelected = !requireRecipeBase || CurrentRecipe.RecipeBase == null;
        recipeBaseCorrect = recipeBaseSelected;
        UpdateTimer();

        Debug.Log($"[Round] Start {round} ({CurrentRecipe.RecipeName}), required: {RequiredNumber}");
        RoundStarted?.Invoke(round, CurrentRecipe);

        roundTimer = StartCoroutine(RoundTimer());
    }

    private IEnumerator RoundTimer()
    {
        while (RemainingTime > 0f)
        {
            RemainingTime = Mathf.Max(0f, RemainingTime - Time.deltaTime);
            UpdateTimer();
            yield return null;
        }

        RoundFail();
    }

    private void UpdateTimer()
    {
        if (timerText != null)
            timerText.text = RemainingTime.ToString("F1");

        TimeChanged?.Invoke(RemainingTime);
    }

    public void DecreaseTime(float seconds)
    {
        if (currentState != RoundState.Playing)
            return;

        RemainingTime = Mathf.Max(0f, RemainingTime - seconds);
        UpdateTimer();

        Debug.Log($"[Round] Time decreased by {seconds} seconds. Remaining: {RemainingTime:F1}");

        if (RemainingTime <= 0f)
            RoundFail();
    }

    // 正解の食材がドロップされた時に呼ぶ
    public void AddIngredient()
    {
        if (currentState != RoundState.Playing)
            return;

        availableIngredients++;
        Debug.Log($"[Round] Ingredient {availableIngredients}/{RequiredNumber}");
        IngredientAdded?.Invoke(availableIngredients, RequiredNumber);

        TryComplete();
    }

    // 材料がそろい、生地も正しければクリア
    private void TryComplete()
    {
        if (availableIngredients >= RequiredNumber && recipeBaseSelected && recipeBaseCorrect)
            RoundSuccess();
    }

    public void SetRecipeBaseResult(bool isCorrect)
    {
        if (currentState != RoundState.Playing)
            return;

        Debug.Log($"[Round] Recipe Base result: {isCorrect}");

        // 間違った生地はペナルティだけ（DropZone側）。正しい生地で選択完了
        if (!isCorrect)
            return;

        recipeBaseSelected = true;
        recipeBaseCorrect = true;
        TryComplete();
    }

    public void RoundSuccess()
    {
        if (currentState != RoundState.Playing)
            return;

        if (!recipeBaseSelected || !recipeBaseCorrect)
            return; // 生地がまだ → クリアしない（待つ）

        StopRoundTimer();
        currentState = RoundState.Success;

        Debug.Log($"[Round] {round} Success!");
        RoundEnded?.Invoke(true);

        Invoke(nameof(NextRound), timerBeforeNextRound);
    }

    public void RoundFail()
    {
        if (currentState != RoundState.Playing)
            return;

        StopRoundTimer();
        RemainingTime = 0f;
        UpdateTimer();
        currentState = RoundState.Fail;

        Debug.Log($"[Round] {round} Failed!");
        RoundEnded?.Invoke(false);
    }

    private void StopRoundTimer()
    {
        if (roundTimer == null)
            return;

        StopCoroutine(roundTimer);
        roundTimer = null;
    }

    public void NextRound()
    {
        if (currentState != RoundState.Success)
        {
            Debug.LogWarning("[Round] Cannot Move To Next Round");
            return;
        }

        round++;
        currentState = RoundState.Ready;
        StartRound();
    }
}
