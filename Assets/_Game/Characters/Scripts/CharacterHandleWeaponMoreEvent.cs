using UnityEngine;
using MoreMountains.TopDownEngine;

public class CharacterHandleWeaponMoreEvent : CharacterHandleWeapon
{
    public delegate void OnWeaponShootDelegate(Weapon usedWeapon);
    public OnWeaponShootDelegate OnWeaponShoot;

    public delegate void OnWeaponStopDelegate();
    public OnWeaponStopDelegate OnWeaponStop;

    public override void ForceStop()
    {
        base.ForceStop();
        OnWeaponStop?.Invoke();
    }

    public override void ShootStart()
    {
        if (!AbilityAuthorized || (CurrentWeapon == null) || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal))
        {
            return;
        }

        if (BufferInput && (CurrentWeapon.WeaponState.CurrentState != Weapon.WeaponStates.WeaponIdle))
        {
            ExtendBuffer();
        }

        if (BufferInput && RequiresPerfectTile && (_characterGridMovement != null))
        {
            if (!_characterGridMovement.PerfectTile)
            {
                ExtendBuffer();
                return;
            }
            else
            {
                _buffering = false;
            }
        }

        PlayAbilityStartFeedbacks();
        CurrentWeapon.WeaponInputStart();
        OnWeaponShoot?.Invoke(CurrentWeapon);
    }
}
