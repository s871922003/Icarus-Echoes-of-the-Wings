using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDeckConfiguration", menuName = "Game/Deck Configuration")]
public class DeckConfigurationData : ScriptableObject
{
    [Tooltip("配置名稱，可用於儲存或區分版本")]
    public string ConfigurationName;

    [Tooltip("包含的所有甲板模組")]
    public List<DeckModuleData> Modules = new List<DeckModuleData>();
}
