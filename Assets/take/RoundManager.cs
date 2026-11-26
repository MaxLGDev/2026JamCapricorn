
using UnityEngine;
using UnityEngine.Events;
using TMPro;
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
    [Header("Round Settings")]
    [SerializeField] private int round = 1;
    [SerializeField] private float roundTime = 10f;

    [Header("Ingredient Settings")]
    [SerializeField] private int baseRequiredNumber = 3;

    private int availableIngredients = 0;

    public int AvailableIngredients => availableIngredients;

    public int RequiredNumber => baseRequiredNumber + round - 1;

    [Header("Timer UI")]
    [SerializeField] private TMP_Text timerText;

    private RoundState currentState = RoundState.Ready;
    private Coroutine roundTimer;

    [Header("Round Events")]
    public UnityEvent OnRoundStart;
    public UnityEvent OnRoundSuccess;
    public UnityEvent OnRoundFail;
    public UnityEvent OnNextRound;

    [Header("Ingredient Events")]
    public UnityEvent OnIngredientAdded;

    public int CurrentRound => round;
    public RoundState CurrentState => currentState;

    public float RemainingTime { get; private set; }

    private void Start()
    {
        Debug.Log("RoundManager Initialized");
        StartRound();
    }

    // ラウンド開始
    public void StartRound()
    {
        if (currentState != RoundState.Ready)
        {
            Debug.LogWarning("Cannot Start Round");
            return;
        }

        currentState = RoundState.Playing;

        RemainingTime = roundTime;

        // 食材数リセット
        availableIngredients = 0;

        UpdateTimerText();

        Debug.Log($"Round {round} Started");
        Debug.Log($"Required Number: {RequiredNumber}");

        OnRoundStart?.Invoke();

        roundTimer = StartCoroutine(RoundTimer());
    }

    // ラウンドタイマー
    private IEnumerator RoundTimer()
    {
        while (RemainingTime > 0f)
        {
            RemainingTime -= Time.deltaTime;

            if (RemainingTime < 0f)
                RemainingTime = 0f;

            UpdateTimerText();

            yield return null;
        }

        RoundFail();
    }

    // タイマー表示
    private void UpdateTimerText()
    {
        if (timerText != null)
        {
            timerText.text = RemainingTime.ToString("F1");
        }
    }

    // 食材を1個追加（別スクリプトから呼び出す）
    public void AddIngredient()
    {
        if (currentState != RoundState.Playing)
            return;

        // 必要数以上は追加しない
        if (availableIngredients >= RequiredNumber)
            return;

        availableIngredients++;

        Debug.Log(
            $"Ingredient Added: {availableIngredients}/{RequiredNumber}"
        );

        OnIngredientAdded?.Invoke();

        // 必要数と同数になったらクリア
        if (availableIngredients == RequiredNumber)
        {
            RoundSuccess();
        }
    }

    // ラウンドクリア
    public void RoundSuccess()
    {
        if (currentState != RoundState.Playing)
            return;

        StopRoundTimer();

        currentState = RoundState.Success;

        Debug.Log($"Round {round} Success!");

        OnRoundSuccess?.Invoke();
    }

    // ラウンド失敗
    public void RoundFail()
    {
        if (currentState != RoundState.Playing)
            return;

        StopRoundTimer();

        RemainingTime = 0f;
        UpdateTimerText();

        currentState = RoundState.Fail;

        Debug.Log($"Round {round} Failed!");

        OnRoundFail?.Invoke();
    }

    // タイマー停止
    private void StopRoundTimer()
    {
        if (roundTimer != null)
        {
            StopCoroutine(roundTimer);
            roundTimer = null;
        }
    }

    // 次のラウンド
    public void NextRound()
    {
        if (currentState != RoundState.Success &&
            currentState != RoundState.Fail)
        {
            Debug.LogWarning("Cannot Move To Next Round");
            return;
        }

        round++;

        currentState = RoundState.Ready;

        Debug.Log($"Moving To Round {round}");

        OnNextRound?.Invoke();

        StartRound();
    }
}
