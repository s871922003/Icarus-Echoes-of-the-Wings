using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using TooltipAttribute = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;

[TaskDescription("Plays a trigger animation on the specified Animator and waits until it finishes.")]
[TaskCategory("Custom/Utility")]
public class PlayAnimatorTrigger : Action
{
    [Tooltip("The animator to play the animation on.")]
    public SharedAnimator targetAnimator;

    [Tooltip("The name of the trigger parameter to set.")]
    public SharedString triggerName;

    [Tooltip("Animator layer to check (optional, default is 0).")]
    public SharedInt layerIndex = 0;

    [Tooltip("Maximum wait time before failing (0 = unlimited).")]
    public SharedFloat timeout = 0f;

    private float _startTime;
    private bool _triggerSet = false;
    private int _triggerHash;
    private AnimatorStateInfo _lastState;

    public override void OnStart()
    {
        if (targetAnimator.Value == null || string.IsNullOrEmpty(triggerName.Value))
        {
            Debug.LogWarning("[PlayAnimatorTrigger] Invalid animator or trigger.");
            return;
        }

        _triggerSet = false;
        _triggerHash = Animator.StringToHash(triggerName.Value);
        _startTime = Time.time;

        targetAnimator.Value.ResetTrigger(triggerName.Value); // �T�O���ݯd�e�@��
        targetAnimator.Value.SetTrigger(triggerName.Value);
    }

    public override TaskStatus OnUpdate()
    {
        if (targetAnimator.Value == null)
        {
            return TaskStatus.Failure;
        }

        AnimatorStateInfo currentState = targetAnimator.Value.GetCurrentAnimatorStateInfo(layerIndex.Value);

        // �ˬd�ʵe�O�_�w�g�}�l����
        if (!_triggerSet)
        {
            if (currentState.shortNameHash == _triggerHash || currentState.IsTag(triggerName.Value))
            {
                _triggerSet = true;
                _lastState = currentState;
            }
        }

        // �Y�ʵe�w�g���񧹦��A�γQ��L���A�л\�A�^�� Success
        if (_triggerSet)
        {
            if (!currentState.Equals(_lastState) && currentState.normalizedTime > 0.01f)
            {
                return TaskStatus.Success;
            }

            if (currentState.normalizedTime >= 1.0f)
            {
                return TaskStatus.Success;
            }
        }

        // Timeout �ˬd
        if (timeout.Value > 0f && Time.time - _startTime > timeout.Value)
        {
            Debug.LogWarning($"[PlayAnimatorTrigger] Timed out waiting for animation '{triggerName.Value}'.");
            return TaskStatus.Failure;
        }

        return TaskStatus.Running;
    }

    public override void OnReset()
    {
        triggerName = "";
        targetAnimator = null;
        timeout = 0f;
        layerIndex = 0;
        _triggerSet = false;
    }
}
