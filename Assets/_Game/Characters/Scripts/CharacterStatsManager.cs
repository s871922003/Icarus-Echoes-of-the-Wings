using MoreMountains.TopDownEngine;
using UnityEngine;
using BehaviorDesigner.Runtime;
using System.Collections;

public class CharacterStatsManager : MonoBehaviour
{
    [Header("Stats Reference")]
    public CharacterStatsData StatsData;

    private Character _character;
    private Health _health;
    private CharacterMovement _movement;
    private BehaviorTree _behaviorTree;


    // Runtime values
    private float _currentBaseHealth;
    private float _currentFollowDistance;
    private float _currentMoveSpeed;


    private void Awake()
    {
        _character = GetComponent<Character>();
        _health = GetComponent<Health>();
        _movement = _character?.FindAbility<CharacterMovement>();
        _behaviorTree = GetComponent<BehaviorTree>();
    }

    private void Start()
    {
        ApplyStats();
    }

    public void ApplyStats()
    {
        if (StatsData == null)
        {
            Debug.LogWarning("[CharacterStatsManager] StatsData is not assigned.");
            return;
        }

        ApplyHealth();
        ApplyMovement();
        ApplyAIStats();
    }

    protected virtual void ApplyHealth()
    {
        if (_health == null) return;

        _currentBaseHealth = StatsData.BaseHealth;

        _health.InitialHealth = _currentBaseHealth;
        _health.MaximumHealth = _currentBaseHealth;
        if (_health.Initialized)
        {
            _health.SetHealth(_currentBaseHealth);
        }
    }

    protected virtual void ApplyMovement()
    {
        if (_movement == null) return;

        _currentMoveSpeed = StatsData.WalkSpeed;

        _movement.WalkSpeed = _currentMoveSpeed;
        _movement.MovementSpeed = _currentMoveSpeed;
    }

    protected virtual void ApplyAIStats()
    {
        if (_behaviorTree == null) return;

        _currentFollowDistance = StatsData.FollowDistance;

        UpdateBehaviorTreeData("FollowDistance", _currentFollowDistance);
    }


    private void UpdateBehaviorTreeData(string variableName, float value)
    {
        if (_behaviorTree == null) return;
        var shared = _behaviorTree.GetVariable(variableName) as SharedFloat;
        if (shared != null)
        {
            shared.Value = value;
        }
    }

    public float GetBaseHealth() => _currentBaseHealth;

    public void SetBaseHealth(float newBase)
    {
        _currentBaseHealth = newBase;
        if (_health != null)
        {
            _health.InitialHealth = newBase;
            _health.MaximumHealth = newBase;
            if (_health.Initialized)
            {
                _health.SetHealth(newBase);
            }
        }
    }
}
