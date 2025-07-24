using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �����v�������d��~�H����m���ĤH�ͦ��h�C
/// �C�i�ĤH�۰ʭp��q�ù��~��i�����y�Шåͦ��C
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
        [Tooltip("�i���W�١A�Ȩѳ]�p�ɰѦҡA�L��ڥ\��")]
        public string WaveName;

        [Tooltip("�i�������A�ѳ]�p�ɲz�Ѧ��i���@��")]
        [TextArea]
        public string Description;

        [Tooltip("�i���}�l�ɶ��]��^�A�ͦ��t�αҰʫ�g�L�o�Ӭ�ƮɱҰʦ��i��")]
        public float StartTime;

        [Tooltip("�i������ɶ��]��^�A�q�}�l�_����h�[�i��ͩ��޿�")]
        public float Duration;

        [Tooltip("���i���|�H���q�o�ǼĤH���i��ͦ��A�̷��v���M�w���v")]
        public List<WeightedEnemyEntry> EnemyPrefabs;

        [Tooltip("�C���ͦ����j���n�ͦ����ĤH�ƶq")]
        public int SpawnPerInterval = 5;

        [Tooltip("�C���ͦ��ᵥ�ݪ���ơA�M��A���եͦ��U�@��")]
        public float SpawnInterval = 1f;

        [Tooltip("�Ӫi���ҥͦ����ĤH�̤j�P�ɦs���ƶq�A�]�� -1 �N��L�W��")]
        public int MaxConcurrentEnemies = 20;

        [Tooltip("�C���ͦ������\�ϥΪ��̤j�ͦ��I�ƶq�]�ù���t�H����m�^")]
        public int SpawnPointsPerInterval = 1;
    }

    [Header("�ĤH�i���]�w")]
    public List<EnemySpawnWave> Waves = new List<EnemySpawnWave>();

    [Header("�ͦ��Ѽ�")]
    public float SpawnDistanceFromView = 2f;

    [Header("Z �b�ץ�")]
    [Tooltip("�O�_�j��ϥΩT�w Z �b�A�Ӥ��O�̾���v����m����")]
    public bool UseFixedZ = false;

    [Tooltip("�p�G�ҥΩT�w Z �b�A�ĤH�|�H�o�ӭȧ@�� Z")]
    public float FixedSpawnZ = 0f;

    [Tooltip("�p�G���ҥΩT�w Z �b�A�|�H��v����m + ���ȧ@�� Z")]
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
        Debug.Log("[CameraBasedEnemySpawnLayer] �Ұʥͦ��y�{");
        _isSpawning = true;
        _elapsedTime = 0f;
        _spawnRoutine = StartCoroutine(SpawnWavesCoroutine());
    }

    protected override void OnStopSpawning()
    {
        Debug.Log("[CameraBasedEnemySpawnLayer] ����ͦ��y�{");
        _isSpawning = false;
        if (_spawnRoutine != null)
        {
            StopCoroutine(_spawnRoutine);
            _spawnRoutine = null;
        }
    }

    protected override void OnClearAllSpawned()
    {
        Debug.Log("[CameraBasedEnemySpawnLayer] �M���Ҧ��w�ͦ��ĤH");
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
        Debug.Log($"[Wave] �Ұ�: {wave.WaveName}");

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

        Debug.Log($"[Wave] ����: {wave.WaveName}");
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
            Debug.LogWarning("[CameraBasedEnemySpawnLayer] �䤣�� MainCamera");
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
