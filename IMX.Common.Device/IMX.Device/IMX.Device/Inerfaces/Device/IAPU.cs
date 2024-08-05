using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMX.Common.Device.Inerfaces
{
  public interface IAPU : IDeviceOperate
    {
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        OperateResult Device_Set(IList<(double Vvalue, double Ivalue)> value);

        /// <summary>
        /// 开关操作
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        OperateResult Device_SetOnOff(int channel, string value);

        /// <summary>
        /// 设置挡位
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        OperateResult Device_SetRange(string value);

        /// <summary>
        /// 设置测试功能
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        OperateResult Device_LoadMode(int mode);

        /// <summary>
        /// 设置过压保护
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        OperateResult Device_OVPVolValue(float value);

        //OperateResult Device_LoadValue(float OutVol, float OutCurr, float VolSlope, float CurrSlope, float VolMax, float VolMin, float CurrMax, float CurrMin);


    }
}
