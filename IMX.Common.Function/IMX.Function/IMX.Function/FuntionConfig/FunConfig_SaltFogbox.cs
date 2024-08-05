using IMX.Common.Device.Inerfaces;
using IMX.Common.Function.Enumerations;
using Newtonsoft.Json;
using Super.Zoo.Framework.Logger;
using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IMX.Common.Function
{
    public class  FunConfig_SaltFogbox: Function_Config
    {
        #region 公共属性
        [JsonIgnore]
        public override Guid Identify { get; } = new Guid();

        //[JsonIgnore]
        //public override IDeviceOperate Operate { get; set; }

        [JsonIgnore]
        public override FuncitonType SupportFuncitonType => FuncitonType.SALTFOGBOXFUNC;

        public override string DeviceAddress { get; set; } = "SaltFogbox_0";

        /// <summary>
        /// 设备开关操作
        /// </summary>
        public override FOutPutStateType OperateType { get; set; } = FOutPutStateType.Null;


        #endregion

        #region 构造方法
        public FunConfig_SaltFogbox() : base()
        {
        }

        public FunConfig_SaltFogbox(ILogger logger) : base(logger)
        {
        }
        #endregion

        #region 析构方法
        ~FunConfig_SaltFogbox()
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
                    Logger.Error(nameof(FunConfig_SaltFogbox), nameof(Execute), LastError);
                    return OperateResult.Failed(LastError);
                }
                if (!(device is ISaltFogbox operate))
                {
                    LastError = $"设备类型异常：【{device.GetType()}】";
                    Logger.Error(nameof(FunConfig_SaltFogbox), nameof(Execute), LastError);
                    return OperateResult.Failed(LastError);
                }
                string InfoString = string.Empty;

                Logger.Info(nameof(FunConfig_SaltFogbox), nameof(Execute), InfoString);

                if (OperateType != FOutPutStateType.Null)
                {
                    OperateResult OnOffRlt = operate.SetonOffAll(OperateType == FOutPutStateType.ON);

                    if (!OnOffRlt)
                    {
                        LastError = $"设备打开异常:{OnOffRlt.Message}";
                        Logger.Error(nameof(FunConfig_SaltFogbox), nameof(Execute), LastError);
                        return OperateResult.Failed(LastError);
                    }

                    InfoString = OperateType == FOutPutStateType.ON ? "打开" : "关闭";

                    Logger.Info(nameof(FunConfig_SaltFogbox), nameof(Execute), $"{InfoString}");
                }

                if (DelayAfterRun > 0)
                {
                    Thread.Sleep(DelayAfterRun);
                }

                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                Logger.Exception(nameof(FunConfig_SaltFogbox), nameof(Execute), ex);
                return OperateResult.Failed();
            }


        }
    }
}
