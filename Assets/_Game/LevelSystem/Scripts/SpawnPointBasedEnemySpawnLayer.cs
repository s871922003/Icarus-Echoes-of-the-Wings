using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ĤH�M�Ϊ��ͦ��h�A�i�䴩���ƪi���B�ɶ�����B�H���ͦ��I�P�񭫿�ܡC
/// �C�Ӫi���i�H�]�w�ͦ����j�B����ɶ��B�ͦ��I�P�̦h�P�ɦb���ĤH�Ƶ��ѼơC
/// </summary>
public class SpawnPointBasedEnemySpawnLayer : ChallengeSpawnLayer
{
    [System.Serializable]
    public class WeightedEnemyEntry
    {
        [Tooltip("�n�ͦ����ĤH Prefab")]
        public GameObject EnemyPrefab;

        [Tooltip("���ĤH���ͦ��v���A�ȶV���V�`�X�{")]
        public int Weight = 1;
    }

    [System.Serializable]
    public class EnemySpawnWave
    {
        [Tooltip("�o�@�i���W��")]
        public string WaveName;

        [Tooltip("�o�@�i���y�z�P�Ƶ�")]
        public string Description;

        [Tooltip("�Ӫi���}�l���ɶ� (��)")]
        public float StartTime;

        [Tooltip("�Ӫi���������ɶ� (��)")]
        public float EndTime;

        [Tooltip("�q�o�ǼĤH���H���ͦ��A�̾��v��")]
        public List<WeightedEnemyEntry> EnemyPrefabs;

        [Tooltip("���i���C���n�ͦ����ĤH�`�ơC�p�G SpawnPointsPerInterval < SpawnCount�A�h�����ͦ��I�|���ƥͦ��C")]
        public int SpawnCount = 5;

        [Tooltip("�C���ͦ��������ɶ����j (��)")]
        public float SpawnInterval = 1f;

        [Tooltip("�O�_�`�����ư���o�Ӫi��")]
        public bool Loop = false;

        [Tooltip("�P�ɳ̦h���\�b�����ĤH�ơA�Y�� -1 ��ܵL����")]
        public int MaxConcurrentEnemies = 20;

        [Tooltip("���i���n�ϥΪ��ͦ��I (�i�h��)")]
        public List<Transform> SpawnPoints;

        [Tooltip("�C���ͦ��ɭn�ҥΪ��ͦ��I�ƶq�A�C�ӥͦ��I�N���ղ��� SpawnCount / SpawnPointsPerInterval ���ĤH")]
        public int SpawnPointsPerInterval = 1;

        [Tooltip("�b�C�ӥͦ��I��m�[�W�@���H�������d�� (X, Y)�A��쬰�@�ɮy��")]
        public Vector2 SpawnPositionJitterMin;
        public Vector2 SpawnPositionJitterMax;
    }

    [Header("�ĤH�i���]�w")]
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
