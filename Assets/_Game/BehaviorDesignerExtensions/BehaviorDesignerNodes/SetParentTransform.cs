using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using TooltipAttribute = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;

/// <summary>
/// Sets this GameObject's parent to the specified Transform.
/// </summary>
[TaskDescription("Sets the parent of this GameObject to the specified Transform.")]
[TaskCategory("Custom/Utility")]
public class SetParentTransform : Action
{
    [Tooltip("The Transform to set as the parent.")]
    public SharedTransform newParent;

    [Tooltip("Should keep world position unchanged when parenting?")]
    public SharedBool preserveWorldPosition = false;

    public override TaskStatus OnUpdate()
    {
        if (newParent.Value == null)
        {
            Debug.LogWarning("[SetParentTransform] New parent is null.");
            return TaskStatus.Failure;
        }

        // Set parent
        transform.SetParent(newParent.Value, preserveWorldPosition.Value);

        // If we don't preserve world position, reset local position to ensure exact snap to parent
        if (!preserveWorldPosition.Value)
        {
            transform.localPosition = Vector3.zero;
        }

        return TaskStatus.Success;
    }

    public override void OnReset()
    {
        newParent = null;
        preserveWorldPosition = false;
    }
}
