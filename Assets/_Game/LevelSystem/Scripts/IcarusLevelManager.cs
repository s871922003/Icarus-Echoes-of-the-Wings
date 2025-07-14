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

    [Header("挑戰設定")]
    [Tooltip("設定挑戰開始時，玩家要被傳送到的位置")]
    [SerializeField] private Transform challengeStartPosition;

    [Tooltip("設定挑戰會發生在哪個 Room，以便控制相機與轉場")]
    [SerializeField] private Room challengeRoom;

    [Header("轉場設定")]
    [SerializeField] private int faderID = 0;
    [SerializeField] private float fadeOutDuration = 0.2f;
    [SerializeField] private float delayBetweenFades = 0.3f;
    [SerializeField] private float fadeInDuration = 0.2f;
    [SerializeField] private MMTweenType fadeTween = new MMTweenType(MMTween.MMTweenCurve.EaseInCubic);
    [SerializeField] private bool fadeIgnoresTimescale = false;

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

    public void StartChallenge()
    {
        if (ChallengeStarted) return;

        ChallengeStarted = true;
        OnChallengeStarted?.Invoke();

        Character player = LevelManager.Instance?.Players?[0];
        if (player != null && challengeStartPosition != null)
        {
            StartCoroutine(PerformRoomTransition(player, challengeStartPosition.position));
        }
        else
        {
            Debug.LogWarning("[IcarusLevelManager] 無法啟動挑戰：找不到玩家或未指定起始位置");
        }
    }

    private IEnumerator PerformRoomTransition(Character player, Vector3 targetPosition)
    {
        // 停止攝影機跟隨與凍結角色
        MMCameraEvent.Trigger(MMCameraEventTypes.StopFollowing);
        player.Freeze();

        // 淡出
        MMFadeInEvent.Trigger(fadeOutDuration, fadeTween, faderID, fadeIgnoresTimescale, player.transform.position);
        yield return new WaitForSecondsRealtime(fadeOutDuration);

        // 找出目前房間並退出
        Room currentRoom = FindCurrentRoomForPlayer();
        if (currentRoom != null)
        {
            currentRoom.PlayerExitsRoom();
            Debug.Log("有離開房間" + currentRoom.gameObject.name);
        }

        // 傳送玩家
        player.transform.position = targetPosition;

        // 進入挑戰房間
        if (challengeRoom != null)
        {
            challengeRoom.PlayerEntersRoom();

            if (challengeRoom.VirtualCamera != null)
            {
                challengeRoom.VirtualCamera.Priority = 10;
                challengeRoom.VirtualCamera.enabled = true;
            }

            // 移動遮罩
            MMSpriteMaskEvent.Trigger(
                MMSpriteMaskEvent.MMSpriteMaskEventTypes.MoveToNewPosition,
                challengeRoom.RoomColliderCenter,
                challengeRoom.RoomColliderSize,
                delayBetweenFades,
                MMTween.MMTweenCurve.EaseInCubic
            );
        }

        yield return new WaitForSecondsRealtime(delayBetweenFades);

        // 淡入
        MMFadeOutEvent.Trigger(fadeInDuration, fadeTween, faderID, fadeIgnoresTimescale, player.transform.position);
        yield return new WaitForSecondsRealtime(fadeInDuration);

        // 恢復攝影機與角色控制
        MMCameraEvent.Trigger(MMCameraEventTypes.StartFollowing);
        player.UnFreeze();

        // 模擬 Teleporter 的核心行為
        TopDownEngineEvent.Trigger(TopDownEngineEventTypes.LevelStart, player) ;
    }

    public Room FindCurrentRoomForPlayer()
    {
        if (!LevelManager.HasInstance || LevelManager.Instance.Players.Count == 0)
        {
            return null;
        }

        Vector3 playerPosition = LevelManager.Instance.Players[0].transform.position;
        return RoomUtility.FindRoomContaining(playerPosition);
    }
}
