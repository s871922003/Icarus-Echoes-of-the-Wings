using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

/// <summary>
/// ChallengeSpawnManager �t�d�b�D�Զ}�l��ͦ��D�ԥΪ���A�p�ĤH�B�ɵ��B��ê�����C
/// ���� IcarusLevelManager ����A�䴩�h�h�����ͦ�����P����M���޿�C
/// </summary>
public class ChallengeSpawnManager : MonoBehaviour
{
    [SerializeField] IcarusLevelManager icarusLevelManager;

    [Header("�ͦ��h�w�q")]
    public List<ChallengeSpawnLayer> SpawnLayers;

    [Header("�D�Զ}�l�۰ʱҰ�")]
    [Tooltip("�O�_�b IcarusLevelManager �ҰʬD�Ԯɦ۰ʶ}�l�ͦ��D�Ԫ���")]
    public bool AutoStartOnChallenge = true;

    protected bool _spawningActive = false;

    private void OnEnable()
    {
        if (icarusLevelManager == null) 
        {
            Debug.Log("�ͦ��t�λݭn IcarusLevelManager ���ѦҤ~��B�@�A�T�{�@�U�ѦҪ��A");
            return;
        } 

        icarusLevelManager.OnChallengeStarted.AddListener(HandleChallengeStarted);
    }

    private void OnDisable()
    {
        if (icarusLevelManager == null)
        {
            Debug.Log("�ͦ��t�λݭn IcarusLevelManager ���ѦҤ~��B�@�A�T�{�@�U�ѦҪ��A");
            return;
        }

        icarusLevelManager.OnChallengeStarted.RemoveListener(HandleChallengeStarted);
    }

    protected void HandleChallengeStarted()
    {
        if (AutoStartOnChallenge)
        {
            StartChallengeSpawning();
        }
    }

    /// <summary>
    /// �}�l����D�Ԫ��ͦ��y�{
    /// </summary>
    public void StartChallengeSpawning()
    {
        if (_spawningActive) return;

        _spawningActive = true;
        foreach (var layer in SpawnLayers)
        {
            layer.StartSpawning();
        }
    }

    /// <summary>
    /// ����Ҧ��ͦ��y�{
    /// </summary>
    public void StopAllSpawning()
    {
        foreach (var layer in SpawnLayers)
        {
            layer.StopSpawning();
        }
    }

    /// <summary>
    /// �M���Ҧ����g�ͦ�������
    /// </summary>
    public void ClearAllSpawnedObjects()
    {
        foreach (var layer in SpawnLayers)
        {
            layer.ClearAllSpawned();
        }
    }

    /// <summary>
    /// ���m�ͦ����A�i�b�D�Ե����ɩI�s
    /// </summary>
    public void ResetSpawnManager()
    {
        StopAllSpawning();
        ClearAllSpawnedObjects();
        _spawningActive = false;
    }
}
