using Foundation;
using UIKit;
using com.organo.x4ever.Helpers;
using com.organo.x4ever.ios.Helpers;
using com.organo.x4ever.Services;
using System;
using System.Net;
using System.Threading.Tasks;
using com.organo.x4ever.Extensions;
using com.organo.x4ever.Handler;
using Xamarin.Forms;

[assembly:Dependency(typeof(AppVersionProvider))]

namespace com.organo.x4ever.ios.Helpers
{
    public class AppVersionProvider : IAppVersionProvider
    {
        static string GetBundleValue(string key)
            => NSBundle.MainBundle.ObjectForInfoDictionary(key).ToString();

        public string PackageName => $"{GetBundleValue("CFBundleIdentifier")}";
        public string Version => NSBundle.MainBundle.InfoDictionary[new NSString("CFBundleVersion")].ToString();

        public string VersionCode =>
            $"{NSBundle.MainBundle.InfoDictionary[new NSString("CFBundleVersionCode")].ToString()}";

        //https://itunes.apple.com/us/app/x4ever/id1413386991?ls=1&mt=8
        public string StoreUri => $"https://itunes.apple.com/us/app/{AppNameForStore}?ls=1&mt=8";
        public string AppVersionApi => $"https://mapp.oghq.ca/?id={PackageName}";
        public string MarketUri => $"market://details?id={PackageName}";
        public string AppName => DependencyService.Get<IDeviceInfo>().GetAppName;
        public string AppNameForStore { get; set; }
        private string _updateVersion { get; set; }
        public string UpdateVersion => _updateVersion;

        public string Get(string key)
        {
            return NSBundle.MainBundle.InfoDictionary[new NSString(key)].ToString();
        }

        public async Task<string> GetAsync(string key)
        {
            return await Task.Factory.StartNew(() =>
            {
                var info = NSBundle.MainBundle.InfoDictionary[new NSString(key)].ToString();
                return info;
            });
        }

        public async Task<bool> CheckAppVersionAsync(Action updateCallback)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(1500));
            return await CheckAppVersionAPIAsync(updateCallback);
        }

        private async Task<bool> CheckAppVersionAPIAsync(Action updateCallback)
        {
            return await Task.Run<bool>(async () =>
            {
                double.TryParse(this.Version.Replace(".", ""), out double currentVersion);
                var appInfoService = DependencyService.Get<IAppInfoService>();
                var appInfoModel = await appInfoService.GetAsync();

                AppNameForStore = appInfoModel?.AppName ?? "";
                _updateVersion = appInfoModel?.Version ?? "0";
                double.TryParse(_updateVersion.Replace(".", ""), out double appVersion);

                System.Diagnostics.Debug.WriteLine($"{this.Version} :: {_updateVersion}");
                System.Diagnostics.Debug.WriteLine($"{appVersion > currentVersion}");

                if (appVersion > currentVersion)
                {
                    updateCallback();
                    return true;
                }

                return false;
            });
        }

        public void GotoAppleAppStoreAsync()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    Device.OpenUri(new Uri(this.StoreUri));
                }
                catch (System.Exception ex)
                {
                    new ExceptionHandler(typeof(AppVersionProvider).FullName, ex);
                }
            });
        }
    }

    // For ANDROID

    //[assembly: Dependency(typeof(AppVersionProvider))]
    //namespace MyApp.Droid
    //{
    //    public class AppVersionProvider : IAppVersionProvider
    //    {
    //        public string AppVersion
    //        {
    //            get
    //            {
    //                var context = Android.App.Application.Context;
    //                var info = context.PackageManager.GetPackageInfo(context.PackageName, 0);

    //                return $"{info.VersionName}.{info.VersionCode.ToString()}";
    //            }
    //        }
    //    }
    //}

}