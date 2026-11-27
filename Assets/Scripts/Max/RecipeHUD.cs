using TMPro;
using UnityEngine;
using UnityEngine.UI;

// ラウンド開始時にレシピ・必要な材料・道具・ラウンド番号を表示する
public class RecipeHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text roundText;        // ラウンド番号（画像の上に置く）
    [SerializeField] private TMP_Text recipeNameText;   // 任意
    [SerializeField] private Image recipeImage;         // 任意
    [SerializeField] private Image baseSlot;            // 必要な生地のアイコン（任意）
    [SerializeField] private Image[] ingredientSlots;   // 必要な材料のアイコン
    [SerializeField] private Image[] toolSlots;         // 必要な道具のアイコン

    // Base Slot が空なら、材料スロット1つ目をコピーして左側に自動で作る
    private void Awake()
    {
        if (baseSlot != null || ingredientSlots == null || ingredientSlots.Length == 0 || ingredientSlots[0] == null)
            return;

        Image first = ingredientSlots[0];
        baseSlot = Instantiate(first, first.transform.parent);
        baseSlot.name = "BaseSlot (auto)";
        baseSlot.transform.SetSiblingIndex(first.transform.GetSiblingIndex());

        RectTransform a = first.rectTransform;
        RectTransform b = baseSlot.rectTransform;
        Vector2 step = new Vector2(a.rect.width + 10f, 0f);
        if (ingredientSlots.Length > 1 && ingredientSlots[1] != null)
            step = ingredientSlots[1].rectTransform.anchoredPosition - a.anchoredPosition;
        b.anchoredPosition = a.anchoredPosition - step;
    }

    private void OnEnable()  => RoundManager.RoundStarted += Show;
    private void OnDisable() => RoundManager.RoundStarted -= Show;

    private void Show(int round, RecipeData recipe)
    {
        if (roundText != null) roundText.text = round.ToString();
        if (recipeNameText != null) recipeNameText.text = recipe.RecipeName;
        if (recipeImage != null && recipe.RecipeImage != null) recipeImage.sprite = recipe.RecipeImage;

        if (baseSlot != null)
        {
            Sprite baseSprite = FindKijiSprite(recipe.RecipeBase);
            baseSlot.gameObject.SetActive(baseSprite != null);
            if (baseSprite != null) baseSlot.sprite = baseSprite;
        }

        FillIngredients(recipe.Ingredients);
        FillTools(recipe.RequiredTool);
    }

    // 生地パネルと同じ絵を使う（なければ SO の画像）
    private Sprite FindKijiSprite(IngredientData kiji)
    {
        if (kiji == null) return null;

        foreach (ClickChoice choice in FindObjectsByType<ClickChoice>(FindObjectsSortMode.None))
        {
            if (choice.Kiji == kiji && choice.PanelSprite != null)
                return choice.PanelSprite;
        }

        return kiji.IngredientSprite;
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
        // レシピに道具がない時はツールパネルをそのまま表示する
        if (toolSlots == null || tools == null || tools.Length == 0) return;

        for (int i = 0; i < toolSlots.Length; i++)
        {
            if (toolSlots[i] == null) continue;

            bool used = tools != null && i < tools.Length && tools[i] != null;
            toolSlots[i].gameObject.SetActive(used);
            if (used) toolSlots[i].sprite = tools[i].ToolImage;
        }
    }
}
