using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMX.Common.Device.Inerfaces
{
    public interface ISaltFogbox : IDeviceOperate
    {
        OperateResult SetonOffAll(bool value);


    }
}
