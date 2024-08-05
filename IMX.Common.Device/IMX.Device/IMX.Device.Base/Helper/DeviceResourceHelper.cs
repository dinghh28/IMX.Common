using Ivi.Visa;
using Super.Zoo.Framework;
using System;
using Device.Common;
using Piggy.VehicleBus.Device.ControlAPI;

namespace Device.Base
{
    public class DeviceResourceHelper
    {
        #region 常量(配置字符串中的常量KEY)
        //TimeoutMS=2000; BaudRate=9600; Parity=None; DataBits=8; StopBits=1; FlowControl=XOnXOff; Address=0;
        /// <summary>
        /// 奇偶性
        /// </summary>
        public const string Parity = "parity";
        /// <summary>
        /// 数据位
        /// </summary>
        public const string DataBits = "databits";
        /// <summary>
        /// 停止位
        /// </summary>
        public const string StopBits = "stopbits";
        /// <summary>
        /// 流控制
        /// </summary>
        public const string FlowControl = "flowcontrol";
        /// <summary>
        /// 硬件地址;
        /// </summary>
        public const string Address = "address";
        /// <summary>
        /// 是否使用原生串口
        /// </summary>
        public const string UseSerial = "useserial";
        #endregion

        /// <summary>
        /// 根据传入的完整的设备资源地址名称, 解析出通讯类型和对应参数;
        /// <para>串口示例：ASRL1::INSTR (参数1 = 串口号)</para>
        /// <para>GPIB示例：GPIB0::9::INSTR (参数1 = 地址号)</para>
        /// <para>USB示例：USB0::0x2A8D::0x0101::MY57501899::INSTR (参数1 = 0x厂家ID, 参数2 = 0x产品ID, 参数3 = 设备编号)</para>
        /// <para>TCPIP示例：TCPIP0::192.168.0.26::inst0::INSTR (参数1 = IP地址)</para>
        /// <para>CANFD示例：CANFD0::INSTR:(参数1 = IP地址)</para>//CANFD::设备::设备ID=设备类型::设备型号::设备序号
        /// </summary>
        /// <param name="resourceString">完整的设备资源地址名称</param>
        /// <param name="eType">解析得到的通讯类型</param>
        /// <param name="strParam1">解析得到的参数1</param>
        /// <param name="strParam2">解析得到的参数2</param>
        /// <param name="strParam3">解析得到的参数3</param>
        /// <returns></returns>
        public static OperateResult DecodeResourceString(string resourceString, ref DriveType eType, ref string strParam1, ref string strParam2, ref string strParam3)
        {
            eType = DriveType.NULL;
            strParam1 = string.Empty;
            strParam2 = string.Empty;
            strParam3 = string.Empty;
            string strErr;
            try
            {
                if (resourceString.IndexOf("CANFD") >= 0)
                {
                    eType = DriveType.CAN;
                    string strTemp = resourceString.Replace("CANFD::", "").Replace("CANFD0::", "").Replace("CANFD1::", "").Replace("CANFD2::", "").Replace("CANFD3::", "").Replace("CANFD4::", "").Replace("CANFD5::", "").Replace("::INSTR", "").Trim();
                    if (string.IsNullOrWhiteSpace(strTemp))
                    {
                        strErr = $"资源字符串[{resourceString}].参数不完整";
                        return OperateResult.Failed(strErr);
                    }
                    string[] strArray = resourceString.Split(new string[] { "::" }, StringSplitOptions.None);
                    if (strArray == null || strArray.Length != 4)
                    {
                        strErr = $"资源字符串[{resourceString}].参数个数 != 4";
                        return OperateResult.Failed(strErr);
                    }
                    strParam1 = strArray[0].Trim();
                    strParam2 = strArray[1].Trim();
                    strParam3 = strArray[2].Trim();
                    if (string.IsNullOrWhiteSpace(strParam1))
                    {
                        strErr = $"资源字符串[{resourceString}].参数1.CANFD设备类型为空";
                        return OperateResult.Failed(strErr);
                    }
                    if (string.IsNullOrWhiteSpace(strParam2))
                    {
                        strErr = $"资源字符串[{resourceString}].参数2.CANFD设备型号为空";
                        return OperateResult.Failed(strErr);
                    }
                    if (string.IsNullOrWhiteSpace(strParam3))
                    {
                        strErr = $"资源字符串[{resourceString}].参数3.CANFD设备序号为空";
                        return OperateResult.Failed(strErr);
                    }
                }
                //ASRL1::INSTR:COM1
                else if (resourceString.IndexOf("ASRL") >= 0)
                {
                    //char asrl = ':';
                    eType = DriveType.ASRL;
                    string strTemp = resourceString.Replace("ASRL", "").Replace("::INSTR", "").Replace(":", "").Trim();
                    if (string.IsNullOrWhiteSpace(strTemp))
                    {
                        strErr = $"资源字符串[{resourceString}].串口号没有填写";
                        return OperateResult.Failed(strErr);
                    }
                    strParam1 = resourceString;
                }
                else if (resourceString.IndexOf("GPIB") >= 0)
                {
                    eType = DriveType.GPIB;
                    string strTemp = resourceString.Replace("GPIB0::", "").Replace("GPIB1::", "").Replace("GPIB2::", "").Replace("::INSTR", "").Replace(":", "").Trim();
                    if (string.IsNullOrWhiteSpace(strTemp))
                    {
                        strErr = $"资源字符串[{resourceString}].地址号没有填写";
                        return OperateResult.Failed(strErr);
                    }
                    strParam1 = Convert.ToInt32(strTemp).ToString();
                }
                else if (resourceString.IndexOf("USB") >= 0)
                {
                    eType = DriveType.USB;
                    string strTemp = resourceString.Replace("USB0::", "").Replace("USB1::", "").Replace("USB2::", "").Replace("USB3::", "").Replace("USB4::", "").Replace("::INSTR", "").Trim();
                    if (string.IsNullOrWhiteSpace(strTemp))
                    {
                        strErr = $"资源字符串[{resourceString}].参数不完整";
                        return OperateResult.Failed(strErr);
                    }
                    string[] strArray = strTemp.Split(new string[] { "::" }, StringSplitOptions.None);
                    if (strArray == null || strArray.Length != 3)
                    {
                        strErr = $"资源字符串[{resourceString}].参数个数 != 3";
                        return OperateResult.Failed(strErr);
                    }
                    strParam1 = strArray[0].Trim();
                    strParam2 = strArray[1].Trim();
                    strParam3 = strArray[2].Trim();
                    if (string.IsNullOrWhiteSpace(strParam1))
                    {
                        strErr = $"资源字符串[{resourceString}].参数1.解析厂家ID为空";
                        return OperateResult.Failed(strErr);
                    }
                    if (string.IsNullOrWhiteSpace(strParam2))
                    {
                        strErr = $"资源字符串[{resourceString}].参数2.解析产品ID为空";
                        return OperateResult.Failed(strErr);
                    }
                }
                else if (resourceString.IndexOf("TCPIP") >= 0)
                {
                    eType = DriveType.TCPIP;
                    string strTemp = resourceString.Replace("TCPIP0::", "").Replace("TCPIP1::", "").Replace("TCPIP2::", "").Replace("TCPIP3::", "").Replace("TCPIP4::", "").Replace("::SOCKET", "");
                    if (string.IsNullOrWhiteSpace(strTemp))
                    {
                        strErr = $"资源字符串[{resourceString}].IP地址没有填写";
                        return OperateResult.Failed(strErr);
                    }

                    string[] strArray = strTemp.Split(new string[] { "::" }, StringSplitOptions.None);
                    if (strArray == null || strArray.Length <= 0)
                    {
                        strErr = $"资源字符串[{resourceString}].参数个数 <= 0";
                        return OperateResult.Failed(strErr);
                    }
                    strParam1 = strArray.Length > 0 ? strArray[0].Trim() : string.Empty;
                    strParam2 = strArray.Length > 1 ? strArray[1].Trim() : string.Empty;
                    strParam3 = strArray.Length > 2 ? strArray[2].Trim() : string.Empty;
                    if (string.IsNullOrWhiteSpace(strParam1))
                    {
                        strErr = $"资源字符串[{resourceString}].参数1.解析IP地址为空";
                        return OperateResult.Failed(strErr);
                    }

                    string IPRegex = @"^((25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(25[0-5]|2[0-4]\d|[01]?\d\d?)$";
                    if (!System.Text.RegularExpressions.Regex.IsMatch(strParam1, IPRegex))
                    {
                        strErr = $"资源字符串[{resourceString}].参数1.解析IP格式不正确";
                        return OperateResult.Failed(strErr);
                    }
                }
                //else if (resourceString.IndexOf("CAN") >= 0)
                //{

                //     }
                else
                {
                    strErr = $"资源字符串[{resourceString}].未找到\"ASRL\" || \"GPIB\" || \"USB\"|| \"TCPIP\"|| \"CAN\"|| \"CANFD\"|| \"MQTT\"";
                    return OperateResult.Failed(strErr);
                }

                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                strErr = ex.GetMessage(); ;
                return OperateResult.Failed(strErr);
            }
        }

