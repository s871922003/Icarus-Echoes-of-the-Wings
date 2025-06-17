using UnityEngine;
using System.Collections.Generic;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Holds a list of weapons indexed by string ID, useful for AI or ability-driven weapon switching.
    /// </summary>
    [CreateAssetMenu(fileName = "NewWeaponLibrary", menuName = "TopDown Engine/Weapons/Weapon Library", order = 1)]
    public class WeaponLibrary : ScriptableObject
    {
        [System.Serializable]
        public struct WeaponEntry
        {
            public string ID;
            public Weapon WeaponPrefab;

            [Header("Stats Overrides")]
            public float AttackInterval;
            public float Damage;
            public float Range;
        }

        [Tooltip("List of weapon entries (ID ¡÷ prefab + stats)")]
        public List<WeaponEntry> WeaponEntries = new List<WeaponEntry>();

        protected Dictionary<string, Weapon> _weaponLookup;
        protected Dictionary<string, WeaponEntry> _weaponEntryLookup;

        /// <summary>
        /// Call before accessing any weapon if you plan to use this at runtime
        /// </summary>
        public void Initialize()
        {
            _weaponLookup = new Dictionary<string, Weapon>();
            _weaponEntryLookup = new Dictionary<string, WeaponEntry>();

            foreach (var entry in WeaponEntries)
            {
                if (!string.IsNullOrEmpty(entry.ID) && entry.WeaponPrefab != null)
                {
                    _weaponLookup[entry.ID] = entry.WeaponPrefab;
                    _weaponEntryLookup[entry.ID] = entry;
                }
            }
        }

        /// <summary>
        /// Returns a weapon prefab by ID, or null if not found.
        /// </summary>
        public Weapon GetWeaponByID(string id)
        {
            if (_weaponLookup == null)
            {
                Initialize();
            }

            if (_weaponLookup.TryGetValue(id, out Weapon weapon))
            {
                return weapon;
            }
            else
            {
                Debug.LogWarning($"[WeaponLibrary] Weapon ID '{id}' not found.");
                return null;
            }
        }

        /// <summary>
        /// Returns stat configuration for a weapon ID, or null if not found.
        /// </summary>
        public WeaponEntry? GetStatsByID(string id)
        {
            if (_weaponEntryLookup == null)
            {
                Initialize();
            }

            if (_weaponEntryLookup.TryGetValue(id, out WeaponEntry entry))
            {
                return entry;
            }

            return null;
        }
    }
}
