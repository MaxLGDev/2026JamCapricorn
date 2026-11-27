using TMPro;
using UnityEngine;
using UnityEngine.UI;

// ラウンド開始時にレシピ・必要な材料・道具・ラウンド番号を表示する
public class RecipeHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text roundText;        // ラウンド番号（画像の上に置く）
    [SerializeField] private TMP_Text recipeNameText;   // 任意
    [SerializeField] private Image recipeImage;         // 任意
    [SerializeField] private Image[] ingredientSlots;   // 必要な材料のアイコン
    [SerializeField] private Image[] toolSlots;         // 必要な道具のアイコン

    private void OnEnable()  => RoundManager.RoundStarted += Show;
    private void OnDisable() => RoundManager.RoundStarted -= Show;

    private void Show(int round, RecipeData recipe)
    {
        if (roundText != null) roundText.text = round.ToString();
        if (recipeNameText != null) recipeNameText.text = recipe.RecipeName;
        if (recipeImage != null && recipe.RecipeImage != null) recipeImage.sprite = recipe.RecipeImage;

        FillIngredients(recipe.Ingredients);
        FillTools(recipe.RequiredTool);
    }

    private void FillIngredients(IngredientData[] items)
    {
        if (ingredientSlots == null) return;

        for (int i = 0; i < ingredientSlots.Length; i++)
        {
            if (ingredientSlots[i] == null) continue;

            bool used = items != null && i < items.Length && items[i] != null;
            ingredientSlots[i].gameObject.SetActive(used);
            if (used) ingredientSlots[i].sprite = items[i].IngredientSprite;
        }
    }

    private void FillTools(ToolData[] tools)
    {
        if (toolSlots == null) return;

        for (int i = 0; i < toolSlots.Length; i++)
        {
            if (toolSlots[i] == null) continue;

            bool used = tools != null && i < tools.Length && tools[i] != null;
            toolSlots[i].gameObject.SetActive(used);
            if (used) toolSlots[i].sprite = tools[i].ToolImage;
        }
    }
}
