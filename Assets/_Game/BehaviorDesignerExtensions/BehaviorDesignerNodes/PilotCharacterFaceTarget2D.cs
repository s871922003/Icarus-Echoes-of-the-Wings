using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using MoreMountains.TopDownEngine;
using TooltipAttribute = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;

/// <summary>
/// Forces the CharacterOrientation2D ability to face toward a given target Transform.
/// Simplified version without LeftRightOnly.
/// Always faces using 4-directional (N/S/E/W) logic.
/// </summary>
[TaskCategory("Custom/Character")]
[TaskDescription("Forces the CharacterOrientation2D ability to face toward a given target Transform using 4-directional logic.")]
public class PilotCharacterFaceTarget2D : Action
{
    [Tooltip("The target Transform to face.")]
    public SharedTransform TargetTransform;

    protected Character _character;
    protected CharacterOrientation2D _orientation2D;
    protected CharacterOrientation2D.FacingModes _initialFacingMode;

    public override void OnStart()
    {
        _character = GetComponent<Character>();
        _orientation2D = _character?.FindAbility<CharacterOrientation2D>();

        if (_orientation2D != null)
        {
            _initialFacingMode = _orientation2D.FacingMode;
            _orientation2D.FacingMode = CharacterOrientation2D.FacingModes.None;
        }
    }

    public override TaskStatus OnUpdate()
    {
        if (_character == null || _orientation2D == null || TargetTransform.Value == null)
        {
            return TaskStatus.Failure;
        }

        FaceTarget();

        return TaskStatus.Success;
    }

    public override void OnEnd()
    {
        if (_orientation2D != null)
        {
            _orientation2D.FacingMode = _initialFacingMode;
        }
    }

    protected void FaceTarget()
    {
        Vector3 targetPosition = TargetTransform.Value.position;
        Vector2 distance = targetPosition - _character.transform.position;

        Character.FacingDirections newFacing;

        if (Mathf.Abs(distance.y) > Mathf.Abs(distance.x))
        {
            newFacing = (distance.y > 0) ? Character.FacingDirections.North : Character.FacingDirections.South;
        }
        else
        {
            newFacing = (distance.x > 0) ? Character.FacingDirections.East : Character.FacingDirections.West;
        }

        _orientation2D.Face(newFacing);
    }
}
