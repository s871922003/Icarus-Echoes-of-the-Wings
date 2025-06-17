/// <summary>
/// 任何希望在場景轉換時保留並自行恢復的物件可實作此介面
/// </summary>
public interface IPersistentObject
{
    /// 進入新場景前，暫時關閉或備份狀態
    void OnBeforeSceneUnload();

    /// 新場景開始時，恢復狀態與重設初始化邏輯
    void OnSceneLoadComplete();
}