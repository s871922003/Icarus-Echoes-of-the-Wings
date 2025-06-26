using UnityEngine;
using MoreMountains.TopDownEngine;

/// <summary>
/// �X�R�� DamageOnTouch�A�[�J�䴩������h����������
/// </summary>
public class PrecisionDamageOnTouch : DamageOnTouch, IPrecisionVersion
{
    [Header("Precision Settings")]
    [Tooltip("�ҥκ�ǼҦ��ɨϥΪ� KnockbackForce")]
    public Vector3 PrecisionKnockbackForce = new Vector3(20f, 20f, 0f);

    [Tooltip("�ҥκ�ǼҦ��ɨϥΪ� KnockbackDirection")]
    public KnockbackDirections PrecisionKnockbackDirection = KnockbackDirections.BasedOnScriptDirection;

    [Tooltip("�O�_�w�]���ҥκ�Ǫ���")]
    public bool StartWithPrecisionActive = false;

    // �ƥ���l��
    protected Vector3 _originalKnockbackForce;
    protected KnockbackDirections _originalKnockbackDirection;

    /// <summary>
    /// ��l�ƨîM�Ϊ�l���A
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        _originalKnockbackForce = DamageCausedKnockbackForce;
        _originalKnockbackDirection = DamageCausedKnockbackDirection;

        ActivePrecisionVersion(StartWithPrecisionActive);
    }

    /// <summary>
    /// �M�Ω��٭��ǳ]�w
    /// </summary>
    /// <param name="usePrecision">�O�_�ҥκ�Ǫ���</param>
    public void ActivePrecisionVersion(bool usePrecision)
    {
        if (usePrecision)
        {
            DamageCausedKnockbackForce = PrecisionKnockbackForce;
            DamageCausedKnockbackDirection = PrecisionKnockbackDirection;
        }
        else
        {
            DamageCausedKnockbackForce = _originalKnockbackForce;
            DamageCausedKnockbackDirection = _originalKnockbackDirection;
        }
    }
}
