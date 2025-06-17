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

    private CompanionAIContext _context;

    protected override void Awake()
    {
        base.Awake();
        _context = this.gameObject.GetComponent<CompanionAIContext>();
    }

    public override void OnBeforeSceneUnload()
    {
        if (_context != null && _context.CurrentBehaviorState == CompanionAIContext.CompanionBehaviorState.Mounted)
        {
            Debug.Log("[CompanionPersistenceHandler] �˴��쥿�b�M���A����۰ʤU��");

            // ����U��
            var player = _context.OwnerCharacter;
            var companion = _character;

            if (player != null && companion != null)
            {
                var playerTransform = player.transform;
                playerTransform.SetParent(null);

                // �����v���^���a
                CompanionMountSwapManager.SwapControlToCompanion(companion, player);

                // ��_���a���
                var controller = player.GetComponent<TopDownController2D>();
                if (controller != null)
                {
                    controller.SetKinematic(false);
                    controller.enabled = true;
                }

                // �o�X�U�������ƥ�
                MMCompanionActionEvent.Trigger(companion, MMCompanionActionEventTypes.OnGetOffMountComplete);
            }
        }

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
