using UnityEngine;
[CreateAssetMenu(fileName = "RecipeData", menuName = "Cooking/Recipe Data")]

public class RecipeData : ScriptableObject
{
    [Header("料理の名前")][SerializeField] private string recipeName;//料理の名前
    [Header("料理の画像")][SerializeField] private Sprite recipeImage;//料理の画像
    [Header("メイン材料")][SerializeField] private IngredientData mainIngredient; //メイン材料
    [Header("必要な材料")][SerializeField] private IngredientData[] ingredients;  //必要材料
    [Header("必要道具")][SerializeField] private ToolData[] requiredTools;        //必要道具
}
