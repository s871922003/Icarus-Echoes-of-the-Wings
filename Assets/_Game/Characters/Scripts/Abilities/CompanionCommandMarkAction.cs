using UnityEngine;
using MoreMountains.Tools;

public class CompanionCommandMarkAction : CompanionCommand
{
    //public override string HelpBoxText() => "�o�X���O���٦����аO�ʧ@";

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
        Debug.Log("�аO�ʧ@�R�O");
        MMCompanionCommandEvent.Trigger(_character, MMCompanionCommandEventTypes.CommandAction);
    }
}
