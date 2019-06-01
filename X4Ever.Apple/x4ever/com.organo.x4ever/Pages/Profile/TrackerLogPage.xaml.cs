
using System;
using System.Threading.Tasks;
using com.organo.x4ever.Pages.Base;
using com.organo.x4ever.ViewModels.Profile;
using Xamarin.Forms;

namespace com.organo.x4ever.Pages.Profile
{
    public partial class TrackerLogPage : TrackerLogPageXaml, IDisposable
    {
        private ProfileEnhancedViewModel _model;

        public TrackerLogPage(ProfileEnhancedViewModel model)
        {
            try
            {
                InitializeComponent();
                _model = model;
                Init();
            }
            catch (Exception ex)
            {
                _ = ex;
            }
        }

        public sealed override async void Init(object obj = null)
        {
            BindingContext = _model;
            await App.Configuration.InitialAsync(this);
            NavigationPage.SetHasNavigationBar(this, true);
            SetGridTracker();
        }

        private async void SetGridTracker()
        {
            await Task.Factory.StartNew(() =>
            {
                gridTracker.ProfileModel = _model;
                gridTracker.Source = _model.UserTrackers;
                gridTracker.CloseAction = async () =>
                {
                    _model.ShowTrackerDetail = false;
                    await Navigation.PopAsync();
                    await gridTracker.ProfileModel.GetUserAsync(gridTracker.ProfileModel.UserDetail
                        .IsTrackerRequiredAfterDelete);
                };
            });
        }

        public void Dispose()
        {
            _model.ShowTrackerDetail = false;
            if (!isDispose)
            {
                isDispose = true;
                gridTracker.Dispose();
                GC.SuppressFinalize(this);
            }
        }

        private bool isDispose = false;
    }

    public abstract class TrackerLogPageXaml : ModelBoundContentPage<ProfileEnhancedViewModel>
    {
    }
}