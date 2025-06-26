using UnityEngine;
using MoreMountains.Tools;

public class CompanionCommandMarkAction : CompanionCommand
{
    //public override string HelpBoxText() => "發出指令讓夥伴執行標記動作";

    protected override void HandleInput()
    {
        base.HandleInput();

        if (_inputManager.CompanionCommandMark.State.CurrentState == MMInput.ButtonStates.ButtonDown)
        {
            TriggerCommand();
        }
    }

    protected override void TriggerCommand()
    {
        base.TriggerCommand();
        Debug.Log("標記動作命令");
        MMCompanionCommandEvent.Trigger(_character, MMCompanionCommandEventTypes.CommandAction);
    }
}
