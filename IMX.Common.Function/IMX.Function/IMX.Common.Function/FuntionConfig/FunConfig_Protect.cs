using Device;
using IMX.Common.Device.Inerfaces;
using IMX.Common.Device.Models;
using IMX.Common.Function.Enumerations;
using Newtonsoft.Json;
using Super.Zoo.Framework;
using Super.Zoo.Framework.Logger;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IMX.Common.Function
{
    /// <summary>
    /// 保护配置类
    /// </summary>
    public class FunConfig_Protect : Function_Config
    {
        #region 公共属性
        [JsonIgnore]
        public override Guid Identify { get; } = new Guid();

        //[JsonIgnore]
        //public override IDeviceOperate Operate { get; set; }

        [JsonIgnore]
        public override FuncitonType SupportFuncitonType => FuncitonType.PROTECTFUNC;

        public override string DeviceAddress { get; set; }

        #region 试验参数
        /// <summary>
        /// 是否开启保护线程
        /// </summary>
        public bool IsStartProtectThread { get; set; } = false;

        /// <summary>
        /// 是否触发保护
        /// </summary>
        public bool IsProtected { get; set; } = false;

        //public List<ModProtect_Range> ProtectIteam_Range { get; set; } = new List<ModProtect_Range> 
        //{
        //    //new ModProtectInfo{ Name = ""},
        //    //new ModProtectInfo{ Name = ""},
        //    //new ModProtectInfo{ Name = ""},
        //    //new ModProtectInfo{ Name = ""},
        //    //new ModProtectInfo{ Name = ""},
        //    //new ModProtectInfo{ Name = ""},
        //    //new ModProtectInfo{ Name = ""},
        //    //new ModProtectInfo{ Name = ""},
        //    //new ModProtectInfo{ Name = ""},
        //    //new ModProtectInfo{ Name = ""},
        //    //new ModProtectInfo{ Name = ""},
        //    //new ModProtectInfo{ Name = ""},
        //    //new ModProtectInfo{ Name = ""},
        //    //new ModProtectInfo{ Name = ""},
        //};

        //public List<ModProtect_Trage> ProtectIteam_Trage = new List<ModProtect_Trage>
        //{
        //    //new ModProtectInfo{ Name = ""},
        //    //new ModProtectInfo{ Name = ""},
        //    //new ModProtectInfo{ Name = ""},
        //    //new ModProtectInfo{ Name = ""},
        //    //new ModProtectInfo{ Name = ""},
        //    //new ModProtectInfo{ Name = ""},
        //    //new ModProtectInfo{ Name = ""},
        //    //new ModProtectInfo{ Name = ""},
        //    //new ModProtectInfo{ Name = ""},
        //    //new ModProtectInfo{ Name = ""},
        //    //new ModProtectInfo{ Name = ""},
        //    //new ModProtectInfo{ Name = ""},
        //    //new ModProtectInfo{ Name = ""},
        //    //new ModProtectInfo{ Name = ""},
        //};

        /// <summary>
        /// 范围保护
        /// </summary>
        public List<ModDeviceReadData> ProtectIteam_Range { get; set; } = new List<ModDeviceReadData>();

        /// <summary>
        /// 触发保护
        /// </summary>
        public List<ModDeviceReadData> ProtectIteam_Trage { get; set; } = new List<ModDeviceReadData>();
        #endregion

        #endregion

        #region 构造方法
        public FunConfig_Protect() : base() { }

        //public FunConfig_Protect(IDeviceOperate operate) : base(null, operate)
        //{
        //}


        public FunConfig_Protect(ILogger logger) : base(logger)
        {
        }
        #endregion

        #region 析构方法
        ~FunConfig_Protect()
        {
            Dispose();
        }
        #endregion

        public override OperateResult Execute(object device)
        {
            try
            {
                lock (this)
                {
                    string ProtectInfo = string.Empty;

                    //if (ProtectIteam_Range.Any(x => x.IsUse == true && x.IsInRange == false)) 
                    //{
                    //    IsProtected = true;
                    //}

                    foreach (var mod in ProtectIteam_Range)
                    {
                        //试验结束，不继续进行保护判断
                        if (!IsTestRunning[0])
                        {
                            IsProtected = false;
                            return OperateResult.Succeed();
                        }

                        if (mod.IsUse)
                        {
                            if (!mod.IsInRange)
                            {
                                IsProtected = true;
                                ProtectInfo = $"{mod.DataInfo.Name}超过设定范围\r\n当前{mod.DataInfo.Name}上报值{mod.DataInfo.Value},上限{mod.Limits_Upper},下限{mod.Limits_Lower}";
                                return OperateResult.Failed(ProtectInfo);
                            }
                        }
                    }

                    //if (ProtectIteam_Trage.Any(x => x.IsUse == true && x.IsInRange == false))
                    //{
                    //    IsProtected = true;
                    //}

                    foreach (var item in ProtectIteam_Trage)
                    {
                        //试验结束，不继续进行保护判断
                        if (!IsTestRunning[0])
                        {
                            IsProtected = false;
                            return OperateResult.Succeed();
                        }

                        if (item.IsUse)
                        {
                            if (!item.IsInRange)
                            {
                                IsProtected = true;
                                ProtectInfo = $"{item.DataInfo.Name}触发";
                                return OperateResult.Failed(ProtectInfo);
                            }
                        }
                    }

                    return OperateResult.Succeed();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(nameof(FunConfig_Protect), nameof(Execute), ex);
                return OperateResult.Failed();
            }
            
        }
    }

    ///// <summary>
    ///// 范围保护参数模型
    ///// </summary>
    //public class ModProtect_Range
    //{
    //    /// <summary>
    //    /// 保护项
    //    /// </summary>
    //    public string Name { get; set; } = string.Empty;

    //    /// <summary>
    //    /// 读取值
    //    /// </summary>
    //    public double Value { get; set; }

    //    /// <summary>
    //    /// 上限
    //    /// </summary>
    //    public double Limits_Upper { get; set; }

    //    private double limits_Lower;
    //    /// <summary>
    //    /// 下限
    //    /// </summary>
    //    public double Limits_Lower 
    //    {
    //        get 
    //        {
    //            if (IsTrage)
    //            {
    //                limits_Lower = Limits_Upper;
    //            }
    //            return limits_Lower;
    //        }
    //        set 
    //        {
    //            if (!IsTrage)
    //            {
    //                limits_Lower = value;
    //            }
    //        } 
    //    }

    //    /// <summary>
    //    /// 是否在设定范围内
    //    /// </summary>
    //    public bool IsInRange => Value <= Limits_Upper && Value >= Limits_Lower;

    //    /// <summary>
    //    /// 是否为触发值
    //    /// </summary>
    //    public bool IsTrage { get; set; }

    //    /// <summary>
    //    /// 是否参与保护
    //    /// </summary>
    //    public bool IsUse { get; set; } = false;
    //}

    ///// <summary>
    ///// 触发保护参数模型
    ///// </summary>
    //public class ModProtect_Trage
    //{
    //    /// <summary>
    //    /// 保护项
    //    /// </summary>
    //    public string Name { get; set; } = string.Empty;

    //    /// <summary>
    //    /// 读取值
    //    /// </summary>
    //    public double Value { get; set; }

    //    /// <summary>
    //    /// 触发值
    //    /// </summary>
    //    public double TrageValue { get; set; }

    //    /// <summary>
    //    /// 是否触发
    //    /// </summary>
    //    public bool IsTrage => Value == TrageValue;

    //    /// <summary>
    //    /// 是否参与保护
    //    /// </summary>
    //    public bool IsUse { get; set; } = false;
    //}
}
