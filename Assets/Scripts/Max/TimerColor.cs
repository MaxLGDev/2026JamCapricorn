using TMPro;
using UnityEngine;

// 残り時間が少ない時にタイマーの文字を赤くする
public class TimerColor : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private float warnAt = 3f;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color warnColor = Color.red;

    private void OnEnable()  => RoundManager.TimeChanged += OnTimeChanged;
    private void OnDisable() => RoundManager.TimeChanged -= OnTimeChanged;

    private void OnTimeChanged(float remaining)
    {
        if (text != null)
            text.color = remaining <= warnAt ? warnColor : normalColor;
    }
}
