using BehaviorDesigner.Runtime;
using System;

[Serializable]
public class SharedEnemyBehaviorState : SharedVariable<EnemyAIContext.EnemyBehaviorState>
{
    public static implicit operator SharedEnemyBehaviorState(EnemyAIContext.EnemyBehaviorState value)
    {
        return new SharedEnemyBehaviorState { Value = value };
    }
}
