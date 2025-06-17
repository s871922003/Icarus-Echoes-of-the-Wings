using UnityEngine;
using MoreMountains.TopDownEngine;

/// <summary>
/// 夥伴角色的跨場景存續處理器，會在轉場前自動下馬（如果正在騎乘）
/// </summary>
public class CompanionPersistenceHandler : CharacterPersistenceHandler
{

    [Header("Companion Offset Settings")]
    [Tooltip("進場時與玩家初始點之間的偏移量")]
    public Vector3 Offset = new Vector3(1f, 0f, 0f);

    protected override void Awake()
    {
        base.Awake();
    }

    public override void OnBeforeSceneUnload()
    {     
        base.OnBeforeSceneUnload();
    }

    public override void OnSceneLoadComplete()
    {
        // 設定移動位置（基於 InitialSpawnPoint + offset）
        if (LevelManager.HasInstance && LevelManager.Instance.InitialSpawnPoint != null)
        {
            this.transform.position = LevelManager.Instance.InitialSpawnPoint.transform.position + Offset;
        }

        base.OnSceneLoadComplete();
    }
}
