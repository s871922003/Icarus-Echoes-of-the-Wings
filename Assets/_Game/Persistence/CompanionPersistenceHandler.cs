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

    private CompanionAIContext _context;

    protected override void Awake()
    {
        base.Awake();
        _context = this.gameObject.GetComponent<CompanionAIContext>();
    }

    public override void OnBeforeSceneUnload()
    {
        if (_context != null && _context.CurrentBehaviorState == CompanionAIContext.CompanionBehaviorState.Mounted)
        {
            Debug.Log("[CompanionPersistenceHandler] 檢測到正在騎乘，執行自動下馬");

            // 執行下馬
            var player = _context.OwnerCharacter;
            var companion = _character;

            if (player != null && companion != null)
            {
                var playerTransform = player.transform;
                playerTransform.SetParent(null);

                // 控制權切回玩家
                CompanionMountSwapManager.SwapControlToCompanion(companion, player);

                // 恢復玩家控制器
                var controller = player.GetComponent<TopDownController2D>();
                if (controller != null)
                {
                    controller.SetKinematic(false);
                    controller.enabled = true;
                }

                // 發出下馬完成事件
                MMCompanionActionEvent.Trigger(companion, MMCompanionActionEventTypes.OnGetOffMountComplete);
            }
        }

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
