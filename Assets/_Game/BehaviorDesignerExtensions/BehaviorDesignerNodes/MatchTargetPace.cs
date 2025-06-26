using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using MoreMountains.TopDownEngine;
using TooltipAttribute = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;

/// <summary>
/// Moves the character toward a target using CharacterMovement, applying acceleration based on distance,
/// and directly mirrors the target's movement and speed when within a certain range.
/// </summary>
[TaskCategory("Custom/Character")]
[TaskDescription("Smoothly moves toward the target using CharacterMovement with distance-based acceleration. Mirrors speed and direction when close.")]
public class MatchTargetPace : Action
{
    [Tooltip("The target to move toward.")]
    public SharedTransform TargetTransform;

    [Tooltip("Minimum distance to the target before switching to pace matching.")]
    public SharedFloat MinimumDistance = 1f;

    [Tooltip("Multiplier to convert distance into acceleration ratio.")]
    public SharedFloat AccelerationMultiplier = 0.25f;

    [Tooltip("Maximum speed multiplier relative to WalkSpeed.")]
    public SharedFloat MaxSpeedMultiplier = 2f;

    protected Character _character;
    protected CharacterMovement _characterMovement;
    protected Character _targetCharacter;
    protected CharacterMovement _targetCharacterMovement;
    protected TopDownController _targetController;
    protected float _originalMovementSpeed;
    protected float _originalSpeedMultiplier;

    public override void OnStart()
    {
        _character = GetComponent<Character>();
        _characterMovement = _character?.FindAbility<CharacterMovement>();

        if (_characterMovement != null)
        {
            _characterMovement.AnalogInput = true;
            _originalMovementSpeed = _characterMovement.MovementSpeed;
            _originalSpeedMultiplier = _characterMovement.MovementSpeedMultiplier;
        }

        if (TargetTransform.Value != null)
        {
            _targetCharacter = TargetTransform.Value.GetComponent<Character>();
            _targetCharacterMovement = _targetCharacter?.FindAbility<CharacterMovement>();
            _targetController = _targetCharacter?.GetComponent<TopDownController2D>();
        }
    }

    public override TaskStatus OnUpdate()
    {
        if (_characterMovement == null || TargetTransform.Value == null)
        {
            return TaskStatus.Failure;
        }

        Vector2 toTarget = (Vector2)TargetTransform.Value.position - (Vector2)transform.position;
        float distance = toTarget.magnitude;

        if (distance < MinimumDistance.Value)
        {
            if (_targetController != null)
            {
                Vector3 targetMovement = _targetController.CurrentMovement;
                _characterMovement.SetMovement(new Vector2(targetMovement.x, targetMovement.y).normalized);

                _characterMovement.MovementSpeed = targetMovement.magnitude;
                _characterMovement.MovementSpeedMultiplier = 1f;
            }
            else
            {
                _characterMovement.SetMovement(Vector2.zero);
            }
        }
        else
        {
            Vector2 direction = toTarget.normalized;
            float speedRatio = Mathf.Clamp01(distance * AccelerationMultiplier.Value);
            _characterMovement.MovementSpeed = _originalMovementSpeed;
            _characterMovement.MovementSpeedMultiplier = Mathf.Lerp(1f, MaxSpeedMultiplier.Value, speedRatio);
            _characterMovement.SetMovement(direction);
        }

        return TaskStatus.Running;
    }

    public override void OnEnd()
    {
        if (_characterMovement != null)
        {
            _characterMovement.SetMovement(Vector2.zero);
            _characterMovement.MovementSpeed = _originalMovementSpeed;
            _characterMovement.MovementSpeedMultiplier = _originalSpeedMultiplier;
        }
    }

    public override void OnReset()
    {
        TargetTransform = null;
        MinimumDistance = 1f;
        AccelerationMultiplier = 0.25f;
        MaxSpeedMultiplier = 2f;
    }
}
