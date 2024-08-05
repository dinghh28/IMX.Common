using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device.Common
{
    /// <summary>
    /// 设备类型
    /// </summary>
    public enum EDeviceType
    {
        /// <summary>
        /// 未知设备
        /// </summary>
        Unknow,
        /// <summary>
        /// 产品
        /// </summary>
        [Description("产品")]
        Product,
        /// <summary>
        /// AC交流源
        /// </summary>
        [Description("交流电源")]
        ACsource,
        /// <summary>
        /// 直流源
        /// </summary>
        [Description("直流电源")]
        DCSource,
        /// <summary>
        /// ACDC双向源
        /// </summary>
        [Description("双向源")]
        ACDCInverter,
        /// <summary>
        /// DCDC负载
        /// </summary>
        [Description("DCDC负载")]
        DCload,
        /// <summary>
        /// 水浴
        /// </summary>
        [Description("水浴设备")]
        WaterBox,
        /// <summary>
        /// 温箱
        /// </summary>
        [Description("温箱设备")]
        TemperatureBox,
        /// <summary>
        /// 直流稳压源
        /// </summary>
        [Description("直流稳压源")]
        APU,
        /// <summary>
        /// CC\CP调节器
        /// </summary>
        [Description("调节器")]
        Relay,
        /// <summary>
        /// 并联工装
        /// </summary>
        [Description("并联工装")]
        BLEquip,
        /// <summary>
        /// 电池模拟器
        /// </summary>
        [Description("电池模拟器")]
        BatterySimulator,
        /// <summary>
        /// 数字采集系统
        /// </summary>
        [Description("数字采集系统")]
        DAQSysteam,
        /// <summary>
        /// 旋变模拟器
        /// </summary>
        [Description("旋变模拟器")]
        Gyroscopic,
        /// <summary>
        /// 盐雾箱
        /// </summary>
        [Description("盐雾箱")]
        SaltFogbox,
        /// <summary>
        /// 振动台
        /// </summary>
        [Description("振动台")]
        VibrationTable,
    }
}
