using MoreMountains.TopDownEngine;
using UnityEngine;
using System.Collections.Generic;

public class CharacterHandleWeaponCenter : MonoBehaviour
{
    [Tooltip("If true, only one weapon's sprite will be visible at a time.")]
    public bool DisplayOnlyOneVisual = true;

    private List<CharacterHandleWeaponMoreEvent> _weaponHandlers = new List<CharacterHandleWeaponMoreEvent>();
    private Weapon _lastUsedWeapon;
    private Weapon _defaultWeapon;

    private void Start()
    {
        Initialization();
    }

    protected void Initialization()
    {
        FindWeaponHandlers();
        if (_weaponHandlers.Count > 0 && _weaponHandlers[0].CurrentWeapon != null)
        {
            _defaultWeapon = _weaponHandlers[0].CurrentWeapon;
            _lastUsedWeapon = _defaultWeapon;
        }
        UpdateWeaponVisibility();
    }

    private void FindWeaponHandlers()
    {
        _weaponHandlers.Clear();
        _weaponHandlers.AddRange(GetComponents<CharacterHandleWeaponMoreEvent>());

        foreach (var handler in _weaponHandlers)
        {
            handler.OnWeaponShoot += NotifyWeaponUsed;
            handler.OnWeaponStop += RestoreDefaultWeapon;
        }
    }

    public void NotifyWeaponUsed(Weapon usedWeapon)
    {
        if (usedWeapon == null) return;
        _lastUsedWeapon = usedWeapon;
        if (DisplayOnlyOneVisual)
        {
            UpdateWeaponVisibility();
        }
    }

    public void RestoreDefaultWeapon()
    {
        Debug.Log("Restore Default Weapon");

        if (_defaultWeapon != null)
        {
            _lastUsedWeapon = _defaultWeapon;
            UpdateWeaponVisibility();
        }
    }

    private void UpdateWeaponVisibility()
    {
        
        foreach (var handler in _weaponHandlers)
        {
            if (handler.CurrentWeapon != null)
            {
                SpriteRenderer spriteRenderer = handler.CurrentWeapon.GetComponentInChildren<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    // Enable only the weapon in use, disable others
                    spriteRenderer.enabled = (handler.CurrentWeapon == _lastUsedWeapon);
                    
                }
            }
        }
    }
}