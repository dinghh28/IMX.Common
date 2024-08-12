using Device;
using IMX.Common.Device.Inerfaces;
using IMX.Common.Function.Enumerations;
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
    public class FunConfig_APU : Function_Config
    {
        #region 公共属性

        [JsonIgnore]
        public override Guid Identify { get; } = new Guid();

        //[JsonIgnore]
        //public override IDeviceOperate Operate { get; set; }

        [JsonIgnore]
        public override FuncitonType SupportFuncitonType => FuncitonType.APUFUNC;

        public override string DeviceAddress { get; set; } = "APU_0";

        public override FOutPutStateType OperateType { get; set; } = FOutPutStateType.Null;

        /// <summary>
        /// 设置电压挡位
        /// </summary>
        public string OperateRange { get; set; }

        /// <summary>
        /// 设置电压
        /// </summary>
        public double Set_Vol { get; set; }

        /// <summary>
        /// 设置电流
        /// </summary>
        public double Set_Cur { get; set; }

        #endregion

        #region 构造方法
        public FunConfig_APU() : base()
        {
        }

        //public FunConfig_ACDCInverter(IDeviceOperate operate):base(operate)
        //{
        //}


        public FunConfig_APU(ILogger logger) : base(logger)
        {
        }
        #endregion

        #region 析构方法
        ~FunConfig_APU()
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
                    Logger.Error(nameof(FunConfig_APU), nameof(Execute), LastError);
                    return OperateResult.Failed(LastError);
                }
                if (!(device is IAPU operate))
                {
                    LastError = $"设备类型异常：【{device.GetType()}】";
                    Logger.Error(nameof(FunConfig_APU), nameof(Execute), LastError);
                    return OperateResult.Failed(LastError);
                }

                string InfoString = string.Empty;
                OperateResult operateValueResult = operate.Device_SetRange(OperateRange);
                if (!operateValueResult)
                {
                    LastError = $"设备挡位设置异常：【{operateValueResult.Message}】";
                    Logger.Error(nameof(FunConfig_APU), nameof(Execute), LastError);
                    return OperateResult.Failed(LastError);
                }
                InfoString = $"设置设备挡位：{OperateRange}";
                Logger.Info(nameof(FunConfig_APU), nameof(Execute), InfoString);

                //Thread.Sleep(10);
                OperateResult SetRlt = operate.Device_Set(new List<(double Vvalue, double Ivalue)> { (Set_Vol, Set_Cur) });

                if (!SetRlt)
                {
                    LastError = $"设备参数设置异常：【{SetRlt.Message}】";
                    Logger.Error(nameof(FunConfig_APU), nameof(Execute), LastError);
                    return OperateResult.Failed(LastError);
                }
                //Thread.Sleep(10);
                InfoString = $"设置\n[启动电压]{Set_Vol}\n[限制电流]{Set_Cur}成功";
                Logger.Info(nameof(FunConfig_APU), nameof(Execute), InfoString);

                if (OperateType != FOutPutStateType.Null)
                {
                    operate.Device_SetOnOff(1, OperateType.ToString());

                    InfoString = OperateType == FOutPutStateType.ON ? "打开" : "关闭";

                    Logger.Info(nameof(FunConfig_APU), nameof(Execute), $"设备已{InfoString}");
                }

                if (DelayAfterRun > 0)
                {
                    Thread.Sleep(DelayAfterRun);
                }

                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                Logger.Exception(nameof(FunConfig_APU), nameof(Execute), ex);
                return OperateResult.Failed();
            }

        }

        #endregion
    }
}
