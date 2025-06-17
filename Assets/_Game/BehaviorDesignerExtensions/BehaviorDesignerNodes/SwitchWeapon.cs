using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using TooltipAttribute = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;
using MoreMountains.TopDownEngine;
using System.Collections;

namespace MoreMountains.TopDownEngine
{
    [TaskCategory("Custom/Character")]
    [TaskDescription("Switches the character's weapon using a string-based weapon ID via the WeaponLibrary and applies associated stats.")]
    public class SwitchWeapon : Action
    {
        [Tooltip("The string ID of the weapon to switch to")]
        public SharedString WeaponID;

        [Tooltip("The WeaponLibrary instance to search weapons from")]
        public SharedWeaponLibrary WeaponLibrary;

        protected Character _character;
        protected CharacterHandleWeapon _handleWeapon;
        protected CharacterStatsManager _statsManager;

        public override void OnAwake()
        {
            _character = GetComponent<Character>();
            _handleWeapon = _character?.FindAbility<CharacterHandleWeapon>();
            _statsManager = _character?.GetComponent<CharacterStatsManager>();
        }

        public override TaskStatus OnUpdate()
        {
            if (_character == null || _handleWeapon == null || WeaponLibrary.Value == null || string.IsNullOrEmpty(WeaponID.Value))
            {
                Debug.LogWarning("[SwitchWeapon] Missing references: character, handler, library, or weapon ID.");
                return TaskStatus.Failure;
            }

            Weapon targetWeapon = WeaponLibrary.Value.GetWeaponByID(WeaponID.Value);

            if (targetWeapon == null)
            {
                Debug.LogWarning($"[SwitchWeapon] Weapon with ID '{WeaponID.Value}' not found in WeaponLibrary.");
                return TaskStatus.Failure;
            }

            _handleWeapon.ChangeWeapon(targetWeapon, WeaponID.Value);

            var weaponStats = WeaponLibrary.Value.GetStatsByID(WeaponID.Value);
            if (weaponStats.HasValue && _statsManager != null)
            {
                _statsManager.OverrideWeaponStats(weaponStats.Value.AttackInterval, weaponStats.Value.Damage, weaponStats.Value.Range);
            }

            return TaskStatus.Success;
        }
    }
}
