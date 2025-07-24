using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools;

public class IcarusLevelManager : MonoBehaviour
{
    [Header("關卡資料")]
    [SerializeField] private List<LevelData> levels;
    [SerializeField] private int currentIndex = 0;

    public LevelData CurrentLevel => levels[currentIndex];
    public bool ChallengeStarted { get; private set; } = false;

    public UnityEvent OnLevelChanged;
    public UnityEvent OnChallengeStarted;
    public UnityEvent OnChallengeFinished;

    public void SwitchToNextLevel()
    {
        currentIndex = (currentIndex + 1) % levels.Count;
        OnLevelChanged?.Invoke();
    }

    public void NotifyChallengeCompleted()
    {
        Debug.Log("[IcarusLevelManager] 挑戰結束，將切換場景");
        OnChallengeFinished?.Invoke();

        // 可選：自動切關
        LevelManager.Instance.GotoLevel("IcarusBase");
    }

    public void StartChallenge()
    {
        if (ChallengeStarted) return;

        ChallengeStarted = true;
        OnChallengeStarted?.Invoke();

        if (LevelManager.HasInstance)
        {
            LevelManager.Instance.GotoLevel(CurrentLevel.SceneName);
        }
        else
        {
            Debug.LogWarning("[IcarusLevelManager] 無法啟動挑戰：LevelManager 尚未初始化");
        }
    }
}
