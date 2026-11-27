using TMPro;
using UnityEngine;
using UnityEngine.Events;
public class StartTimer : MonoBehaviour//ゲーム始まる前のタイマー
{
    [SerializeField] public TextMeshProUGUI timerText;
    [SerializeField] public TextMeshProUGUI recipeText;        // 料理名
    [SerializeField] public TextMeshProUGUI ingredientsText;   // ← 追加: 必要食材を出すテキスト
    [SerializeField] public RecipeData[] recipes;
    [SerializeField] public float countTime = 3f;
    [SerializeField] public float hideDelay = 1f;
    public UnityEvent onCountdownFinished;

    public RecipeData CurrentRecipe { get; private set; }

    private void Start()
    {
        recipeText.text = "";
        ingredientsText.text = "";
    }

    private void Update()
    {
        if (countTime <= 0f) return;

        countTime -= Time.deltaTime;

        if (countTime > 0f)
        {
            timerText.text = Mathf.CeilToInt(countTime).ToString();
        }
        else
        {
            timerText.text = "START!";
            ShowRecipe();
            onCountdownFinished?.Invoke();
            Invoke(nameof(HideTimer), hideDelay);
        }
    }

    private void ShowRecipe()
    {
        if (recipes == null || recipes.Length == 0)
        {
            Debug.LogWarning("recipes が未設定です", this);
            return;
        }

        CurrentRecipe = recipes[Random.Range(0, recipes.Length)];
        recipeText.text = CurrentRecipe.RecipeName;
        ShowIngredients();
    }

    private void ShowIngredients()
    {
        string text = "必要な材料\n";
        foreach (var item in CurrentRecipe.Ingredients)   // ← 変数名は実際のものに合わせる
        {
            if (item == null) continue;                   // 未設定の欄はスキップ
            text += "・" + item.IngredientName + "\n";    // ← 食材名の変数名も合わせる
        }
        ingredientsText.text = text;
    }

    private void HideTimer()
    {
        timerText.gameObject.SetActive(false);
    }
}
