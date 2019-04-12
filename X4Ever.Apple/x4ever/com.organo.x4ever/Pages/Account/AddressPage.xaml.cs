using com.organo.x4ever.Extensions;
using com.organo.x4ever.Localization;
using com.organo.x4ever.Models;
using com.organo.x4ever.Models.Validation;
using com.organo.x4ever.Pages.Base;
using com.organo.x4ever.Services;
using com.organo.x4ever.Statics;
using com.organo.x4ever.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using com.organo.x4ever.Globals;
using com.organo.x4ever.Models.User;
using Xamarin.Forms;

namespace com.organo.x4ever.Pages.Account
{
    public partial class AddressPage : AddressPageXaml
    {
        private AddressViewModel _model;
        private UserFirstUpdate _user;
        private readonly IMetaPivotService _metaPivotService;
        private readonly IHelper _helper;
        
        public AddressPage(UserFirstUpdate user)
        {
            try
            {
                InitializeComponent();
                _user = user;
                _metaPivotService = DependencyService.Get<IMetaPivotService>();
                _helper = DependencyService.Get<IHelper>();
                Init();
            }
            catch (Exception)
            {
                // Comment
            }
        }

        public sealed override void Init(object obj = null)
        {
            App.Configuration.Initial(this);
            NavigationPage.SetHasNavigationBar(this, false);
            _model = new AddressViewModel();
            BindingContext = _model;
            Initialization();
        }

        private async void Initialization()
        {
            try
            {
                await this._model.GetCountryList();
                if (_user?.UserMetas?.Count > 0)
                {
                    _model.CountryName = _user.UserMetas.ToList().Get(MetaEnum.country);
                    _model.Address = _user.UserMetas.ToList().Get(MetaEnum.address);
                    _model.CityName = _user.UserMetas.ToList().Get(MetaEnum.city);
                    _model.PostalCode = _user.UserMetas.ToList().Get(MetaEnum.postalcode);
                    if (!string.IsNullOrEmpty(_model.CountryName))
                        _model.GetStateList(_model.CountryName);
                    _model.StateName = _user.UserMetas.ToList().Get(MetaEnum.state);
                }

                pickerCountry.Focused += (sender, e) =>
                {
                    if (pickerCountry.ItemsSource == null || pickerCountry.ItemsSource.Count == 0)
                    {
                        pickerCountry.Unfocus();
                        _model.SetActivityResource(showError: true, errorMessage: TextResources.NoRecordToProcess);
                    }
                };
                pickerCountry.SelectedIndexChanged += (sender1, e1) =>
                {
                    var countrySelected = pickerCountry.SelectedItem;
                    if (countrySelected != null)
                    {
                        _model.CountryName = countrySelected.ToString();
                        _model.GetStateList(_model.CountryName);
                        entryAddress.Focus();
                    }
                };
                StateSetup();
                buttonNext.Clicked += async (sender, e) => { await NextStepAsync(); };
            }
            catch
            {
                //
            }
        }

        private void StateSetup()
        {
            pickerState.Focused += (sender, e) =>
            {
                if (_model.StateList != null && _model.StateList.Count == 0)
                    _model.SetActivityResource(showError: true,
                        errorMessage: string.Format(TextResources.Required_MustBeSelected, TextResources.Country));
            };
            pickerState.SelectedIndexChanged += (sender1, e1) =>
            {
                var stateSelected = pickerState.SelectedItem;
                if (stateSelected != null)
                {
                    _model.StateName = stateSelected.ToString();
                    entryPostalCode.Focus();
                }
            };
        }

        private async void entry_Completed(object sender, EventArgs e)
        {
            await NextStepAsync();
        }

        private async Task NextStepAsync()
        {
            var _metas = new List<Meta>();

            _model.SetActivityResource(false, true, busyMessage: TextResources.ProcessingPleaseWait);
            if (Validate())
            {
                _metas.Add(_metaPivotService.AddMeta(_model.CountryName, MetaConstants.COUNTRY,
                    MetaConstants.COUNTRY, MetaConstants.LABEL));
                _metas.Add(_metaPivotService.AddMeta(_model.Address, MetaConstants.ADDRESS,
                    MetaConstants.ADDRESS, MetaConstants.LABEL));
                _metas.Add(_metaPivotService.AddMeta(_model.CityName, MetaConstants.CITY,
                    MetaConstants.CITY, MetaConstants.LABEL));
                _metas.Add(_metaPivotService.AddMeta(_model.StateName, MetaConstants.STATE,
                    MetaConstants.STATE, MetaConstants.LABEL));
                _metas.Add(_metaPivotService.AddMeta(_model.PostalCode, MetaConstants.POSTAL_CODE,
                    MetaConstants.POSTAL_CODE, MetaConstants.LABEL));

                foreach (var meta in _metas)
                {
                    _user.UserMetas.Add(meta);
                }

                if (await _metaPivotService.SaveMetaStep2Async(_metas))
                    App.CurrentApp.MainPage = new UploadPhotoPage(_user);
                else
                    _model.SetActivityResource(showError: true,
                        errorMessage: _helper.ReturnMessage(_metaPivotService.Message));
            }
        }

        private bool Validate()
        {
            ValidationErrors validationErrors = new ValidationErrors();
            if (string.IsNullOrEmpty(_model.CountryName))
                validationErrors.Add(string.Format(TextResources.Required_IsMandatory, TextResources.Country));
            if (string.IsNullOrEmpty(_model.Address))
                validationErrors.Add(string.Format(TextResources.Required_IsMandatory, TextResources.Address));
            if (string.IsNullOrEmpty(_model.CityName))
                validationErrors.Add(string.Format(TextResources.Required_IsMandatory, TextResources.City));
            if (string.IsNullOrEmpty(_model.StateName))
                validationErrors.Add(string.Format(TextResources.Required_IsMandatory, TextResources.State));
            if (string.IsNullOrEmpty(_model.PostalCode))
                validationErrors.Add(string.Format(TextResources.Required_IsMandatory, TextResources.PostalCode));
            if (validationErrors.Count() > 0)
                _model.SetActivityResource(showError: true,
                    errorMessage: validationErrors.Count() > 2
                        ? TextResources.Required_AllInputs
                        : validationErrors.Show(CommonConstants.SPACE));
            return validationErrors.Count() == 0;
        }
    }

    public abstract class AddressPageXaml : ModelBoundContentPage<AddressViewModel>
    {
    }
}