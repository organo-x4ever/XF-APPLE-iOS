using com.organo.x4ever.Controls;
using com.organo.x4ever.ios.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using CoreAnimation;
using CoreGraphics;
using System.Drawing;
using UIKit;

[assembly: ExportRenderer(typeof(EntryWithImageActions), typeof(EntryWithImageActionsRenderer))]

namespace com.organo.x4ever.ios.Renderers
{
    public class EntryWithImageActionsRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || e.NewElement == null)
                return;

            var element = (EntryWithImageActions) this.Element;
            var textField = this.Control;
            if (!string.IsNullOrEmpty(element.ImageLeft))
            {
                var view = GetImageView(element.ImageLeft, element.ImageLeftHeight, element.ImageLeftWidth);
                if (element.ImageLeftCommand != null)
                {
                    view.GestureRecognizers = new[]
                        {new UITapGestureRecognizer(() => { element.ImageLeftCommand?.Invoke(); })};
                }

                textField.LeftViewMode = UITextFieldViewMode.Always;
                textField.LeftView = view;
            }

            if (!string.IsNullOrEmpty(element.ImageRight))
            {
                var view = GetImageView(element.ImageRight, element.ImageRightHeight, element.ImageRightWidth);
                if (element.ImageRightCommand != null)
                {
                    view.GestureRecognizers = new[]
                        {new UITapGestureRecognizer(() => { element.ImageRightCommand?.Invoke(); })};
                }

                textField.RightViewMode = UITextFieldViewMode.Always;
                textField.RightView = view;
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