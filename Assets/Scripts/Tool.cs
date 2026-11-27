using UnityEngine;
using UnityEngine.UI;

public class Tool : MonoBehaviour
{
    [SerializeField] private ToolData data;

    public ToolData Data => data;

    private void Awake()
    {
        ApplyData();
    }

    public void Init(ToolData newData)
    {
        data = newData;
        ApplyData();
    }

    private void ApplyData()
    {
        if (data == null)
            return;

        Image image = GetComponentInChildren<Image>();
        if (image != null)
        {
            image.sprite = data.ToolImage;
        }

        GetComponent<DragAndDrop>().SetData(data);
    }
}
