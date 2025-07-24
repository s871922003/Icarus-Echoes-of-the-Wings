using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ĤH�ͦ��h�A�䴩�C�Ӫi���W�߳]�w�O�_�ϥΫ��w�ͦ��I�C
/// �� UseSpawnPoints �� true �ɡA�Ӫi�|�q���w��m�ͦ��ĤH�A�_�h�|�H��v����t���ͦ��I�C
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
        [Tooltip("�o�i�����}�l�ɶ��]��^")]
        public float StartTime;
        [Tooltip("�o�i��������ɶ��]��^")]
        public float Duration;
        [Tooltip("�n�ͦ����ĤH�P��")]
        public List<WeightedEnemyEntry> EnemyPrefabs;
        [Tooltip("�C�Ӷ��j�ͦ��h�ּĤH")]
        public int SpawnPerInterval = 5;
        [Tooltip("�C���ͦ����������j�]��^")]
        public float SpawnInterval = 1f;
        [Tooltip("���i���P�ɤ��\�̦h�s�b���ĤH�ƶq�]-1 ���L����^")]
        public int MaxConcurrentEnemies = 20;
        [Tooltip("�C���ͦ��ɷ|��ܦh�֭Ӧ�m�@���ͦ��I")]
        public int SpawnPointsPerInterval = 1;
        [Tooltip("�O�_�ϥΫ��w���ͦ��I�F�_�h�|�q��v����t�ͦ�")]
        public bool UseSpawnPoints = false;
        [Tooltip("�Y�ҥΥͦ��I�Ҧ��A���B���w�i�Ϊ���m Transform �M��")]
        public List<Transform> SpawnPoints;
    }

    [Header("�ĤH�i���]�w")]
    public List<EnemySpawnWave> Waves = new List<EnemySpawnWave>();

    [Header("��v���ͦ��Ҧ��Ѽ�")]
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
        Debug.Log("[EnemySpawnLayer] �Ұʥͦ��y�{");
        _isSpawning = true;
        _elapsedTime = 0f;
        _spawnRoutine = StartCoroutine(SpawnWavesCoroutine());
    }

    protected override void OnStopSpawning()
    {
        Debug.Log("[EnemySpawnLayer] ����ͦ��y�{");
        _isSpawning = false;
        if (_spawnRoutine != null)
        {
            StopCoroutine(_spawnRoutine);
            _spawnRoutine = null;
        }
    }

    protected override void OnClearAllSpawned()
    {
        Debug.Log("[EnemySpawnLayer] �M���Ҧ��w�ͦ��ĤH");
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
        Debug.Log("[EnemySpawnLayer] �}�l�i����{");
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
