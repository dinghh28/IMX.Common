using Device.Base.Inerfaces;
using Device.Common;
using Ivi.Visa;
using Super.Zoo.Framework;
using Super.Zoo.Framework.Logger;
using System;
using System.Text;
using IMX.Common.Logger;
using System.Threading;

namespace Device.Base.Operate
{
    public abstract class VisaOprate : SuperDriveOprate, IVisaDrive
    {
        #region 公共属性

        /// <summary>
        /// 当前设备对象
        /// </summary>
        public  IMessageBasedSession Session { get; private protected set; }

        ///// <summary>
        ///// 是否清空缓存区
        ///// </summary>
        //public bool IsClearBuffer { get; set; } = true;

        //public bool IsDriveOpen { get; private protected set; } = false;

        //public ModDriveConfig DriveConfig { get; set; }

        ///// <summary>
        ///// 日志记录器
        ///// </summary>
        //public ILogger Logger { get; }

        ///// <summary>
        ///// 使用传入日志记录器
        ///// </summary>
        //public bool OutterLogger { get; set; }

        ///// <summary>
        ///// 最后一次故障信息
        ///// </summary>
        //public string LastError { get; set; }

        //public virtual int ReOpen_MaxWriteFailCount { get; set; } = 3;
        //public virtual int ReOpen_CurWriteFailCount { get; set; } = 3;
        //public virtual int ReOpen_MinIntervalMS { get; set; } = 10000;

        //public virtual bool IsWriteCmdWithNewLine { get; set; } = true;
        //public virtual string WriteCmdWithNewLine { get; set; } = "\r\n";

        ///// <summary>
        ///// 每次读取之间延时毫秒(默认100ms)
        ///// </summary>
        //public virtual int DelayMS_Read_ByteArray { get; set; } = 100;

        ////public List<Guid> DeviceList { get; private set; } = new List<Guid>();

        //public abstract DriveType SupportFuncitonType { get; }
        #endregion

        //#region 私有变量
        ///// <summary>
        ///// 驱动类型字典 [驱动类型, 驱动]
        ///// </summary>
        //private static Dictionary<DriveType, Type> dicVisaDriveSuffix = null;

        ///// <summary>
        ///// 驱动类型列表
        ///// </summary>
        //private static readonly List<Type> lstVisaDriveType = new List<Type>
        //{
        //    typeof(ASRLOprate),
        //    typeof(GPIBOprate),
        //    typeof(TCPIPOprate),
        //    typeof(USBOprate),
        //};
        //#endregion

        #region 公共方法

        #region 参数判断 + 解析获取
        public virtual OperateResult GetConfig(ModDriveConfig config) 
        {
            if (config == null)
            {
                LastError = $"资源字符不存在";
                Logger.Error(nameof(VisaOprate), nameof(Open), LastError);
                return OperateResult.Failed(LastError);
            }

            if (string.IsNullOrWhiteSpace(config.ResourceString))
            {
                LastError = $"资源字符串为空";
                Logger.Error(nameof(VisaOprate), nameof(Open), LastError);
                return OperateResult.Failed(LastError);
            }
            if (config.ResourceString.IndexOf("::INSTR") < 0 && config.ResourceString.IndexOf("::SOCKET") < 0)
            {
                LastError = $"资源字符串为必须是以[::INSTR]结尾";
                Logger.Error(nameof(VisaOprate), nameof(Open), LastError);
                return OperateResult.Failed(LastError);
            }
            try
            {
                #region 参数判断 + 解析获取 

                #region 参数resourceString的解析
                DriveType eType = DriveType.NULL;
                string strParam1 = string.Empty;
                string strParam2 = string.Empty;
                string strParam3 = string.Empty;
                OperateResult bRet = DeviceResourceHelper.DecodeResourceString(config.ResourceString, ref eType, ref strParam1, ref strParam2, ref strParam3);
                if (!bRet)
                {
                    LastError = bRet.Message;
                    Logger.Error(nameof(VisaOprate), nameof(Open), LastError);
                    return OperateResult.Failed(LastError);
                }

                if (eType != SupportFuncitonType)
                {
                    LastError = $"Open.串口类型不匹配";
                    Logger.Error(nameof(VisaOprate), nameof(Open), LastError);
                    return OperateResult.Failed(LastError);
                }

                DriveConfig = config;
                return OperateResult.Succeed();
                #endregion

                #endregion
            }
            catch(Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Exception(nameof(VisaOprate), nameof(GetConfig), ex);
                IsDriveOpen = false;
                return OperateResult.Failed(LastError);
            }
        }
        #endregion

