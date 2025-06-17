using UnityEngine;
using MoreMountains.Tools;

public class RoomChallengeTrigger : MonoBehaviour
{
    [Tooltip("此房間的挑戰類型")]
    public RoomType RoomType;

    public void StartChallenge()
    {
        MMRoomChallengeEvent.Trigger(MMRoomChallengeEventType.ChallengeStart, RoomType, this);
    }

    public void CompleteChallenge()
    {
        MMRoomChallengeEvent.Trigger(MMRoomChallengeEventType.ChallengeComplete, RoomType, this);
    }
}
