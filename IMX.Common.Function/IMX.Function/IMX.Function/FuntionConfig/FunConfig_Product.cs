using Device;
using IMX.Common.Device.Inerfaces;
using IMX.Common.Function.Enumerations;
using Newtonsoft.Json;
using Super.Zoo.Framework;
using Super.Zoo.Framework.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace IMX.Common.Function
{
    public class FunConfig_Product : Function_Config
    {
        #region 公共属性

        #region 继承属性
        [JsonIgnore]
        public override Guid Identify { get; } = new Guid();

        //[JsonIgnore]
        //public override IDeviceOperate Operate { get; set; }

        [JsonIgnore]
        public override FuncitonType SupportFuncitonType => FuncitonType.PRODUCTFUNC;

        public override string DeviceAddress { get; set; } = "Product_0";
        #endregion

        /// <summary>
        /// 报文接收操作
        /// </summary>
        public FOutPutStateType RevOperateType { get; set; } = FOutPutStateType.Null;

        /// <summary>
        /// 产品发送列表
        /// </summary>
        public List<DBCSignalModel> SendMessages { get; set; } = new List<DBCSignalModel>();
        #endregion

        #region 构造方法
        public FunConfig_Product() : base()
        {
        }

        //public FunConfig_ACDCInverter(IDeviceOperate operate):base(operate)
        //{
        //}


        public FunConfig_Product(ILogger logger) : base(logger)
        {
        }
        #endregion

        #region 析构方法
        ~FunConfig_Product()
        {
            Dispose();
        }
        #endregion

        public override OperateResult Execute(object operate)
        {
            try
            {
                //下发CAN数据
                string InfoString = string.Empty;
                if (operate is null)
                {
                    LastError = $"设备类型不存】";
                    Logger.Error(nameof(FunConfig_Product), nameof(Execute), LastError);
                    return OperateResult.Failed(LastError);
                }

                if (!(operate is ICanProduct device))
                {
                    LastError = $"{DeviceAddress}设备类型异常：【{operate.GetType()}】";
                    Logger.Error(nameof(FunConfig_Product), nameof(Execute), LastError);
                    return OperateResult.Failed(LastError);
                }
                if (OperateType != FOutPutStateType.Null)
                {
                    InfoString = OperateType == FOutPutStateType.ON ? "打开" : "关闭";
                    var rlt
                        = (OperateType == FOutPutStateType.ON ? device.StartCommunication() : device.StopCommunication());
                    //.AttachIfFailed(result =>
                    //{
                    //    LastError = $"{DeviceAddress}产品信号发送线程{InfoString}失败:{result.Message}";
                    //    Logger.Error(nameof(FunConfig_Product), nameof(Execute), LastError);
                    //})
                    //.AttachIfSucceed(result =>
                    //{
                    //    Logger.Info(nameof(FunConfig_Product), nameof(Execute), $"{DeviceAddress}产品信号发送线程{InfoString}成功");
                    //});

                    //operate.MessageInteractor.IsSendData = OperateType == FOutPutStateType.ON?=;
                    if (!rlt)
                    {
                        LastError = $"{DeviceAddress}产品信号发送线程{InfoString}失败:{rlt.Message}";
                        Logger.Error(nameof(FunConfig_Product), nameof(Execute), LastError);
                    }

                    Logger.Info(nameof(FunConfig_Product), nameof(Execute), $"产品信号发送线程{InfoString}{rlt.Message}");
                }

                if (RevOperateType != FOutPutStateType.Null)
                {
                    InfoString = RevOperateType == FOutPutStateType.ON ? "打开" : "关闭";
                    var rlt = device
                        .SetReceiveState(RevOperateType == FOutPutStateType.ON);
                        //.AttachIfFailed(result =>
                        //{
                        //    LastError = $"产品信号接收线程{InfoString}失败:{result.Message}";
                        //    Logger.Error(nameof(FunConfig_Product), nameof(Execute), LastError);
                        //})
                        //.AttachIfSucceed(result =>
                        //{
                        //    Logger.Info(nameof(FunConfig_Product), nameof(Execute), $"产品信号接收线程{InfoString}{result.Message}");
                        //});

                    if (!rlt)
                    {
                        LastError = $"产品信号接收线程{InfoString}失败:{rlt.Message}";
                        Logger.Error(nameof(FunConfig_Product), nameof(Execute), LastError);
                    }
                    Logger.Info(nameof(FunConfig_Product), nameof(Execute), $"产品信号接收线程{InfoString}{rlt.Message}");
                }

                SendMessages?.ToList().ForEach(x =>
                {
                    OperateResult rlt = null;
                    //增加或删除发送信号

                    if (x.IsSend)
                    {
                        rlt = device.StartSignalSend(x.SignalName);
                        if (!rlt)
                        {
                            LastError = $"产品增加下发信号失败:{rlt.Message}";
                            Logger.Error(nameof(FunConfig_Product), nameof(Execute), LastError);
                        }
                    }
                    else
                    {
                        rlt = device.StopSignalSend(x.SignalName);
                        if (!rlt)
                        {
                            LastError = $"产品删除下发信号失败:{rlt.Message}";
                            Logger.Error(nameof(FunConfig_Product), nameof(Execute), LastError);
                        }
                    }

                    //rlt = x.IsSend ?
                    //device
                    //.StartSignalSend(x.SignalName)
                    //.AttachIfFailed(result =>
                    //{
                    //    LastError = $"产品增加下发信号失败:{result.Message}";
                    //    Logger.Error(nameof(FunConfig_Product), nameof(Execute), LastError);
                    //})
                    //: device
                    //.StopSignalSend(x.SignalName)
                    //.AttachIfFailed(result =>
                    //{
                    //    LastError = $"产品删除下发信号失败:{result.Message}";
                    //    Logger.Error(nameof(FunConfig_Product), nameof(Execute), LastError);
                    //});

                    //修改发送信号值
                    device
                .SetSendSignalValue(x.SignalName, Convert.ToDouble(x.SignalValue))
                    .AttachIfFailed(result =>
                    {
                        LastError = $"产品修改发送信号值失败:{result.Message}";
                        Logger.Error(nameof(FunConfig_Product), nameof(Execute), LastError);
                    });
                });

                //foreach (var item in dicMessages)
                //{
                //    if (device.DicMessage[item.Key].Signals.All(signal => signal.EnableSend == false))
                //    {
                //        device.StopMessageSend(item.Value);
                //    }
                //    else
                //    {
                //        device.StartMessageSend(item.Value);
                //    }
                //}

                //计算下发列表
                device
                    .CalcMessageValue()
                    .AttachIfFailed(result =>
                    {
                        LastError = $"产品信号计算下发值失败:{result.Message}";
                        Logger.Error(nameof(FunConfig_Product), nameof(Execute), result.Message);
                    });

                if (DelayAfterRun > 0)
                {
                    Thread.Sleep(DelayAfterRun);
                }

                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                Logger.Exception (nameof(FunConfig_Product), nameof(Execute), ex);
                return OperateResult.Failed();
            }

        }
    }

    public class DBCSignalModel
    {
        /// <summary>
        /// 是否发送信号
        /// </summary>
        public bool IsSend { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 信号ID
        /// </summary>
        public uint MessageID { get; set; }
        /// <summary>
        /// 消息名
        /// </summary>
        public string MessageName { get; set; }
        /// <summary>
        /// 信号名
        /// </summary>
        public string SignalName { get; set; }
        /// <summary>
        /// 自定义信号名
        /// </summary>
        public string CustomName { get; set; }
        /// <summary>
        /// 发送值
        /// </summary>
        public string SignalValue { get; set; }

    }
}
