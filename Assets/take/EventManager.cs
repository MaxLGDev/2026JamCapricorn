
using UnityEngine;

public class EventManager : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;

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
        Debug.Log("[EventManager] Event Started!");

        StartIngredientMovement();
    }

    // 食材の移動開始イベント
    private void StartIngredientMovement()
    {
        Ingredient[] ingredients = FindObjectsByType<Ingredient>(
            FindObjectsSortMode.None
        );

        foreach (Ingredient ingredient in ingredients)
        {
            Rigidbody2D rb = ingredient.GetComponent<Rigidbody2D>();

            if (rb == null)
                continue;

            Vector2 direction = Random.insideUnitCircle.normalized;

            rb.linearVelocity = direction * moveSpeed;
        }

        Debug.Log("[EventManager] Ingredient Movement Started!");
    }
}
