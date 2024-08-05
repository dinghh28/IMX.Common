using Device;
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
    /// 旋变模拟器
    /// </summary>
    public class FunConfig_Gyroscopic : Function_Config
    {
        #region 公共属性

        [JsonIgnore]
        public override Guid Identify { get; } = new Guid();

        //[JsonIgnore]
        //public override IDeviceOperate Operate { get; set; }

        [JsonIgnore]
        public override FuncitonType SupportFuncitonType => FuncitonType.GYROSCOPICFUNC;

        public override string DeviceAddress { get; set; } = "Gyroscopic_0";

        public override FOutPutStateType OperateType { get; set; } = FOutPutStateType.Null;

        #region 试验参数
        /// <summary>
        /// 是否为单相旋变模拟器
        /// </summary>
        public bool IsSingle { get; set; } = false;

        public List<ModGyroscopicSetValue> SetValues { get; set; }// = new List<ModGyroscopicSetValue>();
        //{
        //    new ModGyroscopicSetValue(),
        //    new ModGyroscopicSetValue(),
        //    new ModGyroscopicSetValue()
        //};
        #endregion

        #endregion

        #region 构造方法
        public FunConfig_Gyroscopic() : base()
        {
        }

        //public FunConfig_Gyroscopic(IDeviceOperate operate) : base(operate)
        //{
        //}


        public FunConfig_Gyroscopic(ILogger logger) : base(logger)
        {
        }
        #endregion

        #region 析构方法
        ~FunConfig_Gyroscopic()
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
                    Logger.Error(nameof(FunConfig_Gyroscopic), nameof(Execute), LastError);
                    return OperateResult.Failed(LastError);
                }
                if (!(device is IGyroscopic operate))
                {
                    LastError = $"设备类型异常：【{device.GetType()}】";
                    Logger.Error(nameof(FunConfig_Gyroscopic), nameof(Execute), LastError);
                    return OperateResult.Failed(LastError);
                }

                string InfoString = string.Empty;
                int index = IsSingle ? 1 : 3;

                for (int i = 0; i < index; i++)
                {
                    var SpeedRlt = operate.Device_SetSpeed(i, SetValues[i].Speed);
                    if (!SpeedRlt)
                    {
                        LastError = $"通道 {i + 1} 转速设置失败：{SpeedRlt.Message}";
                        Logger.Error(nameof(FunConfig_Gyroscopic), nameof(Execute), LastError);
                        return OperateResult.Failed(LastError);
                    }
                    InfoString += $"通道 {i + 1} 设置转速[{SetValues[i].Speed}]";

                    var PoleofPairsRlt = operate.Device_SetPoleofPairs(i, SetValues[i].PoleofPairs);
                    if (!PoleofPairsRlt)
                    {
                        LastError = $"通道 {i + 1} 极对数设置失败：{PoleofPairsRlt.Message}";
                        Logger.Error(nameof(FunConfig_Gyroscopic), nameof(Execute), LastError);
                        return OperateResult.Failed(LastError);
                    }
                    InfoString += $"通道 {i + 1} 设置极对数[{SetValues[i].PoleofPairs}]";

                    var EAngleRlt = operate.Device_SetEAngle(i, SetValues[i].EAngle);
                    if (!EAngleRlt)
                    {
                        LastError = $"通道 {i + 1} 电气角度设置失败：{EAngleRlt.Message}";
                        Logger.Error(nameof(FunConfig_Gyroscopic), nameof(Execute), LastError);
                        return OperateResult.Failed(LastError);
                    }
                    InfoString += $"通道 {i + 1} 设置电气角度[{SetValues[i].EAngle}]";

                    for (int j = 0; j < 2; j++)
                    {
                        var TempRlt = operate.Device_SetTemp(i, j, SetValues[i].Temp[j]);
                        if (!TempRlt)
                        {
                            LastError = $"通道 {i + 1} 温度模拟 {j + 1} 设置失败：{TempRlt.Message}";
                            Logger.Error(nameof(FunConfig_Gyroscopic), nameof(Execute), LastError);
                            return OperateResult.Failed(LastError);
                        }
                        InfoString += $"通道 {i + 1} 设置温度模拟 {j + 1} [{SetValues[i].Temp[j]}]";
                    }
                }

                Logger.Info(nameof(FunConfig_Gyroscopic), nameof(Execute), InfoString);

                if (DelayAfterRun > 0)
                {
                    Thread.Sleep(DelayAfterRun);
                }

                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                Logger.Exception(nameof(FunConfig_Gyroscopic), nameof(Execute), ex);
                return OperateResult.Failed();
            }
            
        }
    }

    public class ModGyroscopicSetValue
    {
        /// <summary>
        /// 转速
        /// </summary>
        public int Speed { get; set; }

        /// <summary>
        /// 极对数
        /// </summary>
        public int PoleofPairs { get; set; }

        /// <summary>
        /// 电气角度
        /// </summary>
        public int EAngle { get; set; }

        /// <summary>
        /// 温度模拟
        /// </summary>
        public List<int> Temp { get; set; }
    }
}
