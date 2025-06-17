using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;

public class CompanionStamina : MonoBehaviour
{
    public event System.Action OnBurnout;
    public event System.Action OnRecover;

    public enum StaminaDrainedMoves
    {
        DefaultAttack,
        MarkAction
    }

    public enum EnergyState { Normal, Burnout }



    [Tooltip("Maximum stamina the companion can have.")]
    public float MaxStamina = 100f;

    [Tooltip("Rate at which stamina regenerates per second.")]
    public float RegenRate = 10f;

    [Tooltip("Minimum stamina required to enable the companion.")]
    public float MinStaminaToEnable = 20f;

    [Tooltip("How much stamina is consumed on enable.")]
    public float EnableCost = 25f;

    [SerializeField] float CurrentStamina = 0;

    public Dictionary<StaminaDrainedMoves, float> EnergyCostTable { get; private set; } = new();

    [SerializeField] EnergyState CurrentEnergyState = EnergyState.Normal;


    protected virtual void Start()
    {
        CurrentStamina = MaxStamina;
        RefreshStaminaBar();
    }

    protected virtual void Update()
    {
        Regenerate();
    }

    protected virtual void Regenerate()
    {
        if (CurrentEnergyState == EnergyState.Burnout)
        {
            CurrentStamina += RegenRate * Time.deltaTime;
            CurrentStamina = Mathf.Min(CurrentStamina, MaxStamina);

            if (CurrentStamina == MaxStamina)
            {
                CurrentEnergyState = EnergyState.Normal;
                OnRecover?.Invoke();
                Debug.Log("[Stamina] Recovered from Burnout");
            }

            RefreshStaminaBar();
        }
    }


    public bool TryConsumeForAction(StaminaDrainedMoves actionType)
    {
        if (EnergyCostTable.TryGetValue(actionType, out float cost))
        {
            if (CurrentStamina >= cost)
            {
                Debug.Log("Stamina Drained");
                CurrentStamina -= cost;
                RefreshStaminaBar();
                if (CurrentStamina <= 0f)
                {
                    EnterBurnout();
                }
                return true;
            }
            else if (CurrentStamina > 0f)
            {
                // 還可以觸發動作，但會燃盡
                CurrentStamina = 0f;
                RefreshStaminaBar();
                EnterBurnout();
                return true;
            }

            return false; // Burnout 中，完全無法扣除
        }

        return true; // 若無對應條目則不消耗
    }

    private void EnterBurnout()
    {
        if (CurrentEnergyState != EnergyState.Burnout)
        {
            CurrentEnergyState = EnergyState.Burnout;
            Debug.Log("[Stamina] Entered Burnout");

            // 通知其他系統（選配）
            OnBurnout?.Invoke();
        }
    }


    public void SetEnergyCostTable(List<EnergyCostEntry> entries)
    {
        EnergyCostTable.Clear();
        foreach (var entry in entries)
        {
            if (!EnergyCostTable.ContainsKey(entry.ActionType))
            {
                EnergyCostTable.Add(entry.ActionType, entry.EnergyCost);
            }
        }
    }

    private void RefreshStaminaBar()
    {
        var bar = GetComponent<CompanionStaminaBar>();
        if (bar != null)
        {
            bar.UpdateBar(CurrentStamina, 0f, MaxStamina, true);
        }
    }

}
