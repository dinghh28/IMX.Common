using Device;
using Device.Common;
using IMX.Common.Device.Models;
using Super.Zoo.Framework;
using Super.Zoo.Framework.Logger;
using System;
using System.Collections.Generic;

namespace IMX.Common.Device
{
    public interface IDeviceOperate : ILogRecord, IIdentify, IDisposable
    {
        #region 属性
        /// <summary>
        /// 初始化状态
        /// </summary>
        bool IsInitOK { get; }

        /// <summary>
        /// 设备报文
        /// </summary>
        IDeviceWarp Warp { get; }

        /// <summary>
        /// 设备配置
        /// </summary>
        //IDeviceConfig Config { get; }
        Device_Config Config { get; }

        /// <summary>
        /// 设备类型
        /// </summary>
        EDeviceType SupportDeviceType { get; }

        /// <summary>
        /// 设备型号编号
        /// </summary>
        string DeviceName { get; }

        /// <summary>
        /// 设备通讯接口信息
        /// </summary>
        DriveOperate Drive { get; set; }

        //Dictionary<string, List<ModDeviceReadData>> DeviceRecInfo { get; set; }

        /// <summary>
        /// 设备读取内容
        /// </summary>
        List<ModDeviceReadData> infos { get; set; }

        /// <summary>
        /// 设备读取内容对应index地址
        /// </summary>
        Dictionary<string, int> dicinfoindex { get; set; }

        //EDeviceRunState DeviceRunState { get; }
        #endregion

        #region 方法
        /// <summary>
        /// 设备初始化
        /// </summary>
        /// <returns></returns>
        //OperateResult Init(Object obj);
        OperateResult Init(Device_Config obj, DriveOperate drive);

        /// <summary>
        /// 设备卸载
        /// </summary>
        /// <returns></returns>
        OperateResult UnInit();
        /// <summary>
        /// 读取设备信息
        /// </summary>
        /// <returns></returns>
        OperateResult<List<ModDeviceReadData>> Device_ReadAll();
        #endregion
    }
}
