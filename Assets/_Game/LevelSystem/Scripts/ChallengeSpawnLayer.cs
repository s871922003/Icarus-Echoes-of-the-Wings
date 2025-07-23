using UnityEngine;

public abstract class ChallengeSpawnLayer : MonoBehaviour
{
    [Header("�ͦ��h�ҥΪ��A")]
    [Tooltip("�O�_�ҥΦ��ͦ��h�C�Y�����A�h���|�b�D�Զ}�l�ɰ������ͦ��欰�C")]
    public bool LayerEnabled = true;

    /// <summary>
    /// �~���� ChallengeSpawnManager �I�s�A�N���h���ӱҰʥͦ��y�{�C
    /// </summary>
    public virtual void StartSpawning()
    {
        if (!LayerEnabled) return;
        OnStartSpawning();
    }

    /// <summary>
    /// �~���� ChallengeSpawnManager �I�s�A�N���h���Ӱ���ͦ��y�{�C
    /// </summary>
    public virtual void StopSpawning()
    {
        OnStopSpawning();
    }

    /// <summary>
    /// �~���� ChallengeSpawnManager �I�s�A�N���h���ӲM���ثe���W���Ҧ��ͦ�����C
    /// </summary>
    public virtual void ClearAllSpawned()
    {
        OnClearAllSpawned();
    }

    /// <summary>
    /// �l���O��@�G�B�z����Ұʦ欰�C
    /// </summary>
    protected abstract void OnStartSpawning();

    /// <summary>
    /// �l���O��@�G�B�z���鰱��欰�C
    /// </summary>
    protected abstract void OnStopSpawning();

    /// <summary>
    /// �l���O��@�G�B�z����M���欰�C
    /// </summary>
    protected abstract void OnClearAllSpawned();
}
