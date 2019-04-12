using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace com.organo.x4ever.Controls
{
    public class EntryWithImageActions : Entry
    {
        public EntryWithImageActions()
        {
            this.VerticalOptions = LayoutOptions.Center;
        }

        #region Image Side COMMON Properties

        public static readonly BindableProperty LineColorProperty =
            BindableProperty.Create(nameof(LineColor), typeof(Xamarin.Forms.Color), typeof(EntryWithImage),
                Color.White);

        public Color LineColor
        {
            get { return (Color) GetValue(LineColorProperty); }
            set { SetValue(LineColorProperty, value); }
        }

        #endregion

        #region Image Side LEFT Properties

        public static readonly BindableProperty ImageLeftProperty =
            BindableProperty.Create(nameof(ImageLeft), typeof(string), typeof(EntryWithImage), string.Empty);

        public static readonly BindableProperty ImageLeftHeightProperty =
            BindableProperty.Create(nameof(ImageLeftHeight), typeof(int), typeof(EntryWithImage), 40);

        public static readonly BindableProperty ImageLeftWidthProperty =
            BindableProperty.Create(nameof(ImageLeftWidth), typeof(int), typeof(EntryWithImage), 40);

        public static readonly BindableProperty ImageLeftCommandProperty =
            BindableProperty.Create(nameof(ImageLeftCommand), typeof(Action), typeof(EntryWithImage), null);

        public string ImageLeft
        {
            get { return (string) GetValue(ImageLeftProperty); }
            set { SetValue(ImageLeftProperty, value); }
        }

        public int ImageLeftHeight
        {
            get { return (int) GetValue(ImageLeftHeightProperty); }
            set { SetValue(ImageLeftHeightProperty, value); }
        }

        public int ImageLeftWidth
        {
            get { return (int) GetValue(ImageLeftWidthProperty); }
            set { SetValue(ImageLeftWidthProperty, value); }
        }

        public Action ImageLeftCommand
        {
            get { return (Action) GetValue(ImageLeftCommandProperty); }
            set { SetValue(ImageLeftCommandProperty, value); }
        }

        #endregion

        #region Image Side RIGHT Properties

        // Image Side Bindable Properties : RIGHT
        public static readonly BindableProperty ImageRightProperty =
            BindableProperty.Create(nameof(ImageRight), typeof(string), typeof(EntryWithImage), string.Empty);

        public static readonly BindableProperty ImageRightHeightProperty =
            BindableProperty.Create(nameof(ImageRightHeight), typeof(int), typeof(EntryWithImage), 40);

        public static readonly BindableProperty ImageRightWidthProperty =
            BindableProperty.Create(nameof(ImageRightWidth), typeof(int), typeof(EntryWithImage), 40);

        public static readonly BindableProperty ImageRightCommandProperty =
            BindableProperty.Create(nameof(ImageRightCommand), typeof(Action), typeof(EntryWithImage), null);

        // Image Side Properties : RIGHT
        public string ImageRight
        {
            get { return (string) GetValue(ImageRightProperty); }
            set { SetValue(ImageRightProperty, value); }
        }

        public int ImageRightHeight
        {
            get { return (int) GetValue(ImageRightHeightProperty); }
            set { SetValue(ImageRightHeightProperty, value); }
        }

        public int ImageRightWidth
        {
            get { return (int) GetValue(ImageRightWidthProperty); }
            set { SetValue(ImageRightWidthProperty, value); }
        }

        public Action ImageRightCommand
        {
            get { return (Action) GetValue(ImageRightCommandProperty); }
            set { SetValue(ImageRightCommandProperty, value); }
        }

        #endregion
    }
}