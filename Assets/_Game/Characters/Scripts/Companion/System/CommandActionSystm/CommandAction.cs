using UnityEngine;

public abstract class CommandAction : ScriptableObject
{
    public string ActionName;
    public Sprite ActionIcon;
    public CompanionAIContext AIContext;

    public abstract bool CanExecute();
    public abstract void Execute();

    public abstract void UpdateCooldown(float deltaTime); // ���D�ɶ����N�o�ϥ�
}
