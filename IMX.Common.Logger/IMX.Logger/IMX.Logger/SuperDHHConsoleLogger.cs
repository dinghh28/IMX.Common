using Super.Zoo.Framework;
using Super.Zoo.Framework.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMX.Common.Logger
{
    public class SuperDHHConsoleLogger : ConsoleLogger
    {
        #region 私有变量

        private readonly string infoPath;
        private readonly string exceptionPath;
        private readonly string tracePath;

        private uint infoFileIndex = 0;
        private uint exceptionFileIndex = 0;
        private uint traceFileIndex = 0;

        #endregion

        #region 公共属性

        public string LoggerFolder { get; }

        /// <summary>
        /// 上次故障记录信息
        /// </summary>
        public string LastMessage { get; private set; } = string.Empty;

        #endregion

        public SuperDHHConsoleLogger(string loggerFolder)
        {
#if DEBUG
            WriteToConsole = true;
            LoggerLevel = LoggerLevel.DEBUG;
#else
            WriteToConsole = false;
            LoggerLevel = LoggerLevel.INFO;
#endif

            LoggerFolder = loggerFolder;

            string loggerPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SuperDHHLog", loggerFolder);

            infoPath = Path.Combine(loggerPath, "Info");
            exceptionPath = Path.Combine(loggerPath, "Exception");
            tracePath = Path.Combine(loggerPath, "Trace");

            WriteLogEvent += SuperDHHConsoleLogger_WriteLogEvent;
            WriteExceptionEvent += SuperDHHConsoleLogger_WriteExceptionEvent;
            WriteTraceEvent += SuperDHHConsoleLogger_WriteTraceEvent;
        }

        private void WriteLog(string folderPath, ref uint fileIndex, LoggerLevel logLevel, string message)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            FileInfo file = new FileInfo(Path.Combine(folderPath, $"{DateTime.Now:yyyy-MM-dd}_{fileIndex}.log"));

            try
            {
                string logMessage = $"{DateTime.Now.ToString(TimeFormat)} [{logLevel}].{message}";
                File.AppendAllLines(file.FullName, new string[] { logMessage });
                LastMessage = logMessage;
            }
            catch
            {
                // 不要再打日志，万一一直不行，就会死的很惨
            }

            if (file.Length > 1024 * 1024 * 20)
            {
                fileIndex++;
            }
        }

        private void SuperDHHConsoleLogger_WriteLogEvent(LoggerLevel logLevel, string model, string function, string message)
        {
            WriteLog(infoPath, ref infoFileIndex, logLevel, $"[{model}.{function}] {message}");
        }

        private void SuperDHHConsoleLogger_WriteExceptionEvent(LoggerLevel logLevel, string model, string function, Exception exception)
        {
            WriteLog(exceptionPath, ref exceptionFileIndex, logLevel, $"[{model}.{function}] {exception.GetMessage()}");
        }

        private void SuperDHHConsoleLogger_WriteTraceEvent(LoggerLevel logLevel, string message, [System.Runtime.CompilerServices.CallerFilePath] string callerFilePath = "", [System.Runtime.CompilerServices.CallerLineNumber] int callerLineNumber = 0, [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = "")
        {
            WriteLog(tracePath, ref traceFileIndex, logLevel, $"[{callerMemberName}] {message}\n  in {callerFilePath} at line {callerLineNumber}");
        }
    }
}
