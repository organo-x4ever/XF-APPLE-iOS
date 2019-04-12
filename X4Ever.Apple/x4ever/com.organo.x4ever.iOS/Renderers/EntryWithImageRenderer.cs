using com.organo.x4ever.Controls;
using com.organo.x4ever.ios;
using CoreAnimation;
using CoreGraphics;
using System.Drawing;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(EntryWithImage), typeof(EntryWithImageRenderer))]

namespace com.organo.x4ever.ios
{
    public class EntryWithImageRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || e.NewElement == null)
                return;

            var element = (EntryWithImage) this.Element;
            var textField = this.Control;
            if (!string.IsNullOrEmpty(element.Image))
            {
                var view = GetImageView(element.Image, element.ImageHeight, element.ImageWidth);
                if (element.Command != null)
                {
                    view.GestureRecognizers = new[] {new UITapGestureRecognizer(() => { element.Command?.Invoke(); })};
                }

                switch (element.ImageAlignment)
                {
                    case ImageAlignment.Left:
                        textField.LeftViewMode = UITextFieldViewMode.Always;
                        textField.LeftView = view;
                        break;
                    case ImageAlignment.Right:
                        textField.RightViewMode = UITextFieldViewMode.Always;
                        textField.RightView = view;
                        break;
                }
            }

            var backgroundColor = Color.Transparent;
            textField.BackgroundColor = backgroundColor.ToUIColor();
            textField.BorderStyle = UITextBorderStyle.None;
            CALayer bottomBorder = new CALayer
            {
                Frame = new CGRect(0.0f, element.HeightRequest - 1, this.Frame.Width, 1.0f),
                BorderWidth = 2.0f,
                BorderColor = element.LineColor.ToCGColor(),
            };
            textField.Layer.AddSublayer(bottomBorder);
            textField.Layer.MasksToBounds = true;
        }

        private UIView GetImageView(string imagePath, int height, int width)
        {
            var uiImageView = new UIImageView(UIImage.FromBundle(imagePath))
            {
                Frame = new RectangleF(0, 0, width, height)
            };
            UIView objLeftView = new UIView(new System.Drawing.Rectangle(0, 0, width + 10, height));
            objLeftView.AddSubview(uiImageView);

            return objLeftView;
        }
    }
}