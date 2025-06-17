using UnityEngine;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools;

/// <summary>
/// ���a�����M��/�Ѱ��M���C�|Ĳ�o CompanionAIContext �� ToggleMountRequest() ��k�C
/// </summary>
[AddComponentMenu("TopDown Engine/Character/Abilities/Companion/Companion Command Mount Toggle")]
public class CompanionCommandMountToggle : CompanionCommand
{
    //public override string HelpBoxText() =>
    //    "���U��J������٦�i�J�θѰ��M�����A�A�|�۰ʤ����������� pending flag�C";

    protected override void HandleInput()
    {
        base.HandleInput();

        // �u�n BindButton ���`�䴩 Input System �� performed/canceled�A�N�i�H�����o�˧P�_
        if (_inputManager.CompanionCommandMountToggle.State.CurrentState == MMInput.ButtonStates.ButtonDown)
        {
            TriggerCommand();
        }
    }


    protected override void TriggerCommand()
    {
        base.TriggerCommand();

        MMCompanionCommandEvent.Trigger(_character, MMCompanionCommandEventTypes.MountAndUnmountAction);

        Debug.Log("Mount Command");
    }
}
