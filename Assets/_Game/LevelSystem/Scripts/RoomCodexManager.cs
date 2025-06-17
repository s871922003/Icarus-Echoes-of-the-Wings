using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomCodexManager : MonoBehaviour
{
    public static RoomCodexManager Instance;

    [Header("全部房型資料")]
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
            Debug.LogWarning($"房型 ID [{roomID}] 不存在於圖鑑中！");
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

    // 依據 RoomType 篩選出已解鎖的房型清單
    public List<RoomData> GetRoomsByType(RoomType type)
    {
        return AllRooms.Where(r => r.RoomType == type && r.IsUnlocked).ToList();
    }

    // 專用：取得起始房型（IsStartRoom 為 true）
    public RoomData GetStartRoom()
    {
        return AllRooms.FirstOrDefault(r => r.IsStartRoom && r.IsUnlocked);
    }

    public List<RoomData> GetAllUnlockedRooms()
    {
        return AllRooms.FindAll(r => r.IsUnlocked);
    }
}
