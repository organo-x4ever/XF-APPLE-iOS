using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.organo.x4ever.Helpers;
using com.organo.x4ever.ios.Helpers;
using Foundation;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(DeviceInfo))]

namespace com.organo.x4ever.ios.Helpers
{
    public class DeviceInfo : IDeviceInfo
    {
        public string GetModel => GetBundleValue("CFBundleVersion");
        public string GetManufacturer => "Apple";
        public string GetVersionString => GetBundleValue("CFBundleShortVersionString");
        public string GetPlatform => "iOS";//GetBundleValue("CFBundleDisplayName") ?? GetBundleValue("CFBundleName");
        public string GetAppName => "X4Ever";

        static string GetBundleValue(string key)
            => NSBundle.MainBundle.ObjectForInfoDictionary(key).ToString();

        //static string PlatformGetPackageName() => GetBundleValue("CFBundleIdentifier");

        //static string PlatformGetName() => GetBundleValue("CFBundleDisplayName") ?? GetBundleValue("CFBundleName");

        //static string PlatformGetVersionString() => GetBundleValue("CFBundleShortVersionString");

        //static string PlatformGetBuild() => GetBundleValue("CFBundleVersion");

        static void PlatformOpenSettings() =>
            UIApplication.SharedApplication.OpenUrl(new NSUrl(UIApplication.OpenSettingsUrlString));
    }
}