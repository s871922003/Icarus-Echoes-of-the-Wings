using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using TooltipAttribute = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;

[TaskCategory("Custom/Enemy")]
[TaskDescription("Returns Success if EnemyBehaviorState1 equals EnemyBehaviorState2.")]
public class CompareEnemyBehaviorState : Conditional
{
    [Tooltip("The first EnemyBehaviorState to compare")]
    public SharedEnemyBehaviorState BehaviorState1;

    [Tooltip("The second EnemyBehaviorState to compare")]
    public SharedEnemyBehaviorState BehaviorState2;

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
        BehaviorState1 = EnemyAIContext.EnemyBehaviorState.None;
        BehaviorState2 = EnemyAIContext.EnemyBehaviorState.None;
    }
}
