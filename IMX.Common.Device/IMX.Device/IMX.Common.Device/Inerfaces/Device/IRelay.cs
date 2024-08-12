using Device.Common;
using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMX.Common.Device.Inerfaces
{
    public interface IRelay : IDeviceOperate
    {
        /// <summary>
        /// 设备参数设置
        /// </summary>
        /// <returns></returns>
        OperateResult Device_Set(string value);

        /// </summary>
        /// <returns></returns>
        OperateResult Device_SetAll(System.Collections.Generic.List<EDeviceOutPutState> Values);
    }
}
