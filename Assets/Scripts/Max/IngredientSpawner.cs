using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public class IngredientSpawner : MonoBehaviour
{
    [SerializeField] private int round1StartCount = 6;
    [SerializeField] private List<IngredientData> ingredients = new List<IngredientData>();
    [SerializeField] private Ingredient ingredientPrefab;

    [SerializeField] private int itemsAddedPerRound = 1;
    [SerializeField] private int maxTotalIngredients = 18;
    [SerializeField] private int maxTotalBonusIngredients = 3;
    
    [SerializeField] private RectTransform spawnArea;
    
    private List<Ingredient> spawnedIngredients = new List<Ingredient>();

    private void OnEnable()
    {
        RoundManager.RoundStarted += Spawn;
        RoundManager.RoundEnded += Clear;
    }

    private void OnDisable()
    {
        RoundManager.RoundStarted -= Spawn;
        RoundManager.RoundEnded -= Clear;
    }

    private void Spawn(int round, RecipeData recipe)
    {
        Clear(true);
        
        var required = recipe.Ingredients.ToList();

        int currentRoundIngredients = round1StartCount + itemsAddedPerRound * (round - 1);
        int total = Mathf.Min(currentRoundIngredients, maxTotalIngredients);
        total = Mathf.Max(total, required.Count);
        int bonusCount = Random.Range(0, maxTotalBonusIngredients + 1);
        
        
        var toSpawn = new List<IngredientData>();

        toSpawn.AddRange(required);

        var bonus = ingredients
            .Where(i => i.IsBonus)
            .OrderBy(_ => Random.value)
            .Take(bonusCount)
            .ToList();
        toSpawn.AddRange(bonus);

        int missing = total - toSpawn.Count;

        var decoys = ingredients
            .Where(i => !i.IsBonus)
            .Except(toSpawn)
            .OrderBy(_ => Random.value)
            .Take(missing)
            .ToList();
        toSpawn.AddRange(decoys);
        
        Rect r = spawnArea.rect;
        
        foreach(IngredientData data in toSpawn)
        {
            Vector2 spawnPosition = new Vector2(Random.Range(r.xMin + 10, r.xMax - 10), Random.Range(r.yMin + 10, r.yMax - 10));
            
            Ingredient spawned = Instantiate(ingredientPrefab, spawnArea);
            ((RectTransform)spawned.transform).anchoredPosition = spawnPosition;

            spawned.IsRequired = required.Contains(data);
            spawnedIngredients.Add(spawned);
            spawned.Init(data);
        }
        
        Debug.Log($"[Spawner] Spawn round {round}, Bonus count: {bonusCount}, {total}, {toSpawn.Count}");
    }

    private void Clear(bool won)
    {
        foreach (Ingredient ingredient in spawnedIngredients)
        {
            if(ingredient != null)
                Destroy(ingredient.gameObject);
        }
        spawnedIngredients.Clear();
        Debug.Log("[Spawner] Clear");
    }
}
