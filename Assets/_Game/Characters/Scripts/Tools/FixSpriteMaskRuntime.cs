using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer))]
public class FixSpriteMaskRuntime : MonoBehaviour
{
    [Header("Mask 修復設定")]
    [Tooltip("指定希望使用的 SpriteMaskInteraction 模式")]
    public SpriteMaskInteraction DesiredInteraction = SpriteMaskInteraction.VisibleInsideMask;

    [Tooltip("是否在 Start 時自動修復")]
    public bool FixOnStart = true;

    protected SpriteRenderer _spriteRenderer;

    protected virtual void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void Start()
    {
        if (FixOnStart)
        {
            ApplyFix();
        }
    }

    /// <summary>
    /// 設定正確的 maskInteraction 並強制刷新 Renderer
    /// </summary>
    public virtual void ApplyFix()
    {
        if (_spriteRenderer == null)
        {
            Debug.LogWarning("[FixSpriteMaskRuntime] 找不到 SpriteRenderer。");
            return;
        }

        _spriteRenderer.maskInteraction = DesiredInteraction;

        // 強制刷新一次以套用遮罩（這是關鍵）
        _spriteRenderer.enabled = false;
        _spriteRenderer.enabled = true;

        Debug.Log($"[FixSpriteMaskRuntime] 已修正 maskInteraction 為 {DesiredInteraction}", this);
    }
}
