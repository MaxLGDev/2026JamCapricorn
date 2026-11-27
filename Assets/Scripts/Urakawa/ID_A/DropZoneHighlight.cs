using UnityEngine;

/// <summary>
/// 食材のドラッグ開始から終了まで、3Dドロップ先をEmissionで強調する。
/// マウス位置には依存せず、UnityEventから開始・終了を受け取る。
/// </summary>
public class DropZoneHighlight : MonoBehaviour
{
    [Header("表示対象")]
    [SerializeField] private Renderer _targetRenderer;

    [Header("発光色")]
    [ColorUsage(false, true)]
    [SerializeField] private Color _highlightEmission = Color.yellow;

    [Header("点滅")]
    [SerializeField] private bool _blink = true;

    [Tooltip("点灯と消灯を合わせた1周期の秒数")]
    [SerializeField, Min(0.1f)] private float _blinkPeriod = 0.8f;

    private static readonly int EmissionColorId =
        Shader.PropertyToID("_EmissionColor");

    private Material _originalMaterial;
    private Material _runtimeMaterial;
    private Color _originalEmission;

    private bool _isInitialized;
    private bool _isHighlighted;
    private bool _isBright;
    private float _elapsedTime;

    /// <summary>
    /// 終了イベントが来ない無効化・削除も検出するため、操作元を保持する。
    /// </summary>
    private DragAndDrop _dragSource;

    /// <summary>
    /// このRenderer専用のマテリアルを作成する。
    /// </summary>
    private void Awake()
    {
        if (_targetRenderer == null ||
            _targetRenderer.sharedMaterial == null)
        {
            Debug.LogError(
                $"[DropZoneHighlight] {name}: RendererまたはMaterialが未設定です。",
                this);
            return;
        }

        _originalMaterial = _targetRenderer.sharedMaterial;

        if (!_originalMaterial.HasProperty(EmissionColorId))
        {
            Debug.LogError(
                $"[DropZoneHighlight] {name}: _EmissionColorに対応していません。",
                this);
            return;
        }

        _runtimeMaterial = new Material(_originalMaterial);
        _targetRenderer.sharedMaterial = _runtimeMaterial;
        _originalEmission = _runtimeMaterial.GetColor(EmissionColorId);

        _isInitialized = true;
    }

    /// <summary>
    /// ドラッグ開始イベントから呼び出し、発光を開始する。
    /// </summary>
    /// <param name="source">今回ドラッグを開始した食材。</param>
    public void StartHighlight(DragAndDrop source)
    {
        if (!_isInitialized || !isActiveAndEnabled) return;

        if (source == null || !source.isActiveAndEnabled ||
            !source.IsDragging)
        {
            return;
        }

        _dragSource = source;
        _elapsedTime = 0f;
        _isHighlighted = true;
        _isBright = true;

        // 開始直後は必ず点灯する。
        _runtimeMaterial.SetColor(EmissionColorId, _highlightEmission);

        Debug.Log($"[DropZoneHighlight] {name}: 発光開始。", this);
    }

    /// <summary>
    /// 操作の中断を確認し、必要なら時間に応じて点滅させる。
    /// </summary>
    private void Update()
    {
        if (!_isHighlighted) return;

        // DragAndDropだけが無効化された場合も解除する。
        if (_dragSource == null ||
            !_dragSource.isActiveAndEnabled ||
            !_dragSource.IsDragging)
        {
            StopHighlight();
            return;
        }

        _elapsedTime += Time.unscaledDeltaTime;

        float period = Mathf.Max(0.1f, _blinkPeriod);
        bool shouldBeBright =
            !_blink || (_elapsedTime % period) < period * 0.5f;

        // 毎フレーム書き換えず、点灯状態が変わったときだけ反映する。
        if (_isBright == shouldBeBright) return;

        _isBright = shouldBeBright;
        _runtimeMaterial.SetColor(
            EmissionColorId,
            _isBright ? _highlightEmission : _originalEmission);
    }

    /// <summary>
    /// 発光を終了し、元のEmissionへ戻す。
    /// ドラッグ終了イベントから呼び出す。
    /// </summary>
    public void StopHighlight()
    {
        if (!_isInitialized || !_isHighlighted) return;

        _isHighlighted = false;
        _dragSource = null;

        _runtimeMaterial.SetColor(EmissionColorId, _originalEmission);

        Debug.Log($"[DropZoneHighlight] {name}: 発光終了。", this);
    }

    /// <summary>
    /// DropZoneの無効化時にも発光を解除する。
    /// </summary>
    private void OnDisable()
    {
        StopHighlight();
    }

    /// <summary>
    /// 元のマテリアルを復元し、専用コピーを解放する。
    /// </summary>
    private void OnDestroy()
    {
        if (_runtimeMaterial == null) return;

        if (_targetRenderer != null &&
            _targetRenderer.sharedMaterial == _runtimeMaterial)
        {
            _targetRenderer.sharedMaterial = _originalMaterial;
        }

        Destroy(_runtimeMaterial);
    }
}