using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using System.Collections.Generic;

public class NextRoomPortalSpawner : MonoBehaviour, MMEventListener<MMRoomChallengeEvent>
{
    [Header("GoToLevelEntryPoint Prefab")]
    [Tooltip("The portal prefab to spawn when the challenge is complete")]
    public GoToLevelEntryPoint PortalPrefab;

    [Header("Spawn Settings")]
    [Tooltip("Offset from the player position to spawn the portal")]
    public Vector3 SpawnOffset = new Vector3(1.5f, 0f, 0f);

    [Header("Optional Room Type Filter")]
    [Tooltip("Set this to filter rooms by type. Leave null to use all unlocked rooms.")]
    public RoomType RoomTypeFilter;

    [Tooltip("Enable this if you want to use the RoomType filter above")]
    public bool UseRoomTypeFilter = false;

    protected virtual void OnEnable()
    {
        this.MMEventStartListening<MMRoomChallengeEvent>();
    }

    protected virtual void OnDisable()
    {
        this.MMEventStopListening<MMRoomChallengeEvent>();
    }

    public void OnMMEvent(MMRoomChallengeEvent challengeEvent)
    {
        if (challengeEvent.EventType != MMRoomChallengeEventType.ChallengeComplete)
        {
            return;
        }

        SpawnNextRoomPortal();
    }

    protected virtual void SpawnNextRoomPortal()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("NextRoomPortalSpawner: Player not found.");
            return;
        }

        DungeonProgressManager progressManager = DungeonProgressManager.Instance;
        RoomCodexManager codexManager = RoomCodexManager.Instance;

        if (progressManager == null || codexManager == null)
        {
            Debug.LogWarning("NextRoomPortalSpawner: Missing DungeonProgressManager or RoomCodexManager.");
            return;
        }

        RoomData nextRoom = null;

        if (progressManager.IsNextRoomBoss())
        {
            nextRoom = codexManager.GetRoomsByType(RoomType.Boss).Find(r => r.IsUnlocked);
        }
        else
        {
            List<RoomData> candidateRooms = UseRoomTypeFilter
                ? codexManager.GetRoomsByType(RoomTypeFilter)
                : codexManager.GetAllUnlockedRooms();

            // 排除 Boss、Start、未知房型等不可用房型
            candidateRooms.RemoveAll(r => r.RoomType == RoomType.Boss || r.RoomType == RoomType.Start || r.RoomType == RoomType.Unknown);

            if (candidateRooms.Count == 0)
            {
                Debug.LogWarning("NextRoomPortalSpawner: No available non-boss unlocked rooms to choose from.");
                return;
            }

            nextRoom = candidateRooms[Random.Range(0, candidateRooms.Count)];
        }

        if (nextRoom == null)
        {
            Debug.LogWarning("NextRoomPortalSpawner: No valid RoomData found.");
            return;
        }

        Vector3 spawnPosition = player.transform.position + SpawnOffset;
        GoToLevelEntryPoint portal = Instantiate(PortalPrefab, spawnPosition, Quaternion.identity);
        portal.LevelName = nextRoom.RoomID;

        Debug.Log($"NextRoomPortalSpawner: Spawned portal to {nextRoom.RoomID} at {spawnPosition}");

        // 註冊房型與更新進度
        progressManager.RegisterRoom(nextRoom);
    }
}