        /// <summary>
        /// 根据传入的配置字符串, 解析出对应配置参数;(串口)
        /// <para>采用"配置名=某值"的方式 且 英文分号隔开</para>
        /// <para>串口通讯示例： BaudRate=9600; Parity=None; DataBits=8; StopBits=1; FlowControl=XOnXOff; </para>
        /// </summary>
        /// <param name="configString">配置字符串, 采用"配置名=某值"的方式 且 英文分号隔开</param>
        /// <param name="ASRL_Parity">串口通讯_校验位</param>
        /// <param name="ASRL_DataBits">串口通讯_数据位</param>
        /// <param name="ASRL_StopBits">串口通讯_停止位</param>
        /// <param name="ASRL_FlowControl">串口通讯_流控制</param>
        /// <param name="ASRL_Address">串口通讯_地址</param>
        /// <param name="IsUseSerial">串口通讯_是否使用原生串口</param>
        /// <returns></returns>
        public static OperateResult DecodeConfigString(string configString,
                                       ref SerialParity ASRL_Parity,
                                       ref int ASRL_DataBits,
                                       ref SerialStopBitsMode ASRL_StopBits,
                                       ref SerialFlowControlModes ASRL_FlowControl,
                                       ref int ASRL_Address,
                                       ref bool IsUseSerial)
        {
            string strErr = string.Empty;
            ASRL_Parity = SerialParity.None;                    // 串口通讯_校验位;
            ASRL_DataBits = 8;                                  // 串口通讯_数据位;
            ASRL_StopBits = SerialStopBitsMode.One;             // 串口通讯_停止位;
            ASRL_FlowControl = SerialFlowControlModes.None;  // 串口通讯_流控制;
            ASRL_Address = 0;
            IsUseSerial = false;
            try
            {
                string[] strArr = configString.Split(';');
                for (int i = 0; i < strArr.Length; i++)
                {
                    string strOne = strArr[i].Trim();
                    if (string.IsNullOrWhiteSpace(strOne) || strOne == ";") continue;

                    string[] oenArr = strOne.Split('=');
                    if (oenArr == null || oenArr.Length != 2)
                    {
                        strErr = $"配置字符串[{configString}].必须采用\"配置名 = 某值\"的方式 且 英文分号隔开";
                        return OperateResult.Failed(strErr);
                    }
                    string name = oenArr[0].Trim();
                    string value = oenArr[1].Trim();
                    if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(value))
                    {
                        strErr = $"配置字符串[{configString}].配置名或配置值未填写, 必须采用\"配置名 = 某值\"的方式 且 英文分号隔开";
                        return OperateResult.Failed(strErr);
                    }

                    //TimeoutMS=2000; BaudRate=9600; Parity=None; DataBits=8; StopBits=1; FlowControl=XOnXOff; Address=0;
                    switch (name.ToLower())
                    {
                        //case CNS_TimeoutMS: timeoutMS = Convert.ToInt32(value); break;
                        //case CNS_BaudRate:
                        //    {
                        //        ASRL_BaudRate = Convert.ToInt32(value);
                        //        List<int> listBaudRate = new List<int>() { 256000, 128000, 115200, 57600, 56000, 43000, 38400, 28800, 19200, 9600, 4800, 2400, 1200, 600, 300, 110 };
                        //        if (!listBaudRate.Contains(ASRL_BaudRate))
                        //        {
                        //            strErr = $"配置字符串[{configString}].[{strOne}].[BaudRate]配置值不正确";
                        //            return OperateResult.Failed(strErr);
                        //        }
                        //    }
                        //    break;
                        case Parity:
                            switch (value)
                            {
                                case "None": ASRL_Parity = SerialParity.None; break;
                                case "Odd": ASRL_Parity = SerialParity.Odd; break;
                                case "Even": ASRL_Parity = SerialParity.Even; break;
                                case "Mark": ASRL_Parity = SerialParity.Mark; break;
                                case "Space": ASRL_Parity = SerialParity.Space; break;
                                default:
                                    {
                                        strErr = $"配置字符串[{configString}].[{strOne}].[Parity]配置值只能为[None 或 Odd 或 Even 或 Mark 或 Space]";
                                        return OperateResult.Failed(strErr);
                                    };
                            }
                            break;
                        case DataBits:
                            switch (value)
                            {
                                case "6": ASRL_DataBits = 6; break;
                                case "7": ASRL_DataBits = 7; break;
                                case "8": ASRL_DataBits = 8; break;
                                default:
                                    {
                                        strErr = $"配置字符串[{configString}].[{strOne}].[DataBits]配置值只能为[6 或 7 或 8]";
                                        return OperateResult.Failed(strErr);
                                    };
                            }
                            break;
                        case StopBits:
                            switch (value)
                            {
                                case "One": ASRL_StopBits = SerialStopBitsMode.One; break;
                                case "OneAndOneHalf": ASRL_StopBits = SerialStopBitsMode.OneAndOneHalf; break;
                                case "Two": ASRL_StopBits = SerialStopBitsMode.Two; break;
                                default:
                                    {
                                        strErr = $"配置字符串[{configString}].[{strOne}].[StopBits]配置值只能为[1 或 1.5 或 2]";
                                        return OperateResult.Failed(strErr);
                                    };
                            }
                            break;
                        case FlowControl:
                            switch (value)
                            {
                                case "None": ASRL_FlowControl = SerialFlowControlModes.None; break;
                                case "XOnXOff": ASRL_FlowControl = SerialFlowControlModes.XOnXOff; break;
                                case "RtsCts": ASRL_FlowControl = SerialFlowControlModes.RtsCts; break;
                                case "DtrDsr": ASRL_FlowControl = SerialFlowControlModes.DtrDsr; break;
                                default:
                                    {
                                        strErr = $"配置字符串[{configString}].[{strOne}].[FlowControl]配置值只能为[None 或 XOnXOff 或 RtsCts 或 DtrDsr]";
                                        return OperateResult.Failed(strErr);
                                    };
                            }
                            break;
                        case Address: ASRL_Address = Convert.ToInt32(value); break;
                        case UseSerial: IsUseSerial = Convert.ToBoolean(value); break;
                        default:
                            {
                                strErr = $"配置字符串[{configString}].[{strOne}].传入了未知的配置名称";
                                return OperateResult.Failed(strErr);
                            };
                    }
                }

                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                strErr = ex.GetMessage();
                return OperateResult.Failed(strErr);
            }
        }

