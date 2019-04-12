using System.Threading.Tasks;
using com.organo.x4ever.Handler;
using com.organo.x4ever.Services;
using com.organo.x4ever.Statics;
using UIKit;
using Xamarin.Forms;

namespace com.organo.x4ever.ios
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            try
            {
                // if you want to use a different Application Delegate class from "AppDelegate"
                // you can specify it here.
                UIApplication.Main(args, null, "AppDelegate");
            }
            catch (TaskCanceledException ex)
            {
                var e = new ExceptionHandler(typeof(Application).FullName, ex);
            }
            catch (System.Exception ex)
            {
                var e = new ExceptionHandler(typeof(Application).FullName, ex);
            }
        }
    }
}