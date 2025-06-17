using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Runtime.Tasks.Custom
{
    [TaskCategory("Custom/Math")]
    [TaskDescription("Compares the distance between two transforms with a target float using a specified operation.")]
    public class DistanceComparison : Conditional
    {
        public enum Operation
        {
            LessThan,
            LessThanOrEqualTo,
            EqualTo,
            NotEqualTo,
            GreaterThanOrEqualTo,
            GreaterThan
        }

        [Tooltip("The first transform")]
        public SharedTransform transform1;

        [Tooltip("The second transform")]
        public SharedTransform transform2;

        [Tooltip("The float distance to compare with")]
        public SharedFloat distance;

        [Tooltip("The operation to perform")]
        public Operation operation;

        public override TaskStatus OnUpdate()
        {
            if (transform1.Value == null || transform2.Value == null)
            {
                return TaskStatus.Failure;
            }

            float actualDistance = Vector3.Distance(transform1.Value.position, transform2.Value.position);

            switch (operation)
            {
                case Operation.LessThan:
                    return actualDistance < distance.Value ? TaskStatus.Success : TaskStatus.Failure;
                case Operation.LessThanOrEqualTo:
                    return actualDistance <= distance.Value ? TaskStatus.Success : TaskStatus.Failure;
                case Operation.EqualTo:
                    return Mathf.Approximately(actualDistance, distance.Value) ? TaskStatus.Success : TaskStatus.Failure;
                case Operation.NotEqualTo:
                    return !Mathf.Approximately(actualDistance, distance.Value) ? TaskStatus.Success : TaskStatus.Failure;
                case Operation.GreaterThanOrEqualTo:
                    return actualDistance >= distance.Value ? TaskStatus.Success : TaskStatus.Failure;
                case Operation.GreaterThan:
                    return actualDistance > distance.Value ? TaskStatus.Success : TaskStatus.Failure;
            }

            return TaskStatus.Failure;
        }

        public override void OnReset()
        {
            transform1 = null;
            transform2 = null;
            distance = 0f;
            operation = Operation.LessThan;
        }
    }
}
