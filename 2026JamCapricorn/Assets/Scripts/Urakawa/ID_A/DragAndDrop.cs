using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// UIのImageなどをドラッグし、離した位置に配置する。
/// ドラッグ開始・終了時にUnityEventで他の処理へ通知する。
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class DragAndDrop : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    /// <summary>
    /// ドラッグ開始時に呼び出すイベント。
    /// </summary>
    [SerializeField]
    private UnityEvent _onDragStarted = new UnityEvent();

    /// <summary>
    /// ドラッグ終了時に呼び出すイベント。
    /// </summary>
    [SerializeField]
    private UnityEvent _onDragEnded = new UnityEvent();

    /// <summary>
    /// 移動対象のUIのRectTransform。
    /// </summary>
    private RectTransform _rectTransform;

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
    /// 移動に使用するコンポーネントを取得する。
    /// </summary>
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
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

        // 親UIの平面に沿って動かすため、座標変換の基準として保持する。
        _dragPlane = _rectTransform.parent as RectTransform;

        if (_dragPlane == null)
        {
            Debug.LogError($"[DragAndDrop] {name}: 親にRectTransformが必要です。",this);
            return;
        }

        // 最初に押した位置とのずれを維持する。
        // pressEventCameraはCanvasに対応するカメラを渡す。
        // Screen Space - Overlayの場合はnullになり、そのまま使用できる。
        if (!RectTransformUtility.ScreenPointToWorldPointInRectangle(
                _dragPlane,
                eventData.pressPosition,
                eventData.pressEventCamera,
                out Vector3 pointerPosition))
        {
            Debug.LogWarning($"[DragAndDrop] {name}: ドラッグ位置を取得できません。",this);
            return;
        }

        _offset = _rectTransform.position - pointerPosition;
        _pointerId = eventData.pointerId;
        _isDragging = true;

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
    /// ドラッグを終了し、終了イベントを呼び出す。
    /// 配置先の判定や元の位置への復帰は、このメソッドでは行わない。
    /// </summary>
    /// <param name="eventData">UIのポインター操作情報。</param>
    public void OnEndDrag(PointerEventData eventData)
    {
        // 未開始の操作や、別のポインターによる終了通知を無視する。
        if (!_isDragging || eventData.pointerId != _pointerId) return;

        _isDragging = false;

        Debug.Log(
            $"[DragAndDrop] {name}: ドラッグ終了。" +
            $" Itemのposition={_rectTransform.anchoredPosition}",
            this);

        _onDragEnded.Invoke();
    }

    /// <summary>
    /// オブジェクトまたはスクリプトの無効化時に操作状態を解除する。
    /// </summary>
    private void OnDisable()
    {
        if (!_isDragging) return;

        _isDragging = false;

        Debug.Log($"[DragAndDrop] {name}: 無効化されたためドラッグを中断。",this);
    }
}