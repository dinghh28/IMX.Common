using Device;
using IMX.Common.Device.Inerfaces;
using IMX.Common.Function.Enumerations;
using Newtonsoft.Json;
using Super.Zoo.Framework;
using Super.Zoo.Framework.Logger;
using System;
using System.Threading;

namespace IMX.Common.Function
{
    /// <summary>
    /// 温箱试验配置类
    /// </summary>
    public class FunConfig_TempBox : Function_Config
    {
        #region 公共属性
        [JsonIgnore]
        public override Guid Identify { get; } = new Guid();

        //[JsonIgnore]
        //public override IDeviceOperate Operate { get; set; }

        [JsonIgnore]
        public override FuncitonType SupportFuncitonType => FuncitonType.TEMPBOXFUNC;

        public override string DeviceAddress { get; set; } = "TemperatureBox_0";

        /// <summary>
        /// 设备开关操作
        /// </summary>
        public override FOutPutStateType OperateType { get; set; } = FOutPutStateType.Null;

        #region 试验参数
        /// <summary>
        /// 设置试验温度值
        /// </summary>
        public double Set_Temperature { get; set; }
        /// <summary>
        /// 设置温度斜率值
        /// </summary>
        public uint Set_TempSlope { get; set; }
        /// <summary>
        /// 设置试验湿度值
        /// </summary>
        public double Set_Humidity { get; set; }

        /// <summary>
        /// 设置湿度斜率
        /// </summary>
        public uint Set_HumidSlope { get; set; }
        /// <summary>
        /// 设置试验时间--时
        /// </summary>
        public uint Set_TestHour { get; set; }
        /// <summary>
        /// 设置试验时间--分
        /// </summary>
        public uint Set_TestMin { get; set; }

        public bool Set_WaitOn { get; set; }

        public uint Set_TempRange { get; set; }

        public uint Set_HumidRange { get; set; }
        public uint Set_WaitTime { get; set; }
        #endregion

        #endregion

        #region 构造方法
        public FunConfig_TempBox() : base()
        {
        }

        //public FunConfig_TempBox(IDeviceOperate operate) : base(null, operate)
        //{
        //}


        public FunConfig_TempBox(ILogger logger) : base(logger)
        {
        }
        #endregion

        #region 析构方法
        ~FunConfig_TempBox()
        {
            Dispose();
        }
        #endregion

