﻿using com.organo.x4ever.Globals;
using com.organo.x4ever.Handler;
using com.organo.x4ever.Models.Authentication;
using com.organo.x4ever.Pages;
using com.organo.x4ever.Pages.ErrorPages;
using com.organo.x4ever.Pages.MainPage;
using com.organo.x4ever.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace com.organo.x4ever
{
    public partial class App : Application
    {
        private readonly string TAG = typeof(App).FullName;
        private static Application app;
        public static Application CurrentApp => app;
        public static IAppConfiguration Configuration { get; set; }
        public static AuthenticationResult CurrentUser { get; set; }
        private static Page LastActivePage { get; set; }
        private static string Action { get; set; }

        //public static double DisplayScreenWidth = 0f;
        //public static double DisplayScreenHeight = 0f;
        //public static double DisplayScaleFactor = 0f;

        //public App()
        //{

        //    string ScreenDetails = Device.OS.ToString() + " Device Screen Size:\n" +
        //                           $"Width: {DisplayScreenWidth}\n" +
        //                           $"Height: {DisplayScreenHeight}\n" +
        //                           $"Scale Factor: {DisplayScaleFactor}";

        //    // The root page of your application
        //    var content = new ContentPage
        //    {
        //        Title = "Xamarin_GetDeviceScreensize",
        //        Content = new StackLayout
        //        {
        //            VerticalOptions = LayoutOptions.Center,
        //            Children = {
        //                new Label {
        //                    HorizontalTextAlignment = TextAlignment.Center,
        //                    FontSize = Device.GetNamedSize (NamedSize.Large, typeof(Label)),
        //                    Text = ScreenDetails
        //                }
        //            }
        //        }
        //    };

        //    MainPage = new NavigationPage(content);
        //}
        public App()
        {
            try
            {
                InitializeComponent();
                app = this;
            }
            catch (Exception ex)
            {
                var exceptionHandler = new ExceptionHandler(TAG, ex);
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            GoToInitialPage();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            LastActivePage = CurrentApp.MainPage;
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
            CurrentApp.MainPage = LastActivePage;
        }

        private async void GoToInitialPage()
        {
            Configuration = new AppConfiguration();
            GoToAccountPage();
            await Configuration.GetConnectionInfoAsync();
            if (!Configuration.IsConnected)
                CurrentApp.MainPage = new InternetConnectionPage();
        }

        public static async Task LogoutAsync() => await DependencyService.Get<IAuthenticationService>().LogoutAsync();

        public static void GoToAccountPage(bool loggedIn = false)
        {
            //CurrentApp.MainPage = new Pages.Media.AudioPlayerPage();
            if (loggedIn)
                CurrentApp.MainPage = new RootPage();
            else
                CurrentApp.MainPage = new MainPage();
        }
    }
}