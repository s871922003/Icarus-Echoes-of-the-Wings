using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    /// <summary>
    /// �q List ���H�����o�@�Ӥ����A�Y List ���ŷ|�^�ǹw�]��
    /// </summary>
    public static T PickRandom<T>(this List<T> list)
    {
        if (list == null || list.Count == 0)
        {
            Debug.LogWarning("PickRandom �I�s�ɦC���šI");
            return default;
        }

        return list[Random.Range(0, list.Count)];
    }
}
