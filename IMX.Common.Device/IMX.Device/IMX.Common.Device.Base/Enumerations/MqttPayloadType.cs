using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device.Base.Enumerations
{
    /// <summary>
    /// MQTT发布消息格式
    /// </summary>
    public enum MqttPayloadType
    {
        JSON,
        PLAINTEXT,
        BASE64,
        HEX
    }
}
