using UnityEngine;

[System.Serializable]
public class DeckModuleData
{
    [Tooltip("模組 ID，用來判斷是否解鎖等")]
    public string ID;

    [Tooltip("模組在甲板上的座標位置（使用本地 Grid 單位）")]
    public Vector2Int GridPosition;

    [Tooltip("要生成的實體 Prefab")]
    public GameObject Prefab;
}
