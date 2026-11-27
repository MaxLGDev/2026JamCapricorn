using UnityEngine;
using UnityEngine.EventSystems;
using System;

/// <summary>
/// ドロップされた食材を判定し、結果をラウンド管理へ通知する。
/// </summary>
public class DropZone : MonoBehaviour, IDropHandler
{
    [Header("ラウンド管理")] 
    [SerializeField] private RoundManager _roundManager;

    [Header("時間ボーナス・ペナルティ（秒）")]
    [SerializeField] private float _bonusTime = 3f;
    [SerializeField] private float _wrongPenalty = 1f;
    [SerializeField] private float _penaltyItemTime = 3f;

    /// <summary>ボーナス食材がドロップされた時（追加秒数）。</summary>
    public static event Action<float> OnBonusCollected;

    /// <summary>生地がドロップされた時（正解かどうか）。</summary>
    public static event Action<bool> OnRecipeBaseDropped;

    /// <summary>
    /// 3Dのドロップ判定に必要なColliderを確認する。
    /// </summary>
    private void Awake()
    {
        if (!TryGetComponent<Collider>(out var dropCollider))
        {
            Debug.LogError(
                $"[DropZone] {name}: ドロップ判定用の3D Colliderが必要です。",
                this);
            return;
        }

        if (!dropCollider.enabled)
        {
            Debug.LogWarning(
                $"[DropZone] {name}: Colliderが無効になっています。",
                this);
        }
    }
    
    public static event Action<bool, bool> OnDropJudged;

    /// <summary>クリック選択（生地・道具）からも同じ判定イベントを出す。</summary>
    public static void Judge(bool isCorrect, bool isTool) => OnDropJudged?.Invoke(isCorrect, isTool);

    /// <summary>
    /// 食材を判定し、ドラッグの終了処理を完了してから結果を通知する。
    /// </summary>
    /// <param name="eventData">ドロップ操作の情報。</param>
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedItem = eventData.pointerDrag;

        if (droppedItem == null)
        {
            Debug.LogWarning(
                $"[DropZone] {name}: ドロップされたアイテムが見つかりません。",
                this);
            return;
        }

        // タグは使わない：DragAndDropがあれば食材か道具として扱う
        // 生地 = Ingredient コンポーネントのないアイテム（スポーンされない、パネルに置いてあるもの）
        bool isRecipeBase = droppedItem.GetComponent<Ingredient>() == null
            && droppedItem.TryGetComponent<DragAndDrop>(out var baseDrag)
            && baseDrag.IngredientData != null;

        if (!droppedItem.TryGetComponent<DragAndDrop>(out var dragAndDrop))
        {
            Debug.LogWarning(
                $"[DropZone] {name}: {droppedItem.name}にDragAndDropがありません。",
                this);
            return;
        }

        if (dragAndDrop.IngredientData == null && dragAndDrop.ToolData == null)
        {
            Debug.LogWarning(
                $"[DropZone] {name}: {droppedItem.name}の材料または道具データがありません。",
                this);
            return;
        }

        bool isIngredient = dragAndDrop.IngredientData != null;

        if (isRecipeBase)
        {
            bool isCorrectRecipeBase = isIngredient &&
                dragAndDrop.IngredientData == _roundManager.CurrentRecipe.RecipeBase;

            // 生地は次のラウンドでも使うので、元の位置に戻す（消さない）
            if (!dragAndDrop.TryCompleteDrop(eventData.pointerId, false))
            {
                return;
            }

            _roundManager.SetRecipeBaseResult(isCorrectRecipeBase);
            if (!isCorrectRecipeBase)
                _roundManager.DecreaseTime(_wrongPenalty);

            NotifyResult(isCorrectRecipeBase, false);
            OnRecipeBaseDropped?.Invoke(isCorrectRecipeBase);
            Debug.Log($"[DropZone] {name}: Recipe Base result - {isCorrectRecipeBase}", this);
            return;
        }

        bool isCorrect = isIngredient
            ? IsCorrectIngredient(droppedItem)
            : IsCorrectTool(dragAndDrop.ToolData);

        // レシピに不要なボーナス食材なら時間を追加する
        bool isBonus = isIngredient && !isCorrect && IsBonusIngredient(droppedItem);

        // 正解なら現在位置に残し、不正解なら開始位置へ戻す。
        if (!dragAndDrop.TryCompleteDrop(eventData.pointerId, isCorrect))
        {
            return;
        }

        NotifyResult(isCorrect || isBonus, !isIngredient);

        if (isCorrect && isIngredient)
        {
            _roundManager.AddIngredient();
        }
        else if (isBonus)
        {
            _roundManager.DecreaseTime(-_bonusTime); // マイナス = 時間を追加
            OnBonusCollected?.Invoke(_bonusTime);
        }
        else if (!isCorrect && isIngredient)
        {
            _roundManager.DecreaseTime(IsPenaltyIngredient(droppedItem) ? _penaltyItemTime : _wrongPenalty);
        }

        Destroy(droppedItem);
    }

    /// <summary>
    /// ドロップされた食材が正解か判定する。
    /// </summary>
    /// <param name="droppedItem">ドロップされた食材。</param>
    /// <returns>正解ならtrue、不正解ならfalse。</returns>
    private bool IsCorrectIngredient(GameObject droppedItem)
    {
        return droppedItem.TryGetComponent<Ingredient>(out var ingredient) && ingredient.IsRequired;
    }

    private bool IsBonusIngredient(GameObject droppedItem)
    {
        return droppedItem.TryGetComponent<Ingredient>(out var ingredient)
            && ingredient.Data != null
            && ingredient.Data.IsBonus;
    }

    private bool IsPenaltyIngredient(GameObject droppedItem)
    {
        return droppedItem.TryGetComponent<Ingredient>(out var ingredient)
            && ingredient.Data != null
            && ingredient.Data.IsPenalty;
    }

    private bool IsCorrectTool(ToolData toolData)
    {
        ToolData[] requiredTools = _roundManager.CurrentRecipe.RequiredTool;
        if (requiredTools == null)
            return false;

        foreach (ToolData requiredTool in requiredTools)
        {
            if (requiredTool == toolData)
                return true;
        }

        return false;
    }

    /// <summary>
    /// 判定結果をログに記録し、イベントで通知する。
    /// </summary>
    /// <param name="isCorrect">正解かどうか。</param>
    private void NotifyResult(bool isCorrect, bool isTool)
    {
        Debug.Log($"[DropZone] {name}: 判定結果 - {isCorrect}", this);

        OnDropJudged?.Invoke(isCorrect, isTool);
    }
}