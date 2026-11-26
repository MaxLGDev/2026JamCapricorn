using UnityEngine;

public class Ingredient : MonoBehaviour
{
    private IngredientData data;

    public IngredientData Data => data;
    
    public bool IsRequired;

    public void Init(IngredientData newData)
    {
        data = newData;
        GetComponent<SpriteRenderer>().sprite = newData.IngredientSprite;
    }
}
