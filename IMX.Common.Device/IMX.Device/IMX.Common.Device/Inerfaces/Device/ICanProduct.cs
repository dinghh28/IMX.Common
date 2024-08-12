using Piggy.VehicleBus.Common;
using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMX.Common.Device.Inerfaces
{
    public interface ICanProduct : IDeviceOperate
    {
        OperateResult CalcMessageValue();
        /// <summary>
        /// 加载DBC文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        OperateResult LoadMessageFile(string path);

        /// <summary>
        /// 加载DBC文件
        /// </summary>
        /// <param name="fileLoader"></param>
        /// <returns></returns>
        OperateResult LoadMessageFile(IMessageFileLoader fileLoader);

        /// <summary>
        /// 启动交互器通讯
        /// </summary>
        /// <returns></returns>
        OperateResult StartCommunication();

        /// <summary>
        /// 关闭交互器通讯
        /// </summary>
        /// <returns></returns>
        OperateResult StopCommunication();

        /// <summary>
        /// 更改报文接收状态
        /// </summary>
        /// <returns></returns>
        OperateResult SetReceiveState(bool isStart);

        /// <summary>
        /// 设置交互器读取信号
        /// </summary>
        OperateResult SetReadSignal(List<string> signals);

        /// <summary>
        /// 设置交互器发送信号
        /// </summary>
        OperateResult SetSendSignal(string signal);

        /// <summary>
        /// 设置交互器发送信号
        /// </summary>
        OperateResult SetSendSignal(List<string> signal);

        /// <summary>
        /// 设置发送信号值
        /// </summary>
        OperateResult SetSendSignalValue(string signal, double value);

        /// <summary>
        /// 删除信号
        /// </summary>
        /// <param name="signal"></param>
        /// <returns></returns>
        OperateResult DeleteSendSignal(string signal);

        /// <summary>
        /// 信号发送使能
        /// </summary>
        /// <param name="signal">信号名</param>
        /// <returns></returns>
        OperateResult StartSignalSend(string signal);

        /// <summary>
        /// 信号发送使能
        /// </summary>
        /// <param name="signals">信号名列表</param>
        /// <returns></returns>
        OperateResult StartSignalSend(List<string> signals);

        /// <summary>
        /// 停止信号发送(若消息仍在发送，则发送默认值)
        /// </summary>
        /// <param name="signal">信号名</param>
        /// <returns></returns>
       OperateResult StopSignalSend(string signal);

        /// <summary>
        /// 停止信号发送(若消息仍在发送，则发送默认值)
        /// </summary>
        /// <param name="signals">信号名列表</param>
        /// <returns></returns>
        OperateResult StopSignalSend(List<string> signals);

        #region 发送列表操作
        /// <summary>
        /// 开始消息发送
        /// </summary>
        /// <param name="message">消息名称</param>
        /// <returns></returns>
        OperateResult StartMessageSend(string message);

        /// <summary>
        /// 开始消息发送
        /// </summary>
        /// <param name="messages">消息名称列表</param>
        /// <returns></returns>
        OperateResult StartMessageSend(List<string> messages);

        /// <summary>
        /// 开始消息发送
        /// </summary>
        /// <param name="message">消息名称</param>
        /// <returns></returns>
        OperateResult StartMessageSend(uint messageId);

        /// <summary>
        /// 开始消息发送
        /// </summary>
        /// <param name="messages">消息名称列表</param>
        /// <returns></returns>
        OperateResult StartMessageSend(List<uint> messages);

        /// <summary>
        /// 停止消息发送
        /// </summary>
        /// <param name="message">消息名称</param>
        /// <returns></returns>
        OperateResult StopMessageSend(string message);

        /// <summary>
        /// 停止消息发送
        /// </summary>
        /// <param name="messages">消息名称列表</param>
        /// <returns></returns>
        OperateResult StopMessageSend(List<string> messages);

        /// <summary>
        /// 停止消息发送
        /// </summary>
        /// <param name="message">消息帧ID</param>
        /// <returns></returns>
        OperateResult StopMessageSend(uint messageId);

        /// <summary>
        /// 停止消息发送
        /// </summary>
        /// <param name="messages">消息名称列表</param>
        /// <returns></returns>
        OperateResult StopMessageSend(List<uint> messages);
        #endregion

        #region 上报信号操作
        /// <summary>
        /// 获取上报列表所有信号值
        /// </summary>
        /// <returns></returns>
        OperateResult<Dictionary<string, double>> GetSignalsValue();

        /// <summary>
        /// 获取上报信号值
        /// </summary>
        /// <param name="signal"></param>
        /// <returns></returns>
        OperateResult<double> GetSignalValue(string signal);
        #endregion

    }
}
