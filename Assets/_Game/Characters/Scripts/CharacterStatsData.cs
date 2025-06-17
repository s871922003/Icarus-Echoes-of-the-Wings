using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewCharacterStats", menuName = "Character/Stats Data")]
public class CharacterStatsData : ScriptableObject
{
    [Header("Basic Stats")]
    public float BaseHealth = 100f;
    public float WalkSpeed = 5f;
    public float FollowDistance = 2f;
}
