using UnityEngine;

[CreateAssetMenu(fileName = "RecipeData", menuName = "Cooking/Recipe Data")]

public class RecipeData : ScriptableObject
{
    public string RecipeName;//料理の名前
    public Sprite RecipeImage;//料理の画像
    public IngredientData RecipeBase; //メイン材料
    public IngredientData[] Ingredients;  //必要材料
    public ToolData[] RequiredTool;        //必要道具
}
