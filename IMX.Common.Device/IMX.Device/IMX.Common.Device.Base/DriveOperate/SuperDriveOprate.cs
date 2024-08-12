using Device.Common;
using IMX.Common.Logger;
using Super.Zoo.Framework;
using Super.Zoo.Framework.Logger;
using System;
using System.Collections.Generic;

namespace Device.Base.Operate
{
    /// <summary>
    /// 驱动操作类
    /// </summary>
    public abstract class SuperDriveOprate : ISuperDrive
    {
        /// <summary>
        /// 是否清空缓存区
        /// </summary>
        public bool IsClearBuffer { get; set; } = false;

        public bool IsDriveOpen { get; private protected set; } = false;

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

        public virtual int ReOpen_MaxWriteFailCount { get; set; } = 3;
        public virtual int ReOpen_CurWriteFailCount { get; set; } = 3;
        public virtual int ReOpen_MinIntervalMS { get; set; } = 10000;

        public virtual bool IsWriteCmdWithNewLine { get; set; } = false;

        public virtual string WriteCmdWithNewLine { get; set; } = "\r\n";

        /// <summary>
        /// 每次读取之间延时毫秒(默认100ms)
        /// </summary>
        public virtual int DelayMS_Read_ByteArray { get; set; } = 100;


        public abstract DriveType SupportFuncitonType { get; }

        #region 私有变量
        /// <summary>
        /// 试验配置类型字典 [试验类型, 试验配置文件类型]
        /// </summary>
        private static Dictionary<DriveType, Type> dicDriveSuffix = null;

        /// <summary>
        /// 试验配置文件类型列表
        /// </summary>
        private static readonly List<Type> lstDriveType = new List<Type>
        {
            typeof(ASRLOprate),
            typeof(GPIBOprate),
            typeof(TCPIPOprate),
            typeof(USBOprate),
            typeof(DriveCANOprate)
        };
        #endregion

        #region 静态方法

        /// <summary>
        /// 创建驱动
        /// </summary>
        /// <param name="type">驱动类型</param>
        /// <returns></returns>
        public static OperateResult<ISuperDrive> Create(DriveType type) => Create(null, type);

        /// <summary>
        /// 创建驱动
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="type">驱动类型</param>
        /// <returns></returns>
        public static OperateResult<ISuperDrive> Create(ILogger logger, DriveType type)
        {
            if (dicDriveSuffix == null)
            {
                InitDriveSuffixDictionary();
            }

            if (dicDriveSuffix.ContainsKey(type))
            {
                if (Activator.CreateInstance(dicDriveSuffix[type], new object[] { logger }) is ISuperDrive drive)
                {
                    return OperateResult<ISuperDrive>.Succeed(drive);
                }
                else
                {
                    return OperateResult<ISuperDrive>.Failed(null, $"试验方案配置文件创建失败！");
                }
            }

            return OperateResult<ISuperDrive>.Failed(null, $"不支持的配置类型: {type}");
        }

        #region 私有方法
        /// <summary>
        /// 初始化Visa驱动字典
        /// </summary>
        private static void InitDriveSuffixDictionary()
        {
            dicDriveSuffix = new Dictionary<DriveType, Type>();

            foreach (var type in lstDriveType)
            {
                if (!typeof(ISuperDrive).IsAssignableFrom(type)) { continue; }
                if (!(Activator.CreateInstance(type) is ISuperDrive drive)) { continue; }


                if (!dicDriveSuffix.ContainsKey(drive.SupportFuncitonType))
                {
                    dicDriveSuffix.Add(drive.SupportFuncitonType, type);
                }

                drive.Dispose();
            }
        }
        #endregion
        #endregion

        #region 公共方法
        public abstract OperateResult Close();
        public abstract OperateResult Open(ModDriveConfig config);

        public void SetError(string error)
        {
            LastError = error;
        }
        #endregion

        #region 构造方法

        public SuperDriveOprate() : this(null) { }


        public SuperDriveOprate(ILogger logger)
        {
            Logger = logger ?? SuperDHHLoggerManager.DeviceLogger;
            OutterLogger = logger == null;
        }
        #endregion

        #region 析构方法
        ~SuperDriveOprate() 
        {
            Dispose();
        }

        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
