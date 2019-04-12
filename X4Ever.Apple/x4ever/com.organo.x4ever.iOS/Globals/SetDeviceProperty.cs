
using com.organo.x4ever.Globals;
using com.organo.x4ever.ios.Globals;
using Xamarin.Forms;
using Application = Xamarin.Forms.Application;

[assembly:Dependency(typeof(SetDeviceProperty))]

namespace com.organo.x4ever.ios.Globals
{
    public class SetDeviceProperty : ISetDeviceProperty
    {
        public void SetFullScreen()
        {
            //throw new NotImplementedException();
        }

        public void SetStatusBarColor(Color color)
        {
            //throw new NotImplementedException();
        }

        public void SetStatusBarColor(Color color, bool fullScreenMode)
        {
            //throw new NotImplementedException();
        }
    }
}