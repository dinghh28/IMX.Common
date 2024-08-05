
using Newtonsoft.Json;
using Super.Zoo.Framework;
using Super.Zoo.Framework.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using IMX.Common.Function.Enumerations;
using IMX.Common.Device.Inerfaces;

namespace IMX.Common.Function
{
    public class FunConfig_ACSource : Function_Config
    {

        #region 公共属性

        [JsonIgnore]
        public override Guid Identify { get; } = new Guid();

        //[JsonIgnore]
        //public override IDeviceOperate Operate { get; set; }

        [JsonIgnore]
        public override FuncitonType SupportFuncitonType => FuncitonType.ACSOURCEFUNC;

        public override string DeviceAddress { get; set; } = "ACSource_0";

        public override FOutPutStateType OperateType { get; set; } = FOutPutStateType.Null;


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

        public List<ModACSourceSetValue> SetValues { get; set; } = new List<ModACSourceSetValue>(3);

        /// <summary>
        /// 设置拉载模式
        /// </summary>
        public FunOutModeType Set_Model { get; set; } = FunOutModeType.三相独立;

        #endregion

        #region 构造方法
        public FunConfig_ACSource() : base()
        {
        }

        //public FunConfig_ACDCInverter(IDeviceOperate operate):base(operate)
        //{
        //}


        public FunConfig_ACSource(ILogger logger) : base(logger)
        {
        }
        #endregion

        #region 析构方法
        ~FunConfig_ACSource()
        {
            Dispose();
        }
        #endregion

        #region 公共方法
        public override OperateResult Execute(object device)
        {
            try
            {
                if (device == null)
                {
                    LastError = $"设备类型不存】";
                    Logger.Error(nameof(FunConfig_ACSource), nameof(Execute), LastError);
                    return OperateResult.Failed(LastError);
                }
                if (!(device is IACSource operate))
                {
                    LastError = $"设备类型异常：【{device.GetType()}】";
                    Logger.Error(nameof(FunConfig_ACSource), nameof(Execute), LastError);
                    return OperateResult.Failed(LastError);
                }

                string InfoString = string.Empty;

                IList<(byte, double, double)> values = new List<(byte, double, double)>();
                SetValues.ForEach(x => { values.Add((x.Set_Enable, x.Set_LoadVol, x.Set_LoadFrequency)); });

                OperateResult result = operate.Set_Value((byte)Set_Model, values);
                if (!result)
                {
                    LastError = $"设备设置异常:{result.Message}";
                    Logger.Error(nameof(FunConfig_ACSource), nameof(Execute), LastError);
                    return OperateResult.Failed(LastError);
                }

                InfoString = $"所有通道设置\n" +
                       $"[拉载模式] -{Set_Model}" +
                       $"[U相：][电压使能]-{SetValues[0].Set_Enable}-[拉载电压值]-{SetValues[0].Set_LoadVol}-[拉载频率值]-{SetValues[0].Set_LoadFrequency}\n" +
                       $"[V相：][电压使能]-{SetValues[1].Set_Enable}-[拉载电压值]-{SetValues[1].Set_LoadVol}-[拉载频率值]-{SetValues[1].Set_LoadFrequency}\n" +
                       $"[W相：][电压使能]-{SetValues[2].Set_Enable}-[拉载电压值]-{SetValues[2].Set_LoadVol}-[拉载频率值]-{SetValues[2].Set_LoadFrequency}\n";

                Logger.Info(nameof(FunConfig_DCLoad), nameof(Execute), InfoString);

                if (OperateType != FOutPutStateType.Null)
                {
                    OperateResult OnOffRlt = operate.SetOnOff((byte)(OperateType == FOutPutStateType.ON ? 0xFF : 0x00));

                    if (!OnOffRlt)
                    {
                        LastError = $"设备打开异常:{OnOffRlt.Message}";
                        Logger.Error(nameof(FunConfig_DCLoad), nameof(Execute), LastError);
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


                //InfoString = $"设备设置[CV模式]成功";
                //Logger.Info(nameof(FunConfig_ACDCInverter), nameof(Execute), InfoString);

                //operate.Device_SetValue(Set_Vol, Set_CurIn, Set_CurOut, Set_CurUpper, Set_CurLower, Set_PowerUpper);

                //InfoString = $"设备设置[电压{Set_Vol}],[输入电流{Set_CurIn}],[输出电流{Set_CurOut}],[电流上限{Set_CurUpper}],[电流下限{Set_CurLower}],[功率上限{Set_PowerUpper}]成功";
                //Logger.Info(nameof(FunConfig_ACDCInverter), nameof(Execute), InfoString);


                //if (OperateType != FOutPutStateType.Null)
                //{
                //    OperateResult OnOffRlt = operate.Device_SetOnOff(OperateType == FOutPutStateType.ON);

                //    if (!OnOffRlt)
                //    {
                //        LastError = $"设备打开异常:{OnOffRlt.Message}";
                //        Logger.Error(nameof(FunConfig_ACDCInverter), nameof(Execute), LastError);
                //        return OperateResult.Failed(LastError);
                //    }

                //    InfoString = OperateType == FOutPutStateType.ON ? "打开" : "关闭";

                //    Logger.Info(nameof(FunConfig_ACDCInverter), nameof(Execute), $"所有通道已{InfoString}");
                //}

                //return OperateResult.Succeed();
            }
            catch (Exception ex )
            {
                Logger.Exception(nameof(FunConfig_DCLoad), nameof(Execute), ex);
                return OperateResult.Failed();
            }
            
        }
        #endregion
    }
    public class ModACSourceSetValue
    {
        /// <summary>
        /// 相电压使能
        /// </summary>
        public byte Set_Enable { get; set; } = 0x00;

        /// <summary>
        /// 设置拉载电压
        /// </summary>
        public double Set_LoadVol { get; set; }

        /// <summary>
        /// 设置拉载频率
        /// </summary>
        public double Set_LoadFrequency { get; set; }
    }
}
