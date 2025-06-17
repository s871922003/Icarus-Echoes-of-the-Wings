using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tracks the player's current dungeon room index and provides access to current/next room logic.
/// </summary>
public class DungeonProgressManager : MonoBehaviour
{
    public static DungeonProgressManager Instance { get; private set; }

    [Tooltip("The current room index the player is in. Starts at 0 for Start Room.")]
    public int CurrentRoomIndex = 0;

    [Tooltip("The total number of rooms in this dungeon level.")]
    public int TotalRoomsInLevel = 7;

    [Tooltip("The list of room data the player has visited in this dungeon.")]
    public List<RoomData> RoomHistory = new List<RoomData>();

    /// <summary>
    /// Returns the RoomID of the most recently visited room, or "None" if no rooms have been visited.
    /// </summary>
    public string CurrentRoomID => RoomHistory.Count > 0 ? RoomHistory[^1].RoomID : "None";

    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Registers a room the player has just completed and advances the room index.
    /// </summary>
    public virtual void RegisterRoom(RoomData room)
    {
        if (room != null && !RoomHistory.Contains(room))
        {
            RoomHistory.Add(room);
        }
        CurrentRoomIndex++;
    }

    /// <summary>
    /// Call this when the player successfully completes a room.
    /// </summary>
    public virtual void AdvanceRoom()
    {
        CurrentRoomIndex++;
    }

    /// <summary>
    /// Resets dungeon progress, typically when restarting a level.
    /// </summary>
    public virtual void ResetProgress()
    {
        CurrentRoomIndex = 0;
        RoomHistory.Clear();
    }

    /// <summary>
    /// Returns the current index to be used for decision-making.
    /// </summary>
    public int GetCurrentRoomIndex()
    {
        return CurrentRoomIndex;
    }

    /// <summary>
    /// Returns true if the next room should be the Boss room, based on the current index and total rooms.
    /// </summary>
    public bool IsNextRoomBoss()
    {
        return CurrentRoomIndex + 1 == TotalRoomsInLevel;
    }
}
