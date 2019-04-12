using com.organo.x4ever.Globals;
using com.organo.x4ever.Localization;
using com.organo.x4ever.Pages.Base;
using com.organo.x4ever.Services;
using com.organo.x4ever.Statics;
using com.organo.x4ever.ViewModels.ForgotPassword;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using com.organo.x4ever.Models.Validation;
using Xamarin.Forms;

namespace com.organo.x4ever.Pages.ForgotPassword
{
    public partial class RequestPasswordPage : RequestPasswordPageXaml
    {
        private readonly RequestPasswordViewModel _model;
        private readonly IUserPivotService _userPivotService;
        private readonly IHelper _helper;

        public RequestPasswordPage()
        {
            InitializeComponent();
            App.Configuration.Initial(this);
            NavigationPage.SetHasNavigationBar(this, false);
            _model = new RequestPasswordViewModel();
            BindingContext = _model;
            buttonSubmit.Clicked += ButtonSubmit_Clicked;
            _userPivotService = DependencyService.Get<IUserPivotService>();
            _helper = DependencyService.Get<IHelper>();
            var tapGotPassword = new TapGestureRecognizer()
            {
                Command = new Command(GoToLogin)
            };
            linkGotPassword.GestureRecognizers.Add(tapGotPassword);
        }

        private void GoToLogin() => App.GoToAccountPage();

        private async void ButtonSubmit_Clicked(object sender, EventArgs e)
        {
            await RequestPassword();
        }

        private async void Entry_Completed(object sender, EventArgs e)
        {
            await RequestPassword();
        }

        private async Task RequestPassword()
        {
            await Task.Run(() => { _model.SetActivityResource(false, true); });
            if (Validate())
            {
                _model.UserName = _model.EmailAddress.Trim();
                var response = await _userPivotService.RequestForgotPasswordAsync(
                    _model.EmailAddress.Trim(),
                    _model.EmailAddress.Trim());
                if (response != null)
                {
                    _model.SetActivityResource();
                    if (response.Contains(HttpConstants.SUCCESS))
                        App.CurrentApp.MainPage = new NewPassword(_model);
                    else
                        _model.SetActivityResource(true, showError: true,
                            errorMessage: _helper.ReturnMessage(response));
                }
                else
                    _model.SetActivityResource(true, showError: true,
                        errorMessage: TextResources.EmailIDNotFound);
            }
        }

        private bool Validate()
        {
            var validationErrors = new ValidationErrors();
            if (string.IsNullOrEmpty(_model.EmailAddress))
                validationErrors.Add(string.Format(TextResources.Required_IsMandatory, TextResources.EmailAddress));
            else if (!Regex.IsMatch(_model.EmailAddress.Trim(), CommonConstants.EMAIL_VALIDATION_REGEX))
                validationErrors.Add(string.Format(TextResources.Validation_IsInvalid, TextResources.EmailAddress));
            if (validationErrors.Count() > 0)
                _model.SetActivityResource(showError: true, errorMessage: validationErrors.Show(CommonConstants.SPACE));
            return validationErrors.Count() == 0;
        }
        public sealed override void Init(object obj = null)
        {
        }
    }

    public abstract class RequestPasswordPageXaml : ModelBoundContentPage<RequestPasswordViewModel>
    {
    }
}