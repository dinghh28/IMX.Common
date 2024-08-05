using Newtonsoft.Json;
using Super.Zoo.Framework.Logger;
using Super.Zoo.Framework;
using System;
using IMX.Common.Function.Enumerations;
using IMX.Common.Device.Inerfaces;

namespace IMX.Common.Function
{
    /// <summary>
    /// 双向源试验配置类
    /// </summary>
    public class FunConfig_ACDCInverter: Function_Config
    {
        #region 公共属性

        [JsonIgnore]
        public override Guid Identify { get; } = new Guid();
        
        //[JsonIgnore]
        //public override IDeviceOperate Operate { get; set; }

        [JsonIgnore]
        public override FuncitonType SupportFuncitonType => FuncitonType.ACDCINVERTERFUNC;

        public override string DeviceAddress { get; set; } = "ACDCInverter_0";

        public override FOutPutStateType OperateType { get; set; } = FOutPutStateType.Null;


        /// <summary>
        /// 设置电压
        /// </summary>
        public double Set_Vol { get; set; }

        /// <summary>
        /// 设置输入电流
        /// </summary>
        public double Set_CurIn { get; set; }

        /// <summary>
        /// 设置输出电流
        /// </summary>
        public double Set_CurOut{ get; set; }

        /// <summary>
        /// 电流上限值
        /// </summary>
        public double Set_CurUpper { get; set; }

        /// <summary>
        /// 电流下限值
        /// </summary>
        public double Set_CurLower { get; set; }

        /// <summary>
        /// 功率上限值
        /// </summary>
        public double Set_PowerUpper { get; set; }

        #endregion

        #region 构造方法
        public FunConfig_ACDCInverter() : base()
        {
        }

        //public FunConfig_ACDCInverter(IDeviceOperate operate):base(operate)
        //{
        //}


        public FunConfig_ACDCInverter(ILogger logger):base(logger)
        {
        }
        #endregion

        #region 析构方法
        ~FunConfig_ACDCInverter()
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
                    Logger.Error(nameof(FunConfig_ACDCInverter), nameof(Execute), LastError);
                    return OperateResult.Failed(LastError);
                }
                if (!(device is IACDCInverter operate))
                {
                    LastError = $"设备类型异常：【{device.GetType()}】";
                    Logger.Error(nameof(FunConfig_ACDCInverter), nameof(Execute), LastError);
                    return OperateResult.Failed(LastError);
                }

                string InfoString = string.Empty;

                //operate.Device_SetModel(Device.Common.Enumerations.EDeviceRunModel.CV);
                //InfoString = $"设备设置[CV模式]成功";
                //Logger.Info(nameof(FunConfig_ACDCInverter), nameof(Execute), InfoString);

                operate.Device_SetValue(Set_Vol, Set_CurIn, Set_CurOut, Set_CurUpper, Set_CurLower, Set_PowerUpper);

                InfoString = $"设备设置[电压{Set_Vol}],[输入电流{Set_CurIn}],[输出电流{Set_CurOut}],[电流上限{Set_CurUpper}],[电流下限{Set_CurLower}],[功率上限{Set_PowerUpper}]成功";
                Logger.Info(nameof(FunConfig_ACDCInverter), nameof(Execute), InfoString);


                if (OperateType != FOutPutStateType.Null)
                {
                    OperateResult OnOffRlt = operate.Device_SetOnOff(OperateType == FOutPutStateType.ON);

                    if (!OnOffRlt)
                    {
                        LastError = $"设备打开异常:{OnOffRlt.Message}";
                        Logger.Error(nameof(FunConfig_ACDCInverter), nameof(Execute), LastError);
                        return OperateResult.Failed(LastError);
                    }

                    InfoString = OperateType == FOutPutStateType.ON ? "打开" : "关闭";

                    Logger.Info(nameof(FunConfig_ACDCInverter), nameof(Execute), $"所有通道已{InfoString}");
                }

                if (DelayAfterRun > 0)
                {
                    Thread.Sleep(DelayAfterRun);
                }
                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                Logger.Exception(nameof(FunConfig_ACDCInverter), nameof(Execute), ex);
                return OperateResult.Failed();
            }

        }
        #endregion
    }
}
