namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// 提供可切換至精準版本設定的接口，供需要調整行為的觸發器（如擊退方向與力度）實作使用
    /// </summary>
    public interface IPrecisionVersion
    {
        /// <summary>
        /// 啟用或關閉精準模式
        /// </summary>
        /// <param name="usePrecision">true 則使用精準設定，false 則還原原始設定</param>
        void ActivePrecisionVersion(bool usePrecision);
    }
}
