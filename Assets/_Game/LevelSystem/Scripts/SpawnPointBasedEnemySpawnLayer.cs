using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵人專用的生成層，可支援重複波次、時間條件、隨機生成點與比重選擇。
/// 每個波次可以設定生成間隔、持續時間、生成點與最多同時在場敵人數等參數。
/// </summary>
public class SpawnPointBasedEnemySpawnLayer : ChallengeSpawnLayer
{
    [System.Serializable]
    public class WeightedEnemyEntry
    {
        [Tooltip("要生成的敵人 Prefab")]
        public GameObject EnemyPrefab;

        [Tooltip("此敵人的生成權重，值越高越常出現")]
        public int Weight = 1;
    }

    [System.Serializable]
    public class EnemySpawnWave
    {
        [Tooltip("這一波的名稱")]
        public string WaveName;

        [Tooltip("這一波的描述與備註")]
        public string Description;

        [Tooltip("該波次開始的時間 (秒)")]
        public float StartTime;

        [Tooltip("該波次結束的時間 (秒)")]
        public float EndTime;

        [Tooltip("從這些敵人中隨機生成，依據權重")]
        public List<WeightedEnemyEntry> EnemyPrefabs;

        [Tooltip("本波次每次要生成的敵人總數。如果 SpawnPointsPerInterval < SpawnCount，則部分生成點會重複生成。")]
        public int SpawnCount = 5;

        [Tooltip("每次生成之間的時間間隔 (秒)")]
        public float SpawnInterval = 1f;

        [Tooltip("是否循環重複執行這個波次")]
        public bool Loop = false;

        [Tooltip("同時最多允許在場的敵人數，若為 -1 表示無限制")]
        public int MaxConcurrentEnemies = 20;

        [Tooltip("本波次要使用的生成點 (可多個)")]
        public List<Transform> SpawnPoints;

        [Tooltip("每次生成時要啟用的生成點數量，每個生成點將嘗試產生 SpawnCount / SpawnPointsPerInterval 的敵人")]
        public int SpawnPointsPerInterval = 1;

        [Tooltip("在每個生成點位置加上一個隨機偏移範圍 (X, Y)，單位為世界座標")]
        public Vector2 SpawnPositionJitterMin;
        public Vector2 SpawnPositionJitterMax;
    }

    [Header("敵人波次設定")]
    public List<EnemySpawnWave> Waves = new List<EnemySpawnWave>();

    protected List<GameObject> _spawnedEnemies = new List<GameObject>();
    protected Coroutine _spawnRoutine;
    protected bool _isSpawning = false;
    protected float _elapsedTime = 0f;

    protected override void OnStartSpawning()
    {
        _isSpawning = true;
        _elapsedTime = 0f;
        _spawnRoutine = StartCoroutine(SpawnWavesCoroutine());
    }

    protected override void OnStopSpawning()
    {
        _isSpawning = false;
        if (_spawnRoutine != null)
        {
            StopCoroutine(_spawnRoutine);
            _spawnRoutine = null;
        }
    }

    protected override void OnClearAllSpawned()
    {
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
        while (_isSpawning)
        {
            _elapsedTime += Time.deltaTime;

            foreach (var wave in Waves)
            {
                if (_elapsedTime >= wave.StartTime && _elapsedTime <= wave.EndTime)
                {
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

                int pointsToUse = Mathf.Min(wave.SpawnPointsPerInterval, wave.SpawnPoints.Count);
                List<Transform> selectedPoints = new List<Transform>();

                while (selectedPoints.Count < pointsToUse)
                {
                    Transform candidate = GetRandomSpawnPoint(wave.SpawnPoints);
                    if (!selectedPoints.Contains(candidate))
                    {
                        selectedPoints.Add(candidate);
                    }
                }

                int countPerPoint = Mathf.CeilToInt((float)wave.SpawnCount / pointsToUse);

                foreach (Transform spawnPoint in selectedPoints)
                {
                    for (int j = 0; j < countPerPoint && spawned < wave.SpawnCount; j++)
                    {
                        GameObject prefab = GetRandomEnemy(wave.EnemyPrefabs);
                        if (prefab != null)
                        {
                            Vector2 jitter = GetRandomJitter(wave.SpawnPositionJitterMin, wave.SpawnPositionJitterMax);
                            Vector3 spawnPosition = spawnPoint.position + (Vector3)jitter;

                            GameObject enemy = Instantiate(prefab, spawnPosition, Quaternion.identity);
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

    protected Transform GetRandomSpawnPoint(List<Transform> spawnPoints)
    {
        if (spawnPoints == null || spawnPoints.Count == 0) return null;
        return spawnPoints[Random.Range(0, spawnPoints.Count)];
    }

    protected Vector2 GetRandomJitter(Vector2 min, Vector2 max)
    {
        float x = Random.Range(min.x, max.x);
        float y = Random.Range(min.y, max.y);
        return new Vector2(x, y);
    }
}
