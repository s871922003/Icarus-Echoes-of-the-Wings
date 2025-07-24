using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ϥιw�]�� SpawnPoint �C��@���ͦ��̾ڪ��ĤH�ͦ��h�C
/// �~�Ӧ� CameraBasedEnemySpawnLayer�A�мg�ͦ��I���o�覡�C
/// </summary>
public class SpawnPointBasedEnemySpawnLayer : CameraBasedEnemySpawnLayer
{
    [Tooltip("�ͦ��I�C��A�ѳ]�p�v���w")]
    public List<Transform> SpawnPoints;

    protected override List<Vector3> GetRandomScreenEdgePositions(int count)
    {
        List<Vector3> positions = new List<Vector3>();

        if (SpawnPoints == null || SpawnPoints.Count == 0)
        {
            Debug.LogWarning("[SpawnPointBasedEnemySpawnLayer] �S���]�w����ͦ��I");
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
