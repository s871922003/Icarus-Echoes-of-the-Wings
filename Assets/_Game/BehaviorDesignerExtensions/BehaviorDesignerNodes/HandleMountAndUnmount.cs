using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using TooltipAttribute = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;
using MoreMountains.TopDownEngine;

/// <summary>
/// Handles the logic required when the companion is mounted or unmounted.
/// This includes setting the mounting state flag in CompanionAIContext,
/// swapping character control using CompanionMountSwapManager,
/// and performing any additional transform or physics adjustments required for mounting.
/// 
/// Use the `newMountingState` boolean to indicate the desired state:
/// - true  = entering mounted state
/// - false = exiting mounted state
/// </summary>
[TaskCategory("Custom/Utility")]
[TaskDescription("Handles mounting and unmounting behavior, including control transfer, physics toggles, and state updates.")]
public class HandleMountAndUnmount : Action
{
    [Tooltip("Set to true if now in mounting mode, false if unmounted.")]
    public SharedBool newMountingState;
    public SharedCharacter thisCharacter;
    public SharedCharacter ownerCharacter;

    protected CompanionAIContext _context;

    public override void OnStart()
    {
        _context = GetComponent<CompanionAIContext>();
    }

    public override TaskStatus OnUpdate()
    {
        if (_context == null)
        {
            Debug.LogWarning("[AcknowledgeMountStateChange] CompanionAIContext not found.");
            return TaskStatus.Failure;
        }

        // 重設命令與更新騎乘狀態
        //_context.SetMountingState(newMountingState.Value);

        if (newMountingState.Value)
        {
            // 上馬
            CompanionMountSwapManager.SwapControlToCompanion(ownerCharacter.Value, thisCharacter.Value);

            var playerTransform = ownerCharacter.Value.transform;
            var mountPoint = _context.MountPoint;
            if (mountPoint != null)
            {
                playerTransform.SetParent(mountPoint);
                playerTransform.localPosition = Vector3.zero;
            }

            CompanionMountSwapManager.SwapControlToCompanion(ownerCharacter.Value, thisCharacter.Value);

            TopDownController2D controller = ownerCharacter.Value.GetComponent<TopDownController2D>();
            if (controller != null)
            {
                controller.SetKinematic(true);
                controller.enabled = false;
            }
        }
        else
        {
            // 下馬
            var playerTransform = ownerCharacter.Value.transform;
            playerTransform.SetParent(null);

            CompanionMountSwapManager.SwapControlToCompanion(thisCharacter.Value, ownerCharacter.Value);

            TopDownController2D controller = ownerCharacter.Value.GetComponent<TopDownController2D>();
            if (controller != null)
            {
                controller.SetKinematic(false);
                controller.enabled = true;
            }
        }

        return TaskStatus.Success;
    }


    public override void OnReset()
    {
        newMountingState = false;
    }
}
