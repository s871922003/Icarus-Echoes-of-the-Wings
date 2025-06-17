using BehaviorDesigner.Runtime;

[System.Serializable]
public class SharedCompanionBehaviorState : SharedVariable<CompanionAIContext.CompanionBehaviorState>
{
    public static implicit operator SharedCompanionBehaviorState(CompanionAIContext.CompanionBehaviorState value)
    {
        return new SharedCompanionBehaviorState { Value = value };
    }
}