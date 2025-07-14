using UnityEngine;
using MoreMountains.TopDownEngine;

public static class RoomUtility
{
    /// <summary>
    /// 嘗試從目前場景中找到包含指定位置的 Room。
    /// </summary>
    /// <param name="position">欲查找的位置</param>
    /// <returns>若找到則回傳對應 Room，否則回傳 null</returns>
    public static Room FindRoomContaining(Vector3 position)
    {
#if UNITY_2023_1_OR_NEWER
        Room[] allRooms = Object.FindObjectsByType<Room>(FindObjectsSortMode.None);
#else
        Room[] allRooms = Object.FindObjectsOfType<Room>();
#endif
        foreach (Room room in allRooms)
        {
            if (room.RoomBounds.Contains(position))
            {
                return room;
            }
        }
        return null;
    }
}
