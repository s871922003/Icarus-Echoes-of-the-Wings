using BehaviorDesigner.Runtime;

[System.Serializable]
public class SharedCompanionAIContext : SharedVariable<CompanionAIContext>
{
    public static implicit operator SharedCompanionAIContext(CompanionAIContext value)
    {
        return new SharedCompanionAIContext { Value = value };
    }
}
