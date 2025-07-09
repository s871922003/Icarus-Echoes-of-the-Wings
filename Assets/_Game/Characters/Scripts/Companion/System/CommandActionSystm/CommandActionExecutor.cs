using UnityEngine;
using UnityEngine.Events;

public class CommandActionExecutor : MonoBehaviour
{
    [SerializeField]
    private CommandAction _currentCommandAction;

    public CommandAction CurrentCommandAction
    {
        get => _currentCommandAction;
        set
        {
            _currentCommandAction = value;
            InitializeCommandAction();
        }
    }

    protected CompanionAIContext _context;

    protected virtual void Awake()
    {
        _context = GetComponent<CompanionAIContext>();
        InitializeCommandAction();
    }

    protected virtual void Update()
    {
        _currentCommandAction?.UpdateCooldown(Time.deltaTime);
    }

    public virtual void TryExecute()
    {
        if (_currentCommandAction == null)
        {
            Debug.LogWarning("[CommandActionExecutor] No CommandAction assigned.");
            return;
        }

        if (!_currentCommandAction.CanExecute())
        {
            Debug.Log("[CommandActionExecutor] �ʧ@�L�k����A�ثe�N�o�α��󤣲šC");
            return;
        }

        Debug.Log($"[CommandActionExecutor] ����ʧ@�G{_currentCommandAction.ActionName}");
        _currentCommandAction.Execute();
    }

    protected virtual void InitializeCommandAction()
    {
        if (_currentCommandAction != null)
        {
            _currentCommandAction.AIContext = _context;
            Debug.Log($"[CommandActionExecutor] �]�w�s CommandAction: {_currentCommandAction.ActionName}�A�ê�l�� AIContext");
        }
    }
}
