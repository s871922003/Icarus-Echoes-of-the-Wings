using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using TooltipAttribute = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;

[TaskCategory("Custom/Companion")]
[TaskDescription("Returns Success if BehaviorState1 equals BehaviorState2.")]
public class CompareBehaviorState : Conditional
{
    [Tooltip("The first CompanionBehaviorState to compare")]
    public SharedCompanionBehaviorState BehaviorState1;

    [Tooltip("The second CompanionBehaviorState to compare")]
    public SharedCompanionBehaviorState BehaviorState2;

    public override TaskStatus OnUpdate()
    {
        if (BehaviorState1.Value == BehaviorState2.Value)
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Failure;
    }

    public override void OnReset()
    {
        BehaviorState1 = CompanionAIContext.CompanionBehaviorState.None;
        BehaviorState2 = CompanionAIContext.CompanionBehaviorState.None;
    }
}
