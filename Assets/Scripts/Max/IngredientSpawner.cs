using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public class IngredientSpawner : MonoBehaviour
{
    [SerializeField] private int round1StartCount = 6;
    [SerializeField] private List<IngredientData> ingredients = new List<IngredientData>();
    [SerializeField] private Ingredient ingredientPrefab;
    
    [SerializeField] private List<IngredientData> testRequired; // TEMPORARY UNTIL C DATA

    [SerializeField] private int itemsAddedPerRound = 1;
    [SerializeField] private int maxTotalIngredients = 18;
    [SerializeField] private int maxTotalBonusIngredients = 3;
    
    [SerializeField] private BoxCollider2D spawnArea;
    
    private List<Ingredient> spawnedIngredients = new List<Ingredient>();

    private void OnEnable()
    {
        RoundManager.OnRoundStart += Spawn;
        RoundManager.OnRoundEnd += Clear;
    }

    private void OnDisable()
    {
        RoundManager.OnRoundStart -= Spawn;
        RoundManager.OnRoundEnd -= Clear;
    }

    private void Spawn(int round)
    {
        Clear(true);

        int currentRoundIngredients = round1StartCount + itemsAddedPerRound * (round - 1);
        int total = Mathf.Min(currentRoundIngredients, maxTotalIngredients);
        total = Mathf.Max(total, testRequired.Count);
        int bonusCount = Random.Range(0, maxTotalBonusIngredients + 1);
        
        
        var toSpawn = new List<IngredientData>();

        toSpawn.AddRange(testRequired);

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
        
        foreach(IngredientData data in toSpawn)
        {
            float x = Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x);
            float y = Random.Range(spawnArea.bounds.min.y, spawnArea.bounds.max.y);
            Vector2 spawnedPosition = new Vector2(x, y);
            
            Ingredient spawned = Instantiate(ingredientPrefab, spawnedPosition, Quaternion.identity);

            spawned.IsRequired = testRequired.Contains(data);
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
    
    private void OnDrawGizmos()
    {
        if (spawnArea == null)
            return;
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(spawnArea.bounds.center, spawnArea.bounds.size);
    }
}
