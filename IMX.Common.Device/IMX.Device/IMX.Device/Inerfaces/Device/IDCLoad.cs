
using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMX.Common.Device.Inerfaces
{
    public interface IDCLoad : IDeviceOperate
    {

        /// <summary>
        /// 设备开关操作
        /// </summary>
        /// <remarks>
        ///     <description>【Bit标识】</description>
        ///     <list type="bullet">
        ///         <item>
        ///             <term>[Bit0]</term>
        ///             <description>通道 1 负载输入开关</description>
        ///         </item>
        ///          <item>
        ///             <term>[Bit1]</term>
        ///             <description>通道 2 负载输入开关</description>
        ///         </item>
        ///         <item>
        ///             <term>[Bit2]</term>
        ///             <description>通道 3 负载输入开关</description>
        ///         </item>
        ///         <item>
        ///             <term>[Bit3]</term>
        ///             <description>通道 4 负载输入开关</description>
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <param name="value">1 表示输入打开，0 表示输入关闭。 00全闭，0F全开</param>
        /// <returns></returns>
        OperateResult Device_SetOnOff(bool value);

        /// <summary>
        /// 设置所有通道--单寄存器设置
        /// </summary>
        /// <returns></returns>
        OperateResult Device_SetAll(IList<(byte mode, double dvalue, double limvalue)> value);
    }
}
