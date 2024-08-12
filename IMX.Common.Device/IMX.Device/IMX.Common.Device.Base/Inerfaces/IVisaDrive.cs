using Ivi.Visa;
using Super.Zoo.Framework;

namespace Device.Base.Inerfaces
{
    public interface IVisaDrive: ISuperDrive
    {
        #region 公共属性
        /// <summary>
        /// 通讯串口
        /// </summary>
        IMessageBasedSession Session { get; }
        #endregion

        /// <summary>
        /// 驱动输入/输出缓存清除
        /// </summary>
        /// <returns></returns>
        OperateResult ClearBuffer();

        #region 写操作.Only
        /// <summary>
        /// 写数据_字符串
        /// </summary>
        /// <param name="writeValue"></param>
        OperateResult Write(string writeValue);

        /// <summary>
        /// 写数据_字节数组;
        /// </summary>
        /// <param name="writeValue"></param>
        /// <returns></returns>
        OperateResult Write_ByteArray(byte[] writeValue);
        #endregion

        #region 读操作.Only
        /// <summary>
        /// 读数据_返回符字符串
        /// </summary>
        /// <returns></returns>
        OperateResult<string> Read_String();

        /// <summary>
        /// 读数据_返回已去掉换行符字符串
        /// </summary>
        /// <returns></returns>
        OperateResult<string> Read_StringExisting();
        /// <summary>
        /// 读数据_返回字节数组;
        /// </summary>
        /// <param name="readCount">小于等于那就读取整行</param>
        /// <returns></returns>
        OperateResult<byte[]> Read_ByteArray(int readCount);
        #endregion

        #region 写完直接读操作

        /// <summary>
        /// 写入字符串 并 读取字符串结果值;
        /// </summary>
        /// <param name="writeValue">写入的命令字符串(字符串结尾没有换行符会自动补上)</param>
        /// <param name="delayMS">写入之后延时多少毫秒再进行读取(传-1表示使用本对象属性DelayMS的值, 传其他值则按入参值进行延迟读取)</param>
        /// <param name="bIsRead_StringExisting">读取字符串方式(默认false-使用Read_ByteArray(-1), true-使用Read_StringExisting)</param>
        /// <returns></returns>
        OperateResult<string> WriteRead_String(string writeValue, int delayMS = -1, bool bIsRead_StringExisting = false);

        /// <summary>
        /// 写入字符串 并 读取Byte结果值;
        /// </summary>
        /// <param name="writeValue">写入的命令字节</param>
        /// <param name="delayMS">写入之后延时多少毫秒再进行读取(传-1表示使用本对象属性DelayMS的值, 传其他值则按入参值进行延迟读取);</param>
        /// <returns></returns>
        OperateResult<byte[]> WriteRead_ByteArray(byte[] writeValue, int delayMS = -1);

        /// <summary>
        /// 写入字符串 并 读取Byte结果值;
        /// </summary>
        /// <param name="writeValue">写入的命令字节</param>
        /// <param name="Len">读取的字节数</param>
        /// <param name="delayMS">写入之后延时多少毫秒再进行读取(传-1表示使用本对象属性DelayMS的值, 传其他值则按入参值进行延迟读取);</param>
        /// <returns></returns>
        OperateResult<byte[]> WriteRead_ByteArray(byte[] writeValue, int Len, int delayMS = -1);

        #endregion
    }
}
