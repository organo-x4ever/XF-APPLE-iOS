﻿using System;
using Xamarin.Forms;

namespace com.organo.x4ever.Views
{
    public enum FloatingActionButtonSize
    {
        Normal,
        Mini
    }

    public class FloatingActionButtonView : View
    {

        //public static readonly BindableProperty ImageNameProperty = BindableProperty.Create<FloatingActionButtonView, string>(p => p.ImageName, string.Empty);
        public static readonly BindableProperty ImageNameProperty =
            BindableProperty.Create(ImageNamePropertyName, typeof(string), typeof(string), string.Empty);

        private const string ImageNamePropertyName = "ImageName";

        public string ImageName
        {
            get { return (string) GetValue(ImageNameProperty); }
            set { SetValue(ImageNameProperty, value); }
        }

        public static readonly BindableProperty ColorNormalProperty = BindableProperty.Create("ColorNormal", typeof(Color), typeof(Color), Color.White);

        public Color ColorNormal
        {
            get { return (Color) GetValue(ColorNormalProperty); }
            set { SetValue(ColorNormalProperty, value); }
        }

        public static readonly BindableProperty ColorPressedProperty =
            BindableProperty.Create("ColorPressed", typeof(Color), typeof(Color), Color.White);

        public Color ColorPressed
        {
            get { return (Color) GetValue(ColorPressedProperty); }
            set { SetValue(ColorPressedProperty, value); }
        }

        public static readonly BindableProperty ColorRippleProperty =
            BindableProperty.Create("ColorRipple", typeof(Color), typeof(Color), Color.White);

        public Color ColorRipple
        {
            get { return (Color) GetValue(ColorRippleProperty); }
            set { SetValue(ColorRippleProperty, value); }
        }

        public static readonly BindableProperty SizeProperty = BindableProperty.Create("Size",
            typeof(FloatingActionButtonSize), typeof(FloatingActionButtonSize), FloatingActionButtonSize.Normal);

        public FloatingActionButtonSize Size
        {
            get { return (FloatingActionButtonSize) GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        public static readonly BindableProperty HasShadowProperty =
            BindableProperty.Create("HasShadow", typeof(bool), typeof(bool), true);

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