using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.organo.x4ever.Helpers
{
    public interface IDeviceInfo
    {
        string GetModel { get; }
        string GetManufacturer { get; }
        string GetVersionString { get; }
        string GetPlatform { get; }
        string GetAppName { get; }

        int HeightPixels { get; }
        int WidthPixels { get; }
    }
}