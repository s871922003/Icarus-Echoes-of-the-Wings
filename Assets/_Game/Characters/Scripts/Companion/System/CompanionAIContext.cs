using UnityEngine;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools;
using BehaviorDesigner.Runtime;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEditor.Timeline.Actions;

[System.Serializable]
public class CompanionAIContext : MonoBehaviour, MMEventListener<MMCompanionCommandEvent>, MMEventListener<MMCompanionActionEvent>
{
    [Header("Owner")]
    public Character OwnerCharacter { get; private set; }

    [Header("Targeting")]
    public List<string> ViableTargetTags = new List<string> { "Enemy" };
    public float searchRadius = 10f;
    public Transform CurrentTarget { get; private set; }
    public Vector3 LastMarkedTargetPosition { get; private set; }

    [Header("Force Recall")]
    public float ForceRecallRange = 12f;
    public float ForceRecallCountdownDuration = 3f;

    protected Coroutine _forceRecallRoutine;

    [Header("Core Components")]
    public BehaviorTree behaviorTree;
    public CompanionStamina CompanionStamina { get; private set; }
    protected Character _character;
    private Animator _animator;
    public Transform MountPoint;

    public float MarkActionCooldownDuration = 1.5f;

    [Header("Target Selector")]
    public TargetSelector TargetSelectorComponent;

    [Header("Flags")]
    public bool IsMarkActionOnCooldown { get; private set; } = false;

    //public bool IsInCombat { get; private set; } = false;

    //public bool CanEnterCombatState => CompanionStamina.CanEnterCombatState;

    public CompanionBehaviorState CurrentBehaviorState;
    CompanionBehaviorState LastBehaviorState;
    private Coroutine _passiveDetectionRoutine;

    public enum CompanionBehaviorState
    {
        None,
        NoOwner,
        MountTransition,
        UnmountTransition,
        Mounted,
        MarkAction,
        Combat,
        Default,
        Recalled
    }


    [Header("Debug")]
    public bool DebugMode = false;
    [SerializeField] bool _canPerformStaminaDrainedMoves = true;

    protected virtual void Start()
    {
        InitializeComponents();
        InitializeBehaviorTree();
        InitializeCompanion();

        StartCoroutine(MonitorRecallDistance());
    }

    protected void InitializeComponents()
    {
        _character = GetComponent<Character>();
        CompanionStamina = GetComponent<CompanionStamina>();
        behaviorTree = GetComponent<BehaviorTree>();
        _animator = GetComponent<Animator>();

        CompanionStamina.OnBurnout += HandleBurnout;
        CompanionStamina.OnRecover += HandleRecover;
    }

    private void HandleRecover()
    {
        _canPerformStaminaDrainedMoves = true;

        if (CurrentBehaviorState == CompanionBehaviorState.Default)
        {
            StartPassiveEnemyDetection();
        }
    }

    private void HandleBurnout()
    {
        _canPerformStaminaDrainedMoves = false;
        StopPassiveEnemyDetection();
    }

    protected void InitializeBehaviorTree()
    {
        if (behaviorTree == null) return;

        SyncBehaviorTreeVariable("CompanionAIContext", this);
        SyncBehaviorTreeVariable("ThisCharacter", _character);
        SyncBehaviorTreeVariable("ThisTransform", this.transform);
        SyncBehaviorTreeVariable("Animator", _animator);
    }

    protected void InitializeCompanion()
    {
        if (DebugMode)
        {
            Debug.Log("[CompanionAIContext] InitializeCompanion called.");
        }

        
        UpdateCompanionBehaviorState(CompanionBehaviorState.NoOwner);
    }

    private void OnEnable()
    {
        this.MMEventStartListening<MMCompanionCommandEvent>();
        this.MMEventStartListening<MMCompanionActionEvent>();
    }

    private void OnDisable()
    {
        this.MMEventStopListening<MMCompanionCommandEvent>();
        this.MMEventStopListening<MMCompanionActionEvent>();
    }

    public void SetOwner(Character character)
    {
        OwnerCharacter = character;


        UpdateCompanionBehaviorState(CompanionBehaviorState.Default);
        SyncBehaviorTreeVariable("OwnerCharacter", OwnerCharacter);
        SyncBehaviorTreeVariable("OwnerTransform", OwnerCharacter.transform);
    }



