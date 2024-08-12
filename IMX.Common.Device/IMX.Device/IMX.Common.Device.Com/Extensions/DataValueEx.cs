using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device.Common
{
    /// <summary>
    /// 数据处理转换类
    /// </summary>
    public static class DataValueEx
    {
        /// <summary>
        /// 转换字符串
        /// </summary>
        /// <param name="srcList">string列表</param>
        /// <param name="spl">分隔符</param>
        /// <param name="format">默认"X2"</param>
        /// <returns></returns>
        public static string ToString(this IEnumerable<byte> srcList, string spl = ",", string format = "X2")
        {
            if (srcList == null)
                return null;
            if (srcList.Count() <= 0)
                return string.Empty;

            StringBuilder builder = new StringBuilder();
            int index = 0;
            int count = srcList.Count();
            foreach (var src in srcList)
            {
                builder.Append(string.IsNullOrWhiteSpace(format) ? src.ToString() : src.ToString(format));
                if (index != count - 1)
                    builder.Append(spl);
                index++;
            }
            return builder.ToString();
        }
    }
}
