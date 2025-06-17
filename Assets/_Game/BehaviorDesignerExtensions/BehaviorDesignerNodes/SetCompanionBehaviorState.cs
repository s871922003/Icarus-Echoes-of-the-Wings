using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using TooltipAttribute = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;

[TaskCategory("Custom/Companion")]
[TaskDescription("Updates the CompanionAIContext's behavior state.")]
public class SetCompanionBehaviorState : Action
{
    [Tooltip("The CompanionAIContext to modify.")]
    public SharedCompanionAIContext CompanionContext;

    [Tooltip("The new state to set.")]
    public CompanionAIContext.CompanionBehaviorState NewState;

    public override void OnStart()
    {
        //if (CompanionContext == null || CompanionContext.Value == null)
        //{
        //    Debug.LogWarning("[CompanionAISetBehaviorState] CompanionAIContext is null.");
        //    return;
        //}

        //CompanionContext.Value.UpdateCompanionBehaviorState(NewState);

        //if (CompanionContext.Value.DebugMode)
        //{
        //    Debug.Log($"[CompanionAISetBehaviorState] Set CompanionBehaviorState to {NewState}");
        //}
    }

    public override TaskStatus OnUpdate()
    {
        Debug.Log("行為樹內有地方嘗試在修改狀態");
        return TaskStatus.Success;
    }
}
