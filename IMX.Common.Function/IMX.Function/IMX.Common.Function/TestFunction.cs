using IMX.Common.Function.Enumerations;
using Super.Zoo.Framework;
using Super.Zoo.Framework.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace IMX.Common.Function
{
    /// <summary>
    /// 试验方案类
    /// </summary>
    public class TestFunction : IIdentify, IDisposable
    {
        #region 公共属性
        public Guid Identify { get; } = Guid.NewGuid();

        /// <summary>
        /// 运行步骤
        /// </summary>
        public uint Step { get; set; }

        // /// <summary>
        // /// 试验类型
        // /// </summary>
        //public  FuncitonType Type { get; private set; }

        /// <summary>
        /// 配置信息接口
        /// </summary>
        public IFuntionConfig Config { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Comments { get; set; }
        #endregion

        #region 静态方法

        public static TestFunction Create(IFuntionConfig config) => new TestFunction(config);

        public static TestFunction Create(FuncitonType type) => new TestFunction(type);

        public static TestFunction Create(ILogger logger, FuncitonType type) => new TestFunction(logger, type);

        //public static TestFunction Create(IFuntionConfig config) => new TestFunction(config);
        #endregion


        #region 构造方法

        public TestFunction(FuncitonType type) : this(null, type)
        {

        }

        //public TestFunction(FuncitonType type, IDeviceOperate operate) :this(null, type, operate){ }

        public TestFunction(ILogger logger, FuncitonType type)
        {
            Config = Function_Config.Create(logger, type).Data;
        }

        public TestFunction(IFuntionConfig config)
        {
            Config = config;
        }

        //public TestFunction(IDeviceOperate operate, IFuntionConfig config) 
        //{
        //    Config = config;
        //    Config.Operate = operate;
        //}
        #endregion

        #region 析构方法
        ~TestFunction()
        {
            Dispose();
        }

        public void Dispose()
        {
            Config?.Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion



        #region 公共方法

        #endregion

    }
}
