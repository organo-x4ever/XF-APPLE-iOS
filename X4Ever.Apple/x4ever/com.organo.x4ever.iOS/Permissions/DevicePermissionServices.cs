using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Xamarin.Forms;
using com.organo.x4ever.ios.Permissions;
using com.organo.x4ever.Permissions;
using System.Threading.Tasks;

[assembly: Dependency(typeof(DevicePermissionServices))]
namespace com.organo.x4ever.ios.Permissions
{
    public class DevicePermissionServices : IDevicePermissionServices
    {
        public async Task<bool> RequestCameraPermission()
        {
            var result = true;
            return result;
        }

        public async Task<bool> RequestReadStoragePermission()
        {
            var result = true;
            return result;
        }

        public async Task<bool> RequestWriteStoragePermission()
        {
            var result = true;
            return result;
        }
    }
}