using System.Xml.Serialization;

namespace Device.Common.Models
{
    /// <summary>
    /// 设备信息
    /// </summary>
    public class ModDeviceConfig
    {
        /// <summary>
        /// 设备类型
        /// </summary>
        [XmlAttribute("Type")]
        public EDeviceType DeviceType { get; set; }

        /// <summary>
        /// 设备型号\名称
        /// </summary>
        [XmlAttribute("Name")]
        public string Name { get; set; }

        /// <summary>
        /// 硬件地址
        /// </summary>
        public int Address { get; set; }

        /// <summary>
        /// 所属工位(-1则所有工位共享)
        /// </summary>
        public int BelongTo { get; set; }

        /// <inheritdoc/>
        public ModDriveConfig DriveConfig { get; set; }
    }
}
