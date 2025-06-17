using UnityEngine;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools;
using BehaviorDesigner.Runtime;
using System.Collections;

[System.Serializable]
public class CompanionAIContext : MonoBehaviour, MMEventListener<MMCompanionCommandEvent>, MMEventListener<MMCompanionActionEvent>
{
    [Header("Owner")]
    public Character OwnerCharacter { get; private set; }

    [Header("Core Components")]
    public BehaviorTree behaviorTree;
    protected Character _character;
    private Animator _animator;

    public float MarkActionCooldownDuration = 1.5f;

    [Header("Flags")]
    public bool IsMarkActionOnCooldown { get; private set; } = false;

    public CompanionBehaviorState CurrentBehaviorState;
    CompanionBehaviorState LastBehaviorState;

    public enum CompanionBehaviorState
    {
        None,
        NoOwner,
        MarkAction,
        Default,
    }

    protected virtual void Start()
    {
        InitializeComponents();
        InitializeBehaviorTree();
        InitializeCompanion();
    }

    protected void InitializeComponents()
    {
        _character = GetComponent<Character>();
        behaviorTree = GetComponent<BehaviorTree>();
        _animator = GetComponent<Animator>();
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


    public void OnMMEvent(MMCompanionCommandEvent companionCommandEvent)
    {
        Debug.Log("���쪱�a�R�O");
        if (companionCommandEvent.EventInvokerCharacter != OwnerCharacter) return;

        switch (companionCommandEvent.EventType)
        {
            case MMCompanionCommandEventTypes.MarkAction:
                TryEnterMarkAcionState();
                break;
        }
    }

    private void TryEnterMarkAcionState()
    {
        // �p�G�w�g�b�аO�ʧ@���A���A�������i��аO�ʧ@���L�{���S���Ʀ�����O�ɡA�����_��e�аO�ʧ@
        if (CurrentBehaviorState == CompanionBehaviorState.MarkAction) return;

        // �N�o���h�����\�i�J���A
        if (IsMarkActionOnCooldown) return;

        // �����i�J�аO�ʧ@
        UpdateCompanionBehaviorState(CompanionBehaviorState.MarkAction);
    }

    public void OnMMEvent(MMCompanionActionEvent actionEvent)
    {
        switch (actionEvent.EventType)
        {
            case MMCompanionActionEventTypes.OnMarkActionStart:
                break;

            case MMCompanionActionEventTypes.OnMarkActionEnd:
                break;

            default:
                Debug.Log("�����T�w�q���ƥ�QĲ�o");
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
            LastBehaviorState = CurrentBehaviorState;
            HandleStateExit(LastBehaviorState);
            CurrentBehaviorState = newState;

            switch (newState)
            {
                case CompanionBehaviorState.NoOwner:
                    SyncBehaviorTreeVariable("CurrentBehaviorState", CompanionBehaviorState.NoOwner);
                    break;


                case CompanionBehaviorState.MarkAction:
                    SyncBehaviorTreeVariable("CurrentBehaviorState", CompanionBehaviorState.MarkAction);
                    TriggerMarkActionCooldown();
                    break;

                case CompanionBehaviorState.Default:
                    SyncBehaviorTreeVariable("CurrentBehaviorState", CompanionBehaviorState.Default);
                    break;

                default:
                    Debug.LogWarning($"[CompanionAIContext] Unhandled CompanionBehaviorState: {newState}");
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


            case CompanionBehaviorState.MarkAction:
                break;


            default: break;
                // �i�̻ݨD�~���X�R�A�Ҧp Recalled�BNoOwner ���� Reset
        }
    }
}
