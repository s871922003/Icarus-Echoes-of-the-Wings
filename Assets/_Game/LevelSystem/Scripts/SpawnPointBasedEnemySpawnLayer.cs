using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 使用預設的 SpawnPoint 列表作為生成依據的敵人生成層。
/// 繼承自 CameraBasedEnemySpawnLayer，覆寫生成點取得方式。
/// </summary>
public class SpawnPointBasedEnemySpawnLayer : CameraBasedEnemySpawnLayer
{
    [Tooltip("生成點列表，由設計師指定")]
    public List<Transform> SpawnPoints;

    protected override List<Vector3> GetRandomScreenEdgePositions(int count)
    {
        List<Vector3> positions = new List<Vector3>();

        if (SpawnPoints == null || SpawnPoints.Count == 0)
        {
            Debug.LogWarning("[SpawnPointBasedEnemySpawnLayer] 沒有設定任何生成點");
            return positions;
        }

        for (int i = 0; i < count; i++)
        {
            Transform spawnPoint = SpawnPoints[Random.Range(0, SpawnPoints.Count)];
            positions.Add(spawnPoint.position);
        }

        return positions;
    }
}
