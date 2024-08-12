using Device.Common;
using Piggy.VehicleBus.Common;
using Piggy.VehicleBus.Device;
using Piggy.VehicleBus.Device.ControlAPI;
using Piggy.VehicleBus.OperateInterface;
using Super.Zoo.Framework;
using Super.Zoo.Framework.Logger;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Device.Base.Operate
{
    public class DriveCANOprate : SuperDriveOprate
    {
        #region 公共属性
        public override DriveType SupportFuncitonType => DriveType.CAN;

        ///<inheritdoc/>
        public override bool IsWriteCmdWithNewLine { get; set; } = false;
        ///<inheritdoc/>
        public override string WriteCmdWithNewLine { get; set; } = "\r\n";

        /// <summary>
        /// CAN总线操作接口
        /// </summary>
        public VehicleBusOperator DeviceOperator { get; private set; } = new VehicleBusOperator();

        /// <summary>
        /// 交互器
        /// </summary>
        public MessageInteractor Interactor { get; private set; }
        #endregion

        #region 私有变量
        private IVehicleBusDeviceArgs DeviceArgs;
        #endregion

        #region 公共方法

        #region 通讯设备操作
        public override OperateResult Open(ModDriveConfig config)
        {
            if (IsDriveOpen)
            {
                LastError = $"设备已打开，请勿重复操作";
                Logger.Error(nameof(DriveCANOprate), nameof(Open), LastError);
                return OperateResult.Failed(LastError);
            }

            if (config == null)
            {
                LastError = $"资源字符不存在";
                Logger.Error(nameof(DriveCANOprate), nameof(Open), LastError);
                return OperateResult.Failed(LastError);
            }

            if (string.IsNullOrWhiteSpace(config.ResourceString))
            {
                LastError = $"资源字符串为空";
                Logger.Error(nameof(DriveCANOprate), nameof(Open), LastError);
                return OperateResult.Failed(LastError);
            }
            if (config.ResourceString.IndexOf("::INSTR") < 0 && config.ResourceString.IndexOf("::SOCKET") < 0)
            {
                LastError = $"资源字符串为必须是以[::INSTR]结尾";
                Logger.Error(nameof(DriveCANOprate), nameof(Open), LastError);
                return OperateResult.Failed(LastError);
            }

            #region 参数resourceString的解析
            DriveType eType = DriveType.NULL;

            string strParam1 = string.Empty;//判断是否"zlg_canfd"
            string strParam2 = string.Empty;//CANType,判断是否是usbcanfd800u
            string strParam3 = string.Empty;//DeviceIndex，设置地址
            OperateResult bRet = DeviceResourceHelper.DecodeResourceString(config.ResourceString, ref eType, ref strParam1, ref strParam2, ref strParam3);
            if (!bRet)
            {
                LastError = bRet.Message;
                Logger.Error(nameof(DriveCANOprate), nameof(Open), LastError);
                return OperateResult.Failed(LastError);
            }

            if (eType != DriveType.CAN)//&& eType != DriveType.CANFD)
            {
                LastError = $"串口类型不匹配";
                Logger.Error(nameof(DriveCANOprate), nameof(Open), LastError);
                return OperateResult.Failed(LastError);
            }
            #endregion

            switch (strParam1.ToLower().Trim())
            {
                case "zlg_canfd":
                    DriveConfig = config;
                    DeviceArgs = new ZLGCANFDDeviceArgs
                    {
                        DeviceType = (ZCAN_DeviceType)Enum.Parse(typeof(ZCAN_DeviceType), strParam2),
                        DeviceIndex = Convert.ToUInt32(strParam3.Trim()),
                        ReceiveThreadDelay = DriveConfig.BeforeReadDelayMS
                    };
                    break;
                default:
                    DeviceArgs = new ZLGCANFDDeviceArgs();
                    break;
            }

            try
            {
                //打开设备
                OperateResult result = DeviceOperator.Open<ZLGCANFDDevice>(DeviceArgs);
                if (!result)
                {
                    LastError = result.Message;
                    Logger.Error(nameof(DriveCANOprate), nameof(Open), LastError);
                    return OperateResult.Failed(LastError);
                }

                //启动接收线程
                result = DeviceOperator.StartReceive();
                if (!result)
                {
                    LastError = result.Message;
                    Logger.Error(nameof(DriveCANOprate), nameof(Open), LastError);
                    return OperateResult.Failed(LastError);
                }
                IsDriveOpen = true;
                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                IsDriveOpen = false;
                DeviceOperator?.Dispose();
                LastError = ex.GetMessage();
                Logger.Exception(nameof(DriveCANOprate), nameof(Open), ex);
                return OperateResult.Failed(LastError);
            }
        }

        public override OperateResult Close()
        {

            //OperateResult result = DeviceOperator.StopReceive();
            //Thread.Sleep(500);
            try
            {

                return DeviceOperator.StopReceive()
                       .And(DeviceOperator.Close())
                       .AttachIfFailed(result =>
                       {
                           LastError = result.Message;
                           Logger.Error(nameof(DriveCANOprate), nameof(Close), LastError);
                       });
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
                Logger.Exception(nameof(DriveCANOprate), nameof(Close), ex);
                return OperateResult.Failed(LastError);
            }
            finally
            {
                //result = DeviceOperator.Close();
                IsDriveOpen = false;
                DeviceOperator?.Dispose();
            }
        }
        #endregion

        #region 交互器操作
        /// <summary>
        /// 注册交互器
        /// </summary>
        /// <param name="channelArgs"></param>
        /// <returns></returns>
        public OperateResult<IMessageInteractor> RegisterInteractor(IVehicleBusChannelArgs channelArgs)
        {
            //if (!IsDriveOpen)
            //{
            //    LastError = $"总线控制器未打开！";
            //    Logger.Error(nameof(DriveCANOprate), nameof(RegisterInteractor), LastError);
            //    return OperateResult.Failed(LastError);
            //}

            return DeviceOperator.RegisterInteractor(channelArgs).AttachIfFailed(result =>
            {
                LastError = result.Message;
            });
        }

        /// <summary>
        /// 注册交互器
        /// </summary>
        /// <param name="channelArgs"></param>
        /// <param name="recorder"></param>
        /// <returns></returns>
        public OperateResult<IMessageInteractor> RegisterInteractor(IVehicleBusChannelArgs channelArgs, IRecorder recorder)
        {
            //if (!IsDriveOpen)
            //{
            //    LastError = $"总线控制器未打开！";
            //    Logger.Error(nameof(DriveCANOprate), nameof(RegisterInteractor), LastError);
            //    return OperateResult.Failed(LastError);
            //}

            return DeviceOperator.RegisterInteractor(channelArgs, recorder).AttachIfFailed(result =>
            {
                LastError = result.Message;
            });
        }

        /// <summary>
        /// 注销交互器
        /// </summary>
        /// <param name="index">can通道号</param>
        /// <returns></returns>
        public OperateResult UnRegisterInteractor(uint index)
        {
            return DeviceOperator.UnRegisterInteractor(index).AttachIfFailed(result =>
            {
                LastError = result.Message;
            });
        }

        /// <summary>
        /// 打开注册器(同时会对注册器初始化)
        /// </summary>
        /// <param name="index">驱动通道号</param>
        /// <returns></returns>
        public OperateResult OpenInteractor(uint index)
        {
            return DeviceOperator
                .FetchInteractor(index)
                .ThenAnd(result => result.Data
                .Open()
                .And(result.Data
                .Initialize())
                .ConvertTo(result.Data))
                .AttachIfFailed(result =>
                {
                    LastError = result.Message;
                });
        }

        /// <summary>
        /// 关闭注册器
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public OperateResult CloseInteractor(uint index) 
        {
            return DeviceOperator
            .FetchInteractor(index)
            .ThenAnd(result => result.Data
            .Close()
            .ConvertTo(result.Data))
            .AttachIfFailed(result => 
            {
                LastError = result.Message;
            });
        }

        #endregion

        /// <summary>
        /// 发送信号
        /// </summary>
        /// <param name="channelIndex"></param>
        /// <param name="frameObject"></param>
        /// <returns></returns>
        public OperateResult SendFrame(uint channelIndex, FrameObject frameObject)
        {
            var channel = DeviceOperator.VehicleBusDevice.FetchChannel(channelIndex);
            if (!channel)
            {
                LastError = $"获取通道[{channelIndex}]失败: {channel.Message}";
                Logger.Error(nameof(DriveCANOprate), nameof(SendFrame), LastError);
                return OperateResult.Failed(LastError);
            }

            var result = channel.Data.SendFrame(frameObject);
            if (!result)
            {
                LastError = $"发送单帧报文失败: {result.Message}";
                Logger.Error(nameof(DriveCANOprate), nameof(SendFrame), LastError);
                return OperateResult.Failed(LastError);
            }

            return OperateResult.Succeed();
        }
        #endregion

        #region 静态方法
        /// <summary>
        /// 创建设备CAN操作类
        /// </summary>
        /// <returns></returns>
        public static DriveCANOprate Create() => Create(null);

        /// <summary>
        /// 创建设备CAN操作类
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <returns></returns>
        public static DriveCANOprate Create(ILogger logger) => new DriveCANOprate(logger);
        #endregion

        #region 构造方法
        /// <summary>
        /// 无参构造
        /// </summary>
        public DriveCANOprate() : base() { }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="logger">日志记录器</param>
        public DriveCANOprate(ILogger logger) : base(logger) { }
        #endregion

        #region 析构方法
        ~DriveCANOprate()
        {
            Dispose();
        }
        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool isDispose)
        {
            if (!isDispose)
            {
                return;
            }

            if (IsDriveOpen)
            {
                Close();
            }
            //DeviceOperator?.Dispose();
        }
        #endregion
    }
}
