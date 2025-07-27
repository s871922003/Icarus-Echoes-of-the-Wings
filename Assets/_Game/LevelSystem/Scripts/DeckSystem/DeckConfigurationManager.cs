using UnityEngine;
using System.Collections.Generic;

public class DeckConfigurationManager : MonoBehaviour
{
    public static DeckConfigurationManager Instance { get; private set; }

    [Header("初始 Deck 配置")]
    [Tooltip("遊戲啟動時的預設甲板配置")]
    public DeckConfigurationData DefaultConfiguration;

    public DeckConfigurationData CurrentConfiguration { get; private set; }

    private void Awake()
    {
        // 單例模式
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        // 啟動時套用預設配置
        CurrentConfiguration = Instantiate(DefaultConfiguration); // 使用複製，避免直接改動原始資源
    }

    /// <summary>
    /// 嘗試加入新模組，如果尚未存在才加入
    /// </summary>
    public void AddModule(DeckModuleData newModule)
    {
        if (!CurrentConfiguration.Modules.Exists(m => m.ID == newModule.ID))
        {
            CurrentConfiguration.Modules.Add(newModule);
        }
    }

    /// <summary>
    /// 透過 ID 來移除某個模組（可選功能）
    /// </summary>
    public void RemoveModuleByID(string id)
    {
        CurrentConfiguration.Modules.RemoveAll(m => m.ID == id);
    }
}
