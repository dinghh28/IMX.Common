using Device.Common;
using MQTTnet.Client;
using MQTTnet;
using Piggy.VehicleBus.Device;
using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MQTTnet.Diagnostics;
using MQTTnet.Exceptions;
using MQTTnet.Server;
using Piggy.VehicleBus.OperateInterface;
using MQTTnet.Protocol;
using Device.Base.Enumerations;

namespace Device.Base.Operate
{
    public class MQTTOprate : SuperDriveOprate
    {
        #region 公共属性

        public override DriveType SupportFuncitonType => DriveType.MQTT;

        /// <summary>
        /// MQTT服务端
        /// </summary>
        public MQTTnet.Client.MqttClient MqttClient { get; private set; }

        /// <summary>
        /// 订阅信息列表
        /// </summary>
        public List<string> TopicList { get; private set; } = new List<string>();
        #endregion

        #region 事件

        #endregion

        #region 通讯操作
        public override OperateResult Close()
        {
            try
            {
                for (Int32 i = 0; i < TopicList.Count; i++)
                {
                    UnsubscribeTopic(TopicList[i]);
                }

                TopicList?.Clear();
                MqttClient?.DisconnectAsync();

                return OperateResult.Succeed();
                //if (!result)
                //{
                //    LastError = result.Message;
                //    Logger.Error(nameof(DriveCANOprate), nameof(Close), LastError);
                //    return OperateResult.Failed(LastError);
                //}
                //result = DeviceOperator.Close();

                //IsDriveOpen = false;
                //return result;
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Exception(nameof(MQTTOprate), nameof(Close), ex);
                return OperateResult.Failed(LastError);
            }
            finally
            {
                //result = DeviceOperator.Close();
                IsDriveOpen = false;
                MqttClient?.Dispose();
            }
        }

        public override OperateResult Open(ModDriveConfig config)
        {
            if (IsDriveOpen)
            {
                LastError = $"设备已打开，请勿重复操作";
                Logger.Error(nameof(MQTTOprate), nameof(Open), LastError);
                return OperateResult.Failed(LastError);
            }

            if (config == null)
            {
                LastError = $"资源字符不存在";
                Logger.Error(nameof(MQTTOprate), nameof(Open), LastError);
                return OperateResult.Failed(LastError);
            }

            if (string.IsNullOrWhiteSpace(config.ResourceString))
            {
                LastError = $"资源字符串为空";
                Logger.Error(nameof(MQTTOprate), nameof(Open), LastError);
                return OperateResult.Failed(LastError);
            }
            if (config.ResourceString.IndexOf("::INSTR") < 0 && config.ResourceString.IndexOf("::SOCKET") < 0)
            {
                LastError = $"资源字符串为必须是以[::INSTR]结尾";
                Logger.Error(nameof(MQTTOprate), nameof(Open), LastError);
                return OperateResult.Failed(LastError);
            }

            #region 参数resourceString的解析
            DriveType eType = DriveType.NULL;

            string strParam1 = string.Empty;//ip地址
            string strParam2 = string.Empty;//端口号
            string strParam3 = string.Empty;//无效
            OperateResult bRet = DeviceResourceHelper.DecodeResourceString(config.ResourceString, ref eType, ref strParam1, ref strParam2, ref strParam3);
            if (!bRet)
            {
                LastError = bRet.Message;
                Logger.Error(nameof(MQTTOprate), nameof(Open), LastError);
                return OperateResult.Failed(LastError);
            }

            if (string.IsNullOrEmpty(strParam2))
            {
                LastError = "端口号为空";
                Logger.Error(nameof(MQTTOprate), nameof(Open), LastError);
                return OperateResult.Failed(LastError);
            }

            if (!Int32.TryParse(strParam2, out int port))
            {
                LastError = $"端口[{strParam2}]传参异常";
                Logger.Error(nameof(MQTTOprate), nameof(Open), LastError);
                return OperateResult.Failed(LastError);
            }

            //if (eType != DriveType.CAN)//&& eType != DriveType.CANFD)
            //{
            //    LastError = $"串口类型不匹配";
            //    Logger.Error(nameof(DriveCANOprate), nameof(Open), LastError);
            //    return OperateResult.Failed(LastError);s 
            //}
            #endregion

            #region 参数ConfigString的解析

            string username = string.Empty;
            string password = string.Empty;

            bRet = DeviceResourceHelper.DecodeConfigString(config.ConfigString, ref username, ref password);
            if (!bRet)
            {
                LastError = bRet.Message;
                Logger.Error(nameof(MQTTOprate), nameof(Open), LastError);
                return OperateResult.Failed(LastError);
            }
            #endregion
            try
            {
                ////打开设备
                //OperateResult result = DeviceOperator.Open<ZLGCANFDDevice>(DeviceArgs);
                //if (!result)
                //{
                //    LastError = result.Message;
                //    Logger.Error(nameof(DriveCANOprate), nameof(Open), LastError);
                //    return OperateResult.Failed(LastError);
                //}

                ////启动接收线程
                //result = DeviceOperator.StartReceive();
                //if (!result)
                //{
                //    LastError = result.Message;
                //    Logger.Error(nameof(DriveCANOprate), nameof(Open), LastError);
                //    return OperateResult.Failed(LastError);
                //}
                //IsDriveOpen = true;
                return Connect(strParam1, port, username,password);
            }
            catch (Exception ex)
            {
                IsDriveOpen = false;
                MqttClient?.Dispose();
                LastError = ex.GetMessage();
                Logger.Exception(nameof(MQTTOprate), nameof(Open), ex);
                return OperateResult.Failed(LastError);
            }
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 订阅主题
        /// </summary>
        /// <param name="topicname">主题名称</param>
        /// <returns></returns>
        public OperateResult SubscribeTopic(string topicname) 
        {
            if (!IsDriveOpen)
            {
                LastError = $"设备未打开";
                Logger.Error(nameof(MQTTOprate), nameof(SubscribeTopic), LastError);
                return OperateResult.Failed(LastError);
            }

            if (TopicList.Contains(topicname))
            {
                LastError = $"主题[{topicname}]已订阅,请勿重复订阅";
                Logger.Error(nameof(MQTTOprate), nameof(SubscribeTopic), LastError);
                return OperateResult.Failed(LastError);
            }

            try
            {
                MqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topicname).Build());
                TopicList.Add(topicname);

                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Exception(nameof(MQTTOprate), nameof(SubscribeTopic), ex);
                return OperateResult.Failed(LastError);
            }
            
        }

