using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;

[AddComponentMenu("TopDown Engine/Character/Abilities/Character Handle Companion")]
public class CharacterHandleCompanion : CharacterAbility
{
    public override string HelpBoxText() => "控制夥伴進入與退出戰鬥狀態";

    protected override void HandleInput()
    {
        if (_inputManager.CompanionActiveSwitch.State.CurrentState == MMInput.ButtonStates.ButtonDown)
        {
            MMCompanionCommandEvent.Trigger(_character, MMCompanionCommandEventTypes.SwitchActive);
        }
    }
}
