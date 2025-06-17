using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using MoreMountains.TopDownEngine;
using TooltipAttribute = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;

[TaskCategory("Custom/Character")]
[TaskDescription("Triggers the CharacterPound dash movement toward a target position or mouse position.")]
public class PilotCharacterPound : Action
{
    [Tooltip("The target Transform to pound toward (ignored if UseMousePosition is true).")]
    public SharedTransform TargetTransform;

    [Tooltip("If true, uses the mouse position at OnStart as the target.")]
    public SharedBool UseMousePosition;

    protected Character _character;
    protected CharacterPound _characterPound;
    protected Vector3 _lockedTargetPosition;

    public override void OnStart()
    {
        _character = GetComponent<Character>();
        _characterPound = _character?.FindAbility<CharacterPound>();

        if (_character == null || _characterPound == null)
        {
            Debug.LogWarning("[PilotCharacterPound] Missing Character or CharacterPound.");
            return;
        }

        if (UseMousePosition.Value)
        {
            _lockedTargetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _lockedTargetPosition.z = _character.transform.position.z;
        }
        else if (TargetTransform.Value != null)
        {
            _lockedTargetPosition = TargetTransform.Value.position;
            _lockedTargetPosition.z = _character.transform.position.z;
        }
        else
        {
            Debug.LogWarning("[PilotCharacterPound] No valid target provided.");
            return;
        }

        _characterPound.TriggerPound(_lockedTargetPosition);
    }

    public override TaskStatus OnUpdate()
    {
        if (_characterPound == null)
            return TaskStatus.Failure;

        return _characterPound.IsPoundInProgress ? TaskStatus.Running : TaskStatus.Success;
    }

    public override void OnEnd()
    {
        if (_characterPound != null && _characterPound.IsPoundInProgress)
        {
            _characterPound.ForceStopPound();
        }
    }
}
