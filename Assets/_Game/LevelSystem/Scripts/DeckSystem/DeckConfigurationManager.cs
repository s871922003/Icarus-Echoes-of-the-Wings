using UnityEngine;
using System.Collections.Generic;

public class DeckConfigurationManager : MonoBehaviour
{
    public static DeckConfigurationManager Instance { get; private set; }

    [Header("��l Deck �t�m")]
    [Tooltip("�C���Ұʮɪ��w�]�ҪO�t�m")]
    public DeckConfigurationData DefaultConfiguration;

    public DeckConfigurationData CurrentConfiguration { get; private set; }

    private void Awake()
    {
        // ��ҼҦ�
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        // �ҰʮɮM�ιw�]�t�m
        CurrentConfiguration = Instantiate(DefaultConfiguration); // �ϥνƻs�A�קK������ʭ�l�귽
    }

    /// <summary>
    /// ���ե[�J�s�ҲաA�p�G�|���s�b�~�[�J
    /// </summary>
    public void AddModule(DeckModuleData newModule)
    {
        if (!CurrentConfiguration.Modules.Exists(m => m.ID == newModule.ID))
        {
            CurrentConfiguration.Modules.Add(newModule);
        }
    }

    /// <summary>
    /// �z�L ID �Ӳ����Y�ӼҲա]�i��\��^
    /// </summary>
    public void RemoveModuleByID(string id)
    {
        CurrentConfiguration.Modules.RemoveAll(m => m.ID == id);
    }
}
