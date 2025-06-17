using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using MoreMountains.TopDownEngine;
using TooltipAttribute = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;

[TaskCategory("Custom/Character")]
[TaskDescription("Moves the companion toward its OwnerCharacter using CharacterMovement")]
public class PilotCharacterMoveTowardOwner2D : Action
{
    [Tooltip("The Companion AI Context reference")]
    public SharedCompanionAIContext companionContext;

    [Tooltip("Enable to stop within a certain distance from the owner")]
    public bool useMinimumDistance = true;

    [Tooltip("Minimum distance to stop moving toward the owner")]
    public float minimumDistance = 1f;

    private Character _companionCharacter;
    private CharacterMovement _characterMovement;

    public override void OnStart()
    {
        if (companionContext.Value == null)
        {
            Debug.LogWarning("[PilotCharacterMoveTowardOwner2D] CompanionAIContext is null.");
            return;
        }

        _companionCharacter = companionContext.Value.GetComponent<Character>();

        if (_companionCharacter == null)
        {
            Debug.LogWarning("[PilotCharacterMoveTowardOwner2D] Companion Character not found.");
            return;
        }

        _characterMovement = _companionCharacter.FindAbility<CharacterMovement>();

        if (_characterMovement == null)
        {
            Debug.LogWarning("[PilotCharacterMoveTowardOwner2D] CharacterMovement not found on companion.");
        }
    }

    public override TaskStatus OnUpdate()
    {
        if (companionContext.Value == null || companionContext.Value.OwnerCharacter == null || _characterMovement == null)
        {
            return TaskStatus.Failure;
        }

        Transform ownerTransform = companionContext.Value.OwnerCharacter.transform;
        Transform companionTransform = _companionCharacter.transform;

        Vector2 currentPos = companionTransform.position;
        Vector2 targetPos = ownerTransform.position;
        Vector2 direction = (targetPos - currentPos).normalized;

        float distance = Vector2.Distance(currentPos, targetPos);

        if (useMinimumDistance && distance <= minimumDistance)
        {
            _characterMovement.SetHorizontalMovement(0f);
            _characterMovement.SetVerticalMovement(0f);
            return TaskStatus.Success;
        }

        // Movement per axis
        float horizontal = (Mathf.Abs(targetPos.x - currentPos.x) > minimumDistance)
            ? Mathf.Sign(targetPos.x - currentPos.x)
            : 0f;

        float vertical = (Mathf.Abs(targetPos.y - currentPos.y) > minimumDistance)
            ? Mathf.Sign(targetPos.y - currentPos.y)
            : 0f;

        _characterMovement.SetHorizontalMovement(horizontal);
        _characterMovement.SetVerticalMovement(vertical);

        return TaskStatus.Running;
    }

    public override void OnEnd()
    {
        if (_characterMovement != null)
        {
            _characterMovement.SetHorizontalMovement(0f);
            _characterMovement.SetVerticalMovement(0f);
        }
    }
}
