using UnityEngine;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools;

public enum MMCompanionActionEventTypes
{
    OnMarkActionStart,
    OnMarkActionEnd,
}

public struct MMCompanionActionEvent
{
    public Character EventInvokerCharacter;
    public MMCompanionActionEventTypes EventType;

    public MMCompanionActionEvent(Character character, MMCompanionActionEventTypes eventType)
    {
        EventInvokerCharacter = character;
        EventType = eventType;
    }

    static MMCompanionActionEvent e;
    public static void Trigger(Character character, MMCompanionActionEventTypes eventType)
    {
        e.EventInvokerCharacter = character;
        e.EventType = eventType;
        MMEventManager.TriggerEvent(e);
    }
}
