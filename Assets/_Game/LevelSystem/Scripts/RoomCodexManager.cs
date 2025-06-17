using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomCodexManager : MonoBehaviour
{
    public static RoomCodexManager Instance;

    [Header("�����Ы����")]
    public List<RoomData> AllRooms;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public RoomData GetRoomDataByID(string id)
    {
        return AllRooms.Find(r => r.RoomID == id);
    }

    public void DiscoverRoom(string roomID)
    {
        var data = GetRoomDataByID(roomID);
        if (data == null)
        {
            Debug.LogWarning($"�Ы� ID [{roomID}] ���s�b���Ų���I");
            return;
        }

        if (!data.IsUnlocked)
        {
            data.IsUnlocked = true;
        }

        if (data.CurrentDiscoveryCount < data.MaxDiscoveryCount)
        {
            data.CurrentDiscoveryCount++;
        }
    }

    // �̾� RoomType �z��X�w���ꪺ�Ы��M��
    public List<RoomData> GetRoomsByType(RoomType type)
    {
        return AllRooms.Where(r => r.RoomType == type && r.IsUnlocked).ToList();
    }

    // �M�ΡG���o�_�l�Ы��]IsStartRoom �� true�^
    public RoomData GetStartRoom()
    {
        return AllRooms.FirstOrDefault(r => r.IsStartRoom && r.IsUnlocked);
    }

    public List<RoomData> GetAllUnlockedRooms()
    {
        return AllRooms.FindAll(r => r.IsUnlocked);
    }
}
