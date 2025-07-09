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
        Debug.Log("收到玩家命令");
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
        // 如果已經在標記動作狀態中，換言之進行標記動作的過程中又重複收到指令時，不打斷當前標記動作
        if (CurrentBehaviorState == CompanionBehaviorState.CommandAction) return;

        // 正式進入標記動作
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
                Debug.Log("未明確定義的事件被觸發");
                break;
        }
    }

    private IEnumerator WaitToResumeDefaultBehavior()
    {
        while (!CanDoDefaultBehavior)
        {
            yield return null; // 每幀等待直到能恢復預設行為
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
                // 可依需求繼續擴充，例如 Recalled、NoOwner 不需 Reset
        }
    }
}
