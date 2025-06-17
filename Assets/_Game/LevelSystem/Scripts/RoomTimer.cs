using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;

namespace Game.Rooms
{
    /// <summary>
    /// 正式用於 TimeAttack / Survive 等房間挑戰的倒數計時器。
    /// 需由 RoomEventBroadcaster 啟動，倒數結束後會觸發 OnTimesUp。
    /// </summary>
    public class RoomTimer : MonoBehaviour, MMEventListener<MMRoomChallengeEvent>
    {
        [Header("Timer Settings")]
        [Tooltip("倒數計時秒數")]
        public float CountdownDuration = 20f;

        [Tooltip("是否在開始時自動啟動倒數 (建議關閉，由事件控制)")]
        public bool AutoStart = false;

        [Header("Events")]
        [Tooltip("倒數時間結束時觸發的事件")]
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
