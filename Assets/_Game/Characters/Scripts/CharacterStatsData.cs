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
    public float AttackDistance = 1.5f; // 主要用於攻擊迴圈當中，目標與自己的距離小於此時距離判定即完成，後續可能就攻擊之類的
    public float StaminaRegenRate = 2f;
    public float BasePoundDistance = 5f;
    public float PoundSpeed = 5f;
    public float ForceRecallRange = 10f; // TODO - 夥伴專用參數，當夥伴距離主人超過此距離後會進行到數，時間結束後就會把遠方的夥伴自己拉回主人身邊
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
