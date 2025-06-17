using UnityEngine;
using MoreMountains.TopDownEngine;

/// <summary>
/// �٦񨤦⪺������s��B�z���A�|�b����e�۰ʤU���]�p�G���b�M���^
/// </summary>
public class CompanionPersistenceHandler : CharacterPersistenceHandler
{

    [Header("Companion Offset Settings")]
    [Tooltip("�i���ɻP���a��l�I�����������q")]
    public Vector3 Offset = new Vector3(1f, 0f, 0f);

    protected override void Awake()
    {
        base.Awake();
    }

    public override void OnBeforeSceneUnload()
    {     
        base.OnBeforeSceneUnload();
    }

    public override void OnSceneLoadComplete()
    {
        // �]�w���ʦ�m�]��� InitialSpawnPoint + offset�^
        if (LevelManager.HasInstance && LevelManager.Instance.InitialSpawnPoint != null)
        {
            this.transform.position = LevelManager.Instance.InitialSpawnPoint.transform.position + Offset;
        }

        base.OnSceneLoadComplete();
    }
}
