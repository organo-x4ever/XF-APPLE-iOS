using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using com.organo.x4ever.Connection;
using com.organo.x4ever.ios.Connection;
using Xamarin.Forms;
using System.Threading.Tasks;
using Connectivity.Plugin;

[assembly: Dependency(typeof(InternetConnection))]

namespace com.organo.x4ever.ios.Connection
{
    public class InternetConnection : IInternetConnection
    {
        public bool Check()
        {
            return CrossConnectivity.Current.IsConnected;
        }

        public async Task<bool> CheckAsync()
        {
            return await Task.Factory.StartNew(() => { return CrossConnectivity.Current.IsConnected; });
        }
    }
}