using UnityEngine;
using BehaviorDesigner.Runtime;
using MoreMountains.TopDownEngine;

public abstract class BaseAIContext : MonoBehaviour
{
    [Header("Core Components")]
    public BehaviorTree behaviorTree { get; protected set; }
    protected Character _character;
    protected Animator _animator;

    protected virtual void Start()
    {
        InitializeComponents();
        InitializeBehaviorTree();
        InitializeAI(); // 留給子類擴展
    }

    protected virtual void InitializeComponents()
    {
        _character = GetComponent<Character>();
        behaviorTree = GetComponent<BehaviorTree>();
        _animator = GetComponent<Animator>();
    }

    protected virtual void InitializeBehaviorTree()
    {
        if (behaviorTree == null) return;

        SyncBehaviorTreeVariable("ThisCharacter", _character);
        SyncBehaviorTreeVariable("ThisTransform", this.transform);
        SyncBehaviorTreeVariable("Animator", _animator);
    }

    protected virtual void InitializeAI() { }

    protected void SyncBehaviorTreeVariable(string variableName, object value)
    {
        if (behaviorTree != null)
        {
            behaviorTree.SetVariableValue(variableName, value);
        }
    }
}
