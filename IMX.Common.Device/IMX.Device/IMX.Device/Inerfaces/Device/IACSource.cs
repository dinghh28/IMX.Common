using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMX.Common.Device.Inerfaces
{
    /// <summary>
    /// 交流源操作接口
    /// </summary>
    public interface IACSource : IDeviceOperate
    {
        /// <summary>
        /// 设备开关操作
        /// </summary>
        /// <param name="value">00表示输入关闭，FF 表示输入打开。</param>
        /// <returns></returns>
        OperateResult SetOnOff(byte value);

        /// <summary>
        /// 参数设置,模式：0x021待机设置、0x050启机设置
        /// </summary>
        /// <param name="mode">工作模式</param>
        /// <param name="Udvalue">启动电压</param>
        /// <param name="Vdvalue">启动频率</param>
        /// <returns></returns>
        OperateResult Set_Value(byte mode, IList<(byte enble, double dvalue, double frevalue)> value);

    }
}
