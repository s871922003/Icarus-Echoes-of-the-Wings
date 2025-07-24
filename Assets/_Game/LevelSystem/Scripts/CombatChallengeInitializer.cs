using UnityEngine;

/// <summary>
/// CombatChallengeInitializer 將在 IcarusCombat 開場時自動啟動挑戰流程（例如生成敵人）
/// </summary>
public class CombatChallengeInitializer : MonoBehaviour
{
    [SerializeField] private ChallengeSpawnManager spawnManager;

    [Tooltip("是否在 Start 時自動開始挑戰")]
    [SerializeField] private bool autoStart = true;

    private void Start()
    {
        if (autoStart && spawnManager != null)
        {
            spawnManager.StartChallengeSpawning();
        }
    }
}
