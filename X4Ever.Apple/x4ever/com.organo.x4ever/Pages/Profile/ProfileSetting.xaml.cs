﻿using com.organo.x4ever.Globals;
using com.organo.x4ever.Localization;
using com.organo.x4ever.Models.User;
using com.organo.x4ever.Models.Validation;
using com.organo.x4ever.Pages.Base;
using com.organo.x4ever.Services;
using com.organo.x4ever.Statics;
using com.organo.x4ever.ViewModels.Profile;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace com.organo.x4ever.Pages.Profile
{
    public partial class ProfileSetting : ProfileSettingXaml
    {
        private readonly SettingsViewModel _model;
        private readonly IMetaPivotService _metaPivotService;

        public ProfileSetting(RootPage root, SettingsViewModel model)
        {
            try
            {
                InitializeComponent();
                App.Configuration.InitialAsync(this);
                NavigationPage.SetHasNavigationBar(this, false);
                _model = model;
                _model.Root = root;
                BindingContext = _model;
                _model.SetActivityResource();
                _model.CurrentPassword = string.Empty;
                _model.NewPassword = string.Empty;
                _model.ConfirmNewPassword = string.Empty;
                Init();
                _metaPivotService = DependencyService.Get<IMetaPivotService>();
            }
            catch (Exception ex)
            {
                DependencyService.Get<IMessage>().AlertAsync(TextResources.Alert,
                    ex.InnerException != null ? ex.InnerException.Message : ex.Message, TextResources.Ok);
            }
        }

        public sealed override void Init(object obj = null)
        {
            pickerCountry.SelectedIndexChanged += (sender1, e1) =>
            {
                if (pickerCountry.SelectedItem != null && _model.ProfileLoadingComplete)
                    entryAddress.Focus();
            };
            pickerState.SelectedIndexChanged += (sender1, e1) =>
            {
                if (pickerState.SelectedItem != null && _model.ProfileLoadingComplete)
                    entryPostalCode.Focus();
            };

            buttonSubmit.Clicked += async (sender, e) => { await UpdateProfileAsync(); };
        }

        private async Task UpdateProfileAsync()
        {
            await Task.Run(() => { _model.SetActivityResource(false, true); });
            if (Validate())
            {
                List<Meta> metaList = new List<Meta>();
                metaList.Add(_metaPivotService.AddMeta(_model.CountryName, MetaConstants.COUNTRY, MetaConstants.COUNTRY,
                    MetaConstants.LABEL));
                metaList.Add(_metaPivotService.AddMeta(_model.Address, MetaConstants.ADDRESS, MetaConstants.ADDRESS,
                    MetaConstants.LABEL));
                metaList.Add(_metaPivotService.AddMeta(_model.CityName, MetaConstants.CITY, MetaConstants.CITY,
                    MetaConstants.LABEL));
                metaList.Add(_metaPivotService.AddMeta(_model.StateName, MetaConstants.STATE, MetaConstants.STATE,
                    MetaConstants.LABEL));
                metaList.Add(_metaPivotService.AddMeta(_model.PostalCode, MetaConstants.POSTAL_CODE,
                    MetaConstants.POSTAL_CODE, MetaConstants.LABEL));
                var response = await _metaPivotService.SaveMetaAsync(metaList);
                _model.SetActivityResource();
                if (response == HttpConstants.SUCCESS)
                    _model.UserMeta = null;
                _model.SetActivityResource(showError: true,
                    errorMessage: response == HttpConstants.SUCCESS
                        ? TextResources.MessageUserDetailSaveSuccessful
                        : TextResources.MessageSomethingWentWrong);
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

        private async void entry_Completed(object sender, EventArgs e)
        {
            await UpdateProfileAsync();
        }
    }

    public abstract class ProfileSettingXaml : ModelBoundContentPage<SettingsViewModel>
    {
    }
}