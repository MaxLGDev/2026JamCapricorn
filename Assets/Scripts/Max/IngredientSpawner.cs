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
    [SerializeField] private int maxTotalPenaltyIngredients = 2;
    
    [SerializeField] private RectTransform spawnArea;
    [SerializeField] private float minDistance = 0f; // 0 = プレハブの大きさから自動
    
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
            .Where(i => i != null && i.IngredientSprite != null && i.IsBonus)
            .OrderBy(_ => Random.value)
            .Take(bonusCount)
            .ToList();
        toSpawn.AddRange(bonus);

        var penalty = ingredients
            .Where(i => i != null && i.IngredientSprite != null && i.IsPenalty)
            .OrderBy(_ => Random.value)
            .Take(Random.Range(0, maxTotalPenaltyIngredients + 1))
            .ToList();
        toSpawn.AddRange(penalty);

        int missing = total - toSpawn.Count;

        var decoys = ingredients
            .Where(i => i != null && i.IngredientSprite != null && !i.IsBonus && !i.IsPenalty)
            .Except(toSpawn)
            .OrderBy(_ => Random.value)
            .Take(missing)
            .ToList();
        toSpawn.AddRange(decoys);
        
        Rect r = spawnArea.rect;
        var usedPositions = new List<Vector2>();
        float dist = minDistance > 0f ? minDistance : GetPrefabSize();
        
        foreach(IngredientData data in toSpawn)
        {
            // 他の食材と重ならない位置を探す（見つからなければ一番離れた候補）
            Vector2 spawnPosition = Vector2.zero;
            float bestDist = -1f;
            for (int tryCount = 0; tryCount < 50; tryCount++)
            {
                Vector2 p = new Vector2(Random.Range(r.xMin + dist / 2, r.xMax - dist / 2), Random.Range(r.yMin + dist / 2, r.yMax - dist / 2));
                float nearest = float.MaxValue;
                foreach (Vector2 used in usedPositions)
                    nearest = Mathf.Min(nearest, Vector2.Distance(p, used));

                if (nearest > bestDist) { bestDist = nearest; spawnPosition = p; }
                if (nearest >= dist) break;
            }
            usedPositions.Add(spawnPosition);
            
            Ingredient spawned = Instantiate(ingredientPrefab, spawnArea);
            ((RectTransform)spawned.transform).anchoredPosition = spawnPosition;

            spawned.IsRequired = required.Contains(data);
            spawnedIngredients.Add(spawned);
            spawned.Init(data);
        }
        
        Debug.Log($"[Spawner] Spawn round {round}, Bonus count: {bonusCount}, {total}, {toSpawn.Count}");
    }

    private float GetPrefabSize()
    {
        Rect size = ((RectTransform)ingredientPrefab.transform).rect;
        return Mathf.Max(size.width, size.height, 50f);
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
