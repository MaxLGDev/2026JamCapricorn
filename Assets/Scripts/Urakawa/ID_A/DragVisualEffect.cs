using UnityEngine;

/// <summary>
/// ドラッグ中の拡大と影を切り替える。
/// 操作や正解判定は担当しない。
/// </summary>
public class DragVisualEffect : MonoBehaviour
{
    [Header("表示対象")]
    [SerializeField] private RectTransform _visual;
    [SerializeField] private GameObject _shadow;

    [Header("拡大倍率")]
    [SerializeField, Range(0.1f, 3.0f)] private float _dragScale = 1.1f;

    /// <summary>
    /// ドラッグ終了時に戻す元のスケール。
    /// </summary>
    private Vector3 _originalScale;

    /// <summary>
    /// 元のスケールを保存し、演出を操作できる状態か。
    /// </summary>
    private bool _isInitialized;

    /// <summary>
    /// 参照を確認し、演出の初期状態を保存する。
    /// </summary>
    private void Awake()
    {
        if (_visual == null || _shadow == null)
        {
            Debug.LogError(
                $"[DragVisualEffect] {name}: VisualまたはShadowが未設定です。",
                this);
            return;
        }

        _originalScale = _visual.localScale;
        _isInitialized = true;

        ResetVisual();
    }

    /// <summary>
    /// 食材の見た目を拡大し、影を表示する。
    /// </summary>
    public void ShowDragging()
    {
        if (!_isInitialized) return; // 初期化前や参照未設定でもエラーにならないようにする。
        // _visualのスケールを元のスケール × 拡大倍率に設定する。
        _visual.localScale = _originalScale * _dragScale;
        // 影を表示する。
        _shadow.SetActive(true);
    }

    /// <summary>
    /// 元のスケールに戻し、影を非表示にする。
    /// </summary>
    public void ResetVisual()
    {
        if (!_isInitialized) return; // 初期化前や参照未設定でもエラーにならないようにする。
        // 保存したスケールへ戻す。
        _visual.localScale = _originalScale;
        // 影を非表示にする。
        _shadow.SetActive(false);
    }

    /// <summary>
    /// 無効化時に見た目を戻し、再表示時に演出が残るのを防ぐ。
    /// </summary>
    private void OnDisable()
    {
        ResetVisual();
    }
}