        #region 驱动操作
        public override OperateResult Close()
        {
            try
            {
                if (!IsDriveOpen)
                {
                    LastError = $"设备未启动";
                    Logger.Error(nameof(VisaOprate), nameof(Open), LastError);
                    return OperateResult.Failed(LastError);
                }

                Session?.Dispose();

                IsDriveOpen = false;
                DriveConfig = null;
                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Exception(nameof(VisaOprate), nameof(Close), ex);
                return OperateResult.Failed(LastError);
            }
            finally
            {
                Session?.Dispose();
                IsDriveOpen = false;
                DriveConfig = null;
            }
        }

        public virtual OperateResult ClearBuffer()
        {
            try
            {
                if (!IsDriveOpen)
                {
                    LastError = $"设备未启动";
                    Logger.Error(nameof(VisaOprate), nameof(Open), LastError);
                    return OperateResult.Failed(LastError);
                }

                Session?.Clear();

                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Exception(nameof(VisaOprate), nameof(ClearBuffer), ex);
                return OperateResult.Failed(LastError);
            }
        }
        #endregion

        #region 写操作.Only
        /// <summary>
        /// 上次发送的命令字符串;
        /// </summary>
        private string _strLastWriteCmd = string.Empty;
        public virtual OperateResult Write(string writeValue)
        {
            try
            {
                if (!IsDriveOpen || Session == null)
                {
                    LastError = $"串口未打开";
                    Logger.Error(nameof(VisaOprate), nameof(Write), LastError);
                    return OperateResult.Failed(LastError);
                }

                if (this.IsWriteCmdWithNewLine && !writeValue.EndsWith("\n")) writeValue += this.WriteCmdWithNewLine;

                Session.RawIO.Write(writeValue);
                //this._strLastWriteCmd = writeValue;

                // 复位失败次数标记置为0

                this.ReOpen_CurWriteFailCount = 0;

                return OperateResult.Succeed();
            }
            catch (Ivi.Visa.IOTimeoutException ioex)
            {
                LastError = ioex.GetMessage();
                Logger.Exception(nameof(VisaOprate), nameof(Write), ioex);
                return OperateResult.Failed(LastError);
            }
            catch (System.Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Exception(nameof(VisaOprate), nameof(Write), ex);
                return OperateResult.Failed(LastError);
            }
            
        }

        public virtual OperateResult Write_ByteArray(byte[] writeValue)
        {
            try
            {
                if (!IsDriveOpen || Session == null)
                {
                    LastError = $"串口未打开";
                    Logger.Error(nameof(VisaOprate), nameof(Write_ByteArray), LastError);
                    return OperateResult.Failed(LastError);
                }

                if (writeValue == null || writeValue.Length <= 0)
                {
                    LastError = "传入参数为NULL";
                    Logger.Error(nameof(VisaOprate), nameof(Write_ByteArray), LastError);
                    return OperateResult.Failed(LastError);
                }
                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        // 进行读取操作的代码
                        this.Session.RawIO.Write(writeValue);
                        break; // 如果读取成功，则跳出循环
                    }
                    catch (IOTimeoutException)
                    {
                        // 处理超时异常，等待一段时间后重试
                        Thread.Sleep(5000);
                    }
                }

                //Session.FormattedIO.FlushWrite(true);
                //this._strLastWriteCmd = writeValue.ToString(" ", "X2");

