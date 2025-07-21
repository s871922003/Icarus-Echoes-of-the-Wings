using BehaviorDesigner.Runtime;
using System;

[Serializable]
public class SharedEnemyAIContext : SharedVariable<EnemyAIContext>
{
    public static implicit operator SharedEnemyAIContext(EnemyAIContext value)
    {
        return new SharedEnemyAIContext { Value = value };
    }
}
