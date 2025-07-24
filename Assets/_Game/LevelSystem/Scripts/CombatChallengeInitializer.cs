using UnityEngine;

/// <summary>
/// CombatChallengeInitializer �N�b IcarusCombat �}���ɦ۰ʱҰʬD�Ԭy�{�]�Ҧp�ͦ��ĤH�^
/// </summary>
public class CombatChallengeInitializer : MonoBehaviour
{
    [SerializeField] private ChallengeSpawnManager spawnManager;

    [Tooltip("�O�_�b Start �ɦ۰ʶ}�l�D��")]
    [SerializeField] private bool autoStart = true;

    private void Start()
    {
        if (autoStart && spawnManager != null)
        {
            spawnManager.StartChallengeSpawning();
        }
    }
}
