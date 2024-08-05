using Device.Base;
using Device.Base.Operate;
using Device.Common;
using Device.Common.Models;
using IMX.Common.Device;
using IMX.Logger;
using Piggy.VehicleBus.Device;
using Super.Zoo.Framework;
using Super.Zoo.Framework.Logger;

namespace Device
{
    public class DriveOperate : ILogRecord, IDisposable
    {
        #region 公共属性
        /// <summary>
        /// 设备通讯接口
        /// </summary>
        public ISuperDrive Drive { get; set; }

        /// <summary>
        /// 通讯接口信息
        /// </summary>
        public ModDriveConfig DriveConfig { get; set; }

        /// <summary>
        /// 日志记录器
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// 使用传入日志记录器
        /// </summary>
        public bool OutterLogger { get; set; }

        /// <summary>
        /// 最后一次故障信息
        /// </summary>
        public string LastError { get; set; }

        public List<IDeviceOperate> Devices => dicDeviceOperate.Values.ToList();
        #endregion

        #region 私有变量
        /// <summary>
        /// 设备操作锁对象
        /// </summary>
        private static readonly object deviceOperation = new object();

        /// <summary>
        /// 设备操作器字典
        /// </summary>
        private readonly Dictionary<string, IDeviceOperate> dicDeviceOperate = new Dictionary<string, IDeviceOperate>();

        ///// <summary>
        ///// 设备字典
        ///// </summary>
        //private readonly Dictionary<string, IDeviceOperate> dicDevice = new Dictionary<string, IDeviceOperate>();

        ///// <summary>
        ///// 试验配置类型字典 [试验类型, 试验配置文件类型]
        ///// </summary>
        //private static Dictionary<DriveType, Type> dicDriveSuffix = null;

        ///// <summary>
        ///// 试验配置文件类型列表
        ///// </summary>
        //private static readonly List<Type> lstDriveType = new List<Type>
        //{
        //    typeof(ASRLOprate),
        //    typeof(GPIBOprate),
        //    typeof(TCPIPOprate),
        //    typeof(USBOprate),
        //    typeof(DriveCANOprate)
        //};
        #endregion

        #region 静态方法
        public static DriveOperate Creat() => Creat(null);

        public static DriveOperate Creat(ILogger logger) => new DriveOperate(logger);
        #endregion

        #region 公共方法

        #region 串口操作
        public OperateResult Open(ModDriveConfig dirveconfig)
        {
            //switch (comconfig.CommunicationType)
            //{
            //    case DriveType.GPIB:
            //        Drive =GPIBOprate.Create(Logger);
            //        break;
            //    case DriveType.ASRL:
            //        Drive = ASRLOprate.Create(Logger);
            //        break;
            //    case DriveType.LAN:
            //        Drive = TCPIPOprate.Create(Logger);
            //        break;
            //    case DriveType.USB:
            //        Drive = USBOprate.Create(Logger);
            //        break;
            //    case DriveType.CAN:
            //        Drive = DriveCANOprate.Create(Logger);
            //        break;
            //    case DriveType.PXI:
            //    case DriveType.GPIBVXI:
            //    case DriveType.VXI:
            //    case DriveType.NULL:
            //    default:
            //        LastError = $"串口设备类型{comconfig.CommunicationType}暂不可使用";
            //        Logger.Error(nameof(DriveOperate), nameof(Open), LastError);
            //        return OperateResult.Failed(LastError);
            //}

            var Rlt = SuperDriveOprate.Create(dirveconfig.CommunicationType);
            if (!Rlt)
            {
                LastError = Rlt.Message;
                Logger.Error(nameof(DriveOperate), nameof(Open), LastError);
                return OperateResult.Failed(LastError);
            }

            Drive = Rlt.Data;

            DriveConfig = dirveconfig;
            return Drive.Open(dirveconfig);
        }

        public OperateResult Close()
        {
            RemoveAllDevice();
            return Drive.Close();
        }
        #endregion

