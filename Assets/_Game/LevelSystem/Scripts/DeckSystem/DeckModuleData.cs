using UnityEngine;

[System.Serializable]
public class DeckModuleData
{
    [Tooltip("�Ҳ� ID�A�ΨӧP�_�O�_���굥")]
    public string ID;

    [Tooltip("�Ҳզb�ҪO�W���y�Ц�m�]�ϥΥ��a Grid ���^")]
    public Vector2Int GridPosition;

    [Tooltip("�n�ͦ������� Prefab")]
    public GameObject Prefab;
}
