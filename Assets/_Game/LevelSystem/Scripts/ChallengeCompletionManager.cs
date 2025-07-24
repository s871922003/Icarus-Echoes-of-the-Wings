using UnityEngine;
using UnityEngine.Events;

public class ChallengeCompletionManager : MonoBehaviour
{
    [Tooltip("挑戰持續時間（秒）")]
    public float ChallengeDuration = 30f;

    [Tooltip("是否啟動時自動開始計時")]
    public bool AutoStart = true;

    public UnityEvent OnChallengeCompleted;

    private float _timer = 0f;
    private bool _running = false;

    private void Start()
    {
        if (AutoStart)
        {
            StartChallenge();
        }
    }

    public void StartChallenge()
    {
        _running = true;
        _timer = 0f;
    }

    private void Update()
    {
        if (!_running) return;

        _timer += Time.deltaTime;
        if (_timer >= ChallengeDuration)
        {
            _running = false;
            Debug.Log("[ChallengeCompletionManager] 挑戰完成");

            OnChallengeCompleted?.Invoke();
        }
    }
}
