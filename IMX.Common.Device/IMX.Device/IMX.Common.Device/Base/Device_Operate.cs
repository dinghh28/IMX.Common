using Device.Common;
using System;
using System.Collections.Generic;
using Super.Zoo.Framework;
using Super.Zoo.Framework.Logger;
using IMX.Common.Device;
using IMX.Common.Device.Models;
using IMX.Common.Logger;


namespace Device
{
    /// <summary>
    /// 设备操作抽象类
    /// </summary>
    public abstract class Device_Operate:IDeviceOperate
    {
        #region 公共属性
        public abstract Guid Identify { get; }

        public bool IsInitOK { get; private set; }

        public abstract IDeviceWarp Warp { get;}

        public Device_Config Config { get; private set; }

        public abstract EDeviceType SupportDeviceType { get; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public abstract string DeviceName { get; }

        /// <summary>
        /// 设备操作
        /// </summary>
        public virtual DriveOperate Drive { get; set; }

        public virtual ILogger Logger { get; }

        public bool OutterLogger { get; set; }

        public string LastError { get; set; }

        public virtual List<ModDeviceReadData> infos { get; set; } =  new List<ModDeviceReadData>();

        public virtual Dictionary<string, int> dicinfoindex { get; set; } = new Dictionary<string, int>();
        #endregion

        #region 私有变量
        /// <summary>
        /// 外部串口传入
        /// </summary>
        private bool outtercom = true;

        #region 静态变量
        /// <summary>
        /// 设备类型字典 [设备类型, 设备操作类型]
        /// </summary>
        private static Dictionary<string, Type> dicDeviceSuffix = null;

        ///// <summary>
        ///// 设备操作类型列表
        ///// </summary>
        //private static readonly List<Type> lstDeviceType = new List<Type>()
        // {
        //     typeof(ACsource_ANFS090A_Operate),
        //    typeof(ACDCInverter_DS91000_Operate),
        //     typeof(ACDCInverter_S7000H_Operate),
        //     typeof(DAQSysteam_DAQ970A_Operate),
        //     typeof(Gyroscopic_DingYu_Operate),
        //     //typeof(APU_DP800_Operate),
        //     typeof(APU_FTP3000_Operate),
        //     typeof(Product_CAN_Operate),
        //     typeof(DCLoad_DD0664_Operate),
        //     typeof(Relay_ZS_Operate),
        //     typeof(SaltFogbox_X1TK870_Operate),
        //     typeof(TempBox_X1TK8070_Operate),
        //     typeof(VibrationTable_SZH_Operate),
        //     typeof(WaterBox_MCx_Operate),
        // };

        /// <summary>
        /// 设备操作类型列表
        /// </summary>
        private static readonly  Dictionary<string,Type> lstDeviceType = new Dictionary<string, Type>()
         {
            //{"ANFS090A", typeof(ACsource_ANFS090A_Operate) },
            //{"DS91000" ,typeof(ACDCInverter_DS91000_Operate) },
            //{"S7000H", typeof(ACDCInverter_S7000H_Operate) },
            //{"DAQ970A", typeof(DAQSysteam_DAQ970A_Operate) },
            //{"AN87330", typeof(DAQSysteam_AN87330_Operate) },
            //{"DD0664",typeof(DCLoad_DD0664_Operate) },
            //{"DingYu", typeof(Gyroscopic_DingYu_Operate) },
            //{"DP800", typeof(APU_DP800_Operate) },
            //{"FTP1000", typeof(APU_FTP1000_Operate) },
            //{"FTP3000", typeof(APU_FTP3000_Operate) },
            //{"Product_CAN",typeof(Product_CAN_Operate) },
            //{"ZhongShen", typeof(Relay_ZS_Operate) },
            //{"X1TK870", typeof(SaltFogbox_X1TK870_Operate) },
            //{"X1TK8070", typeof(TempBox_X1TK8070_Operate) },
            //{"VTX1TK8070", typeof(TempBox_VTX1TK8070_Operate) },
            //{"DONGLING", typeof(VibrationTable_SZH_Operate) },
            //{"LabGenius", typeof(VibrationTable_LabGenius_Operate) },
            //{"LGSmart200", typeof(WaterBox_LGSmart200_Operate) },
            //{"MCx", typeof(WaterBox_MCx_Operate) },
         };

        #endregion
        #endregion

        #region 公共方法

        #region 设备初始化
        public virtual OperateResult Init(Device_Config obj, DriveOperate drive)
        {
            try
            {
                if (obj == null)
                {
                    LastError = $"设备配置信息为空,请传入参数为 {DeviceName} 类型的配置对象";
                    Logger.Error(nameof(Device_Operate), nameof(Init), LastError);
                    return OperateResult.Failed(LastError);
                }

                if (!obj.DeviceConfig.Name.Equals(DeviceName))
                {
                    LastError = $"设备配置信息异常,请传入参数为 {DeviceName} 类型的配置对象";
                    Logger.Error(nameof(Device_Operate), nameof(Init), LastError);
                    return OperateResult.Failed(LastError);
                }

                if (IsInitOK)
                {
                    UnInit();
                }

                //WPS1203H_Config config = (WPS1203H_Config)obj;

                OperateResult bRet = Warp.Device_Init(obj.DeviceConfig, drive);
                if (!bRet)
                {
                    LastError = bRet.Message;
                    Logger.Error(nameof(Device_Operate), nameof(Init), LastError);
                    return OperateResult.Failed($"● {DeviceName} 初始化失败！\r\n● {bRet.Message}\r\n\r\n▄ ▄ ▄ ▄ ▄ ▄ ▄ ▄ ▄ ▄ 请 看 上 面 ▄ ▄ ▄ ▄ ▄ ▄ ▄ ▄ ▄ ▄ ▄ ▄\r\n");
                }

                Config = obj;
                Drive = drive;
                IsInitOK = true;


                infos.Clear();
                dicinfoindex.Clear();

                ////传递设备数据至全局变量
                //for (int i = 0; i < SupportDeviceInfo.DeviceRecInfo[DeviceName]?.Count; i++)
                //{
                //    var value = SupportDeviceInfo.DeviceRecInfo[DeviceName][i];
                //    infos.Add(new ModDeviceReadData
                //    {
                //        DataInfo = new ModTestDataInfo { Name = value.DataInfo.Name},
                //        IsTrage = value.IsTrage,
                //        BelongTo = Config.DeviceConfig.BelongTo,
                //        DeviceTypename = SupportDeviceType.ToString(),
                //    });

                //    //SupportDeviceInfo.DeviceRecInfo[DeviceName][i].DeviceTypename = SupportDeviceType.ToString();
                //    //infos.Add();
                //    //infos[i].BelongTo = Config.DeviceConfig.BelongTo;
                //    dicinfoindex.Add(infos[i].DataInfo.Name, i);
                //}

                //Logger.Debug(nameof(Device_Operate), nameof(Init), $"设备{Identify}操作句柄初始化成功");

                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                return OperateResult.Failed($"● {DeviceName} 初始化失败！\r\n● 失败信息：{ex.Message}\r\n\r\n▄ ▄ ▄ ▄ ▄ ▄ ▄ ▄ ▄ ▄ 请 看 上 面 ▄ ▄ ▄ ▄ ▄ ▄ ▄ ▄ ▄ ▄ ▄ ▄\r\n");

            }
        }

        public virtual OperateResult UnInit()
        {
            if (IsInitOK)
            {
                Warp.Device_Uinit();
            }

            Warp?.Dispose();

            return OperateResult.Succeed();
        }
        #endregion

        #region 读取
        public abstract OperateResult<List<ModDeviceReadData>> Device_ReadAll();
        #endregion

        public void SetError(string error)
        {
            LastError = error;
        }
        #endregion


        #region 静态方法

        /// <summary>
        /// 创建驱动
        /// </summary>
        /// <param name="type">驱动类型</param>
        /// <returns></returns>
        public static OperateResult<IDeviceOperate> Create(EDeviceType type,string name) => Create(null, type, name);

        /// <summary>
        /// 创建驱动
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="type">驱动类型</param>
        /// <returns></returns>
        public static OperateResult<IDeviceOperate> Create(ILogger logger, EDeviceType type, string name)
        {
            if (dicDeviceSuffix == null)
            {
                InitDriveSuffixDictionary(name);
            }

            if (dicDeviceSuffix.ContainsKey(name))
            {
                if (Activator.CreateInstance(dicDeviceSuffix[name], new object[] { logger }) is IDeviceOperate device)
                {
                    return OperateResult<IDeviceOperate>.Succeed(device);
                }
                else
                {
                    return OperateResult<IDeviceOperate>.Failed(null, $"设备操作器创建失败！");
                }
            }

            return OperateResult<IDeviceOperate>.Failed(null, $"不支持的设备操作类型: {type}");
        }

        #region 私有方法
        /// <summary>
        /// 初始化Visa驱动字典
        /// </summary>
        private static void InitDriveSuffixDictionary(string Name)
        {
            dicDeviceSuffix = new Dictionary<string, Type>();

            foreach (var type in lstDeviceType)
            {
                if (!typeof(IDeviceOperate).IsAssignableFrom(type.Value)) { continue; }
                if (!(Activator.CreateInstance(type.Value) is IDeviceOperate device)) { continue; }

                if (!dicDeviceSuffix.ContainsKey(type.Key))
                {
                    dicDeviceSuffix.Add(type.Key, type.Value);
                }

                //if (!dicDeviceSuffix.ContainsKey(device.SupportDeviceType))
                //{
                //    dicDeviceSuffix.Add(device.SupportDeviceType, type.Value);
                //}

                device.Dispose();
            }
        }
        #endregion
        #endregion

        #region 构造方法
        public Device_Operate() : this(null)
        {

        }

        public Device_Operate(ILogger logger)
        {
            Logger = logger ?? SuperDHHLoggerManager.DeviceLogger;
            outtercom = false;
        }

        public Device_Operate(ILogger logger, DriveOperate drive)
        {
            Logger = logger ?? SuperDHHLoggerManager.DeviceLogger;
            Drive = drive ?? new DriveOperate();
            outtercom = drive == null;
        }
        #endregion

        #region 析构方法
        ~Device_Operate()
        {
            Dispose();
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose()
        {
            if (IsInitOK)
            {
                UnInit();
            }

            if (!outtercom)
            {
                Drive?.Dispose();
            }

            Warp?.Dispose();
        }

        #endregion
    }
}
