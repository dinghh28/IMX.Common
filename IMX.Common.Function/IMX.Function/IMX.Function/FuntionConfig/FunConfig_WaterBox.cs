using IMX.Common.Device.Inerfaces;
using IMX.Common.Function.Enumerations;
using Newtonsoft.Json;
using Super.Zoo.Framework;
using Super.Zoo.Framework.Logger;
using System;
using System.Collections.Generic;
using System.Threading;

namespace IMX.Common.Function
{
    /// <summary>
    /// 水浴配置类
    /// </summary>
    public class FunConfig_WaterBox : Function_Config
    {
        #region 公共属性
        [JsonIgnore]
        public override Guid Identify { get; } = new Guid();

        //[JsonIgnore]
        //public override IDeviceOperate Operate { get; set; }

        [JsonIgnore]
        public override FuncitonType SupportFuncitonType => FuncitonType.WATERBOXFUNC;

        public override string DeviceAddress { get; set; } = "WaterBox_0";

        #region 试验参数

        /// <summary>
        /// 设备开关操作
        /// </summary>
        public override FOutPutStateType OperateType { get; set; } = FOutPutStateType.Null;
        ///// <summary>
        ///// 设置通道1温度
        ///// </summary>
        //public double Set_Temperature1 { get; set; }
        ///// <summary>
        ///// 设置通道2温度
        ///// </summary>
        //public double Set_Temperature2 { get; set; }
        ///// <summary>
        ///// 设置通道3温度
        ///// </summary>
        //public double Set_Temperature3 { get; set; }
        ///// <summary>
        ///// 设置通道1流速
        ///// </summary>
        //public double Set_Flow1 { get; set; }
        ///// <summary>
        ///// 设置通道2流速
        ///// </summary>
        //public double Set_Flow2 { get; set; }
        ///// <summary>
        ///// 设置通道3流速
        ///// </summary>
        //public double Set_Flow3 { get; set; }
        ///// <summary>
        ///// 设置通道1压力
        ///// </summary>
        //public double Set_Pressure1 { get; set; }
        ///// <summary>
        ///// 设置通道2压力
        ///// </summary>
        //public double Set_Pressure2 { get; set; }
        ///// <summary>
        ///// 设置通道3压力
        ///// </summary>
        //public double Set_Pressure3 { get; set; }

        /// <summary>
        /// 水浴设置参数
        /// </summary>
        public List<ModWaterBoxSetValue> SetValues { get; set; }

        #endregion

        #endregion

        #region 构造方法
        public FunConfig_WaterBox() : base()
        {
        }

        //public FunConfig_WaterBox(IDeviceOperate operate) : base(operate)
        //{
        //}


        public FunConfig_WaterBox(ILogger logger) : base(logger)
        {
        }
        #endregion

        #region 析构方法
        ~FunConfig_WaterBox()
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
                    Logger.Error(nameof(FunConfig_WaterBox), nameof(Execute), LastError);
                    return OperateResult.Failed(LastError);
                }
                if (!(device is IWaterBox operate))
                {
                    LastError = $"设备类型异常：【{device.GetType()}】";
                    Logger.Error(nameof(FunConfig_WaterBox), nameof(Execute), LastError);
                    return OperateResult.Failed(LastError);
                }

                string InfoString = "所有通道设置\n";
                //List<double> flows = new List<double> { SetValues[0].Set_Flow, SetValues[1].Set_Flow, SetValues[2].Set_Flow };
                //double[] temps = { Set_Temperature1, Set_Temperature2, Set_Temperature3 };
                //List<double> pressures = new List<double> { SetValues[0].Set_Pressure, SetValues[1].Set_Pressure, SetValues[2].Set_Pressure };

                List<(double temp, double tempslope, double flow, double pressure)> values = new List<(double temp, double tempslope, double flow, double pressure)> { };
                for (int i = 0; i < 3; i++)
                {
                    values.Add((SetValues[i].Set_Temperature, SetValues[i].Set_TemperatureSlope, SetValues[i].Set_Flow , SetValues[i].Set_Pressure ));
                }

                OperateResult SetRtl = operate.Device_SetAll(values);
                if (!SetRtl)
                {
                    LastError = $"水浴设备参数设置失败：{SetRtl.Message}";
                    Logger.Error(nameof(FunConfig_WaterBox), nameof(Execute), LastError);
                    return OperateResult.Failed(LastError);
                }


