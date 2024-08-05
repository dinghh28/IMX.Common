using Device;
using Super.Zoo.Framework;
using Super.Zoo.Framework.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IMX.Common.Device.Inerfaces;
using IMX.Common.Function.Enumerations;

namespace IMX.Common.Function
{
    public class FunConfig_DwellTime : Function_Config
    {
        #region 公共属性

        #region 继承属性
        public override Guid Identify => Guid.NewGuid();

        public override FuncitonType SupportFuncitonType => FuncitonType.DWELLTIMEFUNC;

        public override string DeviceAddress { get; set; }
        #endregion

        /// <summary>
        /// 延时总时间（ms）
        /// </summary>
        public long DellTime_TotalTick =>
            TimeSpan.FromHours(DellTime_H).Ticks +
            TimeSpan.FromMinutes(DellTime_M).Ticks +
            TimeSpan.FromSeconds(DellTime_S).Ticks;
        //TimeSpan.FromMilliseconds(DellTime_MS).Ticks;

        /// <summary>
        /// 延时小时
        /// </summary>
        public uint DellTime_H { get; set; } = 0;

        /// <summary>
        /// 延时分钟
        /// </summary>
        public uint DellTime_M { get; set; } = 0;

        /// <summary>
        /// 延时秒
        /// </summary>
        public uint DellTime_S { get; set; } = 0;

        /// <summary>
        /// 延时毫秒
        /// </summary>
        //public uint DellTime_MS { get; set; } = 0;

        #endregion

        #region 构造方法
        public FunConfig_DwellTime() : base()
        {
        }

        //public FunConfig_ACDCInverter(IDeviceOperate operate):base(operate)
        //{
        //}


        public FunConfig_DwellTime(ILogger logger) : base(logger)
        {
        }
        #endregion

        #region 析构方法
        ~FunConfig_DwellTime()
        {
            Dispose();
        }
        #endregion

        #region 保护方法
        public override OperateResult Execute(object operate)
        {
            try
            {
                DateTime dateTime = DateTime.Now;
                long span = 0;
                //Task.Run(async () =>
                //{
                while (span < DellTime_TotalTick && IsTestRunning[0])
                {
                    Thread.Sleep(100); //100ms延时
                    span = (DateTime.Now - dateTime).Ticks;
                    //await Task.Delay(TimeSpan.FromMinutes(Set_DellTime));
                    //Thread.Sleep((int)(Set_DellTime * 60 * 1000));
                    //span += 100;
                    //Trace.WriteLine("驻留时长：" + span);
                }
                //});

                return OperateResult.Succeed();

            }
            catch (Exception ex)
            {
                Logger.Exception(nameof(FunConfig_DwellTime), nameof(Execute), ex);
                return OperateResult.Failed();
            }

        }
        #endregion
    }
}
