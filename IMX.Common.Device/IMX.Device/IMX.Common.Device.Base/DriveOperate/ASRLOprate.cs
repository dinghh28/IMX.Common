using Device.Common;
using Ivi.Visa;
using NationalInstruments.Visa;
using System;
using System.Threading;
using Super.Zoo.Framework;
using Super.Zoo.Framework.Logger;
using System.IO.Ports;

namespace Device.Base.Operate
{
    internal class ASRLOprate : VisaOprate
    {
        #region 公共属性
        public override DriveType SupportFuncitonType => DriveType.ASRL;

        /// <summary>
        /// 是否使用原生接口
        /// </summary>
        public bool IsUseSerial { get; set; } = true;

        public SerialPort PrSerialPort { get; private set; }
        #endregion

        #region 私有变量
        private readonly object objLock = new object();
        #endregion

        #region 公共方法
        #region 串口操作
        public override OperateResult Open(ModDriveConfig config)
        {
            if (config == null)
            {
                LastError = $"资源字符不存在";
                Logger.Error(nameof(DriveCANOprate), nameof(Open), LastError);
                return OperateResult.Failed(LastError);
            }

            if (string.IsNullOrWhiteSpace(config.ResourceString))
            {
                LastError = $"资源字符串为空";
                Logger.Error(nameof(ASRLOprate), nameof(Open), LastError);
                return OperateResult.Failed(LastError);
            }
            if (config.ResourceString.IndexOf("::INSTR") < 0 && config.ResourceString.IndexOf("::SOCKET") < 0)
            {
                LastError = $"资源字符串为必须是以[::INSTR]结尾";
                Logger.Error(nameof(ASRLOprate), nameof(Open), LastError);
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
                    Logger.Error(nameof(ASRLOprate), nameof(Open), LastError);
                    return OperateResult.Failed(LastError);
                }

                if (eType != SupportFuncitonType)
                {
                    LastError = $"串口类型不匹配";
                    Logger.Error(nameof(ASRLOprate), nameof(Open), LastError);
                    return OperateResult.Failed(LastError);
                }
                #endregion

                #region 参数configString的解析
                SerialParity ASRL_Parity = SerialParity.None;                               // 串口通讯_校验位;
                int ASRL_DataBits = 8;                                                      // 串口通讯_数据位;
                SerialStopBitsMode ASRL_StopBits = SerialStopBitsMode.One;                  // 串口通讯_停止位;
                SerialFlowControlModes ASRL_FlowControl = SerialFlowControlModes.None;   // 串口通讯_流控制; 
                int ASRL_Address = 0;
                bool isuser = false;
                bRet = DeviceResourceHelper.DecodeConfigString(config.ConfigString, ref ASRL_Parity, ref ASRL_DataBits, ref ASRL_StopBits, ref ASRL_FlowControl, ref ASRL_Address,ref isuser);
                if (!bRet)
                {
                    LastError = bRet.Message;
                    Logger.Error(nameof(ASRLOprate), nameof(Open), LastError);
                    return OperateResult.Failed(LastError);
                } 
                #endregion

                #endregion

                /////////////////////////////////////////
                // 已经打开的 先执行关闭;
                string strErr = string.Empty;
                if (IsDriveOpen)
                {
                    Close();
                }

                IsUseSerial = isuser;

                if (IsUseSerial)
                {
                    PrSerialPort = new SerialPort($"COM{strParam1.Split(':')[0].Replace("ASRL","")}")
                    {
                        BaudRate = Convert.ToInt32(config.BaudRate),
                        DataBits = ASRL_DataBits,
                        StopBits = StopBits.One,
                        Parity = (Parity)ASRL_Parity,
                        ReadTimeout = config.TimeoutMS
                    };
                    PrSerialPort.Open();

                    IsClearBuffer = false;
                }
                else
                {
                    // 执行打开操作;
                    SerialSession serialSession = new SerialSession(config.ResourceString)
                    {
                        BaudRate = Convert.ToInt32(config.BaudRate),
                        Parity = ASRL_Parity,
                        DataBits = (short)ASRL_DataBits,
                        StopBits = ASRL_StopBits,
                        FlowControl = ASRL_FlowControl,
                        ReadTermination = SerialTerminationMethod.None,
                        TimeoutMilliseconds = config.TimeoutMS,
                        TerminationCharacterEnabled = config.TerminationCharacterEnabled,
                    };
                    Session = serialSession;

                    IsClearBuffer = true;
                }

                // 属性赋值;
                IsDriveOpen = true;
                DriveConfig = config;
                

                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Exception(nameof(ASRLOprate), nameof(Open), ex);
                IsDriveOpen = false;
                return OperateResult.Failed(LastError);
            }
        }

