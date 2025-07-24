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
        [Tooltip("波次名稱，僅供設計時參考，無實際功能")]
        public string WaveName;

        [Tooltip("波次說明，供設計時理解此波次作用")]
        [TextArea]
        public string Description;

        [Tooltip("波次開始時間（秒），生成系統啟動後經過這個秒數時啟動此波次")]
        public float StartTime;

        [Tooltip("波次持續時間（秒），從開始起持續多久進行生怪邏輯")]
        public float Duration;

        [Tooltip("此波次會隨機從這些敵人中進行生成，依照權重決定機率")]
        public List<WeightedEnemyEntry> EnemyPrefabs;

        [Tooltip("每次生成間隔內要生成的敵人數量")]
        public int SpawnPerInterval = 5;

        [Tooltip("每次生成後等待的秒數，然後再嘗試生成下一批")]
        public float SpawnInterval = 1f;

        [Tooltip("該波次所生成的敵人最大同時存活數量，設為 -1 代表無上限")]
        public int MaxConcurrentEnemies = 20;

        [Tooltip("每次生成中允許使用的最大生成點數量（螢幕邊緣隨機位置）")]
        public int SpawnPointsPerInterval = 1;
    }

    [Header("敵人波次設定")]
    public List<EnemySpawnWave> Waves = new List<EnemySpawnWave>();

    [Header("生成參數")]
    public float SpawnDistanceFromView = 2f;

    [Header("Z 軸修正")]
    [Tooltip("是否強制使用固定 Z 軸，而不是依據攝影機位置推算")]
    public bool UseFixedZ = false;

    [Tooltip("如果啟用固定 Z 軸，敵人會以這個值作為 Z")]
    public float FixedSpawnZ = 0f;

    [Tooltip("如果未啟用固定 Z 軸，會以攝影機位置 + 此值作為 Z")]
    public float SpawnZOffset = 0f;

    protected List<GameObject> _spawnedEnemies = new List<GameObject>();
    protected Coroutine _spawnRoutine;
    protected bool _isSpawning = false;
    protected float _elapsedTime = 0f;

    private void Update()
    {
        if (_isSpawning)
        {
            _elapsedTime += Time.deltaTime;
        }
    }

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
        foreach (var enemy in _spawnedEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        _spawnedEnemies.Clear();
    }

    protected IEnumerator SpawnWavesCoroutine()
    {
        List<Coroutine> activeCoroutines = new List<Coroutine>();

        foreach (var wave in Waves)
        {
            Coroutine coroutine = StartCoroutine(RunWaveCoroutine(wave));
            activeCoroutines.Add(coroutine);
        }

        foreach (var co in activeCoroutines)
        {
            yield return co;
        }
    }

    protected IEnumerator RunWaveCoroutine(EnemySpawnWave wave)
    {
        yield return new WaitUntil(() => _elapsedTime >= wave.StartTime);
        Debug.Log($"[Wave] 啟動: {wave.WaveName}");

        float waveEndTime = _elapsedTime + wave.Duration;

        while (_isSpawning && _elapsedTime <= waveEndTime)
        {
            _spawnedEnemies.RemoveAll(e => e == null);

            if (wave.MaxConcurrentEnemies > 0 && _spawnedEnemies.Count >= wave.MaxConcurrentEnemies)
            {
                yield return null;
                continue;
            }

            List<Vector3> spawnPositions = GetRandomScreenEdgePositions(wave.SpawnPointsPerInterval);
            int countPerPoint = Mathf.CeilToInt((float)wave.SpawnPerInterval / spawnPositions.Count);
            int spawnedThisRound = 0;

            foreach (Vector3 position in spawnPositions)
            {
                for (int j = 0; j < countPerPoint && spawnedThisRound < wave.SpawnPerInterval; j++)
                {
                    if (wave.MaxConcurrentEnemies > 0 && _spawnedEnemies.Count >= wave.MaxConcurrentEnemies)
                        break;

                    GameObject prefab = GetRandomEnemy(wave.EnemyPrefabs);
                    if (prefab != null)
                    {
                        Vector3 spawnPosition = position;
                        GameObject enemy = Instantiate(prefab, spawnPosition, Quaternion.identity);
                        Debug.Log($"[Enemy Spawned] {wave.WaveName} at {spawnPosition} using {prefab.name}");
                        _spawnedEnemies.Add(enemy);
                        spawnedThisRound++;
                    }
                }
            }

            yield return new WaitForSeconds(wave.SpawnInterval);
        }

        Debug.Log($"[Wave] 結束: {wave.WaveName}");
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

    protected virtual List<Vector3> GetRandomScreenEdgePositions(int count)
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
            Vector3 pos = edge switch
            {
                0 => new Vector3(Random.Range(-camWidth / 2f, camWidth / 2f), camHeight / 2f + dist, 0),
                1 => new Vector3(Random.Range(-camWidth / 2f, camWidth / 2f), -camHeight / 2f - dist, 0),
                2 => new Vector3(-camWidth / 2f - dist, Random.Range(-camHeight / 2f, camHeight / 2f), 0),
                3 => new Vector3(camWidth / 2f + dist, Random.Range(-camHeight / 2f, camHeight / 2f), 0),
                _ => Vector3.zero,
            };

            Vector3 worldPosition = center + pos;
            worldPosition.z = UseFixedZ ? FixedSpawnZ : cam.transform.position.z + SpawnZOffset;
            positions.Add(worldPosition);
        }

        return positions;
    }
}
