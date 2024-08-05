using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMX.Common.Device.Inerfaces
{
    public interface ITempBox : IDeviceOperate
    {
        /// <summary>
        /// 设备参数设置ON;OFF
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        OperateResult SetonOffAll(bool value);

        ///// <summary>
        ///// 设备参数设置
        ///// </summary>
        ///// <returns></returns>
        //OperateResult Device_SetTempandhumid(double temp, double humid);

        ///// <summary>
        ///// 设备参数设置
        ///// </summary>
        ///// <returns></returns>
        //OperateResult Device_SetTempTestInfo(bool ison, uint temprange, uint humidrange, uint waittime, uint tempslope, uint humidslope, uint mintue, uint hour);

        /// <summary>
        /// 设备参数设置
        /// </summary>
        /// <returns></returns>
        OperateResult Device_SetTemperature(double temp);
        /// <summary>
        /// 设备参数设置
        /// </summary>
        /// <returns></returns>
        OperateResult Device_SetTempSlope(uint temp);
        /// <summary>
        /// 设备参数设置
        /// </summary>
        /// <returns></returns>
        OperateResult Device_SetHumid(double value);

        /// <summary>
        /// 设备参数设置
        /// </summary>
        /// <returns></returns>
        OperateResult Device_SetHumidslope(uint value);

        /// <summary>
        /// 设备参数设置
        /// </summary>
        /// <returns></returns>
        OperateResult Device_SetTestHour(uint value);

        /// <summary>
        /// 设备参数设置
        /// </summary>
        /// <returns></returns>
        OperateResult Device_SetTestMin(uint value);


        /// <summary>
        /// 设备参数设置
        /// </summary>
        /// <returns></returns>
        OperateResult Device_SetWaitON(bool value);

        /// <summary>
        /// 设备参数设置
        /// </summary>
        /// <returns></returns>
        OperateResult Device_SetWaitTime(double value);

        /// <summary>
        /// 设备参数设置
        /// </summary>
        /// <returns></returns>
        OperateResult Device_SetTempRange(uint value);

        /// <summary>
        /// 设备参数设置
        /// </summary>
        /// <returns></returns>
        OperateResult Device_SetHumidRange(uint value);
    }
}
