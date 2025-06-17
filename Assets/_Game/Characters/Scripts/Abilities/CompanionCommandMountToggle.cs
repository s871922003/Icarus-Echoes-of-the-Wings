using UnityEngine;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools;

/// <summary>
/// 玩家控制騎乘/解除騎乘。會觸發 CompanionAIContext 的 ToggleMountRequest() 方法。
/// </summary>
[AddComponentMenu("TopDown Engine/Character/Abilities/Companion/Companion Command Mount Toggle")]
public class CompanionCommandMountToggle : CompanionCommand
{
    //public override string HelpBoxText() =>
    //    "按下輸入鍵來讓夥伴進入或解除騎乘狀態，會自動切換成對應的 pending flag。";

    protected override void HandleInput()
    {
        base.HandleInput();

        // 只要 BindButton 正常支援 Input System 的 performed/canceled，就可以直接這樣判斷
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
