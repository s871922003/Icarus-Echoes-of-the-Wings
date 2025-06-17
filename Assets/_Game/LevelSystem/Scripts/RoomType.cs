using UnityEngine;

/// <summary>
/// �w�q�a���ж��������A�C�������N���P���D������P�\��C
/// </summary>
public enum RoomType
{
    /// <summary>
    /// �_�l�ж��C�i�J�C�ӼӼh���Ĥ@���СA�L�D�ԡA���Ѫ�B�����P���[��ܡC
    /// </summary>
    Start,

    /// <summary>
    /// �@��԰��СC���ѩҦ��Ǫ��Y�i�q���A���D�n���԰�����ӷ��C
    /// </summary>
    Battle,

    /// <summary>
    /// �ͦs�СC���a�ݦb�˼Ʈɶ����ͦs�U�ӡA�i��]�t�L���p�ǩ����ҳ����C
    /// </summary>
    Survive,

    /// <summary>
    /// �ɶ��D�ԩСC���w�ɶ��������S�w���ȡA�Ҧp���ǩί}�a����C
    /// </summary>
    TimeAttack,

    /// <summary>
    /// ��^�D�ԩСC�ĤH�ƶq���֦�����j�j�A�㰪���I���^���C
    /// </summary>
    Elite,

    /// <summary>
    /// ���O/�٦����M�ݩСC�ݭn���a�ϥ� MarkAction ���ޯ�P�٦񤬰ʨӸ��������ѼĤH�C
    /// </summary>
    Command,

    /// <summary>
    /// �@��/��a���i�ƥ�СC�i�ΨӸ���s�\��B�i�J���n�@���BĲ�o��ܵ��C
    /// </summary>
    Event,

    /// <summary>
    /// �^�_�СC���ѫ�_�ͩR�B��q�B�ޯ�N�o���귽�����|�C
    /// </summary>
    Recover,

    /// <summary>
    /// �ө��СC�����a�ϥΧ����귽�ʶR�j�ơB�ɵ��θ���ɯšC
    /// </summary>
    Shop,

    /// <summary>
    /// BOSS �СC�Ӽh�̲׬D�ԡA�԰��`���P����]�p�|��ۤ��P�C
    /// </summary>
    Boss,

    /// <summary>
    /// �����Ы��C�i�J�e�L�k���Ѥ��e�A�Ω󭺦��������H��������C
    /// </summary>
    Unknown
}