        public override OperateResult Execute(object device) 
        {
            try
            {
                if (device == null)
                {
                    LastError = $"设备类型不存】";
                    Logger.Error(nameof(FunConfig_TempBox), nameof(Execute), LastError);
                    return OperateResult.Failed(LastError);
                }
                if (!(device is ITempBox operate))
                {
                    LastError = $"设备类型异常：【{device.GetType()}】";
                    Logger.Error(nameof(FunConfig_TempBox), nameof(Execute), LastError);
                    return OperateResult.Failed(LastError);
                }
                string InfoString = string.Empty;
                #region 多个寄存器设置
                //OperateResult SetRtl = operate.Device_SetTempandhumid(Set_Temperature, Set_Humidity);

                //if (!SetRtl)
                //{
                //    LastError = $"温箱设备设置定值湿度失败：{SetRtl.Message}";
                //    Logger.Error(nameof(FunConfig_TempBox), nameof(Execute), LastError);
                //    return OperateResult.Failed(LastError);
                //}
                //SetRtl = operate.Device_SetTempTestInfo(Set_WaitOn, Set_TempRange, Set_HumidRange, Set_WaitTime, Set_TempSlope, Set_HumidSlope, Set_TestMin, Set_TestHour);

                #endregion
                #region 单个寄存器设置
                OperateResult SetRtl = operate.Device_SetTempSlope(Set_TempSlope);
                if (!SetRtl)
                {
                    LastError = $"温箱设备设置定值温度斜率失败：{SetRtl.Message}";
                    Logger.Error(nameof(FunConfig_TempBox), nameof(Execute), LastError);
                    return OperateResult.Failed(LastError);
                }
                SetRtl = operate.Device_SetTemperature(Set_Temperature);
                if (!SetRtl)
                {
                    LastError = $"温箱设备设置定值温度失败：{SetRtl.Message}";
                    Logger.Error(nameof(FunConfig_TempBox), nameof(Execute), LastError);
                    return OperateResult.Failed(LastError);
                }
                SetRtl = operate.Device_SetHumid(Set_Humidity);
                if (!SetRtl)
                {
                    LastError = $"温箱设备设置定值湿度失败：{SetRtl.Message}";
                    Logger.Error(nameof(FunConfig_TempBox), nameof(Execute), LastError);
                    return OperateResult.Failed(LastError);
                }
                SetRtl = operate.Device_SetHumidslope(Set_HumidSlope);
                if (!SetRtl)
                {
                    LastError = $"温箱设备设置定值湿度斜率失败：{SetRtl.Message}";
                    Logger.Error(nameof(FunConfig_TempBox), nameof(Execute), LastError);
                    return OperateResult.Failed(LastError);
                }

                #region 在不发送指令的情况下直接启动运行，定时操作时间默认为0，等待设定默认为不使用
                //SetRtl = operate.Device_SetTestHour(Set_TestHour);
                //if (!SetRtl)
                //{
                //    LastError = $"温箱设备设置定值试验时间（时）失败：{SetRtl.Message}";
                //    Logger.Error(nameof(FunConfig_TempBox), nameof(Execute), LastError);
                //    return OperateResult.Failed(LastError);
                //}
                //SetRtl = operate.Device_SetTestMin(Set_TestMin);
                //if (!SetRtl)
                //{
                //    LastError = $"温箱设备设置试验时间（分）失败：{SetRtl.Message}";
                //    Logger.Error(nameof(FunConfig_TempBox), nameof(Execute), LastError);
                //    return OperateResult.Failed(LastError);
                //}
                //SetRtl = operate.Device_SetWaitON(Set_WaitOn);
                //if (!SetRtl)
                //{
                //    LastError = $"温箱设备定值等待启动失败：{SetRtl.Message}";
                //    Logger.Error(nameof(FunConfig_TempBox), nameof(Execute), LastError);
                //    return OperateResult.Failed(LastError);
                //}
                //SetRtl = operate.Device_SetTempRange(Set_TempRange);
                //if (!SetRtl)
                //{
                //    LastError = $"温箱设备设置定值等待温度范围失败：{SetRtl.Message}";
                //    Logger.Error(nameof(FunConfig_TempBox), nameof(Execute), LastError);
                //    return OperateResult.Failed(LastError);
                //}
                //SetRtl = operate.Device_SetHumidRange(Set_HumidRange);
                //if (!SetRtl)
                //{
                //    LastError = $"温箱设备设置定值等待湿度范围失败：{SetRtl.Message}";
                //    Logger.Error(nameof(FunConfig_TempBox), nameof(Execute), LastError);
                //    return OperateResult.Failed(LastError);
                //}
                //SetRtl = operate.Device_SetWaitTime(Set_WaitTime);
                //if (!SetRtl)
                //{
                //    LastError = $"温箱设备设置定值等待时间失败：{SetRtl.Message}";
                //    Logger.Error(nameof(FunConfig_TempBox), nameof(Execute), LastError);
                //    return OperateResult.Failed(LastError);
                //}
                #endregion

                #endregion


                InfoString = $"温箱定值设置\n[定值温度：]{Set_Temperature}-[定值湿度：]{Set_Humidity}-[定值温度斜率：]{Set_TempSlope}\n" +
                    $"[定值湿度斜率：]{Set_HumidSlope}";
                //-[定值试验时间（时）：]{Set_TestHour}-[定值试验时间（分）：]{Set_TestMin}\n" +
                //    $"[定值是否等待：]{Set_WaitOn}-[定值等待时间：]{Set_WaitTime}-[定值等待温度范围：]{Set_TempRange}-[定值等待湿度范围：]" +
                //    $"{Set_HumidRange}";

                Logger.Info(nameof(FunConfig_TempBox), nameof(Execute), InfoString);

                if (OperateType != FOutPutStateType.Null)
                {
                    OperateResult OnOffRlt = operate.SetonOffAll(OperateType == FOutPutStateType.ON);

                    if (!OnOffRlt)
                    {
                        LastError = $"设备打开异常:{OnOffRlt.Message}";
                        Logger.Error(nameof(FunConfig_TempBox), nameof(Execute), LastError);
                        return OperateResult.Failed(LastError);
                    }

                    InfoString = OperateType == FOutPutStateType.ON ? "打开" : "关闭";

                    Logger.Info(nameof(FunConfig_TempBox), nameof(Execute), $"{InfoString}");
                }

                if (DelayAfterRun > 0)
                {
                    Thread.Sleep(DelayAfterRun);
                }

                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                Logger.Exception(nameof(FunConfig_TempBox), nameof(Execute), ex);
                return OperateResult.Failed();
            }


        }
    }
}