        /// <summary>
        /// 取消主题订阅
        /// </summary>
        /// <param name="topicname">主题名称</param>
        /// <returns></returns>
        public OperateResult UnsubscribeTopic(string topicname) 
        {
            if (!IsDriveOpen)
            {
                LastError = $"设备未打开";
                Logger.Error(nameof(MQTTOprate), nameof(UnsubscribeTopic), LastError);
                return OperateResult.Failed(LastError);
            }

            if (!TopicList.Contains(topicname))
            {
                LastError = $"主题[{topicname}]未订阅";
                Logger.Error(nameof(MQTTOprate), nameof(UnsubscribeTopic), LastError);
                return OperateResult.Failed(LastError);
            }
            
            try
            {
                MqttClient.UnsubscribeAsync(topicname);
                TopicList.Remove(topicname);

                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Exception(nameof(MQTTOprate), nameof(SubscribeTopic), ex);
                return OperateResult.Failed(LastError);
            }
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns></returns>
        public OperateResult PublishMessage(MQTTPublishMessage message) 
        {
            if (!IsDriveOpen)
            {
                LastError = $"设备未打开";
                Logger.Error(nameof(MQTTOprate), nameof(PublishMessage), LastError);
                return OperateResult.Failed(LastError);
            }

            if (message == null)
            {
                LastError = $"发送消息不可为空";
                Logger.Error(nameof(MQTTOprate), nameof(PublishMessage), LastError);
                return OperateResult.Failed(LastError);
            }

            if (!TopicList.Contains(message.Topic))
            {
                LastError = $"主题[{message.Topic}]未订阅";
                Logger.Error(nameof(MQTTOprate), nameof(PublishMessage), LastError);
                return OperateResult.Failed(LastError);
            }

            try
            {
                var mqttAMB = new MqttApplicationMessageBuilder().WithQualityOfServiceLevel(message.QosLevel).WithTopic(message.Topic).WithRetainFlag(true);

                switch (message.PayloadType)
                {
                    case MqttPayloadType.JSON:
                    case MqttPayloadType.PLAINTEXT:
                        mqttAMB.WithPayload(message.Message);
                        break;
                    case MqttPayloadType.BASE64:
                        mqttAMB.WithPayload(Convert.ToBase64String(Encoding.Default.GetBytes(message.Message)));
                        break;
                    case MqttPayloadType.HEX:
                        try
                        {
                            var hexstrs = message.Message.ToLower().Replace("0x", " ").Split(' ');
                            List<byte> hex = new List<byte>();
                            for (int i = 0; i < hexstrs.Length; i++)
                            {
                                hex.Add(Convert.ToByte(hexstrs[i]));
                            }
                            mqttAMB.WithPayload(hex);
                        }
                        catch (Exception ex)
                        {
                            LastError = ex.GetMessage();
                            Logger.Error(nameof(MQTTOprate), nameof(PublishMessage), LastError);
                            return OperateResult.Failed(LastError);
                        }
                        break;
                }

                MqttClient.PublishAsync(mqttAMB.Build());

                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(MQTTOprate), nameof(PublishMessage), LastError);
                return OperateResult.Failed(LastError);
            }
        }
        #endregion