        /// <summary>
        /// 根据传入的配置字符串, 解析出对应配置参数;(MQTT)
        /// </summary>
        /// <param name="configString">配置字符串, 采用"配置名=某值"的方式 且 英文分号隔开</param>
        /// <param name="username">MQTT通讯_用户名</param>
        /// <param name="password">MQTT通讯_用户密码</param>
        /// <returns></returns>
        public static OperateResult DecodeConfigString(string configString, ref string username, ref string password)
        {
            string strErr = string.Empty;
            username = "Admin";                    // 用户名;
            password = "123456";                   // 用户密码

            string[] strArr = configString.Split(';');
            try
            {
                for (int i = 0; i < strArr.Length; i++)
                {
                    string strOne = strArr[i].Trim();
                    if (string.IsNullOrWhiteSpace(strOne) || strOne == ";") continue;

                    string[] oenArr = strOne.Split('=');
                    if (oenArr == null || oenArr.Length != 2)
                    {
                        strErr = $"配置字符串[{configString}].必须采用\"配置名 = 某值\"的方式 且 英文分号隔开";
                        return OperateResult.Failed(strErr);
                    }
                    string name = oenArr[0].Trim();
                    string value = oenArr[1].Trim();
                    if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(value))
                    {
                        strErr = $"配置字符串[{configString}].配置名或配置值未填写, 必须采用\"配置名 = 某值\"的方式 且 英文分号隔开";
                        return OperateResult.Failed(strErr);
                    }

                    switch (name.ToLower())
                    {
                        case "username":
                            username = value; break;
                        case "password":
                            password = value; break;
                        default:
                            {
                                strErr = $"配置字符串[{configString}].[{strOne}].传入了未知的配置名称";
                                return OperateResult.Failed(strErr);
                            };
                    }
                }

                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                strErr = ex.GetMessage();
                return OperateResult.Failed(strErr);
            }
        }


        /// <summary>
        /// 根据传入的配置字符串, 解析出对应配置参数;(CAN)
        /// </summary>
        /// <param name="configString">配置字符串, 采用"配置名=某值"的方式 且 英文分号隔开</param>
        /// <param name="ArbiBaudrate">仲裁波特率</param>
        /// <param name="DataBaudrate">数据波特率</param>
        /// <param name="CanfdSpeed">CANFD加速器</param>
        /// <returns></returns>
        public static OperateResult DecodeConfigString(string configString, ref string CanfdSpeed, ref string ArbiBaudrate, ref string DataBaudrate, ref uint ChannelIndex)
        {
            ArbiBaudrate = "500Kbps";
            DataBaudrate = "500Kbps";

            string strErr;
            try
            {
                string[] strArr = configString.Split(';');
                for (int i = 0; i < strArr.Length; i++)
                {
                    string strOne = strArr[i].Trim();
                    if (string.IsNullOrWhiteSpace(strOne) || strOne == ";") continue;

                    string[] oenArr = strOne.Split('=');
                    if (oenArr == null || oenArr.Length != 2)
                    {
                        strErr = $"配置字符串[{configString}].必须采用\"配置名 = 某值\"的方式 且 英文分号隔开";
                        return OperateResult.Failed(strErr);
                    }
                    string name = oenArr[0].Trim();
                    string value = oenArr[1].Trim();
                    if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(value))
                    {
                        strErr = $"配置字符串[{configString}].配置名或配置值未填写, 必须采用\"配置名 = 某值\"的方式 且 英文分号隔开";
                        return OperateResult.Failed(strErr);
                    }

                    switch (name.ToLower())
                    {
                        case "canfdspeed":
                            CanfdSpeed = value; break;
                        case "arbitbaudrate":
                            ArbiBaudrate = value; break;
                        case "databaudrate":
                            DataBaudrate = value; break;
                        case "channelindex":
                            ChannelIndex = Convert.ToUInt32(value); break;
                        default:
                            {
                                strErr = $"配置字符串[{configString}].[{strOne}].传入了未知的配置名称";
                                return OperateResult.Failed(strErr);
                            };
                    }
                }

                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                strErr = ex.GetMessage();
                return OperateResult.Failed(strErr);
            }
        }

        public static OperateResult DecodeConfigString(string configString, out string CanfdSpeed, out string DataBaudrate, out uint ChannelIndex) 
        {
            //ArbiBaudrate = "500Kbps";
            CanfdSpeed = "否";
            DataBaudrate = "500Kbps";
            ChannelIndex = 0;

            string strErr;
            try
            {
                string[] strArr = configString.Split(';');
                for (int i = 0; i < strArr.Length; i++)
                {
                    string strOne = strArr[i].Trim();
                    if (string.IsNullOrWhiteSpace(strOne) || strOne == ";") continue;

                    string[] oenArr = strOne.Split('=');
                    if (oenArr == null || oenArr.Length != 2)
                    {
                        strErr = $"配置字符串[{configString}].必须采用\"配置名 = 某值\"的方式 且 英文分号隔开";
                        return OperateResult.Failed(strErr);
                    }
                    string name = oenArr[0].Trim();
                    string value = oenArr[1].Trim();
                    if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(value))
                    {
                        strErr = $"配置字符串[{configString}].配置名或配置值未填写, 必须采用\"配置名 = 某值\"的方式 且 英文分号隔开";
                        return OperateResult.Failed(strErr);
                    }

                    switch (name.ToLower())
                    {
                        case "canfdspeed":
                            CanfdSpeed = value; break;
                        case "databaudrate":
                            DataBaudrate = value; break;
                        case "channelindex":
                            ChannelIndex = Convert.ToUInt32(value); break;
                        default:
                            {
                                strErr = $"配置字符串[{configString}].[{strOne}].传入了未知的配置名称";
                                return OperateResult.Failed(strErr);
                            };
                    }
                }

                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                strErr = ex.GetMessage();
                return OperateResult.Failed(strErr);
            }
        }

        public static OperateResult EncryptedResourceString(string drive, uint driveindex, ref string resourcestring)
        {
            resourcestring = $"ZLG_CANFD::{drive}::{driveindex}::INSTR";

            return OperateResult.Succeed();
        }

        /// <summary>
        /// 根据传入配置，生成配置字符串
        /// </summary>
        /// <param name="CanfdSpeed">CANFD加速器</param>
        /// <param name="DataBaudrate">数据波特率</param>
        /// <param name="ChannelIndex">通道地址</param>
        /// <param name="configString">配置字符串, 采用"配置名=某值"的方式 且 英文分号隔开</param>
        /// <returns></returns>
        public static OperateResult EncryptedConfigString(string CanfdSpeed, string DataBaudrate, uint ChannelIndex, ref string configString) 
        {
            configString = $"canfdspeed={CanfdSpeed};DataBaudrate={DataBaudrate};ChannelIndex={ChannelIndex}";

            return OperateResult.Succeed();
        }

        /// <summary>
        /// 根据传入配置，生成配置字符串
        /// </summary>
        /// <param name="ASRL_Parity">串口通讯_校验位</param>
        /// <param name="ASRL_DataBits">串口通讯_数据位</param>
        /// <param name="ASRL_StopBits">串口通讯_停止位</param>
        /// <param name="ASRL_FlowControl">串口通讯_流控制</param>
        /// <param name="UseSerial">串口通讯_是否使用原生串口</param>
        /// <param name="configString">配置字符串, 采用"配置名=某值"的方式 且 英文分号隔开</param>
        /// <returns></returns>
        public static OperateResult EncryptedConfigString(string ASRL_Parity, uint ASRL_DataBits, string ASRL_StopBits, string ASRL_FlowControl, bool UseSerial, ref string configString)
        {
            configString = $"Parity={ASRL_Parity};DataBits={ASRL_DataBits};StopBits={ASRL_StopBits};FlowControl={ASRL_FlowControl};Address=1;UseSerial={UseSerial}";

            return OperateResult.Succeed();
        }

        public static OperateResult EncryptedConfigString(string username, string password, ref string configString) 
        {
            configString = $"username={username};password={password}";

            return OperateResult.Succeed();
        }
    }
}
