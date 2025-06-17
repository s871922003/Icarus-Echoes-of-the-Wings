using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class CompanionMountSwapManager : MonoBehaviour
{
    public static void SwapControlToCompanion(Character player, Character companion)
    {
        if (player == null || companion == null) return;

        var playerSwap = player.FindAbility<CharacterSwap>();
        var companionSwap = companion.FindAbility<CharacterSwap>();

        if (playerSwap == null || companionSwap == null)
        {
            Debug.LogError("[Swap] 缺少 CharacterSwap 元件");
            return;
        }

        playerSwap.ResetCharacterSwap();
        companionSwap.SwapToThisCharacter();

        //LevelManager.Instance.Players[0] = companion;

        MMEventManager.TriggerEvent(new TopDownEngineEvent(TopDownEngineEventTypes.CharacterSwap, null));
        MMCameraEvent.Trigger(MMCameraEventTypes.StartFollowing);
        Debug.Log("控制權已轉移至夥伴");
    }

}
