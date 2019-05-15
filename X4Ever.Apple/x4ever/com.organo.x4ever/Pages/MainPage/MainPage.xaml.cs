﻿
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using com.organo.x4ever.Handler;
using com.organo.x4ever.Helpers;
using com.organo.x4ever.Localization;
using com.organo.x4ever.Models;
using com.organo.x4ever.Models.Validation;
using com.organo.x4ever.Pages.Account;
using com.organo.x4ever.Pages.Base;
using com.organo.x4ever.Services;
using com.organo.x4ever.Statics;
using com.organo.x4ever.ViewModels.Login;
using Xamarin.Forms;

namespace com.organo.x4ever.Pages.MainPage
{
    public partial class MainPage : MainPageXaml
    {
        private IAuthenticationService _authenticationService;
        private IUserPivotService _userPivotService;
        private LoginViewModel _model;
        private string Message { get; set; }
        private readonly IAppVersionProvider _appVersionProvider = DependencyService.Get<IAppVersionProvider>();

        public MainPage(string message = "")
        {
            try
            {
                InitializeComponent();
                Message = message;
                _model = new LoginViewModel();
                BindingContext = _model;
                Init();
            }
            catch (Exception ex)
            {
                new ExceptionHandler(TAG, ex);
            }
        }

        public sealed override async void Init(object obj = null)
        {
            await App.Configuration.InitialAsync(this);
            NavigationPage.SetHasNavigationBar(this, false);
            _model.SetActivityResource(false, true);
            _userPivotService = DependencyService.Get<IUserPivotService>();
            _authenticationService = DependencyService.Get<IAuthenticationService>();
            await GetUserInfo();
        }

        private async Task GetUserInfo() => await _userPivotService.GetAuthenticationAsync(UserRedirect, Page_Load);

        
        private async void entry_Completed(object sender, EventArgs e)
        {
            await LoginCommand();
        }

        private async Task LoginCommand()
        {
            _model.SetActivityResource(false, true, busyMessage: TextResources.ValidatingCredentials);
            if (Validate())
            {
                if (await _authenticationService.AuthenticationAsync(EntryUsername.Text.Trim(),
                    EntryPassword.Text.Trim()))
                {
                    await GetUserInfo();
                }
                else
                {
                    EntryPassword.Text = string.Empty;
                    _model.SetActivityResource(true, showError: true,
                        errorMessage: string.IsNullOrEmpty(_authenticationService.Message)
                            ? TextResources.Message_LoginFailed
                            : _authenticationService.Message);
                }
            }
        }

        private async void UserRedirect()
        {
                _model.SetActivityResource(false, true, busyMessage: TextResources.LoggedInLoadingAccount);
            if (App.CurrentUser?.UserInfo != null)
            {
                await Task.Delay(1);
                var userInfo = App.CurrentUser?.UserInfo;
                var user = new UserFirstUpdate
                {
                    UserEmail = userInfo?.UserEmail,
                    UserFirstName = userInfo?.UserFirstName ?? "",
                    UserLastName = userInfo?.UserLastName ?? "",
                };
                if (userInfo?.UserFirstName?.Trim().Length == 0)
                    App.CurrentApp.MainPage = new BasicInfoPage(user);
                else if (userInfo?.IsMetaExists == false)
                    App.CurrentApp.MainPage = new PersonalInfoPage(user);
                else if (userInfo?.IsAddressExists == false)
                    App.CurrentApp.MainPage = new AddressPage(user);
                else if (userInfo?.IsTrackerExists == false)
                    App.CurrentApp.MainPage = new UploadPhotoPage(user);
                else
                    App.GoToAccountPage(true);
            }
            else
                _model.SetActivityResource();
        }

        private async void Page_Load()
        {
            _model.SetActivityResource();
            ButtonSignIn.Clicked += async (sender, e) => { await LoginCommand(); };
            Initialization(Message);
            VersionCheck();

            if (!App.Configuration.IsUserKeyExists())
            {
                await Task.Delay(TimeSpan.FromSeconds(2));
                App.Configuration.SetUserKey();
                await DependencyService.Get<IUserPushTokenServices>().SaveDeviceTokenUnauthorized();
            }
        }

        private void Initialization(string message = "")
        {
            if (message != null && message.Trim().Length > 0)
                _model.SetActivityResource(showMessage: true, message: message);
            EntryPassword.Text = string.Empty;
            EntryUsername.Text = string.Empty;
            EntryUsername.Focused += (sender, e) => { _model.BoxHeight_Username = 2; };
            EntryUsername.Unfocused += (sender, e) => { _model.BoxHeight_Username = 1; };
            EntryPassword.Focused += (sender, e) => { _model.BoxHeight_Password = 2; };
            EntryPassword.Unfocused += (sender, e) => { _model.BoxHeight_Password = 1; };
        }

        async void VersionCheck()
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            if (!App.Configuration.IsVersionPrompt())
            {
                await _appVersionProvider.CheckAppVersionAsync(PromptUpdate);

                void PromptUpdate()
                {
                    try
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            var updateMessage = string.Format(TextResources.UpdateMessageArgs,
                                _appVersionProvider.AppName,
                                _appVersionProvider.UpdateVersion);
                            var response = await DisplayAlert(TextResources.UpdateTitle, updateMessage,
                                TextResources.Update,
                                TextResources.Later);



                            App.Configuration.VersionPrompted();
                            if (response)
                                _appVersionProvider.GotoAppleAppStoreAsync();
                        });
                    }
                    catch (Exception e)
                    {
                        new ExceptionHandler(TAG, e);
                    }
                }
            }
        }

        private bool Validate()
        {
            var validationErrors = new ValidationErrors();
            if (string.IsNullOrEmpty(EntryUsername?.Text))
                validationErrors.Add(string.Format(TextResources.Required_IsMandatory, TextResources.Username));
            else if (!Regex.IsMatch((EntryUsername?.Text ?? "").Trim(), CommonConstants.EMAIL_VALIDATION_REGEX))
                validationErrors.Add(string.Format(TextResources.Validation_IsInvalid, TextResources.Username));
            if (string.IsNullOrEmpty(EntryPassword?.Text))
                validationErrors.Add(string.Format(TextResources.Required_IsMandatory, TextResources.Password));
            else if (string.IsNullOrWhiteSpace(EntryPassword.Text))
                validationErrors.Add(string.Format(TextResources.Validation_IsInvalid, TextResources.Password));
            else if (EntryPassword.Text.Trim().Length < 5)
                validationErrors.Add(string.Format(TextResources.Validation_LengthMustBeMoreThan,
                    TextResources.Password, 5));
            else if (EntryPassword.Text.Trim().Length > 50)
                validationErrors.Add(string.Format(TextResources.Validation_LengthMustBeLessThan,
                    TextResources.Password, 50));
            if (validationErrors.Count() > 0)
                _model.SetActivityResource(showError: true,
                    errorMessage: validationErrors.Count() > 2
                        ? TextResources.Required_AllInputs
                        : validationErrors.Show(CommonConstants.SPACE));
            return validationErrors.Count() == 0;
        }
    }

    public abstract class MainPageXaml : ModelBoundContentPage<LoginViewModel>
    {
    }
}