                // 复位失败次数标记置为0
                ReOpen_CurWriteFailCount = 0;

                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                Thread.Sleep(50);
                LastError = ex.GetMessage();
                Logger.Exception(nameof(VisaOprate), nameof(Write_ByteArray), ex);
                return OperateResult.Failed(LastError);
            }
        }
        #endregion

        #region 读操作.Only
        public virtual OperateResult<byte[]> Read_ByteArray(int readCount)
        {
            byte[] readByte = null;
            try
            {
                if (!IsDriveOpen || Session == null)
                {
                    LastError = $"Read_ByteArray.串口未打开";
                    Logger.Error(nameof(VisaOprate), nameof(Read_ByteArray), LastError);
                    return OperateResult<byte[]>.Failed(null, LastError);
                }

                if (readCount < 0)
                {
                    readByte = Session.RawIO.Read();
                    if (readByte == null || readByte.Length <= 0)
                    {
                        LastError = $"Read_ByteArray.未读取到相应数据";
                        Logger.Error(nameof(VisaOprate), nameof(Read_ByteArray), LastError);
                        return OperateResult<byte[]>.Failed(null, LastError);
                    }

                    return OperateResult<byte[]>.Succeed(readByte);
                }
                else if (readCount > 0)
                {
                    // 这里有可能读取长度没有读取完成的情况;
                    int nReadDelayMS = DelayMS_Read_ByteArray;     // 每次读取之间延时毫秒(默认100ms);
                    int nTotalCount = 5;        // 允许读取总次数(默认20次, 每次读取之间延时100ms);
                    int nCurReadCount = 0;      // 当前累计读取次数;
                    int nRevIndex = 0;          // 当前接收数据索引位置;
                    byte[] readDataTemp = new byte[1024 * 2];   // 当前读取的临时数据 2K;
                    byte[] revData = new byte[readCount];       // 最终接收到的总数据;
                    bool bIsRevTimeout = true;                  // 是否接收数据超时了;
                    while (nCurReadCount < nTotalCount)
                    {
                        if (nCurReadCount > 0)
                            System.Threading.Thread.Sleep(nReadDelayMS);

                        int leftReadCount = readCount - nRevIndex;
                        readDataTemp = Session.RawIO.Read(leftReadCount);
                        int nReadLen = readDataTemp.Length;

                        // 如果读到的数据长度已经超出剩余接收数据的长度;
                        if (nReadLen > (readCount - nRevIndex))
                            nReadLen = readCount - nRevIndex;

                        Array.Copy(readDataTemp, 0, revData, nRevIndex, nReadLen);
                        nRevIndex += nReadLen;

                        if (nRevIndex >= readCount  // 当前接收到的数据的长度大于要求接收的长度;
                            || nTotalCount <= 1)    // 外面函数调用只要求读取一次数据, 那么在此结束循环且不超时;
                        {
                            bIsRevTimeout = false;
                            readCount = nRevIndex >= readCount ? readCount : nRevIndex;
                            break;
                        }

                        nCurReadCount++;
                    }

                    if (bIsRevTimeout)
                    {
                        readByte = new byte[revData.Length];
                        Array.Copy(revData, readByte, revData.Length);

                        LastError = $"Read_ByteArray.上次Write[{_strLastWriteCmd.Trim()}]\r\n本次Read_ByteArray.读取数据超时[{revData.ToString(" ", "X2")}]";
                        Logger.Error(nameof(VisaOprate), nameof(Read_ByteArray), LastError);
                        return OperateResult<byte[]>.Failed(readByte, LastError);
                    }
                    else
                    {
                        readByte = new byte[revData.Length];
                        Array.Copy(revData, readByte, revData.Length);
                        return OperateResult<byte[]>.Succeed(readByte);
                    }
                }
                else if (readCount == 0)
                {
                    return OperateResult<byte[]>.Succeed(readByte);
                }
                else
                {
                    LastError = $"Read_ByteArray.传入参数readCount不正确[{readCount}]";
                    Logger.Error(nameof(VisaOprate), nameof(Read_ByteArray), LastError);
                    return OperateResult<byte[]>.Failed(null, LastError);
                }
            }
            catch (IOTimeoutException exTimeOut)
            {
                if (exTimeOut.ActualCount <= 0)
                {
                    string strErr = string.Format("Read_ByteArray.上次Write[{0}]\r\n本次Read_ByteArray.读取数据超时：\r\n{1}", _strLastWriteCmd.Trim(), exTimeOut.Message);
                    LastError = strErr;
                    SuperDHHLoggerManager.Error(LoggerType.DEVICELOG, nameof(VisaOprate), nameof(Read_ByteArray), $"Read_ByteArray({readCount})-{exTimeOut.Message}");
                    return OperateResult<byte[]>.Failed(null, LastError);
                }
                else
                {
                    return OperateResult<byte[]>.Succeed(exTimeOut.ActualData);

                }
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Exception(nameof(VisaOprate), nameof(Read_ByteArray), ex);
                return OperateResult<byte[]>.Failed(null, LastError);
            }
        }

        public virtual OperateResult<string> Read_String()
        {
            try
            {
                OperateResult<byte[]> bRet = Read_ByteArray(-1);
                if (!bRet || bRet.Data == null || bRet.Data.Length <= 0)
                {
                    LastError = bRet.Message;
                    Logger.Error(nameof(VisaOprate), nameof(Read_String), LastError);
                    return OperateResult<string>.Failed(string.Empty, bRet);
                }
                return OperateResult<string>.Succeed(Encoding.ASCII.GetString(bRet.Data).Trim());
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Exception(nameof(VisaOprate), nameof(Read_String), ex);
                return OperateResult<string>.Failed(LastError);
            }
        }

        public virtual OperateResult<string> Read_StringExisting()
        {
            try
            {
                if (!this.IsDriveOpen || this.Session == null)
                {
                    LastError = $"Read_StringExisting.串口未打开";
                    Logger.Error(nameof(VisaOprate), nameof(Read_StringExisting), LastError);
                    return OperateResult<string>.Failed(null, LastError);
                }

                string txt = Session.RawIO.ReadString();
                if (string.IsNullOrEmpty(txt))
                {
                    LastError = $"Read_StringExisting.读取到字符串为空";
                    Logger.Error(nameof(VisaOprate), nameof(Read_StringExisting), LastError);
                    return OperateResult<string>.Failed(null, LastError);
                }

                return OperateResult<string>.Succeed(txt.Trim());
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Exception(nameof(VisaOprate), nameof(Read_StringExisting), ex);
                return OperateResult<string>.Failed(null, LastError);
            }
        }
        #endregion

        #region 写完直接读操作
        private readonly object objLock = new object();
        public virtual OperateResult<byte[]> WriteRead_ByteArray(byte[] writeValue, int delayMS = -1)
        {
            try
            {
                lock (objLock)
                {
                    if (this.IsClearBuffer) ClearBuffer();
                    OperateResult bRet = Write_ByteArray(writeValue);
                    if (!bRet.Success)
                    {
                        LastError = bRet.Message;
                        Logger.Error(nameof(VisaOprate), nameof(WriteRead_ByteArray), LastError);
                        return OperateResult<byte[]>.Failed(null, bRet);
                    }

                    //delayMS = 200;
                    if (delayMS <= 0)
                    {
                        if (DriveConfig.BeforeReadDelayMS > 0) { System.Threading.Thread.Sleep(DriveConfig.BeforeReadDelayMS); }
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(delayMS);
                    }

                    OperateResult<byte[]> bRet2 = Read_ByteArray(-1);
                    if (!bRet2)
                    {
                        LastError = bRet2.Message;
                        Logger.Error(nameof(VisaOprate), nameof(WriteRead_ByteArray), LastError);
                        return OperateResult<byte[]>.Failed(null, bRet2);
                    }

                    return OperateResult<byte[]>.Succeed(bRet2.Data);
                }
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Exception(nameof(VisaOprate), nameof(WriteRead_ByteArray), ex);
                return OperateResult<byte[]>.Failed(null, LastError);
            }
        }

        public virtual OperateResult<byte[]> WriteRead_ByteArray(byte[] writeValue, int Len,int delayMS = -1)
        {
            try
            {
                lock (objLock)
                {
                    if (this.IsClearBuffer) ClearBuffer();
                    OperateResult bRet = Write_ByteArray(writeValue);
                    if (!bRet.Success)
                    {
                        LastError = bRet.Message;
                        Logger.Error(nameof(VisaOprate), nameof(WriteRead_ByteArray), LastError);
                        return OperateResult<byte[]>.Failed(null, bRet);
                    }

                    //delayMS = 200;
                    if (delayMS <= 0)
                    {
                        if (DriveConfig.BeforeReadDelayMS > 0) { Thread.Sleep(DriveConfig.BeforeReadDelayMS); }
                    }
                    else
                    {
                        Thread.Sleep(delayMS);
                    }

                    OperateResult<byte[]> bRet2 = Read_ByteArray(Len);
                    if (!bRet2)
                    {
                        LastError = bRet2.Message;
                        Logger.Error(nameof(VisaOprate), nameof(WriteRead_ByteArray), LastError);
                        return OperateResult<byte[]>.Failed(null, bRet2);
                    }

                    return OperateResult<byte[]>.Succeed(bRet2.Data);
                }
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Exception(nameof(VisaOprate), nameof(WriteRead_ByteArray), ex);
                return OperateResult<byte[]>.Failed(null, LastError);
            }
        }

        public virtual OperateResult<string> WriteRead_String(string writeValue, int delayMS = -1, bool bIsRead_StringExisting = false)
        {
            try
            {
                lock (objLock)
                {
                    if (IsClearBuffer)
                    {
                        ClearBuffer();
                    }

                    OperateResult bRet = Write(writeValue);
                    if (!bRet)
                    {
                        LastError = bRet.Message;
                        Logger.Error(nameof(VisaOprate), nameof(WriteRead_String), LastError);
                        return OperateResult<string>.Failed(string.Empty, bRet);
                    }
                    //this._strLastWriteCmd = writeValue;

                    if (delayMS <= 0)
                    {
                        if (DriveConfig.BeforeReadDelayMS > 0) { System.Threading.Thread.Sleep(DriveConfig.BeforeReadDelayMS); }
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(delayMS);
                    }

                    OperateResult<string> bRet2 = bIsRead_StringExisting ? Read_StringExisting() : Read_String();
                    if (!bRet2)
                    {
                        LastError = bRet2.Message;
                        Logger.Error(nameof(VisaOprate), nameof(WriteRead_String), LastError);
                        return OperateResult<string>.Failed(string.Empty, LastError);
                    }


                    return OperateResult<string>.Succeed(bRet2.Data.Trim());
                }
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Exception(nameof(VisaOprate), nameof(WriteRead_String), ex);
                return OperateResult<string>.Failed(string.Empty, LastError);
            }
        }
        #endregion

        #endregion



        //#region 静态方法

        ///// <summary>
        ///// 创建Visa驱动
        ///// </summary>
        ///// <param name="type">驱动类型</param>
        ///// <returns></returns>
        //public static OperateResult<IVisaDrive> Create(DriveType type) => Create(null, type);

        //private static OperateResult<IVisaDrive> Create(ILogger logger, DriveType type)
        //{
        //    if (dicVisaDriveSuffix == null)
        //    {
        //        InitVisaDriveSuffixDictionary();
        //    }

        //    if (dicVisaDriveSuffix.ContainsKey(type))
        //    {
        //        if (Activator.CreateInstance(dicVisaDriveSuffix[type], new object[] { logger }) is IVisaDrive drive)
        //        {
        //            return OperateResult<IVisaDrive>.Succeed(drive);
        //        }
        //        else
        //        {
        //            return OperateResult<IVisaDrive>.Failed(null, $"试验方案配置文件创建失败！");
        //        }
        //    }

        //    return OperateResult<IVisaDrive>.Failed(null, $"不支持的配置类型: {type}");
        //}

        //#region 私有方法
        ///// <summary>
        ///// 初始化Visa驱动字典
        ///// </summary>
        //private static void InitVisaDriveSuffixDictionary()
        //{
        //    dicVisaDriveSuffix = new Dictionary<DriveType, Type>();

        //    foreach (var type in lstVisaDriveType)
        //    {
        //        if (!typeof(IVisaDrive).IsAssignableFrom(type)) { continue; }
        //        if (!(Activator.CreateInstance(type) is IVisaDrive drive)) { continue; }


        //        if (!dicVisaDriveSuffix.ContainsKey(drive.SupportFuncitonType))
        //        {
        //            dicVisaDriveSuffix.Add(drive.SupportFuncitonType, type);
        //        }

        //        drive.Dispose();
        //    }
        //}
        //#endregion
        //#endregion

        #region 构造方法
        public VisaOprate():base(null)
        {
        }

        public VisaOprate(ILogger logger):base(logger)
        {
        }
        #endregion

        #region 析构方法

        ~VisaOprate() 
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
                ClearBuffer();
                Close();
            }

            Session?.Dispose();
            Session = null;
        }

        #endregion
    }
}
