using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using com.organo.x4ever.Controls;
using com.organo.x4ever.ios.Renderers;
using com.organo.x4ever.Services;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly:ExportRenderer(typeof(CustomWebView),typeof(CustomWebViewRenderer))]

namespace com.organo.x4ever.ios.Renderers
{
    public class CustomWebViewRenderer : ViewRenderer<CustomWebView, UIWebView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<CustomWebView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                SetNativeControl(new UIWebView());
            }

            if (e.OldElement != null)
            {
                // Cleanup
            }

            if (e.NewElement != null)
            {
                var customWebView = Element as CustomWebView;
                var fileName = Path.Combine(DependencyService.Get<ILocalFile>().DownloadDirectoryPath(),
                    WebUtility.UrlEncode(customWebView.Uri));
                Control.LoadRequest(new NSUrlRequest(new NSUrl(fileName, false))); 
                Control.ScalesPageToFit = true;
            }
        }
    }
}