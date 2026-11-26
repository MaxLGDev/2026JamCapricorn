using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// ドロップされた食材を判定し、結果をラウンド管理へ通知する。
/// </summary>
public class DropZone : MonoBehaviour, IDropHandler
{
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

        // 受付に失敗した場合は、正解判定や結果通知を行わない。
        if (!dragAndDrop.AcceptDrop())
        {
            return;
        }

        Debug.Log(
            $"[DropZone] {name}: {droppedItem.name}を受け付けました。",
            this);

        // エリア内への受付と、食材の正解・不正解を分けて扱う。
        bool isCorrect = IsCorrectIngredient(droppedItem);
        NotifyResult(isCorrect);
    }

    /// <summary>
    /// ドロップされた食材が正解か判定する。
    /// </summary>
    /// <param name="ingredient">ドロップされた食材。</param>
    /// <returns>正解ならtrue、不正解ならfalse。</returns>
    private bool IsCorrectIngredient(GameObject ingredient)
    {
        // TODO: 食材IDなどを使って正解条件と比較する。

        return false;
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