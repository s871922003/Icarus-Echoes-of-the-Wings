using UnityEngine;
using MoreMountains.Tools;

public class RoomChallengeTrigger : MonoBehaviour
{
    [Tooltip("���ж����D������")]
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
