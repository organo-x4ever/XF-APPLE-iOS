using com.organo.x4ever.Localization;
using System.Globalization;
using com.organo.x4ever.ios.Localization;
using Foundation;
using Xamarin.Forms;

[assembly: Dependency(typeof(Localize))]

namespace com.organo.x4ever.ios.Localization
{
    public class Localize : ILocalize
    {
        public CultureInfo GetCurrentCultureInfo()
        {
            CultureInfo cultureInfo = new CultureInfo(GetLanguage());
            return cultureInfo;
        }

        public CultureInfo GetCurrentCultureInfo(string langCode)
        {
            return new CultureInfo(langCode);
        }

        public string GetLanguage()
        {
            var iOSLocale = NSLocale.CurrentLocale.LocaleIdentifier;
            //var netLanguage = "en-US";
            return iOSLocale.ToString().Replace("_", "-"); // turns pt_BR into pt-BR
            //return netLanguage;
        }

        public string GetLanguage(string langCode)
        {
            var cultureInfo = new CultureInfo(langCode);
            return cultureInfo?.Name ?? langCode;
        }
    }
}