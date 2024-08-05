using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMX.Common.Device.Inerfaces
{
    public interface IDCSource : IDeviceOperate
    {
        /// <summary>
        /// 设备参数设置
        /// </summary>
        /// <returns></returns>
        OperateResult Device_SetOnOff(int channel, string value);

        /// <summary>
        /// 设备参数设置
        /// </summary>
        /// <returns></returns>
        OperateResult Device_Set(double Currvalue, double minCurr, double maxCurr, double Volvalue, double minVol, double maxVol, double Powervalue, double maxpower, double OVPvalue, IList<(double Vvalue, double Ivalue)> value);
    }
}
