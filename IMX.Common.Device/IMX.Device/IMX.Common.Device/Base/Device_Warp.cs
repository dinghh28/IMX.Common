using Device.Common.Models;
using System;
using Super.Zoo.Framework;
using Super.Zoo.Framework.Logger;
using IMX.Common.Device;
using IMX.Common.Logger;


namespace Device
{
    public abstract class Device_Warp :IDeviceWarp
    {
        #region 公共属性
        public abstract Guid Identify { get; }

        public abstract string Version { get; }

        public virtual bool IsInitOK { get; protected private set; } = false;

        public virtual ModDeviceConfig Config { get;protected private set; }

        public virtual DriveOperate Drive { get; protected private set; }

        public bool OutterLogger { get; set; }

        public string LastError { get; set; }

        public virtual ILogger Logger { get; }
        #endregion

        #region 私有变量
        /// <summary>
        /// 使用传入驱动
        /// </summary>
        private bool OutterDrive = false;
        #endregion

        #region 构造方法
        public Device_Warp() : this(null)
        {
        }

        public Device_Warp(ILogger logger)
        {
            Logger = logger ?? SuperDHHLoggerManager.DeviceLogger;
            OutterLogger = logger == null;
        }

        #endregion 

        #region 公共方法

        #region 串口操作
        public virtual OperateResult Device_Init(ModDeviceConfig config, DriveOperate drive = null)
        {
            Config = config;

            //串口初始化
            if (drive == null)
            {
                drive = new DriveOperate();
                OperateResult result = drive.Open(Config.DriveConfig);
                if (!result)
                {
                    LastError = result.Message;
                    Logger.Error(nameof(Device_Warp), nameof(Device_Init), LastError);
                    return OperateResult.Failed(LastError);
                }
                OutterDrive = false;
            }
            else
            {
                OutterDrive = true;
            }

            if (!drive.Drive.IsDriveOpen)
            {
                LastError = $"串口设备未打开，请确定串口";
                Logger.Error(nameof(Device_Warp), nameof(Device_Init), LastError);
                return OperateResult.Failed(LastError);
            }

            Drive = drive;
            IsInitOK = true;
            Logger.Info(nameof(Device_Warp), nameof(Device_Init), $"设备{Identify}初始化成功");
            return OperateResult.Succeed();
        }

        public virtual OperateResult Device_Uinit()
        {
            if (!OutterDrive)
            {
                Drive?.Dispose();
                Drive = null;
            }

            IsInitOK = false;
            Logger.Info(nameof(Device_Warp), nameof(Device_Uinit), $"设备{Identify}卸载成功！");
            return OperateResult.Succeed();
        }

        public virtual void Device_Uinit(bool isUinit) 
        {
            if (IsInitOK) { Device_Uinit(); }
            //Device_Uinit();
        }
        #endregion

        public  void SetError(string error)
        {
            LastError = error;
        }
        #endregion

        #region 析构方法
        ~Device_Warp()
        {
            Dispose();
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose()
        {
            Device_Uinit(IsInitOK);
            //if (IsInitOK) { Device_Uinit(); }

            //if (!OutterLogger) { Logger.Dispose(); }
        }
        #endregion
    }
}
