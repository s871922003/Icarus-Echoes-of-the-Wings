using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

/// <summary>
/// Clears the parent of this GameObject (unparents it).
/// </summary>
[TaskDescription("Clears the parent of this GameObject (sets it to null).")]
[TaskCategory("Custom/Utility")]
public class ClearParentTransform : Action
{
    [BehaviorDesigner.Runtime.Tasks.Tooltip("Should keep world position unchanged when unparenting?")]
    public SharedBool preserveWorldPosition = false;

    public override TaskStatus OnUpdate()
    {
        transform.SetParent(null, preserveWorldPosition.Value);
        return TaskStatus.Success;
    }

    public override void OnReset()
    {
        preserveWorldPosition = false;
    }
}
