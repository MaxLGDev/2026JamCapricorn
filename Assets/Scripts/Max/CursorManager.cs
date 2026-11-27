using UnityEngine;
using UnityEngine.InputSystem;

// マウスカーソルを手の画像にする（押している間は閉じた手）
// 画像の Texture Type を「Cursor」にすること
public class CursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D openHand;
    [SerializeField] private Texture2D closedHand;
    [SerializeField] private bool centerHotspot = true;                // ON = 画像の中心でクリック
    [SerializeField] private Vector2 hotspot = new Vector2(16f, 16f); // OFFの時のクリック位置（左上からのピクセル）
    [SerializeField] private CursorMode cursorMode = CursorMode.ForceSoftware; // Software = 画像サイズのまま表示（大きくできる）

    private bool isClosed;

    private void Start()
    {
        Apply(false);
    }

    private void Update()
    {
        if (Mouse.current == null)
            return;

        bool pressed = Mouse.current.leftButton.isPressed;
        if (pressed != isClosed)
            Apply(pressed);
    }

    private void Apply(bool closed)
    {
        isClosed = closed;
        Texture2D texture = closed ? closedHand : openHand;
        if (texture == null)
            return;

        Vector2 spot = centerHotspot ? new Vector2(texture.width * 0.5f, texture.height * 0.5f) : hotspot;
        Cursor.SetCursor(texture, spot, cursorMode);
    }

    private void OnDisable()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
