using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class RoundManager : Managers<RoundManager>
{
    public static event Action<int> OnRoundStart;
    public static event Action<bool> OnRoundEnd;

    private int round;

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null)
            return;

        if (keyboard.nKey.wasPressedThisFrame)
            StartRound(round + 1);

        if (keyboard.eKey.wasPressedThisFrame)
            EndRound(true);
    }

    public void StartRound(int roundNumber)
    {
        round = roundNumber;
        Debug.Log($"[Round] Start {round}");
        OnRoundStart?.Invoke(round);
    }

    public void EndRound(bool won)
    {
        Debug.Log($"[Round] end, won={won}");
        OnRoundEnd?.Invoke(won);
    }
}