        public override OperateResult Close()
        {
            try
            {
                if (!IsDriveOpen)
                {
                    LastError = $"设备未启动";
                    Logger.Error(nameof(DriveCANOprate), nameof(Open), LastError);
                    return OperateResult.Failed(LastError);
                }

                Session?.Dispose();
                PrSerialPort?.Close();
                PrSerialPort?.Dispose();

                IsDriveOpen = false;
                DriveConfig = null;
                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Exception(nameof(ASRLOprate), nameof(Close), ex);
                return OperateResult.Failed(LastError);
            }
            finally 
            {
                Session?.Dispose();
                IsDriveOpen = false;
                DriveConfig = null;
            }
        }

        public override OperateResult ClearBuffer()
        {
            try
            {
                if (!IsDriveOpen) 
                {
                    LastError = $"设备未启动";
                    Logger.Error(nameof(DriveCANOprate), nameof(Open), LastError);
                    return OperateResult.Failed(LastError);
                }

                Session?.Clear();
                PrSerialPort?.ReadExisting();

                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Exception(nameof(ASRLOprate), nameof(ClearBuffer), ex);
                return OperateResult.Failed(LastError);
            }
        }
        #endregion

        #region 写操作.Only
        /// <summary>
        /// 上次发送的命令字符串;
        /// </summary>
        private string _strLastWriteCmd = string.Empty;
        public override OperateResult  Write(string writeValue)
        {
            if (IsUseSerial)
            {
                if (!IsDriveOpen || PrSerialPort == null)
                {
                    LastError = $"串口未打开";
                    Logger.Error(nameof(ASRLOprate), nameof(Write), LastError);
                    return OperateResult.Failed(LastError);
                }
                try
                {
                    if (this.IsWriteCmdWithNewLine && !writeValue.EndsWith("\n")) writeValue += this.WriteCmdWithNewLine;

                    PrSerialPort.Write(writeValue);
                    //this._strLastWriteCmd = writeValue;

                    // 复位失败次数标记置为0

                    this.ReOpen_CurWriteFailCount = 0;

                    return OperateResult.Succeed();
                }
                catch (Exception ex)
                {
                    LastError = ex.GetMessage();
                    Logger.Exception(nameof(ASRLOprate), nameof(Write), ex);
                    return OperateResult.Failed(LastError);
                }
            }
            else
            {
                return base.Write(writeValue);
            }
        }

