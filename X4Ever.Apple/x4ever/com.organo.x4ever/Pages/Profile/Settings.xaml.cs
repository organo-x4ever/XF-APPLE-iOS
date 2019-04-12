using com.organo.x4ever.ViewModels.Profile;
using System;
using com.organo.x4ever.Handler;
using Xamarin.Forms;

namespace com.organo.x4ever.Pages.Profile
{
    public partial class Settings : SettingsXaml
    {
        private SettingsViewModel _model;

        public Settings(RootPage root)
        {
            try
            {
                InitializeComponent();
                _model = new SettingsViewModel()
                {
                    Root = root
                };
                Init();
            }
            catch (Exception ex)
            {
                new ExceptionHandler("Settings.xaml.cs", ex);
            }
        }

        public sealed override void Init(object obj = null)
        {
            App.Configuration.Initial(this);
            NavigationPage.SetHasNavigationBar(this, false);

            BindingContext = _model;
        }

        private async void EditProfile_Tapped(object sender, EventArgs e)
        {
            if (_model.Selected != TabTitle.EditProfile)
                await _model.TabSelected(TabTitle.EditProfile);
        }

        private async void ChangePassword_Tapped(object sender, EventArgs e)
        {
            if (_model.Selected != TabTitle.ChangePassword)
                await _model.TabSelected(TabTitle.ChangePassword);
        }

        private async void UserSettings_Tapped(object sender, EventArgs e)
        {
            if (_model.Selected != TabTitle.UserSettings)
                await _model.TabSelected(TabTitle.UserSettings);
        }
    }

    public abstract class SettingsXaml : Base.ModelBoundContentPage<SettingsViewModel>
    {
    }
}