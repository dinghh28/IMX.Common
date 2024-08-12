using Device.Common;
using NationalInstruments.Visa;
using System;
using Super.Zoo.Framework;
using Super.Zoo.Framework.Logger;

namespace Device.Base.Operate
{
    internal class GPIBOprate : VisaOprate
    {
        #region 公共属性
        public override DriveType SupportFuncitonType => DriveType.GPIB;
        #endregion

        #region 静态方法
        /// <summary>
        /// 创建设备GPIB操作类
        /// </summary>
        /// <returns></returns>
        public static GPIBOprate Create() => Create(null);

        /// <summary>
        /// 创建设备GPIB操作类
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <returns></returns>
        public static GPIBOprate Create(ILogger logger) => new GPIBOprate(logger);
        #endregion

        #region 构造方法
        public GPIBOprate():base() { }

        public GPIBOprate(ILogger logger):base(logger) { }
        #endregion

        #region 析构方法

        ~GPIBOprate() 
        {
            Dispose();
        }
        #endregion

        #region 公共方法

        #region 串口操作
        public override OperateResult Open(ModDriveConfig config)
        {
            try
            {
                //驱动参数解析
                var bRet = GetConfig(config);
                if (!bRet)
                {
                    Logger.Error(nameof(USBOprate), nameof(Open), LastError);
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
                Session = new GpibSession(config.ResourceString) 
                {
                    TimeoutMilliseconds = config.TimeoutMS,
                    TerminationCharacterEnabled = config.TerminationCharacterEnabled
                };

                // 属性赋值;
                IsDriveOpen = true;

                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Exception(nameof(GPIBOprate), nameof(Open), ex);
                IsDriveOpen = false;
                return OperateResult.Failed(LastError);
            }
        }
        #endregion

        #endregion

    }
}
