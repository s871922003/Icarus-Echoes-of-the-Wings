/// <summary>
/// ����Ʊ�b�����ഫ�ɫO�d�æۦ��_������i��@������
/// </summary>
public interface IPersistentObject
{
    /// �i�J�s�����e�A�Ȯ������γƥ����A
    void OnBeforeSceneUnload();

    /// �s�����}�l�ɡA��_���A�P���]��l���޿�
    void OnSceneLoadComplete();
}