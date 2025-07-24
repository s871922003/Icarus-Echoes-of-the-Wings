using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵人生成層，支援每個波次獨立設定是否使用指定生成點。
/// 當 UseSpawnPoints 為 true 時，該波會從指定位置生成敵人，否則會以攝影機邊緣為生成點。
/// </summary>
public class EnemySpawnLayer : ChallengeSpawnLayer
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
        [Tooltip("這波次的開始時間（秒）")]
        public float StartTime;
        [Tooltip("這波次的持續時間（秒）")]
        public float Duration;
        [Tooltip("要生成的敵人與比重")]
        public List<WeightedEnemyEntry> EnemyPrefabs;
        [Tooltip("每個間隔生成多少敵人")]
        public int SpawnPerInterval = 5;
        [Tooltip("每次生成之間的間隔（秒）")]
        public float SpawnInterval = 1f;
        [Tooltip("此波次同時允許最多存在的敵人數量（-1 為無限制）")]
        public int MaxConcurrentEnemies = 20;
        [Tooltip("每次生成時會選擇多少個位置作為生成點")]
        public int SpawnPointsPerInterval = 1;
        [Tooltip("是否使用指定的生成點；否則會從攝影機邊緣生成")]
        public bool UseSpawnPoints = false;
        [Tooltip("若啟用生成點模式，此處指定可用的位置 Transform 清單")]
        public List<Transform> SpawnPoints;
    }

    [Header("敵人波次設定")]
    public List<EnemySpawnWave> Waves = new List<EnemySpawnWave>();

    [Header("攝影機生成模式參數")]
    public float SpawnDistanceFromView = 2f;
    public bool UseFixedZ = false;
    public float FixedSpawnZ = 0f;
    public float SpawnZOffset = 0f;

    protected List<GameObject> _spawnedEnemies = new List<GameObject>();
    protected Coroutine _spawnRoutine;
    protected bool _isSpawning = false;
    protected float _elapsedTime = 0f;

    protected override void OnStartSpawning()
    {
        Debug.Log("[EnemySpawnLayer] 啟動生成流程");
        _isSpawning = true;
        _elapsedTime = 0f;
        _spawnRoutine = StartCoroutine(SpawnWavesCoroutine());
    }

    protected override void OnStopSpawning()
    {
        Debug.Log("[EnemySpawnLayer] 停止生成流程");
        _isSpawning = false;
        if (_spawnRoutine != null)
        {
            StopCoroutine(_spawnRoutine);
            _spawnRoutine = null;
        }
    }

    protected override void OnClearAllSpawned()
    {
        Debug.Log("[EnemySpawnLayer] 清除所有已生成敵人");
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
        Debug.Log("[EnemySpawnLayer] 開始波次協程");
        foreach (var wave in Waves)
        {
            StartCoroutine(RunWave(wave));
        }
        while (_isSpawning)
        {
            _elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    protected IEnumerator RunWave(EnemySpawnWave wave)
    {
        float waveStart = wave.StartTime;
        float waveEnd = wave.StartTime + wave.Duration;
        float nextSpawnTime = wave.StartTime;

        while (_isSpawning && _elapsedTime <= waveEnd)
        {
            _spawnedEnemies.RemoveAll(e => e == null);

            if (_elapsedTime >= nextSpawnTime)
            {
                if (wave.MaxConcurrentEnemies < 0 || GetWaveActiveEnemyCount(wave) < wave.MaxConcurrentEnemies)
                {
                    List<Vector3> spawnPositions = GetWaveSpawnPositions(wave);
                    int spawnedThisInterval = 0;

                    foreach (var pos in spawnPositions)
                    {
                        if (spawnedThisInterval >= wave.SpawnPerInterval) break;
                        GameObject prefab = GetRandomEnemy(wave.EnemyPrefabs);
                        if (prefab != null)
                        {
                            Vector3 finalPos = pos;
                            finalPos.z = UseFixedZ ? FixedSpawnZ : Camera.main.transform.position.z + SpawnZOffset;

                            GameObject enemy = Instantiate(prefab, finalPos, Quaternion.identity);
                            _spawnedEnemies.Add(enemy);
                            spawnedThisInterval++;
                        }
                    }
                }
                nextSpawnTime += wave.SpawnInterval;
            }

            yield return null;
        }
    }

    protected int GetWaveActiveEnemyCount(EnemySpawnWave wave)
    {
        return _spawnedEnemies.FindAll(e => e != null).Count;
    }

    protected GameObject GetRandomEnemy(List<WeightedEnemyEntry> entries)
    {
        if (entries == null || entries.Count == 0) return null;

        int totalWeight = 0;
        foreach (var entry in entries) totalWeight += entry.Weight;
        int roll = Random.Range(0, totalWeight);
        int current = 0;
        foreach (var entry in entries)
        {
            current += entry.Weight;
            if (roll < current) return entry.EnemyPrefab;
        }
        return entries[entries.Count - 1].EnemyPrefab;
    }

    protected List<Vector3> GetWaveSpawnPositions(EnemySpawnWave wave)
    {
        List<Vector3> positions = new List<Vector3>();

        if (wave.UseSpawnPoints && wave.SpawnPoints != null && wave.SpawnPoints.Count > 0)
        {
            for (int i = 0; i < wave.SpawnPointsPerInterval; i++)
            {
                Transform t = wave.SpawnPoints[Random.Range(0, wave.SpawnPoints.Count)];
                positions.Add(t.position);
            }
        }
        else
        {
            positions = GetRandomScreenEdgePositions(wave.SpawnPointsPerInterval);
        }

        return positions;
    }

    protected List<Vector3> GetRandomScreenEdgePositions(int count)
    {
        List<Vector3> positions = new List<Vector3>();
        Camera cam = Camera.main;
        if (cam == null) return positions;

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
                case 0: pos = new Vector3(Random.Range(-camWidth / 2f, camWidth / 2f), camHeight / 2f + dist, 0); break;
                case 1: pos = new Vector3(Random.Range(-camWidth / 2f, camWidth / 2f), -camHeight / 2f - dist, 0); break;
                case 2: pos = new Vector3(-camWidth / 2f - dist, Random.Range(-camHeight / 2f, camHeight / 2f), 0); break;
                case 3: pos = new Vector3(camWidth / 2f + dist, Random.Range(-camHeight / 2f, camHeight / 2f), 0); break;
            }
            Vector3 worldPos = center + pos;
            positions.Add(worldPos);
        }

        return positions;
    }
}
