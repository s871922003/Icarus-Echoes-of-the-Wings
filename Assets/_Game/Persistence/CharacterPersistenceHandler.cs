using UnityEngine;
using MoreMountains.TopDownEngine;

/// <summary>
/// ������b������ɫO�d�é�s��������l�ƪ��B�z��
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
            Debug.LogWarning("[CharacterPersistenceHandler] �䤣�� Character ����I");
            return;
        }

        this.gameObject.SetActive(true);

        _character.enabled = true;
        _character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Normal);
        _character.MovementState.ChangeState(CharacterStates.MovementStates.Idle);
        _character.SetInputManager();
    }
}
