using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using MoreMountains.TopDownEngine;
using TooltipAttribute = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;

[TaskCategory("Custom/Enemy")]
[TaskDescription("Gets the player from LevelManager and sets it as the target in EnemyAIContext.")]
public class SetPlayerAsTarget : Action
{
    [Tooltip("The EnemyAIContext to update")]
    public SharedEnemyAIContext EnemyContext;

    public override TaskStatus OnUpdate()
    {
        if (EnemyContext.Value == null)
        {
            Debug.LogWarning("[SetPlayerAsTarget] EnemyAIContext is null.");
            return TaskStatus.Failure;
        }

        if (LevelManager.Instance == null || LevelManager.Instance.Players == null || LevelManager.Instance.Players.Count == 0)
        {
            Debug.LogWarning("[SetPlayerAsTarget] Player not found in LevelManager.");
            return TaskStatus.Failure;
        }

        Transform playerTransform = LevelManager.Instance.Players[0].transform;

        if (playerTransform == null)
        {
            Debug.LogWarning("[SetPlayerAsTarget] Player Transform is null.");
            return TaskStatus.Failure;
        }

        EnemyContext.Value.SetTarget(playerTransform);
        return TaskStatus.Success;
    }
}
