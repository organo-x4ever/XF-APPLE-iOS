using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace com.organo.x4ever.Controls
{
    public class CustomWebView : WebView
    {
        public static readonly BindableProperty UriProperty =
            BindableProperty.Create("Uri", typeof(string), typeof(string), default(string));

        public string Uri
        {
            get { return (string) GetValue(UriProperty); }
            set { SetValue(UriProperty, value); }
        }
    }
}