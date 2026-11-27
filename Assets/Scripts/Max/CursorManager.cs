using UnityEngine;
using UnityEngine.InputSystem;

// マウスカーソルを手の画像にする（押している間は閉じた手）
// 画像の Texture Type を「Cursor」にすること
public class CursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D openHand;
    [SerializeField] private Texture2D closedHand;
    [SerializeField] private Vector2 hotspot = new Vector2(16f, 16f); // クリック位置（画像の左上からのピクセル）

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
        if (texture != null)
            Cursor.SetCursor(texture, hotspot, CursorMode.Auto);
    }

    private void OnDisable()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
