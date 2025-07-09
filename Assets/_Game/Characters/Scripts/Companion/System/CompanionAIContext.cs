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
    public CommandActionExecutor CommandExecutor { get; private set; }

    public bool CanDoDefaultBehavior = true;

    public CompanionBehaviorState CurrentBehaviorState;
    CompanionBehaviorState LastBehaviorState;

    public enum CompanionBehaviorState
    {
        None,
        NoOwner,
        CommandAction,
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
        CommandExecutor = GetComponent<CommandActionExecutor>();
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
            case MMCompanionCommandEventTypes.CommandAction:
                TryEnterCommandAcionState();
                break;
        }
    }

    private void TryEnterCommandAcionState()
    {
        // �p�G�w�g�b�аO�ʧ@���A���A�������i��аO�ʧ@���L�{���S���Ʀ�����O�ɡA�����_��e�аO�ʧ@
        if (CurrentBehaviorState == CompanionBehaviorState.CommandAction) return;

        // �����i�J�аO�ʧ@
        UpdateCompanionBehaviorState(CompanionBehaviorState.CommandAction);
    }

    public void OnMMEvent(MMCompanionActionEvent actionEvent)
    {
        switch (actionEvent.EventType)
        {
            case MMCompanionActionEventTypes.OnCommandActionStart:
                CommandExecutor?.TryExecute();
                break;

            case MMCompanionActionEventTypes.OnCommandActionEnd:
                StartCoroutine(WaitToResumeDefaultBehavior());
                break;

            default:
                Debug.Log("�����T�w�q���ƥ�QĲ�o");
                break;
        }
    }

    private IEnumerator WaitToResumeDefaultBehavior()
    {
        while (!CanDoDefaultBehavior)
        {
            yield return null; // �C�V���ݪ�����_�w�]�欰
        }

        UpdateCompanionBehaviorState(CompanionBehaviorState.Default);
    }


    protected void SyncBehaviorTreeVariable(string variableName, object value)
    {
        if (behaviorTree != null)
        {
            behaviorTree.SetVariableValue(variableName, value);
        }
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


                case CompanionBehaviorState.CommandAction:
                    SyncBehaviorTreeVariable("CurrentBehaviorState", CompanionBehaviorState.CommandAction);                    
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


            case CompanionBehaviorState.CommandAction:
                break;


            default: break;
                // �i�̻ݨD�~���X�R�A�Ҧp Recalled�BNoOwner ���� Reset
        }
    }
}
