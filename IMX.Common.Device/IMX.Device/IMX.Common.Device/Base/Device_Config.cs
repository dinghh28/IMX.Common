using Device.Common;
using Device.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device
{
    public class Device_Config : BaseConfig
    {
        /// <summary>
        /// 设备名称
        /// </summary>
        public string Device_Name { get; set; } = "Device";

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="path">完整全新的配置文件全路径名称</param>
        public Device_Config(string path = "", bool isfullpath = false)
            : base(path)
        {
            if (string.IsNullOrEmpty(path)) 
            {
                path = $@"{StartupPath}\Config\ConfigDevice\{Device_Name}_0.xml";//StartupPath + string.Format("\\Config\\ConfigDevice\\{Device_Name}_0.xml");
            }
            else
            {
                path = isfullpath ? path: $@"{StartupPath}Config\ConfigDevice\{path}.xml";
            }
            base.XmlPath = path;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="index">索引第几个设备, 例如传参0表示"\\ConfigDevice\\DCSource_DS1000_0.ini"</param>
        public Device_Config(int index = 0)
          : base("")
        {
            string path = index < 0 ? $@"{StartupPath}\Config\ConfigDevice\{Device_Name}_0.xml" : $@"{StartupPath}\Config\ConfigDevice\{Device_Name}_{index}.xml";
            //StartupPath + string.Format("\\Config\\ConfigDevice\\AM_0.xml") : BaseConfig.StartupPath + string.Format($"\\Config\\ConfigDevice\\AM_{index}.xml");
            base.XmlPath = path;
        }

        /// <summary>
        /// 配置信息
        /// </summary>
        public ModDeviceConfig DeviceConfig
        {
            get => base.GetSections<ModDeviceConfig>();
            set => base.WriteXml<ModDeviceConfig>(value);
        }
    }
}
