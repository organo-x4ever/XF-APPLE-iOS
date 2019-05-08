using System;
using Xamarin.Forms;

namespace com.organo.x4ever.Views
{
    public enum FloatingActionButtonSize
    {
        Normal,
        NormalMini,
        Mini
    }

    public class FloatingActionButtonView : Image
    {
        public FloatingActionButtonView()
        {
           
        }

        public static readonly BindableProperty ImageNameProperty = BindableProperty.Create(
            propertyName: nameof(ImageName), returnType: typeof(string), declaringType: typeof(string),
            defaultValue: "");

        public string ImageName
        {
            get { return (string) GetValue(ImageNameProperty); }
            set { SetValue(ImageNameProperty, value); }
        }

        /// <summary>
        /// Thickness property of border
        /// </summary>
        public static readonly BindableProperty BorderThicknessProperty =
            BindableProperty.Create(propertyName: nameof(BorderThickness), returnType: typeof(float),
                declaringType: typeof(float), defaultValue: 0F);

        /// <summary>
        /// Border thickness of circle image
        /// </summary>
        public float BorderThickness
        {
            get { return (float) GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        /// <summary>
        /// Color property of border
        /// </summary>
        public static readonly BindableProperty BorderColorProperty =
            BindableProperty.Create(propertyName: nameof(BorderColor), returnType: typeof(Color),
                declaringType: typeof(Color), defaultValue: Color.Transparent);


        /// <summary>
        /// Border Color of circle image
        /// </summary>
        public Color BorderColor
        {
            get { return (Color) GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }


        public static readonly BindableProperty ImageWidthProperty =
            BindableProperty.Create(propertyName: nameof(ImageWidth), returnType: typeof(float),
                declaringType: typeof(float), defaultValue: 0F);

        public float ImageWidth
        {
            get { return (float) GetValue(ImageWidthProperty); }
            set { SetValue(ImageWidthProperty, value); }
        }

        public static readonly BindableProperty ImageHeightProperty =
            BindableProperty.Create(propertyName: nameof(ImageHeight), returnType: typeof(float),
                declaringType: typeof(float), defaultValue: 0F);

        public float ImageHeight
        {
            get { return (float) GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }

        /// <summary>
        /// Color property of fill
        /// </summary>
        public static readonly BindableProperty ColorNormalProperty =
            BindableProperty.Create(propertyName: nameof(ColorNormal), returnType: typeof(Color),
                declaringType: typeof(Color), defaultValue: Color.White);

        /// <summary>
        /// Fill color of circle image
        /// </summary>
        public Color ColorNormal
        {
            get { return (Color) GetValue(ColorNormalProperty); }
            set { SetValue(ColorNormalProperty, value); }
        }

        public static readonly BindableProperty ColorPressedProperty =
            BindableProperty.Create(propertyName: nameof(ColorPressed), returnType: typeof(Color),
                declaringType: typeof(Color), defaultValue: Color.White);

        public Color ColorPressed
        {
            get { return (Color) GetValue(ColorPressedProperty); }
            set { SetValue(ColorPressedProperty, value); }
        }

        public static readonly BindableProperty ColorRippleProperty =
            BindableProperty.Create(propertyName: nameof(ColorRipple), returnType: typeof(Color),
                declaringType: typeof(Color), defaultValue: Color.White);

        public Color ColorRipple
        {
            get { return (Color) GetValue(ColorRippleProperty); }
            set { SetValue(ColorRippleProperty, value); }
        }

        public static readonly BindableProperty SizeProperty = BindableProperty.Create(propertyName: nameof(Size),
            returnType: typeof(FloatingActionButtonSize), declaringType: typeof(FloatingActionButtonSize),
            defaultValue: FloatingActionButtonSize.Normal);

        public FloatingActionButtonSize Size
        {
            get { return (FloatingActionButtonSize) GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        public static readonly BindableProperty HasShadowProperty = BindableProperty.Create(
            propertyName: nameof(HasShadow), returnType: typeof(bool), declaringType: typeof(bool), defaultValue: true);

        public bool HasShadow
        {
            get { return (bool) GetValue(HasShadowProperty); }
            set { SetValue(HasShadowProperty, value); }
        }

        public delegate void ShowHideDelegate(bool animate = true);

        public delegate void AttachToListViewDelegate(ListView listView);

        public ShowHideDelegate Show { get; set; }
        public ShowHideDelegate Hide { get; set; }
        public Action<object, EventArgs> Clicked { get; set; }
    }
}