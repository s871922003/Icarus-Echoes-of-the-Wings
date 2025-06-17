using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using TooltipAttribute = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;

[TaskCategory("Custom/Utility")]
[TaskDescription("Compares two transforms, including null cases, with customizable outcome for each case.")]
public class TransformComparisonEx : Conditional
{
    [Tooltip("The first Transform to compare")]
    public SharedTransform TransformA;

    [Tooltip("The second Transform to compare")]
    public SharedTransform TransformB;

    public enum ComparisonOutcome
    {
        ReturnSuccess,
        ReturnFailure
    }

    [Tooltip("What to return when both Transforms are equal (including null == null)")]
    public ComparisonOutcome EqualOutcome = ComparisonOutcome.ReturnSuccess;

    [Tooltip("What to return when Transforms are different")]
    public ComparisonOutcome NotEqualOutcome = ComparisonOutcome.ReturnFailure;

    public override TaskStatus OnUpdate()
    {
        bool areEqual = TransformA.Value == TransformB.Value;

        if (areEqual)
        {
            return EqualOutcome == ComparisonOutcome.ReturnSuccess ? TaskStatus.Success : TaskStatus.Failure;
        }
        else
        {
            return NotEqualOutcome == ComparisonOutcome.ReturnSuccess ? TaskStatus.Success : TaskStatus.Failure;
        }
    }

    public override void OnReset()
    {
        TransformA = null;
        TransformB = null;
    }
}
