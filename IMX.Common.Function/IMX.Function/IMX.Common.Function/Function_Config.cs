using IMX.Common.Function.Enumerations;
using IMX.Common.Logger;
using Newtonsoft.Json;
using Super.Zoo.Framework;
using Super.Zoo.Framework.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using JsonIgnore = Newtonsoft.Json.JsonIgnoreAttribute;

namespace IMX.Common.Function
{
    public abstract class Function_Config : IFuntionConfig
    {
        #region 公共属性

        #region 继承属性

        [JsonIgnore]
        public virtual Guid Identify { get; } = Guid.NewGuid();

        ///<inheritdoc/>
        [JsonIgnore]
        public virtual ILogger Logger { get; }

        ///<inheritdoc/>
        [JsonIgnore]
        public virtual bool OutterLogger { get; set; }

        ///<inheritdoc/>
        [JsonIgnore]
        public string LastError { get; set; }

        /// <summary>
        /// 试验运行状态
        /// </summary>
        [JsonIgnore]
        public List<bool> IsTestRunning { get; set; }

        ///// <summary>
        ///// 操作类
        ///// </summary>
        //[JsonIgnore]
        //public abstract IDeviceOperate Operate { get; set; }

        /// <summary>
        /// 支持的试验方法类型
        /// </summary>
        [JsonIgnore]
        public abstract FuncitonType SupportFuncitonType { get; }

        public abstract string DeviceAddress { get; set; }

        /// <inheritdoc/>
        public int DelayAfterRun { get; set; }
        #endregion

        /// <summary>
        /// 设备开关操作
        /// </summary>
        public virtual FOutPutStateType OperateType { get; set; } = FOutPutStateType.Null;
        #endregion

        #region 私有变量
        /// <summary>
        /// 试验配置类型字典 [试验类型, 试验配置文件类型]
        /// </summary>
        private static Dictionary<FuncitonType, Type> dicFuntionConfigSuffix = null;

        /// <summary>
        /// 试验配置文件类型列表
        /// </summary>
        private static readonly List<Type> lstFuntionConfigType = new List<Type>
        {
            typeof(FunConfig_ACDCInverter),
            typeof(FunConfig_DCSource),
            typeof(FunConfig_DwellTime),
            typeof(FunConfig_Gyroscopic),
            typeof(FunConfig_Product),
            typeof(FunConfig_Protect),
            typeof(FunConfig_Relay),
            typeof(FunConfig_TempBox),
            typeof(FunConfig_WaterBox),
            typeof(FunConfig_VibrationTable),
            typeof(FunConfig_SaltFogbox),
            typeof(FunConfig_APU),
            typeof(FunConfig_Equip),

        };
        #endregion

        #region 构造方法

        public Function_Config() : this(null)
        {
        }

        //public Function_Config(IDeviceOperate operate) :this(null,operate)
        //{
        //}

        public Function_Config(ILogger logger)
        {
            Logger = logger ?? SuperDHHLoggerManager.TestLogger;
            //Operate = operate;
            OutterLogger = logger == null;
        }
        #endregion

        #region 析构方法
        ~Function_Config()
        {
            Dispose();
        }

        public void Dispose()
        {
            //Operate?.Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion

        #region 静态方法

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="type">IFuntionConfig类型</param>
        /// <returns></returns>
        public static OperateResult<IFuntionConfig> Create(FuncitonType type)
        => Create(null, type);

        //public static OperateResult<IFuntionConfig> Create(ILogger logger, FuncitonType type)
        //=> Create(logger, type);

        //public static OperateResult<IFuntionConfig> Create(FuncitonType type, IDeviceOperate operate)
        //=> Create(null, type, operate);

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="type">IFuntionConfig类型</param>
        /// <returns></returns>
        public static OperateResult<IFuntionConfig> Create(ILogger logger, FuncitonType type)
        {
            if (dicFuntionConfigSuffix == null)
            {
                InitFuntionConfigSuffixDictionary();
            }

            if (dicFuntionConfigSuffix.ContainsKey(type))
            {
                if (Activator.CreateInstance(dicFuntionConfigSuffix[type], new object[] { logger }) is IFuntionConfig config)
                {
                    return OperateResult<IFuntionConfig>.Succeed(config);
                }
                else
                {
                    return OperateResult<IFuntionConfig>.Failed(null, $"试验方案配置文件创建失败！");
                }
            }

            return OperateResult<IFuntionConfig>.Failed(null, $"不支持的配置类型: {type}");
        }

        /// <summary>
        /// IFuntionConfig反序列化
        /// </summary>
        /// <param name="type">IFuntionConfig类型</param>
        /// <param name="config">序列化信息</param>
        /// <returns></returns>
        public static OperateResult<IFuntionConfig> DeJson(FuncitonType type, string config)
        {
            if (dicFuntionConfigSuffix == null)
            {
                InitFuntionConfigSuffixDictionary();
            }

            try
            {
                if (!dicFuntionConfigSuffix.ContainsKey(type))
                {
                    return OperateResult<IFuntionConfig>.Failed(null, $"不支持的配置类型: {type}");
                }

                //MethodInfo GetConfig = typeof(JsonConvert).GetMethod("DeserializeObject").MakeGenericMethod(dicFuntionConfigSuffix[type]);

                IFuntionConfig rlt = (IFuntionConfig)JsonConvert.DeserializeObject(config, dicFuntionConfigSuffix[type]);

                return OperateResult<IFuntionConfig>.Succeed(rlt);
            }
            catch (Exception ex)
            {
                return OperateResult<IFuntionConfig>.Excepted(null, ex);
            }

        }

        #region 私有方法

        /// <summary>
        /// 初始化消息文件后缀名字典
        /// </summary>
        private static void InitFuntionConfigSuffixDictionary()
        {
            dicFuntionConfigSuffix = new Dictionary<FuncitonType, Type>();

            foreach (var type in lstFuntionConfigType)
            {
                if (!typeof(IFuntionConfig).IsAssignableFrom(type)) { continue; }
                if (!(Activator.CreateInstance(type) is IFuntionConfig funtionConfig)) { continue; }


                if (!dicFuntionConfigSuffix.ContainsKey(funtionConfig.SupportFuncitonType))
                {
                    dicFuntionConfigSuffix.Add(funtionConfig.SupportFuncitonType, type);
                }

                funtionConfig.Dispose();
            }
        }
        #endregion
        #endregion

        #region 公共方法
        public abstract OperateResult Execute(object operate);

        //public abstract OperateResult Execute(object operate,object operate1,object operate2);

        //public abstract OperateResult Execute(IDeviceOperate operate);

        public void SetError(string error)
        {
            LastError = error;
        }

        public OperateResult<string> ToJson()
        {
            return OperateResult<string>.Succeed(JsonConvert.SerializeObject(this));
        }

        #endregion



    }
}