        public override OperateResult Write_ByteArray(byte[] writeValue)
        {
            //lock (objLock)
            //{
                if (IsUseSerial)
                {
                    if (!IsDriveOpen || PrSerialPort == null)
                    {
                        LastError = $"串口未打开";
                        Logger.Error(nameof(ASRLOprate), nameof(Write), LastError);
                        return OperateResult.Failed(LastError);
                    }

                    if (writeValue == null || writeValue.Length <= 0)
                    {
                        LastError = "传入参数为NULL";
                        Logger.Error(nameof(ASRLOprate), nameof(Write_ByteArray), LastError);
                        return OperateResult.Failed(LastError);
                    }

                    try
                    {
                        //if (this.IsWriteCmdWithNewLine && !writeValue.EndsWith("\n")) writeValue += this.WriteCmdWithNewLine;

                        for (int i = 0; i < 3; i++)
                        {
                            try
                            {
                                // 进行读取操作的代码
                                PrSerialPort.Write(writeValue, 0, writeValue.Length);
                                break; // 如果读取成功，则跳出循环
                            }
                            catch (IOTimeoutException)
                            {
                                // 处理超时异常，等待一段时间后重试
                                Thread.Sleep(5000);
                            }
                        }

                        //this._strLastWriteCmd = writeValue;

                        // 复位失败次数标记置为0

                        this.ReOpen_CurWriteFailCount = 0;

                        return OperateResult.Succeed();
                    }
                    catch (Exception ex)
                    {
                        LastError = ex.GetMessage();
                        Logger.Exception(nameof(ASRLOprate), nameof(Write), ex);
                        return OperateResult.Failed(LastError);
                    }
                }
                else
                {
                    //(Session as SerialSession).WireMode = WireMode.Rs485Wire2Auto;
                    return base.Write_ByteArray(writeValue);
                }
            //}



            //try
            //{
            //    if (!IsDriveOpen || Session == null)
            //    {
            //        LastError = $"串口未打开";
            //        Logger.Error(nameof(ASRLOprate), nameof(Write_ByteArray), LastError);
            //        return OperateResult.Failed(LastError);
            //    }

            //    if (writeValue == null || writeValue.Length <= 0)
            //    {
            //        LastError = "传入参数为NULL";
            //        Logger.Error(nameof(ASRLOprate), nameof(Write_ByteArray), LastError);
            //        return OperateResult.Failed(LastError);
            //    }
            //    for (int i = 0; i < 3; i++)
            //    {
            //        try
            //        {
            //            // 进行读取操作的代码
            //            this.Session.RawIO.Write(writeValue);
            //            break; // 如果读取成功，则跳出循环
            //        }
            //        catch (IOTimeoutException)
            //        {
            //            // 处理超时异常，等待一段时间后重试
            //            Thread.Sleep(5000);
            //        }
            //    }

            //    //Session.FormattedIO.FlushWrite(true);
            //    //this._strLastWriteCmd = writeValue.ToString(" ", "X2");

            //    // 复位失败次数标记置为0
            //    ReOpen_CurWriteFailCount = 0;

            //    return OperateResult.Succeed();
            //}
            //catch (Exception ex)
            //{
            //    Thread.Sleep(50);
            //    LastError = ex.GetMessage();
            //    Logger.Exception(nameof(ASRLOprate), nameof(Write_ByteArray), ex);
            //    return OperateResult.Failed(LastError);
            //}
        }
        #endregion

        #region 读操作.Only

