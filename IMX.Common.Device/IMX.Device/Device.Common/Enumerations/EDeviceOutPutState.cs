using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device.Common
{
    public enum EDeviceOutPutState
    {
        OFF,
        ON
    }

    public enum EDeviceOutPutState_Vibration
    {
        设备未就绪 = -1,
        无状态 = 0,
        表示空闲 = 1,
        系统参数复位 = 2,
        测试系统噪声状态 = 3,
        开闭环检测 = 4,
        预试验阶段 = 5,
        正式试验状态 = 6,
        试验暂停状态 = 7,
        预试验停止状态 = 8,
        去直流偏置阶段 = 9,
        ICP初始化阶段 = 10,
        试验启动阶段 = 11,
        试验停止阶段 = 12,
        试验预览阶段 = 13,
    }
}
