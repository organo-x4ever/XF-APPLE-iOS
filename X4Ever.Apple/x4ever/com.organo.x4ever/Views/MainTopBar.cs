using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.organo.x4ever.Localization;
using com.organo.x4ever.Statics;
using Xamarin.Forms;

namespace com.organo.x4ever.Views
{
    public class MainTopBar : ContentView
    {
        /// <summary>
        /// ShowMenu property of Top Bar
        /// </summary>
        public static readonly BindableProperty ShowMenuProperty =
            BindableProperty.Create(propertyName: nameof(ShowMenu),
                returnType: typeof(bool),
                declaringType: typeof(MainTopBar),
                defaultValue: false);

        /// <summary>
        /// Display menu option
        /// </summary>
        public bool ShowMenu
        {
            get { return (bool) GetValue(ShowMenuProperty); }
            set
            {
                this.imageMenu.IsVisible = value;
                SetValue(ShowMenuProperty, value);
            }
        }
        
        public static readonly BindableProperty ShowSideMenuCommandProperty =
            BindableProperty.Create(propertyName: nameof(ShowSideMenuCommand),
                returnType: typeof(Action),
                declaringType: typeof(MainTopBar),
                defaultValue: null);

        public Action ShowSideMenuCommand
        {
            get { return (Action) GetValue(ShowSideMenuCommandProperty); }
            set { SetValue(ShowSideMenuCommandProperty, value); }
        }

        public Grid GridTopBar = new Grid()
        {
            BackgroundColor = Palette._MainAccent,
            HorizontalOptions = LayoutOptions.FillAndExpand,
            VerticalOptions = LayoutOptions.End,

            ColumnDefinitions =
            {
                new ColumnDefinition() {Width = 35},
                new ColumnDefinition() {Width = GridLength.Star},
                new ColumnDefinition() {Width = 35},
            },
            RowDefinitions =
            {
                new RowDefinition() {Height = 50},
            }
        };

        // IMAGE:MENU
        StackLayout stackLayoutMenu = new StackLayout()
        {
            Style = (Style)App.CurrentApp.Resources["stackMenuBar"],
            VerticalOptions = LayoutOptions.End,
        };

        private Image imageMenu = new Image();

        // IMAGE:LOGO
        StackLayout stackLayoutLogo = new StackLayout()
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.End,
        };

        private Image imageLogo = new Image();

        // RIGHT MOST
        StackLayout stackLayoutRight = new StackLayout()
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.End,
        };
        public MainTopBar()
        {
            try
            {
                if (ShowSideMenuCommand != null)
                {
                    var tapGestureRecognizer = new TapGestureRecognizer()
                    {
                        Command = new Command(ShowSideMenuCommand)
                    };
                    stackLayoutMenu.GestureRecognizers.Add(tapGestureRecognizer);
                }

                var imageSizeMenu =
                    App.Configuration.ImageSizes.FirstOrDefault(s => s.ImageID == ImageIdentity.TOP_BAR_MENU);

                imageMenu = new Image()
                {
                    Source = ImageSource.FromFile(imageSizeMenu?.ImageName),
                    Style = (Style) App.CurrentApp.Resources["imageLogoMenu"],
                    HeightRequest = imageSizeMenu.Height,
                    WidthRequest = imageSizeMenu.Width
                };
                stackLayoutMenu.Children.Add(imageMenu);
                GridTopBar.Children.Add(stackLayoutMenu, 0, 0);

                var imageSizeLogo =
                    App.Configuration.ImageSizes.FirstOrDefault(s => s.ImageID == ImageIdentity.TOP_BAR_LOGO);

                imageLogo = new Image()
                {
                    Source = ImageSource.FromFile(imageSizeLogo?.ImageName),
                    Style = (Style) App.CurrentApp.Resources["imageLogoTransparent"],
                    HeightRequest = imageSizeLogo.Height,
                    WidthRequest = imageSizeLogo.Width
                };
                stackLayoutLogo.Children.Add(imageLogo);
                GridTopBar.Children.Add(stackLayoutLogo, 1, 0);

                GridTopBar.Children.Add(stackLayoutRight, 2, 0);
                Content = GridTopBar;
            }
            catch (Exception ex)
            {
                var msg = ex;
            }
        }
    }
}








//    <Grid BackgroundColor="{x:Static statics:Palette._MainAccent}" HorizontalOptions="FillAndExpand" VerticalOptions="End">
//  <Grid.ColumnDefinitions>
//    <ColumnDefinition Width="35" />
//    <ColumnDefinition Width="*" />
//    <ColumnDefinition Width="35" />
//  </Grid.ColumnDefinitions>
//  <Grid.RowDefinitions>
//    <RowDefinition Height="50" />
//  </Grid.RowDefinitions>
//  <StackLayout Grid.Column="0" Style="{DynamicResource stackMenuBar}" VerticalOptions="End">
//    <Image Source="{translate:Translate icon_menu}" Style="{DynamicResource imageLogoMenu}">
//    </Image>
//    <StackLayout.GestureRecognizers>
//      <TapGestureRecognizer Command="{Binding ShowSideMenuCommand}" />
//    </StackLayout.GestureRecognizers>
//  </StackLayout>
//  <StackLayout Grid.Column="1" HorizontalOptions="Center" VerticalOptions="End">
//    <Image Source="{translate:Translate logo_transparent}" Style="{DynamicResource imageLogoTransparent}"></Image>
//  </StackLayout>
//</Grid>