        #region 注册设备
        /// <summary>
        /// 注册设备
        /// </summary>
        /// <param name="guid">设备操作接口guid</param>
        /// <param name="resourcestring">设备串口资源信息</param>
        /// <returns></returns>
        public OperateResult<IDeviceOperate> RegisterDevice(ModDeviceConfig deviceConfig)
        {
            if (!deviceConfig.DriveConfig.ResourceString.Equals(DriveConfig.ResourceString))
            {
                LastError = $"设备注册失败：串口配置信息与设备不一致\r\n串口资源字符--{deviceConfig.DriveConfig.ResourceString}\r\n设备资源字符--{DriveConfig.ResourceString}";
                Logger.Error(nameof(DriveOperate), nameof(RegisterDevice), LastError);
                return OperateResult<IDeviceOperate>.Failed(null, LastError);
            }
            try
            {
                return AddDevice(Device_Operate.Create(Logger, deviceConfig.DeviceType, deviceConfig.Name).Data);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();

                Logger.Exception(nameof(DriveOperate), nameof(RegisterDevice), ex);
                return OperateResult<IDeviceOperate>.Failed(null, LastError);
            }
        }

        /// <summary>
        /// 设备注销
        /// </summary>
        /// <param name="guid">设备操作接口guid</param>
        /// <returns></returns>
        public OperateResult UnregisterDevice(Guid guid)
        {
            lock (dicDeviceOperate) 
            {
                if (!dicDeviceOperate.ContainsKey(guid.ToString()))
                {
                    LastError = $"未存在设备 {guid}";

                    Logger.Error(nameof(DriveOperate), nameof(UnregisterDevice), LastError);
                    return OperateResult.Failed(LastError);
                }
            }
            //if (DeviceList.FirstOrDefault(x => x == guid) == null)
            //{
            //    LastError = $"设备注销失败：设备{guid}未注册";
            //    Logger.Error(nameof(ComDeviceOprate), nameof(UnregisterDevice), LastError);
            //    return OperateResult.Failed(LastError);
            //}
            dicDeviceOperate.Remove(guid.ToString());
            //DeviceList.Remove(guid);
            return OperateResult.Succeed();
        }


        /// <summary>
        /// 移除所有设备
        /// </summary>
        /// <returns>操作结果</returns>
        public OperateResult RemoveAllDevice()
        {
            try
            {
                lock (dicDeviceOperate)
                {
                    foreach (var device in dicDeviceOperate.Values)
                    {
                        device.Dispose();
                    }

                    dicDeviceOperate.Clear();
                }
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();

                Logger.Exception(nameof(VehicleBusDevice), nameof(RemoveAllDevice), ex);
                return OperateResult.Excepted(ex);
            }

            return OperateResult.Succeed();
        }

        /// <summary>
        /// 获取设备列表
        /// </summary>
        /// <returns></returns>
        //public OperateResult<List<Guid>> GetDeviceList()
        //{
        //    return OperateResult<List<Guid>>.Succeed(DeviceList);
        //}

        public OperateResult<List<IDeviceOperate>> GetDeviceList() => OperateResult<List<IDeviceOperate>>.Succeed(dicDeviceOperate.Values.ToList());

        public void SetError(string error)
        {
            LastError = error;
        }


        #endregion
        #endregion

        #region 私有方法

        #region 设备注册
        private OperateResult<IDeviceOperate> AddDevice(IDeviceOperate device) 
        {
            if (device == null)
            {
                LastError = $"设备对象 {nameof(device)} 为空！";

                Logger.Error(nameof(DriveOperate), nameof(AddDevice), LastError);
                return OperateResult<IDeviceOperate>.Failed(null, LastError);
            }

            try
            {
                lock (deviceOperation)
                {
                    if (dicDeviceOperate.ContainsKey(device.Identify.ToString()))
                    {
                        LastError = $"已存在设备 {device.Identify}";

                        Logger.Error(nameof(DriveOperate), nameof(AddDevice), LastError);
                        return OperateResult<IDeviceOperate>.Failed(null, LastError);
                    }

                    dicDeviceOperate.Add(device.Identify.ToString(), device);
                }
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();

                Logger.Exception(nameof(DriveOperate), nameof(AddDevice), ex);
                return OperateResult<IDeviceOperate>.Failed(null, LastError);
            }

            return OperateResult<IDeviceOperate>.Succeed(device);
        }
        #endregion


        #endregion

        #region 构造函数
        public DriveOperate():this(null)
        {
        }

        public DriveOperate(ILogger logger)
        {
            Logger = logger ?? SuperDHHLoggerManager.DeviceLogger;
        }
        #endregion

        #region 析构函数
        ~DriveOperate() 
        {
            Dispose();
            
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool isDispose) 
        {
            if (!isDispose)
            {
                return;
            }

            RemoveAllDevice();

            Drive?.Dispose();
            Drive = null;

        }
        #endregion
    }
}
