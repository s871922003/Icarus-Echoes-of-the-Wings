using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using MoreMountains.TopDownEngine;

namespace MoreMountains.TopDownEngine
{
    [TaskCategory("Custom/Character")]
    [TaskDescription("Triggers the currently equipped weapon's attack logic (ShootStart or TriggerWeapon).")]
    public class PilotCharacterAttackWithWeapon : Action
    {
        protected Character _character;
        protected CharacterHandleWeapon _handleWeapon;
        protected Weapon _currentWeapon;
        public SharedTransform TargetTransform;

        public override void OnStart()
        {
            _character = GetComponent<Character>();
            _handleWeapon = _character?.FindAbility<CharacterHandleWeapon>();
            _currentWeapon = _handleWeapon?.CurrentWeapon;
        }

        public override TaskStatus OnUpdate()
        {
            if (_character == null || _handleWeapon == null || _handleWeapon.CurrentWeapon == null)
            {
                Debug.LogWarning("[PilotCharacterAttackWithWeapon] Missing character, weapon handler, or current weapon.");
                return TaskStatus.Failure;
            }

            Vector3 characterPosition = _character.transform.position;
            Vector3 targetPosition = TargetTransform.Value.position;

            WeaponAim2DJitterProved weaponAim = _handleWeapon.CurrentWeapon.GetComponent<WeaponAim2DJitterProved>();
            if (weaponAim != null)
            {
                weaponAim.SetScriptAimTarget(targetPosition);
            }
            else
            {
                // fallback ÂÂª©ªZ¾¹
                WeaponAim2D weaponAimLegacy = _handleWeapon.CurrentWeapon.GetComponent<WeaponAim2D>();
                if (weaponAimLegacy != null)
                {
                    Vector3 direction = targetPosition - characterPosition;
                    weaponAimLegacy.SetCurrentAim(direction);
                }
            }

            _handleWeapon.ShootStart(); // ±Ò°Ê§ðÀ»
            return TaskStatus.Success;
        }
    }
}
