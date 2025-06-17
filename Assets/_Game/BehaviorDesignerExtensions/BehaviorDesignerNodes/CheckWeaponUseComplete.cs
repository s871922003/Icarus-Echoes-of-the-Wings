using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using MoreMountains.TopDownEngine;

[TaskCategory("Custom/Utility")]
[TaskDescription("Checks if the current weapon has finished its attack cycle.")]
public class CheckWeaponUseComplete : Conditional
{
    protected CharacterHandleWeapon _handleWeapon;

    public override void OnStart()
    {
        var character = GetComponent<Character>();
        _handleWeapon = character?.FindAbility<CharacterHandleWeapon>();
    }

    public override TaskStatus OnUpdate()
    {
        if (_handleWeapon == null || _handleWeapon.CurrentWeapon == null)
        {
            Debug.LogWarning("[CheckWeaponUseComplete] Weapon not found.");
            return TaskStatus.Failure;
        }

        var weaponState = _handleWeapon.CurrentWeapon.WeaponState.CurrentState;
        return weaponState == Weapon.WeaponStates.WeaponIdle ? TaskStatus.Success : TaskStatus.Running;
    }
}
