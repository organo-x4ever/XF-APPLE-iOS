using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using com.organo.x4ever.Helpers;
using com.organo.x4ever.ios.Renderers;
using com.organo.x4ever.Views;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(FloatingActionButtonView), typeof(FloatingActionButtonViewRenderer))]

namespace com.organo.x4ever.ios.Renderers
{
    /// <summary>
    /// FloatingActionButtonView Implementation
    /// </summary>
    [Preserve(AllMembers = true)]
    public class FloatingActionButtonViewRenderer : ImageRenderer
    {
        /// <summary>
        /// Used for registration with dependency service
        /// </summary>
        public new static void Init()
        {
            var temp = DateTime.Now;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            base.OnElementChanged(e);
            if (Element == null)
                return;
            CreateCircle();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == VisualElement.HeightProperty.PropertyName ||
                e.PropertyName == VisualElement.WidthProperty.PropertyName ||
                e.PropertyName == FloatingActionButtonView.BorderColorProperty.PropertyName ||
                e.PropertyName == FloatingActionButtonView.BorderThicknessProperty.PropertyName ||
                e.PropertyName == FloatingActionButtonView.ColorNormalProperty.PropertyName)
            {
                CreateCircle();
            }
        }

        private void CreateCircle()
        {
            try
            {
                SetFabSize(((FloatingActionButtonView) Element).Size);
                SetFabImage(((FloatingActionButtonView) Element).ImageName,
                    ((FloatingActionButtonView) Element).ImageWidth, ((FloatingActionButtonView) Element).ImageHeight);
                var min = Math.Min(Element.Width, Element.Height);
                Control.Layer.CornerRadius = (nfloat) (min / 2.0);
                Control.Layer.MasksToBounds = false;
                Control.BackgroundColor = ((FloatingActionButtonView) Element).ColorNormal.ToUIColor();
                Control.ClipsToBounds = true;

                ((FloatingActionButtonView) Element).Clicked += Fab_Click;

                //Remove previously added layers
                var tempLayer = Control.Layer.Sublayers?
                    .Where(p => p.Name == borderName)
                    .FirstOrDefault();
                tempLayer?.RemoveFromSuperLayer();

                var externalBorder = new CALayer();
                externalBorder.Name = borderName;
                externalBorder.CornerRadius = Control.Layer.CornerRadius;
                externalBorder.Frame = new CGRect(-.5, -.5, min + 1, min + 1);
                externalBorder.BorderColor = ((FloatingActionButtonView) Element).BorderColor.ToCGColor();
                externalBorder.BorderWidth = ((FloatingActionButtonView) Element).BorderThickness;

                Control.Layer.AddSublayer(externalBorder);
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine("Unable to create FloatingActionButtonView: " + ex);
            }
        }

        private void Fab_Click(object sender, EventArgs e)
        {
            var clicked = ((FloatingActionButtonView) Element).Clicked;
            if (Element != null && clicked != null)
            {
                clicked(sender, e);
            }
        }

        private void SetFabSize(FloatingActionButtonSize size)
        {
            if (size == FloatingActionButtonSize.Mini)
            {
                ((FloatingActionButtonView) Element).WidthRequest = FAB_MINI_FRAME_WIDTH_WITH_PADDING;
                ((FloatingActionButtonView) Element).HeightRequest = FAB_MINI_FRAME_HEIGHT_WITH_PADDING;
            }
            else if (size == FloatingActionButtonSize.NormalMini)
            {
                ((FloatingActionButtonView) Element).WidthRequest = FAB_NORMAL_MINI_FRAME_HEIGHT_WITH_PADDING;
                ((FloatingActionButtonView) Element).HeightRequest = FAB_NORMAL_MINI_FRAME_HEIGHT_WITH_PADDING;
            }
            else
            {
                ((FloatingActionButtonView) Element).WidthRequest = FAB_NORMAL_FRAME_WIDTH_WITH_PADDING;
                ((FloatingActionButtonView) Element).HeightRequest = FAB_NORMAL_FRAME_HEIGHT_WITH_PADDING;
            }
        }

        private void SetFabImage(string imageName, float width, float height)
        {
            if (!string.IsNullOrWhiteSpace(imageName))
            {
                try
                {
                    Element.Source = ImageResizer.ResizeImage(imageName, width, height);
                }
                catch (System.Exception ex)
                {
                    throw new FileNotFoundException("There was no Apple Resource by that name.", ex);
                }
            }
        }

        private const int MARGIN_DIPS_SMALL = 10;
        private const int MARGIN_DIPS = 16;
        private const int FAB_HEIGHT_NORMAL = 56;
        private const int FAB_HEIGHT_NORMAL_MINI = 48;
        private const int FAB_HEIGHT_MINI = 40;
        private const int FAB_NORMAL_FRAME_HEIGHT_WITH_PADDING = (MARGIN_DIPS_SMALL * 2) + FAB_HEIGHT_NORMAL;
        private const int FAB_NORMAL_FRAME_WIDTH_WITH_PADDING = (MARGIN_DIPS_SMALL * 2) + FAB_HEIGHT_NORMAL;

        private const int FAB_NORMAL_MINI_FRAME_HEIGHT_WITH_PADDING = (MARGIN_DIPS_SMALL * 2) + FAB_HEIGHT_NORMAL_MINI;
        private const int FAB_NORMAL_MINI_FRAME_WIDTH_WITH_PADDING = (MARGIN_DIPS_SMALL * 2) + FAB_HEIGHT_NORMAL_MINI;

        private const int FAB_MINI_FRAME_HEIGHT_WITH_PADDING = (MARGIN_DIPS_SMALL * 2) + FAB_HEIGHT_MINI;
        private const int FAB_MINI_FRAME_WIDTH_WITH_PADDING = (MARGIN_DIPS_SMALL * 2) + FAB_HEIGHT_MINI;


        const string borderName = "borderLayerName";
    }
}