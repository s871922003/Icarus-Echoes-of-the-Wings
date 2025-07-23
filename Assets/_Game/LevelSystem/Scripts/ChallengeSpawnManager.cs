using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

/// <summary>
/// ChallengeSpawnManager 負責在挑戰開始後生成挑戰用物件，如敵人、補給、障礙物等。
/// 它由 IcarusLevelManager 控制，支援多層次的生成控制與後續清除邏輯。
/// </summary>
public class ChallengeSpawnManager : MonoBehaviour
{
    [SerializeField] IcarusLevelManager icarusLevelManager;

    [Header("生成層定義")]
    public List<ChallengeSpawnLayer> SpawnLayers;

    [Header("挑戰開始自動啟動")]
    [Tooltip("是否在 IcarusLevelManager 啟動挑戰時自動開始生成挑戰物件")]
    public bool AutoStartOnChallenge = true;

    protected bool _spawningActive = false;

    private void OnEnable()
    {
        if (icarusLevelManager == null) 
        {
            Debug.Log("生成系統需要 IcarusLevelManager 的參考才能運作，確認一下參考狀態");
            return;
        } 

        icarusLevelManager.OnChallengeStarted.AddListener(HandleChallengeStarted);
    }

    private void OnDisable()
    {
        if (icarusLevelManager == null)
        {
            Debug.Log("生成系統需要 IcarusLevelManager 的參考才能運作，確認一下參考狀態");
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
    /// 開始整場挑戰的生成流程
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
    /// 停止所有生成流程
    /// </summary>
    public void StopAllSpawning()
    {
        foreach (var layer in SpawnLayers)
        {
            layer.StopSpawning();
        }
    }

    /// <summary>
    /// 清除所有曾經生成的物件
    /// </summary>
    public void ClearAllSpawnedObjects()
    {
        foreach (var layer in SpawnLayers)
        {
            layer.ClearAllSpawned();
        }
    }

    /// <summary>
    /// 重置生成器，可在挑戰結束時呼叫
    /// </summary>
    public void ResetSpawnManager()
    {
        StopAllSpawning();
        ClearAllSpawnedObjects();
        _spawningActive = false;
    }
}
