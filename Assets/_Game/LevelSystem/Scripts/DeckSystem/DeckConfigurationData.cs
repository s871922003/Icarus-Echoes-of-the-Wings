using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDeckConfiguration", menuName = "Game/Deck Configuration")]
public class DeckConfigurationData : ScriptableObject
{
    [Tooltip("�t�m�W�١A�i�Ω��x�s�ΰϤ�����")]
    public string ConfigurationName;

    [Tooltip("�]�t���Ҧ��ҪO�Ҳ�")]
    public List<DeckModuleData> Modules = new List<DeckModuleData>();
}
