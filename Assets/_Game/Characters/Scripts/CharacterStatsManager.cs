using MoreMountains.TopDownEngine;
using UnityEngine;
using BehaviorDesigner.Runtime;
using System.Collections;

public class CharacterStatsManager : MonoBehaviour
{
    [Header("Stats Reference")]
    public CharacterStatsData StatsData;

    public WeaponLibrary WeaponLibrary;

    private Character _character;
    private Health _health;
    private CharacterMovement _movement;
    private CompanionStamina _stamina;
    private BehaviorTree _behaviorTree;
    private CharacterPound _characterPound;
    private CharacterHandleWeapon _handleWeapon;

    // Runtime values
    private float _currentBaseHealth;
    private float _currentAttackInterval;
    private float _currentFollowDistance;
    private float _currentAttackDistance;
    private float _currentMoveSpeed;
    private float _currentRunSpeed;
    private float _currentStaminaRegen;
    private float _currentBasePoundDistance;
    private float _currentPoundDuration;
    private float _currentForceRecallRange;

    private void Awake()
    {
        _character = GetComponent<Character>();
        _health = GetComponent<Health>();
        _movement = _character?.FindAbility<CharacterMovement>();
        _stamina = GetComponent<CompanionStamina>();
        _behaviorTree = GetComponent<BehaviorTree>();
        _characterPound = _character?.FindAbility<CharacterPound>();
        _handleWeapon = _character?.FindAbility<CharacterHandleWeapon>();

        if (_handleWeapon != null)
        {
            _handleWeapon.OnWeaponChange += OnWeaponChanged;
        }

        // Write WeaponLibrary to BehaviorTree
        if (_behaviorTree != null && WeaponLibrary != null)
        {
            var variable = _behaviorTree.GetVariable("WeaponLibrary") as SharedWeaponLibrary;
            if (variable != null)
            {
                variable.Value = WeaponLibrary;
            }
        }
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
        ApplyStamina();
        ApplyAIStats();
        ApplyWeaponStats();
        ApplyPoundSettings();
        ApplyForceRecallSettings();
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

        _currentMoveSpeed = StatsData.MoveSpeed;
        _currentRunSpeed = StatsData.RunSpeed;

        _movement.WalkSpeed = _currentMoveSpeed;
        _movement.MovementSpeed = _currentMoveSpeed;
    }

    protected virtual void ApplyStamina()
    {
        if (_stamina == null) return;

        _currentStaminaRegen = StatsData.StaminaRegenRate;
        _stamina.RegenRate = _currentStaminaRegen;
        _stamina.MaxStamina = StatsData.MaxStamina;

        _stamina.SetEnergyCostTable(StatsData.EnergyConsumptionTable);
    }

    protected virtual void ApplyAIStats()
    {
        if (_behaviorTree == null) return;

        _currentFollowDistance = StatsData.FollowDistance;
        _currentAttackDistance = StatsData.AttackDistance;

        UpdateBehaviorTreeData("FollowDistance", _currentFollowDistance);
        UpdateBehaviorTreeData("AttackDistance", _currentAttackDistance);
    }

    protected virtual void ApplyWeaponStats()
    {
        if (_handleWeapon == null || WeaponLibrary == null) return;

        var currentWeapon = _handleWeapon.CurrentWeapon;
        if (currentWeapon == null) return;

        var entry = WeaponLibrary.GetStatsByID(currentWeapon.WeaponID);
        if (!entry.HasValue) return;

        currentWeapon.TimeBetweenUses = entry.Value.AttackInterval;
        _currentAttackInterval = entry.Value.AttackInterval;

        // TODO: Support assigning damage and range once Weapon class exposes such properties
        // currentWeapon.DamageOnTarget = entry.Value.Damage;
        // currentWeapon.Range = entry.Value.Range;
    }

    protected virtual void ApplyPoundSettings()
    {
        if (_characterPound == null) return;

        _currentBasePoundDistance = StatsData.BasePoundDistance;
        _characterPound.MaxPoundDistance = _currentBasePoundDistance;

        if (StatsData.PoundSpeed > 0f)
        {
            _currentPoundDuration = _currentBasePoundDistance / StatsData.PoundSpeed;
            _characterPound.PoundSpeed = StatsData.PoundSpeed;
        }
    }


    protected virtual void ApplyForceRecallSettings()
    {
        _currentForceRecallRange = StatsData.ForceRecallRange;

        var context = GetComponent<CompanionAIContext>();
        if (context != null)
        {
            context.SetForceRecallSettings(_currentForceRecallRange, 3f);
        }
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

    public void SetRunSpeed()
    {
        if (_movement != null)
        {
            _movement.MovementSpeed = _currentRunSpeed;
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

    private void OnWeaponChanged()
    {
        StartCoroutine(DelayedApplyWeaponStats());
    }

    private IEnumerator DelayedApplyWeaponStats()
    {
        yield return null;
        ApplyWeaponStats();
    }

    public void OverrideWeaponStats(float interval, float damage, float range)
    {
        _currentAttackInterval = interval;

        if (_handleWeapon != null && _handleWeapon.CurrentWeapon != null)
        {
            _handleWeapon.CurrentWeapon.TimeBetweenUses = interval;

            // TODO: Support assigning damage and range once Weapon class exposes such properties
            // _handleWeapon.CurrentWeapon.DamageOnTarget = damage;
            // _handleWeapon.CurrentWeapon.Range = range;
        }
    }
}
