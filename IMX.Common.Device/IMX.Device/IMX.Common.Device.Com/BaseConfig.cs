using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using IMX.Common.Helper;

namespace Device.Common
{
    public class BaseConfig
    {
        #region
        /// <summary>
        /// 启动目录
        /// </summary>
        public static string StartupPath = AppDomain.CurrentDomain.BaseDirectory;

        public string XmlPath { get; set; }
        #endregion

        public BaseConfig(string path)
        {
            XmlPath = path;
        }

        /// <summary>
        /// 读取节点信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public OperateResult GetSections<T>(out T type) where T : new()
        {
            try
            {
                //using (FileStream stream = new FileStream(XmlPath, FileMode.Open))
                {
                    string configtext = File.ReadAllText(XmlPath);
                    type = XmlserializeHelper.DeSerialize<T>(configtext);
                }
                return OperateResult.Succeed();
            }
            catch (Exception e)
            {
                type = new T();
                return OperateResult.Failed(e.GetMessage());
            }
        }

        /// <summary>
        /// 读取节点信息
        /// </summary>
        /// <typeparam name="T">节点类型</typeparam>
        /// <returns></returns>
        public T GetSections<T>() where T : new()
        {
            try
            {
                T type;
                //using (FileStream stream = new FileStream(XmlPath, FileMode.Open))
                {
                    string configtext = File.ReadAllText(XmlPath);
                    type = XmlserializeHelper.DeSerialize<T>(configtext);
                }
                return type;
            }
            catch
            {
                return new T(); ;
            }
        }

        /// <summary>
        /// 写入节点信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public OperateResult WriteXml<T>(T type)
        {
            try
            {
                //using (FileStream stream = new FileStream(XmlPath, FileMode.OpenOrCreate))
                {
                    string x = XmlserializeHelper.Serialize<T>(type);
                    File.WriteAllText(XmlPath, x);
                }
                return OperateResult.Succeed();
            }
            catch (Exception e)
            {
                return OperateResult.Failed(e.GetMessage());
            }
        }
    }
}
