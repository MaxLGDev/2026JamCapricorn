using UnityEngine;
using UnityEngine.EventSystems;

public class HoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Transform target;
    [SerializeField] private float hoverScale = 1.15f;
    [SerializeField] private float speed = 12f;

    private Vector3 baseScale;
    private Vector3 goalScale;

    private void Awake()
    {
        if (target == null) target = transform;
        baseScale = target.localScale;
        goalScale = baseScale;
    }

    private void OnDisable()
    {
        target.localScale = baseScale;   // don't stay big if hidden while hovered
        goalScale = baseScale;
    }

    public void OnPointerEnter(PointerEventData eventData) => goalScale = baseScale * hoverScale;
    public void OnPointerExit(PointerEventData eventData) => goalScale = baseScale;

    private void Update()
    {
        target.localScale = Vector3.Lerp(target.localScale, goalScale, speed * Time.unscaledDeltaTime);
    }
}