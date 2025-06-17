using UnityEngine;
using MoreMountains.TopDownEngine;

/// <summary>
/// 讓角色在跨場景時保留並於新場景中初始化的處理器
/// </summary>
public class CharacterPersistenceHandler : PersistentObjectBase
{
    protected Character _character;

    protected override void Awake()
    {
        base.Awake();
        _character = this.gameObject.GetComponentInParent<Character>();
    }

    public override void OnBeforeSceneUnload()
    {
        this.gameObject.SetActive(false);
    }

    public override void OnSceneLoadComplete()
    {
        if (_character == null)
        {
            Debug.LogWarning("[CharacterPersistenceHandler] 找不到 Character 元件！");
            return;
        }

        this.gameObject.SetActive(true);

        _character.enabled = true;
        _character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Normal);
        _character.MovementState.ChangeState(CharacterStates.MovementStates.Idle);
        _character.SetInputManager();
    }
}
