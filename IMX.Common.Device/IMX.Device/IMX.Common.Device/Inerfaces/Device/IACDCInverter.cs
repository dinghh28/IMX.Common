using Device.Common.Enumerations;
using Super.Zoo.Framework;

namespace IMX.Common.Device.Inerfaces
{
    public interface IACDCInverter : IDeviceOperate
    {

        /// <summary>
        /// 启/停
        /// </summary>
        /// <param name="isOn"></param>
        /// <returns></returns>
        OperateResult Device_SetOnOff(bool isOn);

        /// <summary>
        /// 运行模式设置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        OperateResult Device_SetModel(EDeviceRunModel model);

        //OperateResult Device_Set_Vol(double value);

        /// <summary>
        /// 设备参数设置
        /// </summary>
        /// <param name="Vvalue">电压设置</param>
        /// <param name="Ivalue_Out">输出电流设置</param>
        /// <param name="IvalueIn">输入电流</param>
        /// <param name="IUpper">电流上限</param>
        /// <param name="ILower">电流下限</param>
        /// <param name="PUpeer">功率上限</param>
        /// <returns></returns>
        OperateResult Device_SetValue(double Vvalue, double Ivalue_Out, double IvalueIn, double IUpper, double ILower, double PUpeer);
    }
}
