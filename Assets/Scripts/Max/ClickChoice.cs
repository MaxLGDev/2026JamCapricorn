using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

// 生地・道具をクリック1回で選ぶ。選ばれたものはパルス（拡大縮小）で光る
// ボタン（KijiButton / ToolButton）に付ける。Kiji か Tool のどちらか片方だけ設定する
public class ClickChoice : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private RoundManager roundManager;

    [Header("どちらか片方だけ")]
    [SerializeField] private IngredientData kiji;
    [SerializeField] private ToolData tool;

    [Header("見た目")]
    [SerializeField] private Transform pulseTarget;   // 空なら親（KijiImage / ToolImage）
    [SerializeField] private float pulseAmount = 0.12f;
    [SerializeField] private float pulseSpeed = 6f;
    [SerializeField] private float shakeTime = 0.25f; // 不正解の時に揺れる

    [Header("不正解のペナルティ（秒）")]
    [SerializeField] private float wrongPenalty = 1f;

    // グループ（生地 / 道具）ごとに選ばれているもの
    private static ClickChoice selectedKiji;
    private static ClickChoice selectedTool;

    private Vector3 baseScale;
    private Vector3 basePos;
    private bool usedThisRound; // 道具はラウンドごとに1回だけ判定

    private bool IsKiji => kiji != null;

    public IngredientData Kiji => kiji;

    // パネルに表示されている絵（RecipeHUD が使う）
    public Sprite PanelSprite
    {
        get
        {
            Transform t = pulseTarget != null ? pulseTarget : (transform.parent != null ? transform.parent : transform);
            UnityEngine.UI.Image img = t.GetComponent<UnityEngine.UI.Image>();
            return img != null ? img.sprite : null;
        }
    }
    private bool IsSelected => (IsKiji ? selectedKiji : selectedTool) == this;

    private void Awake()
    {
        if (pulseTarget == null)
            pulseTarget = transform.parent != null ? transform.parent : transform;

        baseScale = pulseTarget.localScale;
    }

    private void OnEnable()
    {
        RoundManager.RoundStarted += OnRoundStarted;
        RoundManager.RoundEnded += OnRoundEnded;
    }

    private void OnDisable()
    {
        RoundManager.RoundStarted -= OnRoundStarted;
        RoundManager.RoundEnded -= OnRoundEnded;
        if (selectedKiji == this) selectedKiji = null;
        if (selectedTool == this) selectedTool = null;
    }

    private void OnRoundStarted(int round, RecipeData recipe)
    {
        usedThisRound = false;
        Deselect();
    }

    private void OnRoundEnded(bool won)
    {
        if (!won)
            Deselect();
    }

    private void Deselect()
    {
        if (selectedKiji == this) selectedKiji = null;
        if (selectedTool == this) selectedTool = null;
        pulseTarget.localScale = baseScale;
    }

    private void Update()
    {
        if (!IsSelected)
            return;

        float s = 1f + Mathf.Abs(Mathf.Sin(Time.time * pulseSpeed)) * pulseAmount;
        pulseTarget.localScale = baseScale * s;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (roundManager == null || roundManager.CurrentState != RoundState.Playing)
            return;

        if (IsKiji)
            ChooseKiji();
        else if (tool != null)
            ChooseTool();
    }

    private void ChooseKiji()
    {
        if (IsSelected)
            return; // もう正解を選んでいる

        bool correct = kiji == roundManager.CurrentRecipe.RecipeBase;
        DropZone.Judge(correct, false);

        if (correct)
        {
            Select();
            roundManager.SetRecipeBaseResult(true);
        }
        else
        {
            roundManager.DecreaseTime(wrongPenalty);
            StartCoroutine(Shake());
        }
    }

    private void ChooseTool()
    {
        if (usedThisRound)
            return;

        usedThisRound = true;
        ToolData[] required = roundManager.CurrentRecipe.RequiredTool;

        // レシピに道具がない場合は選択の見た目だけ
        if (required == null || required.Length == 0)
        {
            Select();
            return;
        }

        bool correct = System.Array.IndexOf(required, tool) >= 0;
        DropZone.Judge(correct, true);

        if (correct)
            Select();
        else
            StartCoroutine(Shake());
    }

    private void Select()
    {
        ClickChoice old = IsKiji ? selectedKiji : selectedTool;
        if (old != null && old != this)
            old.pulseTarget.localScale = old.baseScale;

        if (IsKiji) selectedKiji = this;
        else selectedTool = this;
    }

    private IEnumerator Shake()
    {
        basePos = pulseTarget.localPosition;
        float t = 0f;
        while (t < shakeTime)
        {
            t += Time.deltaTime;
            pulseTarget.localPosition = basePos + Vector3.right * Mathf.Sin(t * 80f) * 8f;
            yield return null;
        }
        pulseTarget.localPosition = basePos;
    }
}
