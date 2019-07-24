using com.organo.x4ever.Globals;
using com.organo.x4ever.Localization;
using com.organo.x4ever.Models;
using com.organo.x4ever.Pages.Base;
using com.organo.x4ever.Services;
using com.organo.x4ever.ViewModels.Profile;
using System;
using System.Threading.Tasks;
using com.organo.x4ever.Handler;
using Xamarin.Forms;
using com.organo.x4ever.ViewModels.UserSettings;

namespace com.organo.x4ever.Pages.UserSettings
{
    public partial class UserSettingPage : UserSettingPageXaml
    {
        private readonly UserSettingsViewModel _model;
        private bool IsEventAllowed = false;

        public UserSettingPage()
        {
            try
            {
                InitializeComponent();
                _model = new UserSettingsViewModel();
                Init();
            }
            catch (Exception ex)
            {
                new ExceptionHandler(typeof(UserSettingPage).FullName, ex);
            }
        }

        public sealed override async void Init(object obj = null)
        {
            await App.Configuration.InitialAsync(this);
            NavigationPage.SetHasNavigationBar(this, true);
            IsEventAllowed = false;
            BindingContext = _model;
            _model.SetActivityResource();
            await Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromMilliseconds(10));
                await _model.LoadAppLanguages(OnLanguageRetrieve);
                await _model.LoadWeightVolume(BindWeightVolume);
                Device.BeginInvokeOnMainThread(() =>
                {
                    tableView.Focus();
                    LanguageSelect_OnTapped(tableSection[0] as ViewCell, new EventArgs());
                    WeightVolumeSelect_OnTapped(tableSection[0] as ViewCell, new EventArgs());
                    IsEventAllowed = true;
                });
            });
        }

        private void OnLanguageRetrieve()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                languageOption.DataSource = _model.ApplicationLanguages;
                languageOption.ShowSelection = _model.ApplicationLanguages.Count > 1;
                languageOption.OnItemSelectedAction = OnLanguageSelected;
                languageOption.TextStyle = (Style)App.CurrentApp.Resources["labelStyleTableViewItem"];
                languageOption.FlagStyle = (Style)App.CurrentApp.Resources["imageEntryIcon"];
            });
        }

        private void OnLanguageSelected()
        {
            _model.OnLanguageSelected(languageOption.DataSourceSelected);
        }

        private void BindWeightVolume()
        {
            try
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    PickerWeightVolume.Title = TextResources.SelectWeightVolumeType;
                    PickerWeightVolume.ItemsSource = _model.WeightVolumeData;
                    PickerWeightVolume.ItemDisplayBinding = new Binding("DisplayVolume");
                    _model.WeightVolumeClick_Action = OnWeightVolumeClicked;
                    PickerWeightVolume.SelectedIndexChanged += PickerWeightVolume_SelectedIndexChanged;
                });
            }
            catch (Exception ex)
            {
                _ = ex;
            }
        }

        private void OnWeightVolumeClicked()
        {
            if (_model.WeightVolumeData.Count > 1)
            {
                PickerWeightVolume.Focus();
            }
        }

        private void PickerWeightVolume_SelectedIndexChanged(object sender, EventArgs e)
        {
            _model.OnWeightVolumeChange((WeightVolume)PickerWeightVolume.SelectedItem);
        }

        private void LanguageSelect_OnTapped(object sender, EventArgs e)
        {
            if (IsEventAllowed)
                languageOption.LanguageChange_Click();
        }

        private void WeightVolumeSelect_OnTapped(object sender, EventArgs e)
        {
            if (IsEventAllowed)
                _model.WeightVolumeClick_Action();
        }
    }

    public abstract class UserSettingPageXaml : ModelBoundContentPage<UserSettingsViewModel>
    {
    }
}