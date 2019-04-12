using com.organo.x4ever.Globals;
using com.organo.x4ever.Localization;
using com.organo.x4ever.Models.Validation;
using com.organo.x4ever.Pages.Base;
using com.organo.x4ever.Services;
using com.organo.x4ever.Statics;
using com.organo.x4ever.ViewModels.Profile;
using System;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace com.organo.x4ever.Pages.ChangePassword
{
    public partial class ChangePasswordPage : ChangePasswordPageXaml
    {
        private readonly SettingsViewModel _model;
        private readonly IUserPivotService _userPivotService;
        private readonly IHelper _helper;

        public ChangePasswordPage(RootPage root, SettingsViewModel model)
        {
            InitializeComponent();
            App.Configuration.Initial(this);
            _model = model;
            _model.Root = root;
            BindingContext = _model;
            Init();
            _userPivotService = DependencyService.Get<IUserPivotService>();
            _helper = DependencyService.Get<IHelper>();
            _model.SetActivityResource();
            _model.CurrentPassword = string.Empty;
            _model.NewPassword = string.Empty;
            _model.ConfirmNewPassword = string.Empty;
        }

        public sealed override void Init(object obj = null)
        {
            buttonSubmit.Clicked += async (sender, e) => { await RequestChangeAsync(); };
        }

        private async void Entry_Completed(object sender, EventArgs e)
        {
            await RequestChangeAsync();
        }

        private async Task RequestChangeAsync()
        {
            _model.SetActivityResource(false, true);
            if (Validate())
            {
                var response =
                    await _userPivotService.ChangePasswordAsync(_model.CurrentPassword.Trim(),
                        _model.NewPassword.Trim());
                if (response != null && response.Contains(HttpConstants.SUCCESS))
                {
                    _model.CurrentPassword = string.Empty;
                    _model.NewPassword = string.Empty;
                    _model.ConfirmNewPassword = string.Empty;
                }

                _model.SetActivityResource(showError: true, errorMessage: response.Contains(HttpConstants.SUCCESS)
                    ? TextResources.MessagePasswordChanged
                    : _helper.ReturnMessage(response));
            }
        }

        private bool Validate()
        {
            ValidationErrors validationErrors = new ValidationErrors();
            if (string.IsNullOrEmpty(_model.CurrentPassword))
                validationErrors.Add(string.Format(TextResources.Required_IsMandatory,
                    TextResources.CurrentPassword));
            if (string.IsNullOrEmpty(_model.NewPassword))
                validationErrors.Add(string.Format(TextResources.Required_IsMandatory, TextResources.NewPassword));
            else if (string.IsNullOrWhiteSpace(_model.NewPassword))
                validationErrors.Add(string.Format(TextResources.Validation_IsInvalid, TextResources.NewPassword));
            else if (_model.NewPassword.Trim().Length < 5)
                validationErrors.Add(string.Format(TextResources.Validation_LengthMustBeMoreThan,
                    TextResources.Password, 5));
            else if (_model.NewPassword.Trim().Length > 100)
                validationErrors.Add(string.Format(TextResources.Validation_LengthMustBeLessThan,
                    TextResources.Password, 100));
            if (string.IsNullOrEmpty(_model.ConfirmNewPassword))
                validationErrors.Add(string.Format(TextResources.Required_IsMandatory,
                    TextResources.ConfirmNewPassword));
            else if (_model.NewPassword != _model.ConfirmNewPassword)
                validationErrors.Add(TextResources.MessagePasswordAndConfirmPasswordNotMatch);
            if (validationErrors.Count() > 0)
                _model.SetActivityResource(showError: true, errorMessage: validationErrors.Show("\n"));
            return validationErrors.Count() == 0;
        }
    }

    public abstract class ChangePasswordPageXaml : ModelBoundContentPage<SettingsViewModel>
    {
    }
}