using com.organo.x4ever.Globals;
using com.organo.x4ever.Localization;
using com.organo.x4ever.Pages.Base;
using com.organo.x4ever.Services;
using com.organo.x4ever.Statics;
using com.organo.x4ever.ViewModels.ForgotPassword;
using System.Threading.Tasks;
using com.organo.x4ever.Models.Validation;
using Xamarin.Forms;

namespace com.organo.x4ever.Pages.ForgotPassword
{
    public partial class NewPassword : NewPasswordXaml
    {
        private readonly RequestPasswordViewModel _model;
        private readonly IUserPivotService _userPivotService;
        private readonly IHelper _helper;

        public NewPassword(RequestPasswordViewModel model)
        {
            InitializeComponent();
            App.Configuration.Initial(this);
            NavigationPage.SetHasNavigationBar(this, false);
            _model = model;
            BindingContext = _model;
            _userPivotService = DependencyService.Get<IUserPivotService>();
            _helper = DependencyService.Get<IHelper>();
            buttonSubmit.Clicked += ButtonSubmit_Clicked;
            _model.SetActivityResource();
        }

        public sealed override void Init(object obj = null)
        {
        }

        private async void ButtonSubmit_Clicked(object sender, System.EventArgs e)
        {
            await ChangePassword();
        }

        private async void Entry_Completed(object sender, System.EventArgs e)
        {
            await ChangePassword();
        }

        private async Task ChangePassword()
        {
            _model.LayoutOptions = LayoutOptions.Center;
            _model.SetActivityResource(false, true);

            if (Validate())
            {
                var response = await _userPivotService.ChangeForgotPasswordAsync(
                    _model.SecretCode.Trim(), _model.Password.Trim());
                if (response != null)
                {
                    if (response.Contains(HttpConstants.SUCCESS))
                        App.CurrentApp.MainPage = new MainPage.MainPage(TextResources.MessagePasswordChanged);
                    else
                        _model.SetActivityResource(showError: true, errorMessage: _helper.ReturnMessage(response));
                }
            }
        }

        private bool Validate()
        {
            var validationErrors = new ValidationErrors();
            if (string.IsNullOrEmpty(_model.SecretCode))
                validationErrors.Add(string.Format(TextResources.Required_IsMandatory,
                    TextResources.AuthorizationCode));
            else if (string.IsNullOrEmpty(_model.Password))
                validationErrors.Add(string.Format(TextResources.Required_IsMandatory, TextResources.NewPassword));
            else if (_model.Password.Trim().Length < 5)
                validationErrors.Add(string.Format(TextResources.Validation_LengthMustBeMoreThan,
                    TextResources.Password, 5));
            else if (_model.Password.Trim().Length > 100)
                validationErrors.Add(string.Format(TextResources.Validation_LengthMustBeLessThan,
                    TextResources.Password, 100));
            else if (string.IsNullOrEmpty(_model.ConfirmPassword))
                validationErrors.Add(string.Format(TextResources.Required_IsMandatory,
                    TextResources.ConfirmPassword));
            else if (_model.Password.Trim() != _model.ConfirmPassword.Trim())
                validationErrors.Add(TextResources.MessagePasswordAndConfirmPasswordNotMatch);

            if (validationErrors.Count() > 0)
                _model.SetActivityResource(showError: true, errorMessage: validationErrors.Show(CommonConstants.SPACE));
            return validationErrors.Count() == 0;
        }
    }

    public abstract class NewPasswordXaml : ModelBoundContentPage<RequestPasswordViewModel>
    {
    }
}