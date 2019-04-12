using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.organo.x4ever.Globals;
using com.organo.x4ever.Helpers;
using com.organo.x4ever.Localization;
using com.organo.x4ever.Statics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace com.organo.x4ever.Extensions
{
    [ContentProperty("Source")]
    public class ImageResourceExtension : IMarkupExtension
    {
        public string Source { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Source == null)
            {
                return null;
            }

            // Do your translation lookup here, using whatever method you require
            var imageSize = App.Configuration.GetImageSizeByID(Source);
            if (imageSize == null || imageSize.IsDynamic)
                return null;

            return ImageResizer.ResizeImage(imageSize);
        }
    }
}