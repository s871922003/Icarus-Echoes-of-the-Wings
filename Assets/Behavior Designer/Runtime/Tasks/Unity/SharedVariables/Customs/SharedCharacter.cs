using BehaviorDesigner.Runtime;
using MoreMountains.TopDownEngine;

[System.Serializable]

public class SharedCharacter : SharedVariable<Character>
{
    public static implicit operator SharedCharacter(Character value)
    {
        return new SharedCharacter { Value = value };
    }
}
