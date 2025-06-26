using UnityEngine;
using MoreMountains.TopDownEngine;

/// <summary>
/// 擴充自 DamageOnTouch，加入支援精準擊退版本的控制
/// </summary>
public class PrecisionDamageOnTouch : DamageOnTouch, IPrecisionVersion
{
    [Header("Precision Settings")]
    [Tooltip("啟用精準模式時使用的 KnockbackForce")]
    public Vector3 PrecisionKnockbackForce = new Vector3(20f, 20f, 0f);

    [Tooltip("啟用精準模式時使用的 KnockbackDirection")]
    public KnockbackDirections PrecisionKnockbackDirection = KnockbackDirections.BasedOnScriptDirection;

    [Tooltip("是否預設為啟用精準版本")]
    public bool StartWithPrecisionActive = false;

    // 備份原始值
    protected Vector3 _originalKnockbackForce;
    protected KnockbackDirections _originalKnockbackDirection;

    /// <summary>
    /// 初始化並套用初始狀態
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        _originalKnockbackForce = DamageCausedKnockbackForce;
        _originalKnockbackDirection = DamageCausedKnockbackDirection;

        ActivePrecisionVersion(StartWithPrecisionActive);
    }

    /// <summary>
    /// 套用或還原精準設定
    /// </summary>
    /// <param name="usePrecision">是否啟用精準版本</param>
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
