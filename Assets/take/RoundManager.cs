using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
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
    // ラウンド情報
    [Header("Round Settings")]
    [SerializeField] private int round = 1;
    [SerializeField] private float roundTime = 10f;

    // タイマー表示
    [Header("Timer UI")]
    [SerializeField] private TMP_Text timerText;

    private RoundState currentState = RoundState.Ready;
    private Coroutine roundTimer;

    // Unity Events
    [Header("Round Events")]
    public UnityEvent OnRoundStart;
    public UnityEvent OnRoundSuccess;
    public UnityEvent OnRoundFail;
    public UnityEvent OnNextRound;

    // 現在のラウンド
    public int CurrentRound => round;

    // 現在の状態
    public RoundState CurrentState => currentState;

    // 残り時間
    public float RemainingTime { get; private set; }

    // 初期化
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
        UpdateTimerText();

        Debug.Log($"Round {round} Started");

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

    // タイマー表示更新
    private void UpdateTimerText()
    {
        if (timerText != null)
        {
            timerText.text = RemainingTime.ToString("F1");
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