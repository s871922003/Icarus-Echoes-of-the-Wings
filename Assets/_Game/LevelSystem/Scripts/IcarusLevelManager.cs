using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools;

public class IcarusLevelManager : MonoBehaviour
{
    [Header("���d���")]
    [SerializeField] private List<LevelData> levels;
    [SerializeField] private int currentIndex = 0;

    [Header("�D�Գ]�w")]
    [Tooltip("�]�w�D�Զ}�l�ɡA���a�n�Q�ǰe�쪺��m")]
    [SerializeField] private Transform challengeStartPosition;

    [Tooltip("�]�w�D�Է|�o�ͦb���� Room�A�H�K����۾��P���")]
    [SerializeField] private Room challengeRoom;

    [Header("����]�w")]
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
            Debug.LogWarning("[IcarusLevelManager] �L�k�ҰʬD�ԡG�䤣�쪱�a�Υ����w�_�l��m");
        }
    }

    private IEnumerator PerformRoomTransition(Character player, Vector3 targetPosition)
    {
        // ������v�����H�P�ᵲ����
        MMCameraEvent.Trigger(MMCameraEventTypes.StopFollowing);
        player.Freeze();

        // �H�X
        MMFadeInEvent.Trigger(fadeOutDuration, fadeTween, faderID, fadeIgnoresTimescale, player.transform.position);
        yield return new WaitForSecondsRealtime(fadeOutDuration);

        // ��X�ثe�ж��ðh�X
        Room currentRoom = FindCurrentRoomForPlayer();
        if (currentRoom != null)
        {
            currentRoom.PlayerExitsRoom();
            Debug.Log("�����}�ж�" + currentRoom.gameObject.name);
        }

        // �ǰe���a
        player.transform.position = targetPosition;

        // �i�J�D�ԩж�
        if (challengeRoom != null)
        {
            challengeRoom.PlayerEntersRoom();

            if (challengeRoom.VirtualCamera != null)
            {
                challengeRoom.VirtualCamera.Priority = 10;
                challengeRoom.VirtualCamera.enabled = true;
            }

            // ���ʾB�n
            MMSpriteMaskEvent.Trigger(
                MMSpriteMaskEvent.MMSpriteMaskEventTypes.MoveToNewPosition,
                challengeRoom.RoomColliderCenter,
                challengeRoom.RoomColliderSize,
                delayBetweenFades,
                MMTween.MMTweenCurve.EaseInCubic
            );
        }

        yield return new WaitForSecondsRealtime(delayBetweenFades);

        // �H�J
        MMFadeOutEvent.Trigger(fadeInDuration, fadeTween, faderID, fadeIgnoresTimescale, player.transform.position);
        yield return new WaitForSecondsRealtime(fadeInDuration);

        // ��_��v���P���ⱱ��
        MMCameraEvent.Trigger(MMCameraEventTypes.StartFollowing);
        player.UnFreeze();

        // ���� Teleporter ���֤ߦ欰
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
