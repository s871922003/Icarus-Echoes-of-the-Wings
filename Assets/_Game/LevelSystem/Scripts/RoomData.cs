using UnityEngine;


[CreateAssetMenu(fileName = "NewRoomData", menuName = "Dungeon/Room Data")]
public class RoomData : ScriptableObject
{
    [Header("�򥻸��")]
    public string RoomID;
    public GameObject RoomPrefab;
    public Sprite RoomIcon;
    [TextArea] public string Description;

    [Header("��Ų�i��")]
    public bool IsUnlocked = false;
    public int MaxDiscoveryCount = 3;
    public int CurrentDiscoveryCount = 0;

    [Header("�y�{���ݩ�")]
    public RoomType RoomType;
    public bool IsStartRoom = false;
}
