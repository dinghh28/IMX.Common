using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMX.Common.Function.Enumerations
{
    public enum FuncitonType
    {
        /// <summary>
        /// 产品下发指令模板
        /// </summary>
        PRODUCTFUNC,
        /// <summary>
        /// AC源操作模板
        /// </summary>
        ACSOURCEFUNC,
        /// <summary>
        /// AC负载操作模板
        /// </summary>
        ACLODEFUNC,
        /// <summary>
        /// DC负载操作模板
        /// </summary>
        DCLOADFUNC,
        /// <summary>
        /// 双向源操作模板
        /// </summary>
        ACDCINVERTERFUNC,
        /// <summary>
        /// 高压直流源操作模板
        /// </summary>
        DCSOURCEFUNC,
        /// <summary>
        /// 直流稳压源操作模板
        /// </summary>
        APUFUNC,
        /// <summary>
        /// 继电器操作模板
        /// </summary>
        RELAYFUNC,
        /// <summary>
        /// 水浴
        /// </summary>
        WATERBOXFUNC,
        /// <summary>
        /// 温箱
        /// </summary>
        TEMPBOXFUNC,
        /// <summary>
        /// 设置驻留时间
        /// </summary>
        DWELLTIMEFUNC,
        /// <summary>
        /// 数字采样设备操作模板
        /// </summary>
        ADCSYSTEAMFUNC,
        /// <summary>
        /// 电池模拟器
        /// </summary>
        BATTERYSIMULATORFUNC,
        /// <summary>
        /// 结果获取模板
        /// </summary>
        TESTRESULTFUNC,
        /// <summary>
        /// 弹窗模板
        /// </summary>
        POPUPFUNC,
        /// <summary>
        /// 保护模板
        /// </summary>
        PROTECTFUNC,
        /// <summary>
        /// 旋变模拟器
        /// </summary>
        GYROSCOPICFUNC,
        /// <summary>
        /// 盐雾箱
        /// </summary>
        SALTFOGBOXFUNC,
        /// <summary>
        /// 振动台
        /// </summary>
        VIBRATIONTABLEFUNC,
    }
}
