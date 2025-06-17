using BehaviorDesigner.Runtime;
using MoreMountains.TopDownEngine;
using UnityEngine;

[System.Serializable]

public class SharedWeaponLibrary : SharedVariable<WeaponLibrary>
{
    public static implicit operator SharedWeaponLibrary(WeaponLibrary value)
    {
        return new SharedWeaponLibrary { Value = value };
    }
}