        public override OperateResult<byte[]> Read_ByteArray(int readCount)
        {
            //lock (objLock)
            //{
                if (IsUseSerial)
                {
                    byte[] readByte = null;
                    if (!IsDriveOpen || PrSerialPort == null)
                    {
                        LastError = $"串口未打开";
                        Logger.Error(nameof(ASRLOprate), nameof(Write), LastError);
                        return OperateResult<byte[]>.Failed(readByte, LastError);
                    }

                    try
                    {
                        if (PrSerialPort.BytesToRead < 1)
                        {
                            LastError = $"驱动未接收到数据";
                            Logger.Error(nameof(ASRLOprate), nameof(Read_ByteArray), LastError);
                            return OperateResult<byte[]>.Failed(null, LastError);
                        }

                        if (readCount < 0)
                        {
                            readByte = new byte[PrSerialPort.BytesToRead];
                            int readNum = PrSerialPort.Read(readByte, 0, PrSerialPort.BytesToRead);
                            if (readByte == null || readNum <= 0)
                            {
                                LastError = $"未读取到相应数据";
                                Logger.Error(nameof(ASRLOprate), nameof(Read_ByteArray), LastError);
                                return OperateResult<byte[]>.Failed(null, LastError);
                            }

                            return OperateResult<byte[]>.Succeed(readByte);
                        }
                        else if (readCount > 0)
                        {
                            // 这里有可能读取长度没有读取完成的情况;
                            int nReadDelayMS = DelayMS_Read_ByteArray;     // 每次读取之间延时毫秒(默认100ms);
                            int nTotalCount = 3;        // 允许读取总次数(默认20次, 每次读取之间延时100ms);
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
                                //readDataTemp = Session.FormattedIO.ReadListOfByte(leftReadCount);
                                int nReadLen = PrSerialPort.Read(readDataTemp, nRevIndex, leftReadCount);

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

                                LastError = $"上次Write[{_strLastWriteCmd.Trim()}]\r\n本次读取数据超时[{revData.ToString(" ", "X2")}]";
                                Logger.Error(nameof(ASRLOprate), nameof(Read_ByteArray), LastError);
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
                            LastError = $"传入参数readCount[{readCount}]不正确";
                            Logger.Error(nameof(ASRLOprate), nameof(Read_ByteArray), LastError);
                            return OperateResult<byte[]>.Failed(null, LastError);
                        }
                    }
                    catch (Exception ex)
                    {
                        LastError = ex.GetMessage();
                        Logger.Exception(nameof(ASRLOprate), nameof(Read_ByteArray), ex);
                        return OperateResult<byte[]>.Failed(readByte, LastError);
                    }
                }
                else
                {
                    if ((Session as SerialSession)?.BytesAvailable < 1)
                    {
                        LastError = $"驱动未接收到数据";
                        Logger.Error(nameof(ASRLOprate), nameof(Read_ByteArray), LastError);
                        return OperateResult<byte[]>.Failed(null, LastError);
                    }
                    return base.Read_ByteArray(readCount);
                }
            //}
        }

        public override OperateResult<string> Read_StringExisting()
        {
            try
            {
                if (!this.IsDriveOpen || this.Session == null)
                {
                    LastError = $"Read_StringExisting.串口未打开";
                    Logger.Error(nameof(ASRLOprate), nameof(Read_StringExisting), LastError);
                    return OperateResult<string>.Failed(null, LastError);
                }

                string txt = Session.RawIO.ReadString();
                if (!string.IsNullOrEmpty(txt))
                {
                    LastError = $"Read_StringExisting.读取到字符串为空";
                    Logger.Error(nameof(ASRLOprate), nameof(Read_StringExisting), LastError);
                    return OperateResult<string>.Failed(null, LastError);
                }

                return OperateResult<string>.Succeed(txt.Trim());
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Exception(nameof(ASRLOprate), nameof(Read_StringExisting), ex);
                return OperateResult<string>.Failed(null, LastError);
            }
        }
        #endregion

        public override OperateResult<byte[]> WriteRead_ByteArray(byte[] writeValue, int Len, int delayMS = -1) 
        {
            lock (objLock)
            {
                return base.WriteRead_ByteArray(writeValue, Len, delayMS);
            }
        }

        /// <summary>
        /// 获取串口字节数
        /// </summary>
        /// <returns></returns>
        public OperateResult<int> GetByteNum()
        {
            if (IsUseSerial)
            {
                return OperateResult<int>.Succeed(PrSerialPort.BytesToRead);
            }
            if (Session is SerialSession serial)
            {
                return OperateResult<int>.Succeed(serial.BytesAvailable);
            }
            else
            {
                LastError = $"串口类型异常";
                return OperateResult<int>.Failed(0, LastError);
            }
        }
        #endregion

        #region 静态方法
        /// <summary>
        /// 创建设备ASRL操作类
        /// </summary>
        /// <returns></returns>
        public static ASRLOprate Create() => Create(null);

        /// <summary>
        /// 创建设备ASRL操作类
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <returns></returns>
        public static ASRLOprate Create(ILogger logger) => new ASRLOprate(logger);
        #endregion

        #region 构造方法
        public ASRLOprate():base(null)
        {
            
        }

        public ASRLOprate(ILogger logger):base(logger)
        {
        }
        #endregion

        #region 析构方法
        ~ASRLOprate() 
        {
            Dispose();
            //GC.SuppressFinalize(this);
        }
        #endregion

    }
}
