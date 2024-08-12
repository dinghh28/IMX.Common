using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device.Common.Models
{
    /// <summary>
    /// 电气性能信息类
    /// </summary>
    public class ModElectricalInfo
    {
        /// <summary>
        /// 输入电压
        /// </summary>
        public double InVol { get; set; } = 0;

        /// <summary>
        /// 输出电压
        /// </summary>
        public double OutVol { get; set; } = 0;

        /// <summary>
        /// 输入电流
        /// </summary>
        public double InCurr { get; set; } = 0;

        /// <summary>
        /// 输出电流
        /// </summary>
        public double OutCurr { get; set; } = 0;

        /// <summary>
        /// 输入功率
        /// </summary>
        public double InPow { get; set;} = 0;

        /// <summary>
        /// 输出功率
        /// </summary>
        public double OutPow { get; set; } = 0;

    }
}
