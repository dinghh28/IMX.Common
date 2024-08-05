using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device.Common
{
    /// <summary>
    /// 驱动接口类型
    /// </summary>
    public enum DriveType
    {
        /// <summary>
        /// 0-无驱动设备;
        /// </summary>
        NULL = 0,
        /// <summary>
        /// 1-GPIB
        /// </summary>
        GPIB = 1,
        /// <summary>
        /// 2-VXI
        /// </summary>
        VXI = 2,
        /// <summary>
        /// 3-GPIBVXI
        /// </summary>
        GPIBVXI = 3,
        /// <summary>
        /// 4-ASRL串口
        /// </summary>
        ASRL = 4,
        /// <summary>
        /// 5-PXI
        /// </summary>
        PXI = 5,
        /// <summary>
        /// 6-TCP
        /// </summary>
        LAN = 6,
        /// <summary>
        /// 7-USB
        /// </summary>
        USB = 7,
        /// <summary>
        /// 8-TCPIP
        /// </summary>
        TCPIP = 8,
        /// <summary>
        /// 9-CAN
        /// </summary>
        CAN = 9,
        /// <summary>
        /// 10-CANFD
        /// </summary>
        CANFD = 10,
        /// <summary>
        /// 11-MQTT
        /// </summary>
        MQTT = 11,
    }
}
