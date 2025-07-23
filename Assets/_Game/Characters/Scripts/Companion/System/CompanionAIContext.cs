using UnityEngine;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools;
using BehaviorDesigner.Runtime;
using System.Collections;

[System.Serializable]
public class CompanionAIContext : BaseAIContext, MMEventListener<MMCompanionCommandEvent>, MMEventListener<MMCompanionActionEvent>
{
    [Header("Owner")]
    public Character OwnerCharacter { get; private set; }

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

    private bool _isAIInitialized = false;
    private bool _pendingOwnerSync = false;

    protected override void InitializeAI()
    {
        CommandExecutor = GetComponent<CommandActionExecutor>();
        SyncBehaviorTreeVariable("CompanionAIContext", this);

        _isAIInitialized = true;

        // �Y���e���]�w Owner�A�{�b�ɤW�P�B
        if (_pendingOwnerSync && OwnerCharacter != null)
        {
            ApplyOwnerToBehaviorTree();
            UpdateCompanionBehaviorState(CompanionBehaviorState.Default);
        }
        else
        {
            UpdateCompanionBehaviorState(OwnerCharacter != null
                ? CompanionBehaviorState.Default
                : CompanionBehaviorState.NoOwner);
        }
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

        // �p�G�|����l�ơA����欰���ܼƦP�B
        if (!_isAIInitialized)
        {
            _pendingOwnerSync = true;
            return;
        }

        ApplyOwnerToBehaviorTree();
        UpdateCompanionBehaviorState(CompanionBehaviorState.Default);
    }

    private void ApplyOwnerToBehaviorTree()
    {
        SyncBehaviorTreeVariable("OwnerCharacter", OwnerCharacter);
        SyncBehaviorTreeVariable("OwnerTransform", OwnerCharacter.transform);
    }

    public void OnMMEvent(MMCompanionCommandEvent companionCommandEvent)
    {
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
        if (CurrentBehaviorState == CompanionBehaviorState.CommandAction) return;
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
            yield return null;
        }
        UpdateCompanionBehaviorState(CompanionBehaviorState.Default);
    }

    public void UpdateCompanionBehaviorState(CompanionBehaviorState newState)
    {
        if (newState != CurrentBehaviorState)
        {
            LastBehaviorState = CurrentBehaviorState;
            HandleStateExit(LastBehaviorState);
            CurrentBehaviorState = newState;

            SyncBehaviorTreeVariable("CurrentBehaviorState", newState);
        }
    }

    protected void HandleStateExit(CompanionBehaviorState exitedState)
    {
        // �ھڻݭn�X�R
    }
}
