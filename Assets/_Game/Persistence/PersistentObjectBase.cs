using UnityEngine;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools;

/// <summary>
/// 基底類別：自動監聽 TDE 事件、處理物件跨場景存續
/// 子類別只需實作 OnBeforeSceneUnload 與 OnSceneLoadComplete 即可
/// </summary>
public abstract class PersistentObjectBase : MonoBehaviour, IPersistentObject, MMEventListener<TopDownEngineEvent>
{
    protected virtual void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    protected virtual void OnEnable()
    {
        this.MMEventStartListening<TopDownEngineEvent>();
    }

    protected virtual void OnDestroy()
    {
        this.MMEventStopListening<TopDownEngineEvent>();
    }

    public void OnMMEvent(TopDownEngineEvent engineEvent)
    {
        switch (engineEvent.EventType)
        {
            case TopDownEngineEventTypes.LoadNextScene:
                OnBeforeSceneUnload();
                break;

            case TopDownEngineEventTypes.LevelStart:
                OnSceneLoadComplete();
                break;
        }
    }

    /// <summary>
    /// 進入下一場景前會被呼叫，可用於關閉物件、儲存狀態等
    /// </summary>
    public abstract void OnBeforeSceneUnload();

    public abstract void OnSceneLoadComplete();
}