    protected GameObject FindClosestViableTarget(Vector3 referencePosition)
    {
        GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        GameObject closest = null;
        float minDistance = searchRadius;

        foreach (var obj in allObjects)
        {
            if (!ViableTargetTags.Contains(obj.tag)) continue;

            Health health = obj.GetComponent<Health>();
            if (health == null || health.CurrentHealth <= 0) continue;

            float distance = Vector3.Distance(obj.transform.position, referencePosition);
            if (distance < minDistance)
            {
                closest = obj;
                minDistance = distance;
            }
        }

        return closest;
    }

    protected void HandleNewMarkedTarget(Vector3 newPosition)
    {
        if (DebugMode) Debug.Log($"[CompanionAIContext] New marked target at {newPosition}");

        LastMarkedTargetPosition = newPosition;

        if (TargetSelectorComponent == null)
        {
            Debug.LogWarning("[CompanionAIContext] TargetSelectorComponent is null.");
            return;
        }

        // 這邊改用 TargetSelector 搜尋
        Transform foundTarget = TargetSelectorComponent.SearchForClosestTarget();

        if (foundTarget != null)
        {
            SetTarget(foundTarget);
        }
        else
        {
            ClearTarget();

            UpdateCompanionBehaviorState(CompanionBehaviorState.Default);

            if (DebugMode)
            {
                Debug.Log("[CompanionAIContext] No valid target found.");
            }
        }
    }

    protected void SetTarget(Transform target)
    {
        if (DebugMode) Debug.Log($"[CompanionAIContext] Target set: {target.name}");

        CurrentTarget = target;

        SyncBehaviorTreeVariable("CurrentTarget", CurrentTarget);
    }

    protected void ClearTarget()
    {
        if (DebugMode) Debug.Log("[CompanionAIContext] Target cleared.");

        CurrentTarget = null;

        SyncBehaviorTreeVariable("CurrentTarget", null);
    }


    public void OnMMEvent(MMCompanionCommandEvent companionCommandEvent)
    {
        Debug.Log("收到玩家命令");
        if (companionCommandEvent.EventInvokerCharacter != OwnerCharacter) return;

        switch (companionCommandEvent.EventType)
        {
            case MMCompanionCommandEventTypes.MarkAction:
                TryEnterMarkAcionState();
                break;

            case MMCompanionCommandEventTypes.MountAndUnmountAction:
                ToggleMountRequest();
                break;
        }
    }

    private void TryEnterMarkAcionState()
    {
        // 騎乘狀態下不允許進入標記動作狀態
        if (CurrentBehaviorState == CompanionBehaviorState.MountTransition || CurrentBehaviorState == CompanionBehaviorState.UnmountTransition|| CurrentBehaviorState == CompanionBehaviorState.Mounted) return;

        if (!_canPerformStaminaDrainedMoves) return;

        // 如果已經在標記動作狀態中，換言之進行標記動作的過程中又重複收到指令時，不打斷當前標記動作
        if (CurrentBehaviorState == CompanionBehaviorState.MarkAction) return;

        // 冷卻中則不允許進入狀態
        if (IsMarkActionOnCooldown) return;

        // 正式進入標記動作
        UpdateCompanionBehaviorState(CompanionBehaviorState.MarkAction);
    }

