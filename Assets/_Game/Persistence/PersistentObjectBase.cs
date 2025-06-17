using UnityEngine;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools;

/// <summary>
/// �����O�G�۰ʺ�ť TDE �ƥ�B�B�z���������s��
/// �l���O�u�ݹ�@ OnBeforeSceneUnload �P OnSceneLoadComplete �Y�i
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
    /// �i�J�U�@�����e�|�Q�I�s�A�i�Ω���������B�x�s���A��
    /// </summary>
    public abstract void OnBeforeSceneUnload();

    public abstract void OnSceneLoadComplete();
}
