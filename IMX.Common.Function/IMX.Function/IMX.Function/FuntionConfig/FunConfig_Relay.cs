using Device;
using IMX.Common.Device.Inerfaces;
using IMX.Common.Function.Enumerations;
using IMX.Function.Enumerations;
using Newtonsoft.Json;
using Super.Zoo.Framework;
using Super.Zoo.Framework.Logger;
using System;
using System.Collections.Generic;
using System.Threading;

namespace IMX.Common.Function
{
    /// <summary>
    /// 继电器试验配置类
    /// </summary>
    public class FunConfig_Relay : Function_Config
    {
        #region 公共属性
        //[JsonIgnore]
        //public override Guid Identify { get; } = Guid.NewGuid();

        //[JsonIgnore]
        //public override IDeviceOperate Operate { get; set; }

        [JsonIgnore]
        public override FuncitonType SupportFuncitonType => FuncitonType.RELAYFUNC;

        public override string DeviceAddress { get; set; } = "Relayboard_0";

        #region 试验参数
        /// <summary>
        /// 继电器输出状态
        /// </summary>
        public List<EDeviceOutPutState> Set_OutPutStates { get; set; }
        //    = new List<EDeviceOutPutState>()
        //{
        //    EDeviceOutPutState.OFF, EDeviceOutPutState.OFF, EDeviceOutPutState.OFF, EDeviceOutPutState.OFF,
        //    EDeviceOutPutState.OFF, EDeviceOutPutState.OFF, EDeviceOutPutState.OFF, EDeviceOutPutState.OFF,
        //    EDeviceOutPutState.OFF, EDeviceOutPutState.OFF, EDeviceOutPutState.OFF, EDeviceOutPutState.OFF,
        //    EDeviceOutPutState.OFF, EDeviceOutPutState.OFF, EDeviceOutPutState.OFF, EDeviceOutPutState.OFF,
        //};
        #endregion

        #endregion

        #region 构造方法
        public FunConfig_Relay() : base()
        {
        }

        //public FunConfig_Relay(IDeviceOperate operate) : base(operate)
        //{
        //}


        public FunConfig_Relay(ILogger logger) : base(logger)
        {
        }
        #endregion

        #region 析构方法
        ~FunConfig_Relay()
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
                    LastError = $"设备类型不存在";
                    Logger.Error(nameof(FunConfig_Relay), nameof(Execute), LastError);
                    return OperateResult.Failed(LastError);
                }
                if (!(device is IRelay operate))
                {
                    LastError = $"设备类型异常：【{device.GetType()}】";
                    Logger.Error(nameof(FunConfig_Relay), nameof(Execute), LastError);
                    return OperateResult.Failed(LastError);
                }

                string InfoString = string.Empty;

                string valueonoff = "";
                //foreach (EDeviceOutPutState item in Set_OutPutStates)
                //{
                //    valueonoff += (int)item;
                //}
                for (int i = 0; i < Set_OutPutStates.Count; i++)
                {
                    valueonoff += (int)Set_OutPutStates[Set_OutPutStates.Count - 1 - i];
                }

                OperateResult SetRtl = operate.Device_Set(valueonoff);

                if (!SetRtl)
                {
                    LastError = $"继电器操作失败：{SetRtl.Message}";
                    Logger.Error(nameof(FunConfig_Relay), nameof(Execute), LastError);
                    return OperateResult.Failed(LastError);
                }

                for (int i = 0; i < Set_OutPutStates.Count; i++)
                {
                    InfoString += $"[继电器{i}]{Set_OutPutStates[i]}\n";
                }

                Logger.Info(nameof(FunConfig_Relay), nameof(Execute), InfoString);

                if (DelayAfterRun > 0)
                {
                    Thread.Sleep(DelayAfterRun);
                }

                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                Logger.Exception(nameof(FunConfig_Relay), nameof(Execute), ex);
                return OperateResult.Failed();
            }
            
        }
    }
}
