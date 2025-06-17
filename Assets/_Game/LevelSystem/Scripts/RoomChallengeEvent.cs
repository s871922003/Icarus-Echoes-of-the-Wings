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
    public RoomType RoomType;
    public RoomChallengeTrigger Source;

    public MMRoomChallengeEvent(MMRoomChallengeEventType eventType, RoomType roomType, RoomChallengeTrigger source)
    {
        EventType = eventType;
        RoomType = roomType;
        Source = source;
    }

    public static void Trigger(MMRoomChallengeEventType eventType, RoomType roomType, RoomChallengeTrigger source)
    {
        MMEventManager.TriggerEvent(new MMRoomChallengeEvent(eventType, roomType, source));
    }
}
