using UnityEngine;
using UnityEngine.UI;

public class Ingredient : MonoBehaviour
{
    private IngredientData data;

    public IngredientData Data => data;
    
    public bool IsRequired;

    public void Init(IngredientData newData)
    {
        data = newData;
        GetComponentInChildren<Image>().sprite = newData.IngredientSprite;
        
        GetComponent<DragAndDrop>().SetData(newData);
    }
}
