
using UnityEngine;
using System.Collections;

public class EventManager : MonoBehaviour
{
    [Header("Movement Event Settings")]
    [SerializeField, Range(0f, 100f)]
    private float eventProbability = 30f;

    [SerializeField]
    private float moveSpeed = 100f;

    private RectTransform canvasRect;

    private void Awake()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();

        if (canvas != null)
            canvasRect = canvas.GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        RoundManager.RoundStarted += OnRoundStarted;
    }

    private void OnDisable()
    {
        RoundManager.RoundStarted -= OnRoundStarted;
    }

    private void OnRoundStarted(int round, RecipeData recipe)
    {
        Debug.Log("[EventManager] Round Started!");

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

    private IEnumerator StartIngredientMovement()
    {
        yield return null;

        DragAndDrop[] objects =
            FindObjectsByType<DragAndDrop>(FindObjectsSortMode.None);

        foreach (DragAndDrop obj in objects)
        {
            RectTransform rect = obj.GetComponent<RectTransform>();

            if (rect == null || canvasRect == null)
                continue;

            StartCoroutine(MoveIngredient(rect));
        }
    }

    private IEnumerator MoveIngredient(RectTransform rect)
    {
        Vector2 direction =
            Random.insideUnitCircle.normalized;

        while (rect != null)
        {
            Vector2 position = rect.anchoredPosition;

            position += direction * moveSpeed * Time.deltaTime;

            Vector3[] corners = new Vector3[4];
            rect.GetLocalCorners(corners);

            float halfWidth = rect.rect.width * 0.5f;
            float halfHeight = rect.rect.height * 0.5f;

            float margin = 30f;

            float minX = canvasRect.rect.xMin + halfWidth + margin;
            float maxX = canvasRect.rect.xMax - halfWidth - margin;
            float minY = canvasRect.rect.yMin + halfHeight + margin;
            float maxY = canvasRect.rect.yMax - halfHeight - margin;

            if (position.x < minX || position.x > maxX)
            {
                direction.x *= -1;
                position.x = Mathf.Clamp(position.x, minX, maxX);
            }

            if (position.y < minY || position.y > maxY)
            {
                direction.y *= -1;
                position.y = Mathf.Clamp(position.y, minY, maxY);
            }

            rect.anchoredPosition = position;

            yield return null;
        }
    }
}
