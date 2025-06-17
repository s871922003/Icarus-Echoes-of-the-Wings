using UnityEngine;

/// <summary>
/// 定義地城房間的類型，每種類型代表不同的挑戰體驗與功能。
/// </summary>
public enum RoomType
{
    /// <summary>
    /// 起始房間。進入每個樓層的第一間房，無挑戰，提供初步說明與分歧選擇。
    /// </summary>
    Start,

    /// <summary>
    /// 一般戰鬥房。打敗所有怪物即可通關，為主要的戰鬥體驗來源。
    /// </summary>
    Battle,

    /// <summary>
    /// 生存房。玩家需在倒數時間內生存下來，可能包含無限小怪或環境陷阱。
    /// </summary>
    Survive,

    /// <summary>
    /// 時間挑戰房。限定時間內完成特定任務，例如打怪或破壞物件。
    /// </summary>
    TimeAttack,

    /// <summary>
    /// 精英挑戰房。敵人數量較少但個體強大，具高風險高回報。
    /// </summary>
    Elite,

    /// <summary>
    /// 號令/夥伴機制專屬房。需要玩家使用 MarkAction 等技能與夥伴互動來解謎或擊敗敵人。
    /// </summary>
    Command,

    /// <summary>
    /// 劇情/基地推進事件房。可用來解鎖新功能、進入重要劇情、觸發對話等。
    /// </summary>
    Event,

    /// <summary>
    /// 回復房。提供恢復生命、能量、技能冷卻等資源的機會。
    /// </summary>
    Recover,

    /// <summary>
    /// 商店房。讓玩家使用局內資源購買強化、補給或解鎖升級。
    /// </summary>
    Shop,

    /// <summary>
    /// BOSS 房。樓層最終挑戰，戰鬥節奏與機制設計會顯著不同。
    /// </summary>
    Boss,

    /// <summary>
    /// 未知房型。進入前無法辨識內容，用於首次探索或隨機化體驗。
    /// </summary>
    Unknown
}
