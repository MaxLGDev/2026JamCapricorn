using UnityEngine;
[CreateAssetMenu(fileName = "IngredientData", menuName = "Cooking/Ingredient Data")]
public class IngredientData : ScriptableObject
{
    [Header("材料の名前")]
    public string IngredientName;
    [Header("材料の画像")]
    public Sprite IngredientSprite;
    
    public bool IsBonus;
}
