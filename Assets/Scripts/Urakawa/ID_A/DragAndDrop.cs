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

    /// <summary>
    /// DropZoneから食材データを参照するためのプロパティ。
    /// </summary>
    public IngredientData IngredientData => _ingredientData;

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

    /// <summary>
    /// ドラッグを開始したポインターのID。
    /// </summary>
    private int _pointerId;

    /// <summary>
    /// ドラッグ開始時のUI位置。
    /// </summary>
    private Vector2 _startAnchoredPosition;

    /// <summary>
    /// 今回のドラッグで、ドロップエリアに受け付けられたか。
    /// </summary>
    private bool _isDropAccepted;

    /// <summary>
    /// 移動に使用するコンポーネントを取得する。
    /// </summary>
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();

        if (_ingredientData == null)
        {
            Debug.LogError($"[DragAndDrop] IngredientDataが設定されていません。", this);
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
            Debug.LogError($"[DragAndDrop] IngredientDataが設定されていません。", this);
            return;
        }

        // 親UIの平面に沿って動かすため、座標変換の基準として保持する。
        _dragPlane = _rectTransform.parent as RectTransform;

        if (_dragPlane == null)
        {
            Debug.LogError($"[DragAndDrop] {name}: 親にRectTransformが必要です。",this);
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
            Debug.LogWarning($"[DragAndDrop] {name}: ドラッグ位置を取得できません。",this);
            return;
        }
        
        _offset = _rectTransform.position - pointerPosition; // クリックした位置との差を保持する。
        _pointerId = eventData.pointerId;
        _isDragging = true;

        // 背後のDropZoneが入力を受け取れるよう、一時的に無効化する。
        _originalBlocksRaycasts = _canvasGroup.blocksRaycasts;
        _canvasGroup.blocksRaycasts = false;

        // 前回の配置結果を引き継がず、今回の操作の復帰先を保存する。
        _startAnchoredPosition = _rectTransform.anchoredPosition;
        _isDropAccepted = false;

        Debug.Log($"[DragAndDrop] {name}: ドラッグ開始。", this);

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
    /// 未受付なら開始位置へ戻し、入力設定を復元して終了を通知する。
    /// </summary>
    /// <param name="eventData">UIのポインター操作情報。</param>
    public void OnEndDrag(PointerEventData eventData)
    {
        // 未開始の操作や、別のポインターによる終了通知を無視する。
        if (!_isDragging || eventData.pointerId != _pointerId) return;

        _isDragging = false;

        if (!_isDropAccepted)
        {
            ReturnToStartPosition();
        }

        Debug.Log(
            $"[DragAndDrop] {name}: ドラッグ終了。" +
            $" Itemのposition={_rectTransform.anchoredPosition}",
            this);

        _canvasGroup.blocksRaycasts = _originalBlocksRaycasts;
        _onDragEnded.Invoke();
    }


    /// <summary>
    /// ドロップを受け付け済みにする。
    /// ドラッグ中でない場合や、すでに受け付けている場合は拒否する。
    /// </summary>
    /// <returns>今回の受付に成功した場合はtrue。</returns>
    public bool AcceptDrop()
    {
        // ドラッグ中でない場合や、すでに受け付けている場合は拒否する。
        if (!_isDragging || _isDropAccepted)
        {
            Debug.LogWarning(
                $"[DragAndDrop] {name}: ドラッグ中ではないため、受け付けできません。",
                this);
            return false;
        }

        _isDropAccepted = true;
        return true;
    }

    /// <summary>
    /// UIを今回のドラッグ開始位置に戻す。
    /// </summary>
    private void ReturnToStartPosition()
    {
        // 初期位置に戻す。
        _rectTransform.anchoredPosition = _startAnchoredPosition;
        Debug.Log($"[DragAndDrop] {name}: 元の位置へ戻した。", this);
    }

    /// <summary>
    /// 無効化時に操作状態と入力設定を復元する。
    /// </summary>
    private void OnDisable()
    {
        if (!_isDragging) return;

        _isDragging = false;
        _canvasGroup.blocksRaycasts = _originalBlocksRaycasts;

        Debug.Log($"[DragAndDrop] {name}: 無効化されたためドラッグを中断。",this);
    }
}