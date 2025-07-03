using UnityEngine;
using MoreMountains.Tools;

public class RoomChallengeTrigger : MonoBehaviour
{
    public void StartChallenge()
    {
        MMRoomChallengeEvent.Trigger(MMRoomChallengeEventType.ChallengeStart, this);
    }

    public void CompleteChallenge()
    {
        MMRoomChallengeEvent.Trigger(MMRoomChallengeEventType.ChallengeComplete, this);
    }
}
