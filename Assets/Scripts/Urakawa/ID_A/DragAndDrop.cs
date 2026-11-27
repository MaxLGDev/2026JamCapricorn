using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// UIをドラッグし、ドロップが受け付けられなければ開始位置に戻す。
/// ドラッグ中は親のRectTransformを変更しない前提。
/// </summary>
[RequireComponent(typeof(RectTransform)), RequireComponent(typeof(CanvasGroup))]
public class DragAndDrop : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("食材データ")]
    [SerializeField] private IngredientData _ingredientData;

    [Header("道具データ")]
    [SerializeField] private ToolData _toolData;

    /// <summary>
    /// ログ用の食材名。データ未設定時はオブジェクト名を使用する。
    /// </summary>
    private string IngredientDisplayName =>
        _ingredientData != null ? _ingredientData.IngredientName :
        _toolData != null ? _toolData.ToolName : name;

    /// <summary>
    /// DropZoneから食材データを参照するためのプロパティ。
    /// </summary>
    public IngredientData IngredientData => _ingredientData;

    public ToolData ToolData => _toolData;

    [Header("ドラッグイベント")]
    [SerializeField]
    private UnityEvent _onDragStarted = new UnityEvent();
    [SerializeField]
    private UnityEvent _onDragEnded = new UnityEvent();

    /// <summary>
    /// 移動対象のUIのRectTransform。
    /// </summary>
    private RectTransform _rectTransform;

    /// <summary>
    /// 移動対象のUIのCanvasGroup。
    /// </summary>
    private CanvasGroup _canvasGroup;

    /// <summary>
    /// ドラッグ開始前のレイキャスト設定。
    /// </summary>
    private bool _originalBlocksRaycasts;

    /// <summary>
    /// マウス位置を変換する基準となる親UIの平面。
    /// </summary>
    private RectTransform _dragPlane;

    /// <summary>
    /// 押した位置とUIの位置との差。
    /// </summary>
    private Vector3 _offset;

    /// <summary>
    /// 現在ドラッグ中かどうか。
    /// </summary>
    private bool _isDragging;

    public bool IsDragging => _isDragging;

    /// <summary>
    /// ドラッグを開始したポインターのID。
    /// </summary>
    private int _pointerId;

    /// <summary>
    /// ドラッグ開始時のUI位置。
    /// </summary>
    private Vector2 _startAnchoredPosition;

    /// <summary>
    /// 移動に使用するコンポーネントを取得する。
    /// </summary>
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();

        if (_ingredientData == null)
        {
            Debug.LogError($"[DragAndDrop] {IngredientDisplayName}: IngredientDataが設定されていません。", this);
            return;
        }
    }

    /// <summary>
    /// ドラッグ開始時に基準位置と操作状態を記録し、
    /// 開始イベントを呼び出す。
    /// </summary>
    /// <param name="eventData">UIのポインター操作情報。</param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 操作の重複と、右クリックなどでのドラッグを防ぐ。
        if (_isDragging || eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        if (_ingredientData == null)
        {
            Debug.LogError($"[DragAndDrop] {IngredientDisplayName}: IngredientDataが設定されていません。", this);
            return;
        }

        // 親UIの平面に沿って動かすため、座標変換の基準として保持する。
        _dragPlane = _rectTransform.parent as RectTransform;

        if (_dragPlane == null)
        {
            Debug.LogError($"[DragAndDrop] {IngredientDisplayName}: 親にRectTransformが必要です。",this);
            return;
        }

        // 最初に押した位置とのずれを維持する。
        // pressEventCameraはCanvasに対応するカメラを渡す。
        if (!RectTransformUtility.ScreenPointToWorldPointInRectangle(
                _dragPlane,
                eventData.pressPosition,
                eventData.pressEventCamera,
                out Vector3 pointerPosition))
        {
            Debug.LogWarning($"[DragAndDrop] {IngredientDisplayName}: ドラッグ位置を取得できません。",this);
            return;
        }
        
        _offset = _rectTransform.position - pointerPosition; // クリックした位置との差を保持する。
        _pointerId = eventData.pointerId;
        _isDragging = true;

        // 背後のDropZoneが入力を受け取れるよう、一時的に無効化する。
        _originalBlocksRaycasts = _canvasGroup.blocksRaycasts;
        _canvasGroup.blocksRaycasts = false;

        // エリア外や不正解の場合に戻す、今回の開始位置を保存する。
        _startAnchoredPosition = _rectTransform.anchoredPosition;

        Debug.Log($"[DragAndDrop] {IngredientDisplayName}: ドラッグ開始。", this);

        _onDragStarted.Invoke();
    }

    /// <summary>
    /// ドラッグ中のポインター位置に合わせてUIを移動する。
    /// </summary>
    /// <param name="eventData">UIのポインター操作情報。</param>
    public void OnDrag(PointerEventData eventData)
    {
        // 開始したポインター以外からの操作は受け付けない。
        if (!_isDragging || eventData.pointerId != _pointerId) return;

        // スクリーン座標を親UIの平面上のワールド座標へ変換する。
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                _dragPlane,
                eventData.position,
                eventData.pressEventCamera,
                out Vector3 pointerPosition))
        {
            // 押した位置との差を加え、UIの端をつかんだ場合も位置を保つ。
            _rectTransform.position = pointerPosition + _offset;
        }
    }

    /// <summary>
    /// DropZoneで処理されなかった場合、開始位置に戻して終了する。
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_isDragging || eventData.pointerId != _pointerId) return;

        FinishDrag(false);
    }


    /// <summary>
    /// DropZoneから呼び出し、判定結果に応じてドラッグを終了する。
    /// </summary>
    /// <param name="pointerId">操作したポインターのID。</param>
    /// <param name="keepPosition">現在位置に残す場合はtrue。</param>
    /// <returns>終了処理に成功した場合はtrue。</returns>
    public bool TryCompleteDrop(int pointerId, bool keepPosition)
    {
        if (!_isDragging || pointerId != _pointerId)
        {
            Debug.LogWarning(
                $"[DragAndDrop] {IngredientDisplayName}: 対象のドラッグは未開始または終了済みです。",
                this);
            return false;
        }

        FinishDrag(keepPosition);
        return true;
    }

    /// <summary>
    /// 位置と入力設定を確定してから、終了を通知する。
    /// </summary>
    private void FinishDrag(bool keepPosition)
    {
        // 通知先から再び呼ばれても、同じ操作を処理しない。
        _isDragging = false;

        if (!keepPosition)
        {
            ReturnToStartPosition();
        }

        _canvasGroup.blocksRaycasts = _originalBlocksRaycasts;

        Debug.Log(
            $"[DragAndDrop] {IngredientDisplayName}: ドラッグ終了。" +
            $" UI位置={_rectTransform.anchoredPosition}",
            this);

        _onDragEnded.Invoke();
    }

    /// <summary>
    /// UIを今回のドラッグ開始位置に戻す。
    /// </summary>
    private void ReturnToStartPosition()
    {
        // 初期位置に戻す。
        _rectTransform.anchoredPosition = _startAnchoredPosition;
        Debug.Log($"[DragAndDrop] {IngredientDisplayName}: 元の位置へ戻した。", this);
    }

    /// <summary>
    /// 無効化時にドラッグ状態を解除し、レイキャスト設定を復元する。
    /// 中断時は位置を戻さず、終了イベントも呼び出さない。
    /// </summary>
    private void OnDisable()
    {
        if (!_isDragging) return;

        _isDragging = false;
        _canvasGroup.blocksRaycasts = _originalBlocksRaycasts;

        Debug.Log($"[DragAndDrop] {IngredientDisplayName}: 無効化されたためドラッグを中断。",this);
    }

    public void SetData(IngredientData data)
    {
        _ingredientData = data;
        _toolData = null;
    }

    public void SetData(ToolData data)
    {
        _toolData = data;
        _ingredientData = null;
    }
}