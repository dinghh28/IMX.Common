using Device.Common;
using NationalInstruments.Visa;
using System;
using Super.Zoo.Framework;
using Super.Zoo.Framework.Logger;

namespace Device.Base.Operate
{
    internal class TCPIPOprate : VisaOprate
    {
        #region 公共属性
        public override DriveType SupportFuncitonType => DriveType.TCPIP;
        #endregion

        #region 静态方法
        /// <summary>
        /// 创建设备ASRL操作类
        /// </summary>
        /// <returns></returns>
        public static TCPIPOprate Create() => Create(null);

        /// <summary>
        /// 创建设备ASRL操作类
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <returns></returns>
        public static TCPIPOprate Create(ILogger logger) => new TCPIPOprate(logger);
        #endregion

        #region 构造方法
        public TCPIPOprate():base(null)
        {
        }

        public TCPIPOprate(ILogger logger):base(logger)
        {
        }
        #endregion

        #region 析构方法

        ~TCPIPOprate()
        {
            Dispose();
        }

        #endregion

        #region 公共方法

        #region 串口操作
        public override OperateResult Open(ModDriveConfig config)
        {
            //if (config == null)
            //{
            //    LastError = $"资源字符不存在";
            //    Logger.Error(nameof(TCPIPOprate), nameof(Open), LastError);
            //    return OperateResult.Failed(LastError);
            //}

            //if (string.IsNullOrWhiteSpace(config.ResourceString))
            //{
            //    LastError = $"资源字符串为空";
            //    Logger.Error(nameof(TCPIPOprate), nameof(Open), LastError);
            //    return OperateResult.Failed(LastError);
            //}
            //if (config.ResourceString.IndexOf("::INSTR") < 0 && config.ResourceString.IndexOf("::SOCKET") < 0)
            //{
            //    LastError = $"资源字符串为必须是以[::INSTR]结尾";
            //    Logger.Error(nameof(TCPIPOprate), nameof(Open), LastError);
            //    return OperateResult.Failed(LastError);
            //}
            try
            {
                //#region 参数判断 + 解析获取 

                //#region 参数resourceString的解析
                //DriveType eType = DriveType.NULL;
                //string strParam1 = string.Empty;
                //string strParam2 = string.Empty;
                //string strParam3 = string.Empty;
                //OperateResult bRet = DeviceResourceHelper.DecodeResourceString(config.ResourceString, ref eType, ref strParam1, ref strParam2, ref strParam3);
                //if (!bRet)
                //{
                //    LastError = bRet.Message;
                //    Logger.Error(nameof(TCPIPOprate), nameof(Open), LastError);
                //    return OperateResult.Failed(LastError);
                //}

                //if (eType != SupportFuncitonType)
                //{
                //    LastError = $"Open.串口类型不匹配";
                //    Logger.Error(nameof(TCPIPOprate), nameof(Open), LastError);
                //    return OperateResult.Failed(LastError);
                //}
                //#endregion

                //#endregion

                //驱动参数解析
                var bRet = GetConfig(config);
                if (!bRet)
                {
                    Logger.Error(nameof(TCPIPOprate), nameof(Open), LastError);
                    return OperateResult.Failed(bRet.Message);
                }

                /////////////////////////////////////////
                // 已经打开的 先执行关闭;
                string strErr = string.Empty;
                if (IsDriveOpen)
                {
                    Close();
                }

                // 执行打开操作;
                Session = new TcpipSocket(config.ResourceString)
                {
                    TimeoutMilliseconds = config.TimeoutMS,
                    TerminationCharacterEnabled = config.TerminationCharacterEnabled,
                };

                // 属性赋值;
                IsDriveOpen = true;
                //DriveConfig = config;
                //DriveConfig.CommunicationType = eType;

                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Exception(nameof(TCPIPOprate), nameof(Open), ex);
                IsDriveOpen = false;
                return OperateResult.Failed(LastError);
            }
        }
        #endregion

        #endregion
    }
}
