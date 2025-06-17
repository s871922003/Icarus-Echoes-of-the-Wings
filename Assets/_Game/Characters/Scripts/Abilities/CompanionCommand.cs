using UnityEngine;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools;
using System;

public class CompanionCommand : MonoBehaviour, MMEventListener<TopDownEngineEvent>
{
    protected Character _character;
    protected InputManager _inputManager;


    protected virtual void OnEnable()
    {
        this.MMEventStartListening<TopDownEngineEvent>();
    }

    protected virtual void OnDestroy()
    {
        this.MMEventStopListening<TopDownEngineEvent>();
    }

    private void Update()
    {
        HandleInput();
    }

    protected virtual void HandleInput()
    {
        
    }

    protected virtual void TriggerCommand()
    {
        
    }

    public void OnMMEvent(TopDownEngineEvent engineEvent)
    {
        switch (engineEvent.EventType)
        {
            case TopDownEngineEventTypes.LevelStart:
                OnSceneLoadComplete();
                break;
        }
    }

    private void OnSceneLoadComplete()
    {
        _character = this.gameObject.GetComponentInParent<Character>();
        _inputManager = _character.LinkedInputManager;
    }
}
