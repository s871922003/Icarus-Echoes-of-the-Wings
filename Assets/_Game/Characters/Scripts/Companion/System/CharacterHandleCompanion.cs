using UnityEngine;
using MoreMountains.TopDownEngine;

public class CharacterHandleCompanion : MonoBehaviour
{
    [Header("Companion Settings")]
    [Tooltip("夥伴的預設 Prefab")]
    public GameObject companionPrefab;

    [Tooltip("生成夥伴時與玩家的偏移位置")]
    public Vector3 companionSpawnOffset = new Vector3(1f, 0f, 0f);

    [Tooltip("是否在 Start 時自動生成並註冊夥伴")]
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
    /// 生成並註冊夥伴
    /// </summary>
    public virtual void InitializeCompanion()
    {
        if (companionPrefab == null)
        {
            Debug.LogWarning("[CharacterHandleCompanion] Companion Prefab 未設定！");
            return;
        }

        if (_companionInstance != null)
        {
            Debug.Log("[CharacterHandleCompanion] 已經初始化過夥伴了。");
            return;
        }

        // Instantiate at player position + offset
        Vector3 spawnPosition = transform.position + companionSpawnOffset;
        _companionInstance = Instantiate(companionPrefab, spawnPosition, Quaternion.identity);

        // 嘗試取得 CompanionAIContext 並註冊主從關係
        _companionAIContext = _companionInstance.GetComponent<CompanionAIContext>();
        if (_companionAIContext == null)
        {
            Debug.LogError("[CharacterHandleCompanion] Companion Prefab 缺少 CompanionAIContext 組件！");
            return;
        }

        _companionAIContext.SetOwner(_ownerCharacter);

        // 若需要，可以直接啟動夥伴 AI 行為樹
        // _companionAIContext.StartBehavior();

        Debug.Log("[CharacterHandleCompanion] 夥伴初始化與註冊完成！");
    }

    /// <summary>
    /// 外部取得目前的夥伴 GameObject
    /// </summary>
    public GameObject GetCurrentCompanion()
    {
        return _companionInstance;
    }

    /// <summary>
    /// 外部取得夥伴 AIContext
    /// </summary>
    public CompanionAIContext GetCompanionAIContext()
    {
        return _companionAIContext;
    }
}
