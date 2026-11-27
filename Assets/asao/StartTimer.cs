using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class StartTimer : MonoBehaviour//ゲーム始まる前のタイマー
{
    [SerializeField] public TMP_Text timerText; // ← 追加: 必要な道具を出すテキスト
    [SerializeField] public RecipeData[] recipes;
    //  [SerializeField] public ToolData[] tools;   // StartTimerのInspectorで直接登録
    [SerializeField] public Button[] lockButtons;   //カウント中に押せなくするボタン
    [SerializeField] public float countTime = 3f;
    [SerializeField] public float hideDelay = 1f;
    public UnityEvent onCountdownFinished;

    public RecipeData CurrentRecipe { get; private set; }

    public void Start()
    {
        SetButtonsInteractable(false);   //カウント中は無効
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
            SetButtonsInteractable(true);   // ← 追加: カウント終了で有効
            onCountdownFinished?.Invoke();
            Invoke(nameof(HideTimer), hideDelay);
        }
    }
    private void SetButtonsInteractable(bool value)
    {
        foreach (var button in lockButtons)
        {
            if (button != null) button.interactable = value;
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
        //recipeText.text = CurrentRecipe.RecipeName;
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
        //ingredientsText.text = text;
    }
    private void ShowTools()
    {
        string text = "必要な道具\n";
        foreach (var tool in CurrentRecipe.RequiredTool)   // ← Tools ではなく RequiredTool
        {
            if (tool == null) continue;
            text += "・" + tool.ToolName + "\n";
        }
        //toolsText.text = text;
    }

    public void HideTimer()
    {
        timerText.gameObject.SetActive(false);
    }
}