    public void OnMMEvent(MMCompanionActionEvent actionEvent)
    {
        switch (actionEvent.EventType)
        {
            case MMCompanionActionEventTypes.OnGetOnMountComplete:
                UpdateCompanionBehaviorState(CompanionBehaviorState.Mounted);
                break;

            case MMCompanionActionEventTypes.OnGetOffMountComplete:
                UpdateCompanionBehaviorState(CompanionBehaviorState.Default);
                break;

            case MMCompanionActionEventTypes.OnMarkActionStart:
                CompanionStamina.TryConsumeForAction(CompanionStamina.StaminaDrainedMoves.MarkAction);
                break;

            case MMCompanionActionEventTypes.OnMarkActionEnd:
                {
                    // 檢查當前目標是否存在且存活，是則進入戰鬥狀態，否則回到預設狀態
                    bool hasValidTarget = CurrentTarget != null &&
                                          CurrentTarget.TryGetComponent(out Health health) &&
                                          health.CurrentHealth > 0;

                    if (_canPerformStaminaDrainedMoves && hasValidTarget)
                    {
                        UpdateCompanionBehaviorState(CompanionBehaviorState.Combat);
                    }
                    else
                    {
                        UpdateCompanionBehaviorState(CompanionBehaviorState.Default);
                    }
                    break;
                }

            case MMCompanionActionEventTypes.OnTargetSearchStart:
                HandleNewMarkedTarget(transform.position);
                break;

            case MMCompanionActionEventTypes.OnAttackLoopStart:
                {
                    // 檢查當前目標是否存在且存活，否則回到預設狀態
                    bool hasValidTarget = CurrentTarget != null &&
                                          CurrentTarget.TryGetComponent(out Health health) &&
                                          health.CurrentHealth > 0;

                    if (!_canPerformStaminaDrainedMoves || !hasValidTarget)
                    {
                        UpdateCompanionBehaviorState(CompanionBehaviorState.Default);
                    }
                    else
                    {
                        // 有合理的目標，要進行一次攻擊動作了
                        CompanionStamina.TryConsumeForAction(CompanionStamina.StaminaDrainedMoves.DefaultAttack);
                    }

                    break;
                }


            default:
                Debug.Log("未明確定義的事件被觸發");
                break;
        }
    }

    protected void SyncBehaviorTreeVariable(string variableName, object value)
    {
        if (behaviorTree != null)
        {
            behaviorTree.SetVariableValue(variableName, value);
        }
    }

    private void ToggleMountRequest()
    {
        if (CurrentBehaviorState == CompanionBehaviorState.Mounted)
        {
            //SetIsUnmountActionPending(true);
            UpdateCompanionBehaviorState(CompanionBehaviorState.UnmountTransition);
            Debug.Log("嘗試下馬");
        }
        else
        {
            //SetIsMountActionPending(true);
            UpdateCompanionBehaviorState(CompanionBehaviorState.MountTransition);
            Debug.Log("嘗試上馬");
        }
    }


    public void TriggerMarkActionCooldown()
    {
        if (IsMarkActionOnCooldown) return;

        IsMarkActionOnCooldown = true;
        StartCoroutine(MarkActionCooldownCoroutine());
    }

    private IEnumerator MarkActionCooldownCoroutine()
    {
        yield return new WaitForSeconds(MarkActionCooldownDuration);
        IsMarkActionOnCooldown = false;
    }

    public void UpdateCompanionBehaviorState(CompanionBehaviorState newState)
    {
        if (newState != CurrentBehaviorState)
        {
            if (DebugMode)
            {
                Debug.Log($"[CompanionAIContext] State changed from {CurrentBehaviorState} to {newState}");
            }

            LastBehaviorState = CurrentBehaviorState;
            HandleStateExit(LastBehaviorState);
            CurrentBehaviorState = newState;

            switch (newState)
            {
                case CompanionBehaviorState.NoOwner:
                    SyncBehaviorTreeVariable("CurrentBehaviorState", CompanionBehaviorState.NoOwner);
                    break;

                case CompanionBehaviorState.MountTransition:
                    ClearTarget();
                    StopPassiveEnemyDetection();
                    SyncBehaviorTreeVariable("CurrentBehaviorState", CompanionBehaviorState.MountTransition);
                    break;

                case CompanionBehaviorState.UnmountTransition:
                    SyncBehaviorTreeVariable("CurrentBehaviorState", CompanionBehaviorState.UnmountTransition);
                    break;

                case CompanionBehaviorState.Mounted:
                    SyncBehaviorTreeVariable("CurrentBehaviorState", CompanionBehaviorState.Mounted);
                    break;

                case CompanionBehaviorState.MarkAction:
                    SyncBehaviorTreeVariable("CurrentBehaviorState", CompanionBehaviorState.MarkAction);
                    TriggerMarkActionCooldown();
                    break;

                case CompanionBehaviorState.Combat:
                    SyncBehaviorTreeVariable("CurrentBehaviorState", CompanionBehaviorState.Combat);
                    StopPassiveEnemyDetection();
                    break;

                case CompanionBehaviorState.Default:
                    SyncBehaviorTreeVariable("CurrentBehaviorState", CompanionBehaviorState.Default);
                    StartPassiveEnemyDetection(); // 啟動敵人檢測
                    break;

                default:
                    if (DebugMode)
                    {
                        Debug.LogWarning($"[CompanionAIContext] Unhandled CompanionBehaviorState: {newState}");
                    }
                    break;
            }
        }
    }


