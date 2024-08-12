using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMX.Common.Function.Enumerations
{
    /// <summary>
    /// 试验输出操作枚举
    /// </summary>
    public enum FOutPutStateType
    {
        /// <summary>
        /// 不操作
        /// </summary>
        [Description("不操作")]
        Null,
        /// <summary>
        /// 打开设备
        /// </summary>
        [Description("打开设备")]
        ON,
        /// <summary>
        /// 关闭设备
        /// </summary>
        [Description("关闭设备")]
        OFF
    }
}
