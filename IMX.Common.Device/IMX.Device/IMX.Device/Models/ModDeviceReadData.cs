using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMX.Common.Device.Models
{
    /// <summary>
    /// 设备数据类
    /// </summary>
    public class ModDeviceReadData
    {
        /// <summary>
        /// 读取信息
        /// </summary>
        public ModTestDataInfo DataInfo { get; set; }
        ///// <summary>
        ///// 数据项
        ///// </summary>
        //public string Name { get; set; } = string.Empty;

        ///// <summary>
        ///// 读取值
        ///// </summary>
        //public double Value { get; set; } = -1;

        /// <summary>
        /// 上限
        /// </summary>
        public double Limits_Upper { get; set; } = double.PositiveInfinity;

        private double limits_Lower = double.NegativeInfinity;
        /// <summary>
        /// 下限
        /// </summary>
        public double Limits_Lower
        {
            get
            {
                if (IsTrage)
                {
                    limits_Lower = Limits_Upper;
                }
                return limits_Lower;
            }
            set
            {
                if (!IsTrage)
                {
                    limits_Lower = value;
                }
            }
        }

        /// <summary>
        /// 是否在设定范围内
        /// </summary>
        public bool IsInRange
        {
            get
            {
                if (!IsUse)
                {
                    return true;
                }
                if (FunJudgeType == FunJudgeType.不等于)
                {
                    return DataInfo.Value != Limits_Upper;
                }
                return DataInfo.Value <= Limits_Upper && DataInfo.Value >= Limits_Lower;
            }
        }

        /// <summary>
        /// 是否为触发值
        /// </summary>
        public bool IsTrage { get; set; } = false;

        /// <summary>
        /// 判断条件
        /// </summary>
        public FunJudgeType FunJudgeType { get; set; }

        /// <summary>
        /// 是否参与保护
        /// </summary>
        public bool IsUse { get; set; } = false;

        /// <summary>
        /// 数据归属工位序号
        /// </summary>
        public int BelongTo { get; set; } = 1;

        /// <summary>
        /// 数据所在设备类型
        /// </summary>
        public string DeviceTypename { get; set; }

        public List<FunJudgeType> JudgeTypeList { get; set; } = new List<FunJudgeType> { FunJudgeType.等于, FunJudgeType.不等于 };


    }

    /// <summary>
    /// 数据库试验详细数据记录模型
    /// </summary>
    public class ModTestDataInfo
    {
        public string Name { get; set; } = string.Empty;

        public double Value { get; set; } = -1;
    }

    /// <summary>
    /// 试验输出操作枚举
    /// </summary>
    public enum FunJudgeType
    {
        /// <summary>
        /// 等于
        /// </summary>
        [Description("等于")]
        等于,
        /// <summary>
        /// 不等于
        /// </summary>
        [Description("不等于")]
        不等于,
    }
}
