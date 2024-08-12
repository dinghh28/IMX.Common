using Device;
using IMX.Common.Device.Inerfaces;
using IMX.Common.Function.Enumerations;
using IMX.Common.Logger;
using Newtonsoft.Json;
using Super.Zoo.Framework;
using Super.Zoo.Framework.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IMX.Common.Function
{
    public class FunConfig_DCLoad: Function_Config
    {
        #region 公共属性

        [JsonIgnore]
        public override Guid Identify { get; } = new Guid();

        //[JsonIgnore]
        //public override IDeviceOperate Operate { get; set; }

        [JsonIgnore]
        public override FuncitonType SupportFuncitonType => FuncitonType.DCLOADFUNC;

        public override string DeviceAddress { get; set; } = "DCload_0";

        public override FOutPutStateType OperateType { get; set; } = FOutPutStateType.Null;
        public List<ModDCLoadSetValue> SetValues { get; set; } = new List<ModDCLoadSetValue>(4);

        public FunRunModeType RunModeType { get; set; } = FunRunModeType.CC;

        ///// <summary>
        ///// 设置电压
        ///// </summary>
        //public double Set_Vol { get; set; }

        ///// <summary>
        ///// 设置输入电流
        ///// </summary>
        //public double Set_CurIn { get; set; }

        ///// <summary>
        ///// 设置输出电流
        ///// </summary>
        //public double Set_CurOut { get; set; }

        ///// <summary>
        ///// 电流上限值
        ///// </summary>
        //public double Set_CurUpper { get; set; }

        ///// <summary>
        ///// 电流下限值
        ///// </summary>
        //public double Set_CurLower { get; set; }

        ///// <summary>
        ///// 功率上限值
        ///// </summary>
        //public double Set_PowerUpper { get; set; }

        #endregion

        #region 构造方法
        public FunConfig_DCLoad() : base()
        {

        }


        public FunConfig_DCLoad(ILogger logger) : base(logger)
        {
        }
        #endregion

        #region 析构方法
        ~FunConfig_DCLoad()
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
                    Logger.Error(nameof(FunConfig_DCLoad), nameof(Execute), LastError);
                    return OperateResult.Failed(LastError);
                }
                if (!(device is IDCLoad operate))
                {
                    LastError = $"设备类型异常：【{device.GetType()}】";
                    Logger.Error(nameof(FunConfig_DCLoad), nameof(Execute), LastError);
                    return OperateResult.Failed(LastError);
                }

                string InfoString = "所有通道设置\n";
                IList<(byte, double, double)> values = new List<(byte, double, double)>();
                SetValues.ForEach(x=> { values.Add(((byte)(x.Set_Mode), x.Set_LoadValue, x.Set_LimValue)); });

                OperateResult SetRtl = operate.Device_SetAll(values);

                if (!SetRtl)
                {
                    LastError = $"设备设置异常:{SetRtl.Message}";
                    Logger.Error(nameof(FunConfig_DCLoad), nameof(Execute), LastError);
                    return OperateResult.Failed(LastError);
                }

                InfoString = $"所有通道设置\n" +
                    $"[通道1：][模式]-{SetValues[0].Set_Mode}-[拉载值]-{SetValues[0].Set_LoadValue}-[限制电流/启动电压]-{SetValues[0].Set_LimValue}\n" +
                    $"[通道2：][模式]-{SetValues[1].Set_Mode}-[拉载值]-{SetValues[1].Set_LoadValue}-[限制电流/启动电压]-{SetValues[1].Set_LimValue}\n" +
                    $"[通道3：][模式]-{SetValues[2].Set_Mode}-[拉载值]-{SetValues[2].Set_LoadValue}-[限制电流/启动电压]-{SetValues[2].Set_LimValue}\n" +
                    $"[通道4：][模式]-{SetValues[3].Set_Mode}-[拉载值]-{SetValues[3].Set_LoadValue}-[限制电流/启动电压]-{SetValues[3].Set_LimValue}\n";

                Logger.Info(nameof(FunConfig_DCLoad), nameof(Execute), InfoString);

                if (OperateType != FOutPutStateType.Null)
                {
                    OperateResult OnOffRlt = operate.Device_SetOnOff(OperateType == FOutPutStateType.ON);

                    if (!OnOffRlt)
                    {
                        LastError = $"设备打开异常:{OnOffRlt.Message}";
                        SuperDHHLoggerManager.Error(LoggerType.DEVICELOG, nameof(FunConfig_DCLoad), nameof(Execute), LastError);
                        return OperateResult.Failed(LastError);
                    }

                    InfoString = OperateType == FOutPutStateType.ON ? "打开" : "关闭";

                    Logger.Info(nameof(FunConfig_DCLoad), nameof(Execute), $"所有通道已{InfoString}");
                }

                if (DelayAfterRun > 0)
                {
                    Thread.Sleep(DelayAfterRun);
                }

                return OperateResult.Succeed();

                #region     
                //for (int i = 0; i < 4; i++)
                //{
                //    OperateResult SetRtl = operate.Device_SetAll(SetValues);
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

                //if (Set_Model == null)
                //{
                //    LastError = $"模式异常：当前模式设置-{Set_Model}";
                //    SuperDHHLoggerManager.Error(LoggerType.DEVICELOG, nameof(FunConfig_DCLoad), nameof(Execute), LastError);
                //    return OperateResult.Failed(LastError);
                //}

                //IList<(byte mode, double dvalue, double limvalue)> value = new List<(byte, double, double)>();

                //string InfoString = string.Empty;
                //byte cccv = (byte)(Set_Model == "CC" ? 0x0 : 0x1);
                //value.Add((cccv, Set_LoadValue1, Set_LimValue1));
                //value.Add((cccv, Set_LoadValue2, Set_LimValue2));
                //value.Add((cccv, Set_LoadValue3, Set_LimValue3));
                //value.Add((cccv, Set_LoadValue4, Set_LimValue4));

                //OperateResult SetRlt = operate.Device_SetAll(value);

                //if (!SetRlt)
                //{
                //    LastError = $"设备设置异常:{SetRlt.Message}";
                //    SuperDHHLoggerManager.Error(LoggerType.DEVICELOG, nameof(FunConfig_DCLoad), nameof(Execute), LastError);
                //    return OperateResult.Failed(LastError);
                //}

                //InfoString = $"所有通道设置\n[模式]-{Set_Model}\n[拉载值]-[通道1：]{Set_LoadValue1}-[通道2：]{Set_LoadValue2}-[通道3：]{Set_LoadValue3}-[通道4：]{Set_LoadValue4}\n " +
                //    $"[限制电流/启动电压]-[通道1：]{Set_LimValue1}-[通道2：]{Set_LimValue2}-[通道3：]{Set_LimValue3}-[通道4：]{Set_LimValue4}";

                //Logger.Info(nameof(DCLoad_FConfig), nameof(Execute), InfoString);

                //if (OperateType != FOutPutStateType.Null)
                //{
                //    OperateResult OnOffRlt = operate.Device_SetOnOff(OperateType == FOutPutStateType.ON ? 0x0F : 0x00);

                //    if (!OnOffRlt)
                //    {
                //        LastError = $"设备打开异常:{OnOffRlt.Message}";
                //        SuperDHHLoggerManager.Error(LoggerType.DEVICELOG, nameof(FunConfig_DCLoad), nameof(Execute), LastError);
                //        return OperateResult.Failed(LastError);
                //    }

                //    InfoString = OperateType == FOutPutStateType.ON ? "打开" : "关闭";

                //    Logger.Info(nameof(FunConfig_DCLoad), nameof(Execute), $"所有通道已{InfoString}");
                //}

                //return OperateResult.Succeed();
                #endregion
            }
            catch (Exception ex)
            {
                Logger.Exception(nameof(FunConfig_DCLoad), nameof(Execute), ex);
                return OperateResult.Failed();
            }
        }
    }

    public class ModDCLoadSetValue
    {
        /// <summary>
        /// 设置通道拉载模式
        /// </summary>
        public byte Set_Mode { get; set; } 

        /// <summary>
        /// 设置通道拉载值
        /// </summary>
        public double Set_LoadValue { get; set; }

        /// <summary>
        /// 设置通道限制值
        /// </summary>
        public double Set_LimValue { get; set; }
    }
}
