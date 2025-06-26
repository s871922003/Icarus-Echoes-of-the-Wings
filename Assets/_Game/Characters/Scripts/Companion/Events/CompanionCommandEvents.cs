using MoreMountains.Tools;
using MoreMountains.TopDownEngine;


public enum MMCompanionCommandEventTypes
{
    CommandAction,
}

public struct MMCompanionCommandEvent
{
    public Character EventInvokerCharacter;
    public MMCompanionCommandEventTypes EventType;

    public MMCompanionCommandEvent(Character character, MMCompanionCommandEventTypes eventType)
    {
        EventInvokerCharacter = character;
        EventType = eventType;
    }

    static MMCompanionCommandEvent e;
    public static void Trigger(Character character, MMCompanionCommandEventTypes eventType)
    {
        e.EventInvokerCharacter = character;
        e.EventType = eventType;
        MMEventManager.TriggerEvent(e);
    }
}