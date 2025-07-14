using UnityEngine;
using MoreMountains.TopDownEngine;

public static class RoomUtility
{
    /// <summary>
    /// ���ձq�ثe���������]�t���w��m�� Room�C
    /// </summary>
    /// <param name="position">���d�䪺��m</param>
    /// <returns>�Y���h�^�ǹ��� Room�A�_�h�^�� null</returns>
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
