using UnityEngine;

public abstract class CommandAction : ScriptableObject
{
    public string ActionName;
    public Sprite ActionIcon;
    public CompanionAIContext AIContext;
    public bool IsMovementBasedCommand = false;

    public abstract bool CanExecute();
    public abstract void Execute();

    public abstract void UpdateCooldown(float deltaTime); // 給非時間型冷卻使用
}
