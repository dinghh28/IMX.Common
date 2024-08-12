using Device;
using Device.Base;
using Newtonsoft.Json;
using Super.Zoo.Framework;
using Super.Zoo.Framework.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IMX.Common.Device.Inerfaces;
using IMX.Common.Function.Enumerations;

namespace IMX.Common.Function
{
    /// <summary>
    /// 直流源试验配置类
    /// </summary>
    public class FunConfig_DCSource : Function_Config
    {
        #region 公共属性
        [JsonIgnore]
        public override Guid Identify { get; } = new Guid();

        //[JsonIgnore]
        //public override IDeviceOperate Operate { get; set; }

        [JsonIgnore]
        public override FuncitonType SupportFuncitonType => FuncitonType.DCSOURCEFUNC;

        public override string DeviceAddress { get; set; } = "DCSource_0";

        #region 试验参数

        /// <summary>
        /// 设备开关操作
        /// </summary>
        public override FOutPutStateType OperateType { get; set; } = FOutPutStateType.Null;

        /// <summary>
        /// 设置电压
        /// </summary>
        public double Set_Vol { get; set; }

        /// <summary>
        /// 设置电流
        /// </summary>
        public double Set_Cur { get; set; }

        /// <summary>
        /// 设置过压值
        /// </summary>
        public double Set_OverVol { get; set; }
        #endregion

        #endregion

        #region 构造方法
        public FunConfig_DCSource() : base()
        {
        }

        //public FunConfig_DCSource(IDeviceOperate operate) : base(operate)
        //{
        //}


        public FunConfig_DCSource(ILogger logger) : base(logger)
        {
        }
        #endregion

        #region 析构方法
        ~FunConfig_DCSource()
        {
            Dispose();
        }
        #endregion

        public override OperateResult Execute(object device) 
        {
            try
            {
                if (device is null)
                {
                    LastError = $"设备类型不存】";
                    Logger.Error(nameof(FunConfig_DCSource), nameof(Execute), LastError);
                    return OperateResult.Failed(LastError);
                }
                if (!(device is IDCSource operate))
                {
                    LastError = $"设备类型异常：【{device.GetType()}】";
                    Logger.Error(nameof(FunConfig_DCSource), nameof(Execute), LastError);
                    return OperateResult.Failed(LastError);
                }

                string InfoString = string.Empty;

                OperateResult SetRlt = operate.Device_Set(0, 0, 0, 0, 0, Set_OverVol, 0, 0, 0, new List<(double Vvalue, double Ivalue)> { (Set_Vol, Set_Cur) });

                if (!SetRlt)
                {
                    LastError = $"设备参数设置异常：【{SetRlt.Message}】";
                    Logger.Error(nameof(FunConfig_DCSource), nameof(Execute), LastError);
                    return OperateResult.Failed(LastError);
                }

                InfoString = $"设置\n[启动电压]{Set_Vol}\n[限制电流]{Set_Cur}成功";
                Logger.Info(nameof(FunConfig_DCSource), nameof(Execute), InfoString);

                if (OperateType != FOutPutStateType.Null)
                {
                    operate.Device_SetOnOff(0, OperateType == FOutPutStateType.ON ? "开" : "关");

                    InfoString = OperateType == FOutPutStateType.ON ? "打开" : "关闭";

                    Logger.Info(nameof(FunConfig_DCSource), nameof(Execute), $"设备已{InfoString}");
                }

                if (DelayAfterRun > 0)
                {
                    Thread.Sleep(DelayAfterRun);
                }

                return OperateResult.Succeed();
            }
            catch (Exception ex )
            {
                Logger.Error(nameof(FunConfig_DCSource), nameof(Execute), ex.Message);
                return OperateResult.Failed();
            }

        }
    }
}
