using Device.Common;
using Super.Zoo.Framework;
using Super.Zoo.Framework.Logger;
using System;

namespace Device.Base
{
    /// <summary>
    /// 设备通讯设备底层接口
    /// </summary>
    public interface ISuperDrive : ILogRecord, IDisposable
    {
        #region 公共属性

        /// <summary>
        /// 通讯设备打开状态
        /// </summary>
        bool IsDriveOpen { get; }

        ///// <summary>
        ///// 通讯串口
        ///// </summary>
        //IMessageBasedSession Session { get; }

        /// <summary>
        /// 通讯配置
        /// </summary>
        ModDriveConfig DriveConfig { get; }

        /// <summary>
        /// 是否清空缓存区
        /// </summary>
        bool IsClearBuffer { get; set; }

        #region 设备复位相关
        /// <summary>
        /// 设备复位.发送命令失败次数达到3次.那么就启用设备复位功能(默认值为3)
        /// </summary>
        int ReOpen_MaxWriteFailCount { get; set; }
        /// <summary>
        /// 设备复位.当前发送命令失败次数(默认值为3)
        /// <para>a.每次发送成功后该变量置为0</para>
        /// <para>b.每次复位后该变量置为0</para>
        /// </summary>
        int ReOpen_CurWriteFailCount { get; set; }
        /// <summary>
        /// 设备复位.两次复位之间的时间间隔默认至少要间隔10秒以上(默认值为10000ms)
        /// </summary>
        int ReOpen_MinIntervalMS { get; set; }

        /// <summary>
        /// <remarks>
        ///     <description>发送命令字符串的时候是否主要去增加【\n】换行符</description>
        ///         <list type="table">
        ///         <listheader>【默认值】</listheader>
        ///             <item>[ASRL]</item>
        ///             <description>True</description>
        ///             <item>[GPIB]</item>
        ///             <description>True</description>
        ///             <item>[USB]</item>
        ///             <description>True</description>
        ///             <item>[TCPIP]</item>
        ///             <description>True</description>
        ///             <item>[CAN]</item>
        ///             <description>Flase</description>
        ///             <item>[CANFD]</item>
        ///             <description>Flase</description>
        ///             <item>[Others]</item>
        ///             <description>True</description>
        ///         </list>
        /// </remarks>
        /// </summary>
        bool IsWriteCmdWithNewLine { get; set; }
        /// <summary>
        /// 主动加换行符是"\r\n"还是"\n"
        /// </summary>
        string WriteCmdWithNewLine { get; set; }

        /// <summary>
        /// 支持驱动类型
        /// </summary>
        DriveType SupportFuncitonType { get; }

        /// <summary>
        /// 设备列表
        /// </summary>
        //List<Guid> DeviceList { get; }
        #endregion

        #endregion

        #region 公共方法

        #region 驱动操作
        /// <summary>
        /// 打开驱动
        /// </summary>
        /// <returns></returns>
        OperateResult Open(ModDriveConfig config);

        /// <summary>
        /// 关闭驱动
        /// </summary>
        /// <returns></returns>
        OperateResult Close();
        #endregion
        #endregion
    }
}
