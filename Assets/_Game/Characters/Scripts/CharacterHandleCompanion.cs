using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;

[AddComponentMenu("TopDown Engine/Character/Abilities/Character Handle Companion")]
public class CharacterHandleCompanion : CharacterAbility
{
    public override string HelpBoxText() => "����٦�i�J�P�h�X�԰����A";

    protected override void HandleInput()
    {
        if (_inputManager.CompanionActiveSwitch.State.CurrentState == MMInput.ButtonStates.ButtonDown)
        {
            MMCompanionCommandEvent.Trigger(_character, MMCompanionCommandEventTypes.SwitchActive);
        }
    }
}
