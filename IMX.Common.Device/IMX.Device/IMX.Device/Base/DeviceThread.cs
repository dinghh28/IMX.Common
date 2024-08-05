using Device.Common;
using IMX.Common.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Device.Base
{
    public class DeviceThread
    {
        /// <summary>
        /// 线程名字
        /// </summary>
        public string ThreadName { get; set; }

        /// <summary>
        /// 线程Guid
        /// </summary>
        public string ThreadID { get; set; }

        /// <summary>
        /// 是否开启线程通讯
        /// </summary>
        public bool IsStratCommunication { get; set; } = false;

        /// <summary>
        /// 是否接收数据
        /// </summary>
        public bool IsReceiveData { get; set; } = true;

        /// <summary>
        /// 是否运行线程
        /// </summary>
        public bool IsRunning { get; set; } = false;

        /// <summary>
        /// 是否为产品通讯
        /// </summary>
        public bool IsProduct { get; set; } = false;

        /// <summary>
        /// 产品序号
        /// </summary>
        public int ProductIndex { get; set; } = 0;

        /// <summary>
        /// 是否卸载设备
        /// </summary>
        public bool IsUinit { get; set; }

        /// <summary>
        /// 运行线程
        /// </summary>
        public Thread OprateThread { get; set; }
        /// <summary>
        /// 设备操作类
        /// </summary>
        public IDeviceOperate DeviceOperate { get; set; }
        /// <summary>
        /// 设备地址
        /// </summary>
        public string DeviceAddress { get; set; }
        
        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public EDeviceType DeviceType { get; set; }

        /// <summary>
        /// 线程单次运行结束延时(默认100ms)
        /// </summary>
        public int DelayTime { get; set; } = 100;
    }
}
