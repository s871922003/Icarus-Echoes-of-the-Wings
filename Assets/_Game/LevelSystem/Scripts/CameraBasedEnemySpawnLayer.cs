using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 基於攝影機視野範圍外隨機位置的敵人生成層。
/// 每波敵人自動計算從螢幕外圍進場的座標並生成。
/// </summary>
public class CameraBasedEnemySpawnLayer : ChallengeSpawnLayer
{
    [System.Serializable]
    public class WeightedEnemyEntry
    {
        public GameObject EnemyPrefab;
        public int Weight = 1;
    }

    [System.Serializable]
    public class EnemySpawnWave
    {
        public string WaveName;
        public string Description;
        public float StartTime;
        public float EndTime;
        public List<WeightedEnemyEntry> EnemyPrefabs;
        public int SpawnCount = 5;
        public float SpawnInterval = 1f;
        public bool Loop = false;
        public int MaxConcurrentEnemies = 20;
        public int SpawnPointsPerInterval = 1;
        public Vector2 SpawnPositionJitterMin;
        public Vector2 SpawnPositionJitterMax;
    }

    [Header("敵人波次設定")]
    public List<EnemySpawnWave> Waves = new List<EnemySpawnWave>();

    [Header("生成參數")]
    public float SpawnDistanceFromView = 2f;

    protected List<GameObject> _spawnedEnemies = new List<GameObject>();
    protected Coroutine _spawnRoutine;
    protected bool _isSpawning = false;
    protected float _elapsedTime = 0f;

    protected override void OnStartSpawning()
    {
        Debug.Log("[CameraBasedEnemySpawnLayer] 啟動生成流程");
        _isSpawning = true;
        _elapsedTime = 0f;
        _spawnRoutine = StartCoroutine(SpawnWavesCoroutine());
    }

    protected override void OnStopSpawning()
    {
        Debug.Log("[CameraBasedEnemySpawnLayer] 停止生成流程");
        _isSpawning = false;
        if (_spawnRoutine != null)
        {
            StopCoroutine(_spawnRoutine);
            _spawnRoutine = null;
        }
    }

    protected override void OnClearAllSpawned()
    {
        Debug.Log("[CameraBasedEnemySpawnLayer] 清除所有已生成敵人");
        for (int i = _spawnedEnemies.Count - 1; i >= 0; i--)
        {
            if (_spawnedEnemies[i] != null)
            {
                Destroy(_spawnedEnemies[i]);
            }
        }
        _spawnedEnemies.Clear();
    }

    protected IEnumerator SpawnWavesCoroutine()
    {
        Debug.Log("[CameraBasedEnemySpawnLayer] 開始波次協程");
        while (_isSpawning)
        {
            _elapsedTime += Time.deltaTime;

            foreach (var wave in Waves)
            {
                if (_elapsedTime >= wave.StartTime && _elapsedTime <= wave.EndTime)
                {
                    Debug.Log($"[Wave] 開始執行波次: {wave.WaveName}");
                    yield return StartCoroutine(SpawnWave(wave));
                }
            }

            yield return null;
        }
    }

    protected IEnumerator SpawnWave(EnemySpawnWave wave)
    {
        int spawned = 0;

        do
        {
            while (_isSpawning && spawned < wave.SpawnCount)
            {
                _spawnedEnemies.RemoveAll(e => e == null);
                if (wave.MaxConcurrentEnemies > 0 && _spawnedEnemies.Count >= wave.MaxConcurrentEnemies)
                {
                    yield return null;
                    continue;
                }

                List<Vector3> spawnPositions = GetRandomScreenEdgePositions(wave.SpawnPointsPerInterval);
                int countPerPoint = Mathf.CeilToInt((float)wave.SpawnCount / spawnPositions.Count);

                foreach (Vector3 position in spawnPositions)
                {
                    for (int j = 0; j < countPerPoint && spawned < wave.SpawnCount; j++)
                    {
                        GameObject prefab = GetRandomEnemy(wave.EnemyPrefabs);
                        if (prefab != null)
                        {
                            Vector2 jitter = GetRandomJitter(wave.SpawnPositionJitterMin, wave.SpawnPositionJitterMax);
                            Vector3 spawnPosition = position + (Vector3)jitter;

                            GameObject enemy = Instantiate(prefab, spawnPosition, Quaternion.identity);
                            Debug.Log($"[Enemy Spawned] 位置: {spawnPosition}, Prefab: {prefab.name}");
                            _spawnedEnemies.Add(enemy);
                            spawned++;
                        }
                    }
                }

                yield return new WaitForSeconds(wave.SpawnInterval);
            }

            if (!wave.Loop) break;
            spawned = 0;

        } while (_isSpawning);
    }

    protected GameObject GetRandomEnemy(List<WeightedEnemyEntry> entries)
    {
        if (entries == null || entries.Count == 0) return null;

        int totalWeight = 0;
        foreach (var entry in entries)
        {
            totalWeight += entry.Weight;
        }

        int roll = Random.Range(0, totalWeight);
        int current = 0;

        foreach (var entry in entries)
        {
            current += entry.Weight;
            if (roll < current)
            {
                return entry.EnemyPrefab;
            }
        }

        return entries[entries.Count - 1].EnemyPrefab;
    }

    protected Vector2 GetRandomJitter(Vector2 min, Vector2 max)
    {
        float x = Random.Range(min.x, max.x);
        float y = Random.Range(min.y, max.y);
        return new Vector2(x, y);
    }

    protected List<Vector3> GetRandomScreenEdgePositions(int count)
    {
        List<Vector3> positions = new List<Vector3>();

        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("[CameraBasedEnemySpawnLayer] 找不到 MainCamera");
            return positions;
        }

        float dist = SpawnDistanceFromView;
        float camHeight = 2f * cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;
        Vector3 center = cam.transform.position;

        for (int i = 0; i < count; i++)
        {
            int edge = Random.Range(0, 4);
            Vector3 pos = Vector3.zero;

            switch (edge)
            {
                case 0: // 上
                    pos = new Vector3(Random.Range(-camWidth / 2f, camWidth / 2f), camHeight / 2f + dist, 0);
                    break;
                case 1: // 下
                    pos = new Vector3(Random.Range(-camWidth / 2f, camWidth / 2f), -camHeight / 2f - dist, 0);
                    break;
                case 2: // 左
                    pos = new Vector3(-camWidth / 2f - dist, Random.Range(-camHeight / 2f, camHeight / 2f), 0);
                    break;
                case 3: // 右
                    pos = new Vector3(camWidth / 2f + dist, Random.Range(-camHeight / 2f, camHeight / 2f), 0);
                    break;
            }

            positions.Add(center + pos);
        }

        return positions;
    }
}
