using UnityEngine;
using MoreMountains.TopDownEngine;

/// <summary>
/// Fix weapon jitter when character is facing West by stabilizing aim direction calculation.
/// </summary>
[AddComponentMenu("TopDown Engine/Weapons/Weapon Aim 2D Jitter Proved")]
public class WeaponAim2DJitterProved : WeaponAim2D
{
    [Tooltip("Script Aim Target (world position), externally assigned")]
    public Vector3 TargetTransform;

    public void SetScriptAimTarget(Vector3 worldPosition)
    {
        TargetTransform = worldPosition;
    }

    protected override void GetCurrentAim()
    {
        if (!AimControlActive || (_weapon == null) || (_weapon.Owner == null))
        {
            return;
        }

        if ((_weapon.Owner.LinkedInputManager == null) && (_weapon.Owner.CharacterType == Character.CharacterTypes.Player))
        {
            return;
        }

        AutoDetectWeaponMode();

        switch (AimControl)
        {
            case AimControls.Script:
                GetScriptAimStable();
                break;

            case AimControls.Mouse:
                GetMouseAimStable();
                break;

            default:
                base.GetCurrentAim();
                break;
        }
    }

    protected void GetScriptAimStable()
    {
        if (TargetTransform == null) return;

        Vector3 characterPos = _weapon.Owner.transform.position;
        Vector3 targetPos = TargetTransform;

        _currentAim = targetPos - characterPos;
        _currentAimAbsolute = _currentAim;

        var orientation2D = _weapon.Owner.FindAbility<CharacterOrientation2D>();
        if (orientation2D != null && orientation2D.FacingMode != CharacterOrientation2D.FacingModes.None)
        {
            if (!orientation2D.IsFacingRight)
            {
                _currentAim = -_currentAim;
            }
        }

        _direction = -(this.transform.position - (characterPos + _currentAim));
    }

    protected void GetMouseAimStable()
    {
        _mousePosition = InputManager.Instance.MousePosition;
        _mousePosition.z = 10;

        Vector3 mouseWorldPos = _mainCamera.ScreenToWorldPoint(_mousePosition);
        mouseWorldPos.z = _weapon.Owner.transform.position.z;

        Vector3 characterPos = _weapon.Owner.transform.position;

        _currentAim = mouseWorldPos - characterPos;
        _currentAimAbsolute = _currentAim;

        var orientation2D = _weapon.Owner.FindAbility<CharacterOrientation2D>();
        if (orientation2D != null && orientation2D.FacingMode != CharacterOrientation2D.FacingModes.None)
        {
            if (!orientation2D.IsFacingRight)
            {
                _currentAim = -_currentAim;
            }
        }

        _direction = -(this.transform.position - (characterPos + _currentAim));
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (TargetTransform == null) return;

        // Debug Gizmo - Target Point & Weapon Direction
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(TargetTransform, 0.3f);

        Vector3 weaponPos = this.transform.position;
        Vector3 targetPos = TargetTransform;
        Vector3 direction = targetPos - weaponPos;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(weaponPos, targetPos);

        Vector3 arrowDir = direction.normalized * 0.3f;
        Vector3 right = Quaternion.LookRotation(Vector3.forward) * Quaternion.Euler(0, 0, 135) * arrowDir;
        Vector3 left = Quaternion.LookRotation(Vector3.forward) * Quaternion.Euler(0, 0, -135) * arrowDir;

        Gizmos.DrawLine(targetPos, targetPos - right);
        Gizmos.DrawLine(targetPos, targetPos - left);
    }
#endif
}
