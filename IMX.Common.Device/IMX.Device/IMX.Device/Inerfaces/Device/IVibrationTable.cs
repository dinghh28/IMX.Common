using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMX.Common.Device.Inerfaces
{
    public interface IVibrationTable : IDeviceOperate
    {
        /// <summary>
        /// 启/停
        /// </summary>
        /// <param name="isOn"></param>
        /// <returns></returns>
        OperateResult Device_SetOnOff(bool isOn);
    }
}