    protected void HandleStateExit(CompanionBehaviorState exitedState)
    {
        switch (exitedState)
        {
            case CompanionBehaviorState.NoOwner:
                break;

            case CompanionBehaviorState.MountTransition:
                break;

            case CompanionBehaviorState.UnmountTransition:
                break;

            case CompanionBehaviorState.Mounted:
                break;

            case CompanionBehaviorState.MarkAction:
                ResetMarkActionState();
                break;

            case CompanionBehaviorState.Combat:
                ResetCombatState();
                break;

            default: break;
                // 可依需求繼續擴充，例如 Recalled、NoOwner 不需 Reset
        }
    }

    protected void ResetMarkActionState()
    {
        var pound = _character.FindAbility<CharacterPound>();
        if (pound != null)
        {
            if (pound.IsPoundInProgress)
            {
                pound.ForceStopPound();
            }

            pound.ClearPoundTarget();
        }

        if (DebugMode)
        {
            Debug.Log("[CompanionAIContext] MarkAction state reset.");
        }
    }





    protected void ResetCombatState()
    {
        ClearTarget();

        if (DebugMode)
        {
            Debug.Log("[CompanionAIContext] Combat state reset.");
        }
    }

    private void StartPassiveEnemyDetection()
    {

        if (_passiveDetectionRoutine != null)
        {
            Debug.LogWarning("[CompanionAIContext] 鎖敵排程已存在，跳過啟動。");
            return;
        }

        if (!_canPerformStaminaDrainedMoves)
        {
            Debug.LogWarning("[CompanionAIContext] 體力不足，無法啟動鎖敵排程。");
            return;
        }

        _passiveDetectionRoutine = StartCoroutine(PassiveEnemyDetectionCoroutine());
    }


    private void StopPassiveEnemyDetection()
    {

        if (_passiveDetectionRoutine != null)
        {
            StopCoroutine(_passiveDetectionRoutine);
            _passiveDetectionRoutine = null;
        }
    }

    private IEnumerator PassiveEnemyDetectionCoroutine()
    {
        while (CurrentBehaviorState == CompanionBehaviorState.Default)
        {
            if (OwnerCharacter != null && TargetSelectorComponent != null)
            {
                var foundTarget = TargetSelectorComponent.SearchAroundOwnerForTarget(OwnerCharacter.transform);
                if (foundTarget != null)
                {
                    SetTarget(foundTarget);
                    UpdateCompanionBehaviorState(CompanionBehaviorState.Combat);
                    yield break;
                }
            }

            yield return new WaitForSeconds(0.5f); // 控制檢測頻率
        }
    }

    public void SetForceRecallSettings(float range, float countdownDuration)
    {
        ForceRecallRange = range;
        ForceRecallCountdownDuration = countdownDuration;
    }

    private IEnumerator MonitorRecallDistance()
    {
        while (true)
        {
            if (OwnerCharacter != null)
            {
                float distance = Vector3.Distance(this.transform.position, OwnerCharacter.transform.position);
                if (distance > ForceRecallRange)
                {
                    if (_forceRecallRoutine == null)
                    {
                        _forceRecallRoutine = StartCoroutine(HandleForceRecallSequence());
                    }
                }
                else
                {
                    if (_forceRecallRoutine != null)
                    {
                        StopCoroutine(_forceRecallRoutine);
                        _forceRecallRoutine = null;
                        // TODO: 隱藏 UI 提示
                    }
                }
            }

            yield return new WaitForSeconds(0.25f);
        }
    }

    private IEnumerator HandleForceRecallSequence()
    {
        // TODO: 顯示 UI 提示
        float timer = ForceRecallCountdownDuration;

        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        // 傳送回玩家
        this.transform.position = OwnerCharacter.transform.position;

        // 強制切換狀態為 Default
        UpdateCompanionBehaviorState(CompanionBehaviorState.Default);

        _forceRecallRoutine = null;

        // TODO: 隱藏 UI 提示
    }

#if UNITY_EDITOR
    //[Button("Test InitializeCompanion")]
    private void TestInitializeCompanion()
    {
        Debug.Log("[CompanionAIContext] TestInitializeCompanion button pressed.");
        InitializeCompanion();
    }
#endif
}
