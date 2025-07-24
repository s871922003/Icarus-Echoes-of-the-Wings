using UnityEngine;
using UnityEngine.Events;

public class ChallengeCompletionManager : MonoBehaviour
{
    [Tooltip("�D�ԫ���ɶ��]��^")]
    public float ChallengeDuration = 30f;

    [Tooltip("�O�_�Ұʮɦ۰ʶ}�l�p��")]
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
            Debug.Log("[ChallengeCompletionManager] �D�ԧ���");

            OnChallengeCompleted?.Invoke();
        }
    }
}
