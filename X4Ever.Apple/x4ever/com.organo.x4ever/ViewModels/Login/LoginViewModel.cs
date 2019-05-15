using System;
using System.Windows.Input;
using com.organo.x4ever.Globals;
using com.organo.x4ever.Helpers;
using com.organo.x4ever.Localization;
using com.organo.x4ever.Models;
using com.organo.x4ever.Pages.ForgotPassword;
using com.organo.x4ever.Pages.Registration;
using com.organo.x4ever.Statics;
using com.organo.x4ever.ViewModels.Base;
using Xamarin.Forms;

namespace com.organo.x4ever.ViewModels.Login
{
    public class LoginViewModel : BaseViewModel
    {
        public LoginViewModel(INavigation navigation = null) : base(navigation)
        {
                this.EmailAddress = string.Empty;
                this.UserPassword = string.Empty;
                this.PageBackgroundImage = ImageResizer.ResizeImage(App.Configuration.BackgroundImage, 800, 1280);
                this.SetPageImageSize();
                //IsPassword = true;
                BoxHeight_Username = 1;
                BoxHeight_Password = 1;
                ShowPasswordAction = () => { IsPassword = IsPassword == false; };
                ApplicationVersion = string.Format(TextResources.AppVersion, App.Configuration.AppConfig.ApplicationVersion);
        }

        private string _applicationVersion;
        public const string ApplicationVersionPropertyName = "ApplicationVersion";

        public string ApplicationVersion
        {
            get { return _applicationVersion; }
            set { SetProperty(ref _applicationVersion, value, ApplicationVersionPropertyName); }
        }

        private string emailAddress;
        public const string EmailAddressPropertyName = "EmailAddress";

        public string EmailAddress
        {
            get { return emailAddress; }
            set { SetProperty(ref emailAddress, value, EmailAddressPropertyName); }
        }

        private string userPassword;
        public const string UserPasswordPropertyName = "UserPassword";

        public string UserPassword
        {
            get { return userPassword; }
            set { SetProperty(ref userPassword, value, UserPasswordPropertyName); }
        }

        private ImageSource _pageBackgroundImage;
        public const string PageBackgroundImagePropertyName = "PageBackgroundImage";

        public ImageSource PageBackgroundImage
        {
            get { return _pageBackgroundImage; }
            set { SetProperty(ref _pageBackgroundImage, value, PageBackgroundImagePropertyName); }
        }

        private void SetPageImageSize()
        {
            var imageSize = App.Configuration.GetImageSizeByID(ImageIdentity.MAIN_PAGE_LOGO);
            if (imageSize != null)
            {
                this.LogoHeight = imageSize.Height;
                this.LogoWidth = imageSize.Width;
            }

            imageSize = App.Configuration.GetImageSizeByID(ImageIdentity.MAIN_PAGE_XCHALLENGE_LOGO);
            if (imageSize != null)
            {
                this.XLogoHeight = imageSize.Height;
                this.XLogoWidth = imageSize.Width;
            }

            EyeImageSize = App.Configuration.GetImageSizeByID(ImageIdentity.IMAGE_EYE_PASSWORD);
            if (EyeImageSize != null)
            {
                EyeImageHeight = EyeImageSize.Height;
                EyeImageWidth = EyeImageSize.Width;
            }

            var controlImageSize = App.Configuration.GetImageSizeByID(ImageIdentity.ENTRY_EMAIL_ICON);
            if (controlImageSize != null)
            {
                ControlImageHeight = controlImageSize.Height;
                ControlImageWidth = controlImageSize.Width;
            }
        }

        private float logoHeight;
        public const string LogoHeightPropertyName = "LogoHeight";

        public float LogoHeight
        {
            get { return logoHeight; }
            set { SetProperty(ref logoHeight, value, LogoHeightPropertyName); }
        }

        private float logoWidth;
        public const string LogoWidthPropertyName = "LogoWidth";

        public float LogoWidth
        {
            get { return logoWidth; }
            set { SetProperty(ref logoWidth, value, LogoWidthPropertyName); }
        }

        private float xLogoHeight;
        public const string XLogoHeightPropertyName = "XLogoHeight";

