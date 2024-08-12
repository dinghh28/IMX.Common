using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IMX.Common.Logger
{
    /// <summary>
    /// 丁某人的日志管理器
    /// </summary>
    public static class SuperDHHLoggerManager
    {
        #region 公共属性
        /// <summary>
        /// 试验日志接口
        /// </summary>
        public static SuperDHHConsoleLogger TestLogger { get; } = new SuperDHHConsoleLogger("TestLog");

        /// <summary>
        /// 窗口操作日志接口
        /// </summary>
        public static SuperDHHConsoleLogger FromLogger { get; } = new SuperDHHConsoleLogger("FromLogger");

        /// <summary>
        /// 产品日志接口
        /// </summary>
        public static SuperDHHConsoleLogger ProLogger { get; } = new SuperDHHConsoleLogger("ProLogger");

        /// <summary>
        /// 设备日志接口
        /// </summary>
        public static SuperDHHConsoleLogger DeviceLogger { get; } = new SuperDHHConsoleLogger("DeviceLogger");

        /// <summary>
        /// 数据库日志接口
        /// </summary>
        public static SuperDHHConsoleLogger DBLogger { get; } = new SuperDHHConsoleLogger("DBLogger");

        /// <summary>
        /// 界面线程日志接口
        /// </summary>
        public static SuperDHHConsoleLogger ThreadLogger { get; } = new SuperDHHConsoleLogger($"FromLogger//ThreadLogger");

        #endregion

        #region 公共方法

        #region 获取 Logger

        private static SuperDHHConsoleLogger GetLogger(LoggerType loggerType)
        {
            switch (loggerType)
            {
                case LoggerType.TESTLOG: return TestLogger;
                case LoggerType.FROMLOG: return FromLogger;
                case LoggerType.PRODUCTLOG: return ProLogger;
                case LoggerType.DEVICELOG: return DeviceLogger;
                case LoggerType.DBLOG: return DBLogger;
                case LoggerType.THREAD: return ThreadLogger;
            }

            throw new ArgumentException($"Unexpected type of logger: {loggerType}", nameof(loggerType));
        }

        #endregion

        #region 记录日志信息方法
        public static void Debug(LoggerType type, string model, string function, string message)
            => GetLogger(type).Debug(model, function, message);

        public static void Info(LoggerType type, string model, string function, string message)
            => GetLogger(type).Info(model, function, message);

        public static void Warn(LoggerType type, string model, string function, string message)
            => GetLogger(type).Warn(model, function, message);

        public static void Error(LoggerType type, string model, string function, string message)
            => GetLogger(type).Error(model, function, message);

        public static void Fatal(LoggerType type, string model, string function, string message)
            => GetLogger(type).Fatal(model, function, message);
        #endregion

        #region 记录异常信息方法
        public static void Exception(LoggerType type, string model, string function, Exception exception)
            => GetLogger(type).Exception(model, function, exception);
        #endregion

        #region 记录跟踪信息方法
        public static void Trace(LoggerType type, string message, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0, [CallerMemberName] string callerMemberName = "")
            => GetLogger(type).Trace(message, callerFilePath, callerLineNumber, callerMemberName);
        #endregion

        /// <summary>
        /// 获取Log最近记录信息
        /// </summary>
        /// <param name="type">log类型</param>
        /// <returns></returns>
        public static string GetMessage(LoggerType type)
            => GetLogger(type).LastMessage;
        #endregion
    }

    /// <summary>
    /// 记录日志类型
    /// </summary>
    public enum LoggerType
    {
        /// <summary>
        /// 试验日志
        /// </summary>
        TESTLOG,
        /// <summary>
        /// 界面操作日志
        /// </summary>
        FROMLOG,
        /// <summary>
        /// 产品日志
        /// </summary>
        PRODUCTLOG,
        /// <summary>
        /// 设备日志
        /// </summary>
        DEVICELOG,
        /// <summary>
        /// 数据库日志
        /// </summary>
        DBLOG,
        /// <summary>
        /// 界面线程日志
        /// </summary>
        THREAD
    }
}
