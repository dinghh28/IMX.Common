using Device.Common.Models;
using System;
using Super.Zoo.Framework;
using Super.Zoo.Framework.Logger;
using Device;

namespace IMX.Common.Device
{
    public interface IDeviceWarp :IIdentifiedDisposableLoggerObject
    {
        #region 公共属性

        /// <summary>
        /// 版本信息
        /// </summary>
        string Version { get; }

        /// <summary>
        /// 设备初始化状态
        /// </summary>
        bool IsInitOK { get; }

        /// <summary>
        /// 通讯接口
        /// </summary>
        DriveOperate Drive { get;}

        /// <summary>
        /// 设备配置信息
        /// </summary>
        ModDeviceConfig Config { get; }

        #endregion

        #region 公共方法

        /// <summary>
        /// 设备初始化
        /// </summary>
        /// <param name="config">设备配置信息</param>
        /// <param name="com">串口</param>
        /// <returns></returns>
        OperateResult Device_Init(ModDeviceConfig config, DriveOperate drive = null);

        /// <summary>
        /// 设备卸载
        /// </summary>
        /// <returns></returns>
        OperateResult Device_Uinit();
        #endregion
    }
}
