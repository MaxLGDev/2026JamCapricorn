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
    
    public static event Action<bool> OnDropJudged;

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

        if (!droppedItem.CompareTag("Ingredient"))
        {
            Debug.LogWarning(
                $"[DropZone] {name}: ドロップされたアイテムは食材ではありません。",
                this);
            return;
        }

        if (!droppedItem.TryGetComponent<DragAndDrop>(out var dragAndDrop))
        {
            Debug.LogWarning(
                $"[DropZone] {name}: {droppedItem.name}にDragAndDropがありません。",
                this);
            return;
        }

        if (dragAndDrop.IngredientData == null)
        {
            Debug.LogWarning(
                $"[DropZone] {name}: {droppedItem.name}のIngredientDataがありません。",
                this);
            return;
        }

        bool isCorrect = IsCorrectIngredient(droppedItem);

        // 正解なら現在位置に残し、不正解なら開始位置へ戻す。
        if (!dragAndDrop.TryCompleteDrop(eventData.pointerId, isCorrect))
        {
            return;
        }

        // 位置と入力設定の復元後に通知する
        NotifyResult(isCorrect);

        if (isCorrect)
        {
            _roundManager.AddIngredient();
            Destroy(droppedItem);
        }
    }

    /// <summary>
    /// ドロップされた食材が正解か判定する。
    /// </summary>
    /// <param name="ingredientData">ドロップされた食材。</param>
    /// <returns>正解ならtrue、不正解ならfalse。</returns>
    private bool IsCorrectIngredient(GameObject droppedItem)
    {
        return droppedItem.TryGetComponent<Ingredient>(out var ingredient) && ingredient.IsRequired;
    }

    /// <summary>
    /// 判定結果をログに記録し、イベントで通知する。
    /// </summary>
    /// <param name="isCorrect">正解かどうか。</param>
    private void NotifyResult(bool isCorrect)
    {
        Debug.Log($"[DropZone] {name}: 判定結果 - {isCorrect}", this);

        // 判定結果を渡す。
        OnDropJudged?.Invoke(isCorrect);
    }
}