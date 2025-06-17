using UnityEngine;


[CreateAssetMenu(fileName = "NewRoomData", menuName = "Dungeon/Room Data")]
public class RoomData : ScriptableObject
{
    [Header("基本資料")]
    public string RoomID;
    public GameObject RoomPrefab;
    public Sprite RoomIcon;
    [TextArea] public string Description;

    [Header("圖鑑進度")]
    public bool IsUnlocked = false;
    public int MaxDiscoveryCount = 3;
    public int CurrentDiscoveryCount = 0;

    [Header("流程圖屬性")]
    public RoomType RoomType;
    public bool IsStartRoom = false;
}
