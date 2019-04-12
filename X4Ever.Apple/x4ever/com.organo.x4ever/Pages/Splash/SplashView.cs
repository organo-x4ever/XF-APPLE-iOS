using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace com.organo.x4ever.Pages.Splash
{
    public class SplashView : ContentPage
    {
        private Image splashImage;
        private INavigation navigation;

        public SplashView(INavigation navigation)
        {
            try
            {
                this.navigation = navigation;
                App.Configuration.Initial(this, false);
                NavigationPage.SetHasNavigationBar(this, false);
                var sub = new AbsoluteLayout();
                splashImage = new Image()
                {
                    Source = "logo.png",
                    WidthRequest = 150,
                    HeightRequest = 110
                };

                AbsoluteLayout.SetLayoutFlags(splashImage,
                    AbsoluteLayoutFlags.PositionProportional);

                AbsoluteLayout.SetLayoutBounds(splashImage,
                    new Rectangle(0.5, 0.5, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));

                sub.Children.Add(splashImage);
                this.Content = sub;
            }
            catch (Exception ex)
            {
                var m = ex;
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                await splashImage.ScaleTo(1, 1200);
                await splashImage.ScaleTo(0.9, 1000);
                await splashImage.ScaleTo(150, 700);
                await App.CurrentApp.MainPage.Navigation.PopModalAsync(true);
            }
            catch (Exception exception)
            {
                var msg = exception;
            }
        }
    }
}