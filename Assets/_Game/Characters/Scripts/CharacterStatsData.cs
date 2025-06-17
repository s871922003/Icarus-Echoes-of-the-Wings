using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewCharacterStats", menuName = "Character/Stats Data")]
public class CharacterStatsData : ScriptableObject
{
    [Header("Basic Stats")]
    public float BaseHealth = 100f;
    public float MoveSpeed = 5f;
    public float RunSpeed = 8f;
    public float AttackInterval = 1f;
    public float FollowDistance = 3f;
    public float AttackDistance = 1.5f; // �D�n�Ω�����j����A�ؼлP�ۤv���Z���p�󦹮ɶZ���P�w�Y�����A����i��N����������
    public float StaminaRegenRate = 2f;
    public float BasePoundDistance = 5f;
    public float PoundSpeed = 5f;
    public float ForceRecallRange = 10f; // TODO - �٦�M�ΰѼơA��٦�Z���D�H�W�L���Z����|�i���ơA�ɶ�������N�|�⻷�誺�٦�ۤv�Ԧ^�D�H����
    public float MaxStamina = 100f;

    [Header("Energy Cost Table")]
    public List<EnergyCostEntry> EnergyConsumptionTable = new();
}


[System.Serializable]
public class EnergyCostEntry
{
    public CompanionStamina.StaminaDrainedMoves ActionType;
    public float EnergyCost;
}
