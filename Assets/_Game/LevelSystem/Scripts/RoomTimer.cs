using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;

namespace Game.Rooms
{
    /// <summary>
    /// �����Ω� TimeAttack / Survive ���ж��D�Ԫ��˼ƭp�ɾ��C
    /// �ݥ� RoomEventBroadcaster �ҰʡA�˼Ƶ�����|Ĳ�o OnTimesUp�C
    /// </summary>
    public class RoomTimer : MonoBehaviour, MMEventListener<MMRoomChallengeEvent>
    {
        [Header("Timer Settings")]
        [Tooltip("�˼ƭp�ɬ��")]
        public float CountdownDuration = 20f;

        [Tooltip("�O�_�b�}�l�ɦ۰ʱҰʭ˼� (��ĳ�����A�Ѩƥ󱱨�)")]
        public bool AutoStart = false;

        [Header("Events")]
        [Tooltip("�˼Ʈɶ�������Ĳ�o���ƥ�")]
        public UnityEvent OnTimesUp;

        protected float _timeLeft;
        protected bool _running = false;

        protected virtual void Start()
        {
            if (AutoStart)
            {
                StartTimer();
            }
        }

        public virtual void OnMMEvent(MMRoomChallengeEvent e)
        {
            if (e.EventType != MMRoomChallengeEventType.ChallengeStart)
                return;

            StartTimer();
        }

        protected virtual void StartTimer()
        {
            if (_running) return;
            _running = true;
            _timeLeft = CountdownDuration;
            StartCoroutine(TimerCoroutine());
        }

        protected virtual IEnumerator TimerCoroutine()
        {
            while (_timeLeft > 0f)
            {
                _timeLeft -= Time.deltaTime;
                yield return null;
            }

            _running = false;
            OnTimesUp?.Invoke();
        }

        protected virtual void OnEnable()
        {
            this.MMEventStartListening<MMRoomChallengeEvent>();
        }

        protected virtual void OnDisable()
        {
            this.MMEventStopListening<MMRoomChallengeEvent>();
        }
    }
}
