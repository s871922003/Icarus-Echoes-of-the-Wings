using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    /// <summary>
    /// 從 List 中隨機取得一個元素，若 List 為空會回傳預設值
    /// </summary>
    public static T PickRandom<T>(this List<T> list)
    {
        if (list == null || list.Count == 0)
        {
            Debug.LogWarning("PickRandom 呼叫時列表為空！");
            return default;
        }

        return list[Random.Range(0, list.Count)];
    }
}
