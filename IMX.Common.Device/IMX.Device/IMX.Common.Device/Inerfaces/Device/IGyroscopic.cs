using Super.Zoo.Framework;

namespace IMX.Common.Device.Inerfaces
{
    /// <summary>
    /// 旋变器操作接口
    /// </summary>
    public interface IGyroscopic:IDeviceOperate
    {
        /// <summary>
        /// 设置转速
        /// </summary>
        /// <param name="channel">通道号</param>
        /// <param name="speedvalue">转速</param>
        /// <returns></returns>
        OperateResult Device_SetSpeed(int channel, int speedvalue);

        /// <summary>
        /// 设置极对数
        /// </summary>
        /// <param name="channel">通道号</param>
        /// <param name="value">极对数(偶数)</param>
        /// <returns></returns>
        OperateResult Device_SetPoleofPairs(int channel, int value);

        /// <summary>
        /// 设置电气角度
        /// </summary>
        /// <param name="channel">通道号</param>
        /// <param name="value">电气角度</param>
        /// <returns></returns>
        OperateResult Device_SetEAngle(int channel, int value);

        /// <summary>
        /// 设置温度模拟
        /// </summary>
        /// <param name="channel">通道号</param>
        /// <param name="Tempchannel">温度模拟序号</param>
        /// <param name="value">输出电压</param>
        /// <returns></returns>
        OperateResult Device_SetTemp(int channel, int Tempchannel, int value);
    }
}
