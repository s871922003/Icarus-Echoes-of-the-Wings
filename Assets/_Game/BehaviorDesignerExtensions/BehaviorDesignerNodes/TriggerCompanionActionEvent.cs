using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using MoreMountains.TopDownEngine;
using TooltipAttribute = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;
using Unity.VisualScripting;

[TaskCategory("Custom/Companion")]
[TaskDescription("Triggers a MMCompanionActionEvent from Behavior Designer.")]
public class TriggerCompanionActionEvent : Action
{
    [Tooltip("The character who sends this event")]
    public SharedCharacter EventInvoker;

    [Tooltip("The Companion action event type to trigger")]
    public MMCompanionActionEventTypes EventType;

    public override TaskStatus OnUpdate()
    {
        if (EventInvoker.Value == null)
        {
            Debug.LogWarning("[TriggerCompanionActionEvent] EventInvoker is null.");
            return TaskStatus.Failure;
        }

        MMCompanionActionEvent.Trigger(EventInvoker.Value, EventType);
        return TaskStatus.Success;
    }
}
