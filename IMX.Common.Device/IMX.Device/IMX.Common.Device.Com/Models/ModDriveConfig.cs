using Piggy.VehicleBus.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device.Common
{
    /// <summary>
    /// 通讯配置
    /// </summary>
    public class ModDriveConfig
    {
        /// <summary>
        /// 超时时间
        /// </summary>
        public int TimeoutMS { get; set; }

        /// <summary>
        /// 写入之后延时多少毫秒再进行读取(默认50ms);
        /// </summary>
        public int BeforeReadDelayMS { get; set; } = 50;

        /// <summary>
        /// 是否识别单帧结束发送标识
        /// </summary>
        public bool TerminationCharacterEnabled { get; set; } = true;

        ///// <summary>
        ///// 是否使用原生串口
        ///// </summary>
        //public bool IsUseSerial { get; set; } = false;

        /// <summary>
        /// 波特率
        /// </summary>
        public string BaudRate { get; set; }

        /// <summary>
        /// 通讯类型
        /// </summary>
        public DriveType CommunicationType { get; set; }

        /// <summary>
        /// CAN设备类型
        /// </summary>
        public VehicleBusType BusType { get; set; }

        /// <summary>
        /// 资源字符
        /// </summary>
        public string ResourceString { get; set; }

        /// <summary>
        /// 配置字符串
        /// </summary>
        public string ConfigString { get; set; }
    }
}