                //for (int i = 0; i < 3; i++)
                //{
                //    OperateResult SetRtl = operate.Device_SetTemp(i, SetValues[i].Set_Temperature);
                //    if (!SetRtl)
                //    {
                //        LastError = $"水浴设备设置通道{i}温度失败：{SetRtl.Message}";
                //        Logger.Error(nameof(FunConfig_WaterBox), nameof(Execute), LastError);
                //        return OperateResult.Failed(LastError);
                //    }
                //    InfoString += $"[通道 {i} ]温度:{SetValues[i].Set_Temperature};";

                //    SetRtl = operate.Device_SetFlow(i, SetValues[i].Set_Flow);
                //    if (!SetRtl)
                //    {
                //        LastError = $"水浴设备设置通道{i}流速失败：{SetRtl.Message}";
                //        Logger.Error(nameof(FunConfig_WaterBox), nameof(Execute), LastError);
                //        return OperateResult.Failed(LastError);
                //    }
                //    InfoString += $"[通道 {i} ]流速:{SetValues[i].Set_Flow};";

                //    SetRtl = operate.Device_SetPressure(i, SetValues[i].Set_Pressure);
                //    if (!SetRtl)
                //    {
                //        LastError = $"水浴设备设置通道{i}压力失败：{SetRtl.Message}";
                //        Logger.Error(nameof(FunConfig_WaterBox), nameof(Execute), LastError);
                //        return OperateResult.Failed(LastError);
                //    }
                //    InfoString += $"[通道 {i} ]流速:{SetValues[i].Set_Pressure};";
                //}

                InfoString = $"所有通道设置\n[水浴温度]-[通道1：]{SetValues[0].Set_Temperature}-[通道2：]{SetValues[1].Set_Temperature}-[通道3：]{SetValues[2].Set_Temperature}\n" +
                    $"[水浴流速]-[通道1：]{SetValues[0].Set_Flow}-[通道2：]{SetValues[1].Set_Flow}-[通道3：]{SetValues[2].Set_Flow}\n" +
                    $"[水浴流速]-[通道1：]{SetValues[0].Set_Pressure}-[通道2：]{SetValues[1].Set_Pressure}-[通道3：]{SetValues[2].Set_Pressure}\n";

                Logger.Info(nameof(FunConfig_WaterBox), nameof(Execute), InfoString);

                if (OperateType != FOutPutStateType.Null)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        OperateResult OnOffRlt = operate.Device_SetOnOff(i, OperateType == FOutPutStateType.ON);

                        if (!OnOffRlt)
                        {
                            LastError = $"设备打开异常:{OnOffRlt.Message}";
                            Logger.Error(nameof(FunConfig_WaterBox), nameof(Execute), LastError);
                            return OperateResult.Failed(LastError);
                        }
                    }
                    //InfoString = OperateType == FOutPutStateType.ON ? "打开" : "关闭";

                    Logger.Info(nameof(FunConfig_WaterBox), nameof(Execute), $"所有通道{OperateType}");

                }

                if (DelayAfterRun > 0)
                {
                    Thread.Sleep(DelayAfterRun);
                }

                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                Logger.Exception(nameof(FunConfig_WaterBox), nameof(Execute), ex);
                return OperateResult.Failed();
            }


        }
    }

    /// <summary>
    /// 水浴设置参数模型类
    /// </summary>
    public class ModWaterBoxSetValue
    {
        //private double set_Temperature;
        ///// <summary>
        ///// 设置温度
        ///// </summary>
        //public double Set_Temperature
        //{ 
        //    get=> set_Temperature;
        //    set => Set(nameof(Set_Temperature), ref set_Temperature, value);
        //}

        /// <summary>
        /// 设置温度
        /// </summary>
        public double Set_Temperature { get; set; } = 0;

        /// <summary>
        /// 设置温度速率
        /// </summary>
        public double Set_TemperatureSlope { get; set; } = 0;

        //private double set_Flow;
        ///// <summary>
        ///// 设置流速
        ///// </summary>
        //public double Set_Flow
        //{
        //    get => set_Flow;
        //    set => Set(nameof(Set_Flow), ref set_Flow, value);
        //}

        /// <summary>
        /// 设置流速
        /// </summary>
        public double Set_Flow { get; set; } = 0;

        //private double set_Pressure;
        ///// <summary>
        ///// 设置压力
        ///// </summary>
        //public double Set_Pressure
        //{
        //    get => set_Pressure;
        //    set => Set(nameof(Set_Pressure), ref set_Pressure, value);
        //}

        /// <summary>
        /// 设置压力
        /// </summary>
        public double Set_Pressure { get; set; } = 0;
    }
}