        #region 私有方法
        private OperateResult Connect(string ip, int port,string username, string password) 
        {
            MqttClient = (MQTTnet.Client.MqttClient)new MqttFactory().CreateMqttClient();

            try
            {
                var options = new MqttClientOptionsBuilder().WithTcpServer(ip, port)
                                            .WithClientId("IMX-MQTTClient" + Guid.NewGuid().ToString())
                                            .WithCredentials("11","1111")
                                            .WithCleanSession()
                                            .Build();

                MqttClient.ConnectAsync(options);

                //mqttClient.ConnectedAsync += Client_ConnectedAsync;
                //mqttClient.DisconnectedAsync += Client_DisconnectedAsync;
                //mqttClient.ApplicationMessageReceivedAsync += Client_ApplicationMessageReceivedAsync;
                //mqttClient.InspectPacketAsync += Client_InspectPacketAsync;

                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                MqttClient?.Dispose();
                IsDriveOpen = false;
                LastError = ex.GetMessage();
                Logger.Exception(nameof(MQTTOprate), nameof(Connect), ex);
                return OperateResult.Failed(LastError);
            }
        }


        //private Task Client_DisconnectedAsync(MqttClientDisconnectedEventArgs args)
        //{
        //    if (args == null)
        //        return Task.CompletedTask;
        //    //异常导致的掉线
        //    if (args.Exception != null)
        //    {
        //        //被取消连接
        //        if (args.Exception is OperationCanceledException)
        //        {
        //            //已取消连接...
        //        }
        //        else if (args.Exception is MqttCommunicationException)
        //        {
        //            string str = $"{args.Exception.Message} {args.Exception.InnerException?.Message}";
        //        }
        //        else
        //        {
        //            string str = $"{args.Exception.Message} {args.Exception.InnerException?.Message}";
        //        }
        //    }
        //    //非异常导致离线
        //    else
        //    {
        //        //已断开连接..
        //    }

        //    IsDriveOpen = false;
        //    return Task.CompletedTask;
        //}

        //private Task Client_ConnectedAsync(MqttClientConnectedEventArgs args)
        //{
        //    IsDriveOpen = true;
        //    return Task.CompletedTask;
        //}
        #endregion
    }

    /// <summary>
    /// MQTT消息发布模型类
    /// </summary>
    public class MQTTPublishMessage
    {
        /// <summary>
        /// 消息发布主题
        /// </summary>
        public string Topic { get; set; } = string.Empty;

        /// <summary>
        /// 发布消息内容
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 发布消息质量
        /// </summary>
        public MqttQualityOfServiceLevel QosLevel { get; set; } = MqttQualityOfServiceLevel.AtMostOnce;

        /// <summary>
        /// 消息格式
        /// </summary>
        public MqttPayloadType PayloadType { get; set; } = MqttPayloadType.JSON;
    }
}
