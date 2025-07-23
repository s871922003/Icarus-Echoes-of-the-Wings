using UnityEngine;
using MoreMountains.TopDownEngine;

public class CharacterHandleCompanion : MonoBehaviour
{
    [Header("Companion Settings")]
    [Tooltip("�٦񪺹w�] Prefab")]
    public GameObject companionPrefab;

    [Tooltip("�ͦ��٦�ɻP���a��������m")]
    public Vector3 companionSpawnOffset = new Vector3(1f, 0f, 0f);

    [Tooltip("�O�_�b Start �ɦ۰ʥͦ��õ��U�٦�")]
    public bool autoInitialize = true;

    // runtime reference
    protected Character _ownerCharacter;
    protected GameObject _companionInstance;
    protected CompanionAIContext _companionAIContext;

    protected virtual void Awake()
    {
        _ownerCharacter = GetComponent<Character>();
    }

    protected virtual void Start()
    {
        if (autoInitialize)
        {
            InitializeCompanion();
        }
    }

    /// <summary>
    /// �ͦ��õ��U�٦�
    /// </summary>
    public virtual void InitializeCompanion()
    {
        if (companionPrefab == null)
        {
            Debug.LogWarning("[CharacterHandleCompanion] Companion Prefab ���]�w�I");
            return;
        }

        if (_companionInstance != null)
        {
            Debug.Log("[CharacterHandleCompanion] �w�g��l�ƹL�٦�F�C");
            return;
        }

        // Instantiate at player position + offset
        Vector3 spawnPosition = transform.position + companionSpawnOffset;
        _companionInstance = Instantiate(companionPrefab, spawnPosition, Quaternion.identity);

        // ���ը��o CompanionAIContext �õ��U�D�q���Y
        _companionAIContext = _companionInstance.GetComponent<CompanionAIContext>();
        if (_companionAIContext == null)
        {
            Debug.LogError("[CharacterHandleCompanion] Companion Prefab �ʤ� CompanionAIContext �ե�I");
            return;
        }

        _companionAIContext.SetOwner(_ownerCharacter);

        // �Y�ݭn�A�i�H�����Ұʹ٦� AI �欰��
        // _companionAIContext.StartBehavior();

        Debug.Log("[CharacterHandleCompanion] �٦��l�ƻP���U�����I");
    }

    /// <summary>
    /// �~�����o�ثe���٦� GameObject
    /// </summary>
    public GameObject GetCurrentCompanion()
    {
        return _companionInstance;
    }

    /// <summary>
    /// �~�����o�٦� AIContext
    /// </summary>
    public CompanionAIContext GetCompanionAIContext()
    {
        return _companionAIContext;
    }
}
