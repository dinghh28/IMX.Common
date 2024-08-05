using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMX.Common.Device.Inerfaces
{
    public interface IWaterBox : IDeviceOperate
    {
        /// <summary>
        /// 设置所有目标参数
        /// </summary>
        /// <param name="values">所需设置参数</param>
        /// <see cref="values"/>
        /// <returns></returns>
        OperateResult Device_SetAll(IList<(double temp, double tempslope, double flow, double pressure)> values);
        /// <summary>
        /// 设置出液流量
        /// </summary>
        /// <param name="value">流量</param>
        /// <returns></returns>
        OperateResult Device_SetFlow(double value);

        /// <summary>
        /// 设置出液流量
        /// </summary>
        /// <param name="channel">回路</param>
        /// <param name="value">流量</param>
        /// <returns></returns>
        OperateResult Device_SetFlow(int channel, double value);

        /// <summary>
        /// 设置出液温度
        /// </summary>
        /// <param name="value">温度</param>
        /// <returns></returns>
        OperateResult Device_SetTemp(double value);

        /// <summary>
        /// 设置出液温度
        /// </summary>
        /// <param name="channel">回路</param>
        /// <param name="value">温度</param>
        /// <returns></returns>
        OperateResult Device_SetTemp(int channel, double value);

        /// <summary>
        /// 设置出液压力
        /// </summary>
        /// <param name="value">压力</param>
        /// <returns></returns>
        OperateResult Device_SetPressure(double value);

        /// <summary>
        /// 设置出液压力
        /// </summary>
        /// <param name="channel">回路</param>
        /// <param name="value">压力</param>
        /// <returns></returns>
        OperateResult Device_SetPressure(int channel, double value);

        /// <summary>
        /// 设备启停
        /// </summary>
        /// <param name="channel">回路</param>
        /// <param name="isOn">启/停</param>
        /// <returns></returns>
        OperateResult Device_SetOnOff(int channel, bool isOn);
    }
}
