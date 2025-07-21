using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using MoreMountains.TopDownEngine;
using TooltipAttribute = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;

[TaskCategory("Custom/Character")]
[TaskDescription("Moves the character toward the target using CharacterMovement without acceleration scaling.")]
public class ChaseTarget : Action
{
    [Tooltip("The target to chase.")]
    public SharedTransform TargetTransform;

    protected Character _character;
    protected CharacterMovement _characterMovement;

    public override void OnStart()
    {
        _character = GetComponent<Character>();
        _characterMovement = _character?.FindAbility<CharacterMovement>();

        if (_characterMovement != null)
        {
            _characterMovement.AnalogInput = true;
        }
    }

    public override TaskStatus OnUpdate()
    {
        if (_characterMovement == null || TargetTransform.Value == null)
        {
            return TaskStatus.Failure;
        }

        Vector2 toTarget = (Vector2)(TargetTransform.Value.position - transform.position);
        Vector2 direction = toTarget.normalized;

        _characterMovement.SetMovement(direction);

        return TaskStatus.Running;
    }

    public override void OnEnd()
    {
        if (_characterMovement != null)
        {
            _characterMovement.SetMovement(Vector2.zero);
        }
    }

    public override void OnReset()
    {
        TargetTransform = null;
    }
}
