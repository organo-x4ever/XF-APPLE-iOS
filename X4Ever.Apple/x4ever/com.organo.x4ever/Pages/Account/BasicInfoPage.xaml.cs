using com.organo.x4ever.Localization;
using com.organo.x4ever.Models;
using com.organo.x4ever.Models.Validation;
using com.organo.x4ever.Pages.Base;
using com.organo.x4ever.Statics;
using com.organo.x4ever.ViewModels.Account;
using System;
using System.Threading.Tasks;
using com.organo.x4ever.Globals;
using com.organo.x4ever.Services;
using Xamarin.Forms;


namespace com.organo.x4ever.Pages.Account
{
    public partial class BasicInfoPage : AccountCreateXaml
    {
        private AccountViewModel _model;
        private UserFirstUpdate _user;
        private readonly IUserPivotService _userPivotService;
        private IHelper _helper;

        public BasicInfoPage(UserFirstUpdate user)
        {
            try
            {
                InitializeComponent();
                _user = user;
                _userPivotService = DependencyService.Get<IUserPivotService>();
                Init();
            }
            catch (Exception)
            {
                // Comment
            }
        }

        public sealed override async void Init(object obj = null)
        {
            await App.Configuration.InitialAsync(this);
            NavigationPage.SetHasNavigationBar(this, false);
            _helper = DependencyService.Get<IHelper>();
            _model = new AccountViewModel();
            BindingContext = _model;
            await _model.LoadAppLanguages(OnLanguageRetrieve);
            _model.FirstName = _user.UserFirstName;
            _model.LastName = _user.UserLastName;
            buttonSubmit.Clicked += async (sender, e) => { await NextStepAsync(); };
        }

        private void OnLanguageRetrieve()
        {
            languageOption.DataSource = _model.ApplicationLanguages;
            languageOption.ShowSelection = _model.ApplicationLanguages.Count > 1;
            languageOption.OnItemSelectedAction = OnLanguageSelected;
            languageOption.TextStyle = (Style) App.CurrentApp.Resources["labelStyleTableViewItem"];
            languageOption.FlagStyle = (Style) App.CurrentApp.Resources["imageEntryIcon"];
        }

        private void OnLanguageSelected()
        {
            _model.OnLanguageSelected(languageOption.DataSourceSelected);
        }

        private async void Entry_Completed(object sender, EventArgs e)
        {
            await NextStepAsync();
        }

        private async Task NextStepAsync()
        {
            _model.SetActivityResource(false, true, busyMessage: TextResources.ProcessingPleaseWait);
            if (Validate())
            {
                _user.UserFirstName = _model.FirstName;
                _user.UserLastName = _model.LastName;
                if (await _userPivotService.UpdateStep1Async(new UserStep1()
                {
                    UserEmail = _user.UserEmail,
                    UserType = _user.UserType,
                    UserFirstName = _user.UserFirstName,
                    UserLastName = _user.UserLastName
                }))
                    App.CurrentApp.MainPage = new PersonalInfoPage(_user);
                else
                    _model.SetActivityResource(showError: true,
                        errorMessage: _helper.ReturnMessage(_userPivotService.Message));
            }
        }

        private bool Validate()
        {
            ValidationErrors validationErrors = new ValidationErrors();
            if (string.IsNullOrEmpty(_model.FirstName))
                validationErrors.Add(string.Format(TextResources.Required_IsMandatory, TextResources.FirstName));
            if (string.IsNullOrEmpty(_model.LastName))
                validationErrors.Add(string.Format(TextResources.Required_IsMandatory, TextResources.LastName));
            if (validationErrors.Count() > 0)
                _model.SetActivityResource(showError: true, errorMessage: validationErrors.Show(CommonConstants.SPACE));
            return validationErrors.Count() == 0;
        }
    }

    public abstract class AccountCreateXaml : ModelBoundContentPage<AccountViewModel>
    {
    }
}