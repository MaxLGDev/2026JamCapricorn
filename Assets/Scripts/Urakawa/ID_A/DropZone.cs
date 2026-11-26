using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// ドロップされた食材を判定し、結果をラウンド管理へ通知する。
/// </summary>
public class DropZone : MonoBehaviour, IDropHandler
{
    [Header("正解の食材")]
    [SerializeField] private IngredientData _correctIngredient;

    /// <summary>
    /// 正解ならtrue、不正解ならfalseを通知するイベント。
    /// Inspectorでラウンド管理側の受け取りメソッドを登録する。
    /// </summary>
    [SerializeField]
    private UnityEvent<bool> _onDropJudged = new UnityEvent<bool>();

    /// <summary>
    /// ドロップされた食材を受け付け、成功した場合のみ判定結果を通知する。
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

        if (_correctIngredient == null)
        {
            Debug.LogError(
                $"[DropZone] {name}: 正解の食材が設定されていません。",
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

        // エリア内への受付と、食材の正解・不正解を分けて扱う。
        bool isCorrect = IsCorrectIngredient(dragAndDrop.IngredientData);

        if (isCorrect)
        {
            // 正解だけドロップ位置に残す。
            if (!dragAndDrop.AcceptDrop())
            {
                return;
            }

            Debug.Log($"[DropZone] {name}: {droppedItem.name}を受け付けました。", this);
        }
        else
        {
            Debug.Log($"[DropZone] {name}: {droppedItem.name}は不正解です。元の位置に戻します。", this);
        }

        NotifyResult(isCorrect);
    }

    /// <summary>
    /// ドロップされた食材が正解か判定する。
    /// </summary>
    /// <param name="ingredientData">ドロップされた食材。</param>
    /// <returns>正解ならtrue、不正解ならfalse。</returns>
    private bool IsCorrectIngredient(IngredientData ingredientData)
    {
        return ingredientData == _correctIngredient;
    }

    /// <summary>
    /// 判定結果をログに記録し、イベントで通知する。
    /// </summary>
    /// <param name="isCorrect">正解かどうか。</param>
    private void NotifyResult(bool isCorrect)
    {
        Debug.Log($"[DropZone] {name}: 判定結果 - {isCorrect}", this);

        // 判定結果を渡す。
        _onDropJudged.Invoke(isCorrect);
    }
}