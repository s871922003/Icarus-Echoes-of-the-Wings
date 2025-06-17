using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using MoreMountains.TopDownEngine;
using TooltipAttribute = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;

/// <summary>
/// Moves the character toward the specified target transform in a 2D space using CharacterMovement.
/// </summary>
[TaskDescription("Moves the character toward the specified target transform in a 2D space using CharacterMovement.")]
[TaskCategory("Custom/Character")]
public class PilotCharacterMoveToTargetTransform2D : Action
{
    [Tooltip("The target Transform to move towards.")]
    public SharedTransform TargetTransform;

    [Tooltip("The minimum distance to the target before stopping.")]
    public SharedFloat MinimumDistance = 1f;

    protected Character _character;
    protected CharacterMovement _characterMovement;

    public override void OnStart()
    {
        _character = GetComponent<Character>();
        _characterMovement = _character?.FindAbility<CharacterMovement>();
    }

    public override TaskStatus OnUpdate()
    {
        if (_characterMovement == null || TargetTransform.Value == null)
        {
            return TaskStatus.Failure;
        }

        MoveTowardsTarget();

        return TaskStatus.Running;
    }

    protected virtual void MoveTowardsTarget()
    {
        Vector2 direction = (TargetTransform.Value.position - this.transform.position);

        // X軸方向移動
        if (Mathf.Abs(direction.x) > MinimumDistance.Value)
        {
            _characterMovement.SetHorizontalMovement(Mathf.Sign(direction.x));
        }
        else
        {
            _characterMovement.SetHorizontalMovement(0f);
        }

        // Y軸方向移動
        if (Mathf.Abs(direction.y) > MinimumDistance.Value)
        {
            _characterMovement.SetVerticalMovement(Mathf.Sign(direction.y));
        }
        else
        {
            _characterMovement.SetVerticalMovement(0f);
        }
    }

    public override void OnEnd()
    {
        if (_characterMovement != null)
        {
            _characterMovement.SetHorizontalMovement(0f);
            _characterMovement.SetVerticalMovement(0f);
        }

        if (_character != null)
        {
            //_character.Controller.CurrentDirection = Vector3.zero;

            var _controller = GetComponent<TopDownController2D>();
            _controller.CurrentDirection = Vector3.zero;
        }
    }


    public override void OnReset()
    {
        TargetTransform = null;
        MinimumDistance = 1f;
    }
}
