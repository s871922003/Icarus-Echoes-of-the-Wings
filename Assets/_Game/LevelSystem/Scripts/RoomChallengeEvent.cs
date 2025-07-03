using MoreMountains.Tools;
using MoreMountains.TopDownEngine;

public enum MMRoomChallengeEventType
{
    ChallengeStart,
    ChallengeAbort,
    ChallengeComplete
}

public struct MMRoomChallengeEvent
{
    public MMRoomChallengeEventType EventType;
    public RoomChallengeTrigger Source;

    public MMRoomChallengeEvent(MMRoomChallengeEventType eventType, RoomChallengeTrigger source)
    {
        EventType = eventType;
        Source = source;
    }

    public static void Trigger(MMRoomChallengeEventType eventType, RoomChallengeTrigger source)
    {
        MMEventManager.TriggerEvent(new MMRoomChallengeEvent(eventType, source));
    }
}
