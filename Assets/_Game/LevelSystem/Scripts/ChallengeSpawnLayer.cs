using UnityEngine;

public abstract class ChallengeSpawnLayer : MonoBehaviour
{
    [Header("生成層啟用狀態")]
    [Tooltip("是否啟用此生成層。若關閉，則不會在挑戰開始時執行任何生成行為。")]
    public bool LayerEnabled = true;

    /// <summary>
    /// 外部由 ChallengeSpawnManager 呼叫，代表此層應該啟動生成流程。
    /// </summary>
    public virtual void StartSpawning()
    {
        if (!LayerEnabled) return;
        OnStartSpawning();
    }

    /// <summary>
    /// 外部由 ChallengeSpawnManager 呼叫，代表此層應該停止生成流程。
    /// </summary>
    public virtual void StopSpawning()
    {
        OnStopSpawning();
    }

    /// <summary>
    /// 外部由 ChallengeSpawnManager 呼叫，代表此層應該清除目前場上的所有生成物件。
    /// </summary>
    public virtual void ClearAllSpawned()
    {
        OnClearAllSpawned();
    }

    /// <summary>
    /// 子類別實作：處理具體啟動行為。
    /// </summary>
    protected abstract void OnStartSpawning();

    /// <summary>
    /// 子類別實作：處理具體停止行為。
    /// </summary>
    protected abstract void OnStopSpawning();

    /// <summary>
    /// 子類別實作：處理具體清除行為。
    /// </summary>
    protected abstract void OnClearAllSpawned();
}
