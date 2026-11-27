using UnityEngine;
[CreateAssetMenu(fileName = "ToolData", menuName = "Cooking/Tool Data")]
public class ToolData : ScriptableObject//道具のデータを格納するスクリタブルオブジェクト
{
    [Header("道具の名称")]
    [SerializeField] public string toolName;//道具の名称
    [Header("道具の画像")]
    [SerializeField] public Sprite toolImage;//道具の画像
}
