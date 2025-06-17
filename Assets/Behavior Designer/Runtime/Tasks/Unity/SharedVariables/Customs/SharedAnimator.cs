using BehaviorDesigner.Runtime;
using MoreMountains.TopDownEngine;
using UnityEngine;

[System.Serializable]

public class SharedAnimator : SharedVariable<Animator>
{
    public static implicit operator SharedAnimator(Animator value)
    {
        return new SharedAnimator { Value = value };
    }
}