        public float XLogoHeight
        {
            get { return xLogoHeight; }
            set { SetProperty(ref xLogoHeight, value, XLogoHeightPropertyName); }
        }

        private float xLogoWidth;
        public const string XLogoWidthPropertyName = "XLogoWidth";

        public float XLogoWidth
        {
            get { return xLogoWidth; }
            set { SetProperty(ref xLogoWidth, value, XLogoWidthPropertyName); }
        }

        private bool _isPassword;
        public const string IsPasswordPropertyName = "IsPassword";

        public bool IsPassword
        {
            get { return _isPassword; }
            set { SetProperty(ref _isPassword, value, IsPasswordPropertyName); }
        }

        //private void ShowPassword()
        //{
        //    EyeSource = ImageResizer.ResizeImage(IsPassword ? TextResources.icon_eye_hide : TextResources.icon_eye_show,
        //        EyeImageSize);
        //}

        private ImageSource _eyeSource;
        public const string EyeSourcePropertyName = "EyeSource";

        public ImageSource EyeSource
        {
            get { return _eyeSource; }
            set { SetProperty(ref _eyeSource, value, EyeSourcePropertyName); }
        }

        private float eyeImageHeight;
        public const string EyeImageHeightPropertyName = "EyeImageHeight";

        public float EyeImageHeight
        {
            get { return eyeImageHeight; }
            set { SetProperty(ref eyeImageHeight, value, EyeImageHeightPropertyName); }
        }

        private float eyeImageWidth;
        public const string EyeImageWidthPropertyName = "EyeImageWidth";

        public float EyeImageWidth
        {
            get { return eyeImageWidth; }
            set { SetProperty(ref eyeImageWidth, value, EyeImageWidthPropertyName); }
        }

        private ImageSize EyeImageSize { get; set; }


        private float controlImageHeight;
        public const string ControlImageHeightPropertyName = "ControlImageHeight";

        public float ControlImageHeight
        {
            get { return controlImageHeight; }
            set { SetProperty(ref controlImageHeight, value, ControlImageHeightPropertyName); }
        }

        private float controlImageWidth;
        public const string ControlImageWidthPropertyName = "ControlImageWidth";

        public float ControlImageWidth
        {
            get { return controlImageWidth; }
            set { SetProperty(ref controlImageWidth, value, ControlImageWidthPropertyName); }
        }

        private float boxHeight_Username;
        public const string BoxHeight_UsernamePropertyName = "BoxHeight_Username";

        public float BoxHeight_Username
        {
            get { return boxHeight_Username; }
            set { SetProperty(ref boxHeight_Username, value, BoxHeight_UsernamePropertyName); }
        }

        private float boxHeight_Password;
        public const string BoxHeight_PasswordPropertyName = "BoxHeight_Password";

        public float BoxHeight_Password
        {
            get { return boxHeight_Password; }
            set { SetProperty(ref boxHeight_Password, value, BoxHeight_PasswordPropertyName); }
        }

        public void ShowPasswordMethod()
        {
            ShowPasswordAction?.Invoke();
        }

        private Action _showPasswordAction;

        public Action ShowPasswordAction
        {
            get { return _showPasswordAction; }
            set
            {
                _showPasswordAction = value;
                ShowPasswordMethod();
            }
        }

        private Action _emptyAction;

        public Action EmptyAction
        {
            get { return _emptyAction; }
            set { _emptyAction = value; }
        }

        //private ICommand _showPasswordCommand;

        //public ICommand ShowPasswordCommand
        //{
        //    get
        //    {
        //        return _showPasswordCommand ??
        //               (_showPasswordCommand = new Command(() =>
        //               {
        //                   IsPassword = IsPassword == false;
        //               }));
        //    }
        //}

        private ICommand _registerCommand;

        public ICommand RegisterCommand
        {
            get
            {
                return _registerCommand ?? (_registerCommand = new Command(() =>
                {
                    App.CurrentApp.MainPage = new RegisterPage();
                }));
            }
        }

        private ICommand _forgotPasswordCommand;

        public ICommand ForgotPasswordCommand
        {
            get
            {
                return _forgotPasswordCommand ?? (_forgotPasswordCommand = new Command(() =>
                {
                    App.CurrentApp.MainPage = new RequestPasswordPage();
                }));
            }
        }
    }
}