
using UnityEngine;
using System.Collections;

public class EventManager : MonoBehaviour
{
    [Header("Movement Event Settings")]
    [SerializeField, Range(0f, 100f)]
    private float eventProbability = 30f;

    [SerializeField]
    private float moveSpeed = 100f;

    private void OnEnable()
    {
        RoundManager.RoundStarted += OnRoundStarted;
    }

    private void OnDisable()
    {
        RoundManager.RoundStarted -= OnRoundStarted;
    }

    // ラウンド開始イベント
    private void OnRoundStarted(int round, RecipeData recipe)
    {
        Debug.Log("[EventManager] Round Started!");

        // 発生確率を判定
        if (Random.Range(0f, 100f) < eventProbability)
        {
            Debug.Log("[EventManager] Movement Event Triggered!");

            StartCoroutine(StartIngredientMovement());
        }
        else
        {
            Debug.Log("[EventManager] Movement Event Not Triggered!");
        }
    }

    // 食材の移動開始
    private IEnumerator StartIngredientMovement()
    {
        // 食材の生成完了を待つ
        yield return null;

        DragAndDrop[] objects =
            FindObjectsByType<DragAndDrop>(FindObjectsSortMode.None);

        foreach (DragAndDrop obj in objects)
        {
            RectTransform rect = obj.GetComponent<RectTransform>();

            if (rect == null)
                continue;

            StartCoroutine(MoveIngredient(rect));
        }
    }

    // 食材の移動
    private IEnumerator MoveIngredient(RectTransform rect)
    {
        Vector2 direction =
            Random.insideUnitCircle.normalized;

        while (rect != null)
        {
            rect.anchoredPosition +=
                direction * moveSpeed * Time.deltaTime;

            yield return null;
        }
    }
}
