using UnityEngine;
using MoreMountains.TopDownEngine;
using BehaviorDesigner.Runtime;

[System.Serializable]
public class EnemyAIContext : BaseAIContext
{
    public enum EnemyBehaviorState
    {
        None,
        SearchingTarget,
        TargetInteraction,
        Idle,
        Dead,
    }

    [Header("Targeting")]
    public Transform TargetTransform { get; private set; }

    [Header("Runtime State")]
    public EnemyBehaviorState CurrentBehaviorState;
    protected EnemyBehaviorState LastBehaviorState;

    protected Health _ownHealth;
    protected Health _targetHealth;

    protected override void InitializeAI()
    {
        _ownHealth = GetComponent<Health>();
        SyncBehaviorTreeVariable("EnemyAIContext", this);

        if (_ownHealth != null)
        {
            _ownHealth.OnDeath += OnSelfDeath;
        }

        UpdateEnemyBehaviorState(EnemyBehaviorState.SearchingTarget);
    }

    #region Target

    public void SetTarget(Transform target)
    {
        if (CurrentBehaviorState == EnemyBehaviorState.Dead) return;

        TargetTransform = target;
        SyncBehaviorTreeVariable("CurrentTarget", TargetTransform);

        _targetHealth = TargetTransform?.GetComponent<Health>();
        if (_targetHealth != null)
        {
            _targetHealth.OnDeath += OnTargetDeath;
        }

        UpdateEnemyBehaviorState(EnemyBehaviorState.TargetInteraction);
    }

    protected void OnTargetDeath()
    {
        if (_targetHealth != null)
        {
            _targetHealth.OnDeath -= OnTargetDeath;
            _targetHealth = null;
        }

        TargetTransform = null;
        UpdateEnemyBehaviorState(EnemyBehaviorState.Idle);
    }

    #endregion

    #region Self Death

    protected void OnSelfDeath()
    {
        UpdateEnemyBehaviorState(EnemyBehaviorState.Dead);
    }

    #endregion

    #region Behavior State

    public void UpdateEnemyBehaviorState(EnemyBehaviorState newState)
    {
        if (newState != CurrentBehaviorState)
        {
            LastBehaviorState = CurrentBehaviorState;
            HandleStateExit(LastBehaviorState);
            CurrentBehaviorState = newState;

            SyncBehaviorTreeVariable("CurrentBehaviorState", newState);
        }
    }

    protected void HandleStateExit(EnemyBehaviorState exitedState)
    {
        // 根據需要擴充，例如取消目標監聽等
        if (exitedState == EnemyBehaviorState.TargetInteraction && _targetHealth != null)
        {
            _targetHealth.OnDeath -= OnTargetDeath;
        }
    }

    #endregion

    #region Reset (for ObjectPool)

    public virtual void ResetContext()
    {
        if (_ownHealth != null)
        {
            _ownHealth.OnDeath -= OnSelfDeath;
        }

        if (_targetHealth != null)
        {
            _targetHealth.OnDeath -= OnTargetDeath;
        }

        TargetTransform = null;
        _targetHealth = null;

        CurrentBehaviorState = EnemyBehaviorState.None;
        SyncBehaviorTreeVariable("CurrentBehaviorState", CurrentBehaviorState);
    }

    #endregion
}
