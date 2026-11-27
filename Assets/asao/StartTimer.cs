using TMPro;
using UnityEngine;
using UnityEngine.Events;
public class StartTimer : MonoBehaviour//ゲーム始まる前のタイマー
{
    [SerializeField] public TextMeshProUGUI timerText;
    [SerializeField] public TextMeshProUGUI recipeText;        // 料理名
    [SerializeField] public TextMeshProUGUI ingredientsText;   // 必要食材を出すテキスト
    [SerializeField] public TextMeshProUGUI toolsText;         // ← 追加: 必要な道具を出すテキスト
    [SerializeField] public RecipeData[] recipes;
  //  [SerializeField] public ToolData[] tools;   // StartTimerのInspectorで直接登録

    [SerializeField] public float countTime = 3f;
    [SerializeField] public float hideDelay = 1f;
    public UnityEvent onCountdownFinished;

    public RecipeData CurrentRecipe { get; private set; }

    public void Start()
    {
        recipeText.text = "";
        ingredientsText.text = "";
        toolsText.text = ""; // ← 追加
    }

    public void Update()
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

    public void ShowRecipe()
    {
        if (recipes == null || recipes.Length == 0)
        {
            Debug.LogWarning("recipes が未設定です", this);
            return;
        }

        CurrentRecipe = recipes[Random.Range(0, recipes.Length)];
        recipeText.text = CurrentRecipe.RecipeName;
        ShowIngredients();
        ShowTools(); // ← 追加
    }

    public void ShowIngredients()
    {
        string text = "必要な材料\n";
        foreach (var item in CurrentRecipe.Ingredients)
        {
            if (item == null) continue;
            text += "・" + item.IngredientName + "\n";
        }
        ingredientsText.text = text;
    }
    private void ShowTools()
    {
        string text = "必要な道具\n";
        foreach (var tool in CurrentRecipe.RequiredTool)   // ← Tools ではなく RequiredTool
        {
            if (tool == null) continue;
            text += "・" + tool.toolName + "\n";
        }
        toolsText.text = text;
    }

    public void HideTimer()
    {
        timerText.gameObject.SetActive(false);
    }
}
