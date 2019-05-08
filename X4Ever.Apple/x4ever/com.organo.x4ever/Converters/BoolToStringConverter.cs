using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Globalization;
using com.organo.x4ever.Extensions;
using com.organo.x4ever.Statics;
using Xamarin.Forms;

namespace com.organo.x4ever.Converters
{
    public class BoolToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return CommonConstants.NOVALUE.ToCapital();
            return ConvertToString((bool) value, targetType, parameter.ToString(), culture);
        }

        public string ConvertToString(bool value, Type targetType, string parameter, CultureInfo culture)
        {
            var returnValue = "";
            var par = parameter?.ToString().ToLower();
            switch (par)
            {
                case "yesno":
                    if ((bool) value)
                        returnValue = CommonConstants.YES;
                    else
                        returnValue = CommonConstants.NO;
                    break;
                case "onoff":
                    if ((bool) value)
                        returnValue = CommonConstants.ON;
                    else
                        returnValue = CommonConstants.OFF;
                    break;
                case "enabledisable":
                    if ((bool) value)
                        returnValue = CommonConstants.ENABLE;
                    else
                        returnValue = CommonConstants.DISABLE;
                    break;
                default:
                    returnValue = CommonConstants.NOVALUE;
                    break;
            }

            return returnValue.ToCapital();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}