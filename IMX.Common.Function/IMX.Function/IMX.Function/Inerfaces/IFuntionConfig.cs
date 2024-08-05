using IMX.Common.Function.Enumerations;
using Newtonsoft.Json;
using Super.Zoo.Framework;
using Super.Zoo.Framework.Logger;
using System;
using System.Collections.Generic;

namespace IMX.Common.Function
{
    /// <summary>
    /// 试验方案配置接口
    /// </summary>
    public interface IFuntionConfig :IIdentifiedDisposableLoggerObject
    {
        /// <summary>
        /// 设备名称/地址
        /// </summary>
        string DeviceAddress { get; set; }

        /// <summary>
        /// 设备开关操作
        /// </summary>
        FOutPutStateType OperateType { get; set; }

        /// <summary>
        /// 方案运行后延时时间
        /// </summary>
        int DelayAfterRun { get; set; }

        ///// <summary>
        ///// 设备操作类
        ///// </summary>
        //[JsonIgnore]
        //IDeviceOperate Operate { get; set; }

        /// <summary>
        /// 试验运行状态
        /// </summary>
        [JsonIgnore]
        List<bool> IsTestRunning { get; set; }

        /// <summary>
        /// 支持的试验方法类型
        /// </summary>
        [JsonIgnore]
        FuncitonType SupportFuncitonType { get; }

        /// <summary>
        /// 试验执行方法
        /// </summary>
        /// <returns></returns>
        OperateResult Execute(object operate);

        /// <summary>
        /// Json字符串转换
        /// </summary>
        /// <returns></returns>
        OperateResult<string> ToJson();
    }
}
