namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// ���ѥi�����ܺ�Ǫ����]�w�����f�A�ѻݭn�վ�欰��Ĳ�o���]�p���h��V�P�O�ס^��@�ϥ�
    /// </summary>
    public interface IPrecisionVersion
    {
        /// <summary>
        /// �ҥΩ�������ǼҦ�
        /// </summary>
        /// <param name="usePrecision">true �h�ϥκ�ǳ]�w�Afalse �h�٭��l�]�w</param>
        void ActivePrecisionVersion(bool usePrecision);
    }
}
