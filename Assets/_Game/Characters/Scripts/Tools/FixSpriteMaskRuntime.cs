using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer))]
public class FixSpriteMaskRuntime : MonoBehaviour
{
    [Header("Mask �״_�]�w")]
    [Tooltip("���w�Ʊ�ϥΪ� SpriteMaskInteraction �Ҧ�")]
    public SpriteMaskInteraction DesiredInteraction = SpriteMaskInteraction.VisibleInsideMask;

    [Tooltip("�O�_�b Start �ɦ۰ʭ״_")]
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
    /// �]�w���T�� maskInteraction �ñj���s Renderer
    /// </summary>
    public virtual void ApplyFix()
    {
        if (_spriteRenderer == null)
        {
            Debug.LogWarning("[FixSpriteMaskRuntime] �䤣�� SpriteRenderer�C");
            return;
        }

        _spriteRenderer.maskInteraction = DesiredInteraction;

        // �j���s�@���H�M�ξB�n�]�o�O����^
        _spriteRenderer.enabled = false;
        _spriteRenderer.enabled = true;

        Debug.Log($"[FixSpriteMaskRuntime] �w�ץ� maskInteraction �� {DesiredInteraction}", this);
    }
}
