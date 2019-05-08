using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.organo.x4ever.Pages.Base;
using com.organo.x4ever.ViewModels.Profile;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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
            await Task.Factory.StartNew(async () =>
            {
                gridTracker.ProfileModel = _model;
                gridTracker.Source = _model.UserTrackers;
                gridTracker.CloseAction = async () =>
                {
                    _model.ShowTrackerDetail = false;
                    await _model.PopAsync();
                    await gridTracker.ProfileModel.GetUserAsync(
                        gridTracker.ProfileModel.UserDetail.IsTrackerRequiredAfterDelete);
                };
                await Task.Delay(TimeSpan.FromMilliseconds(1000));
                _model.ShowTrackerDetail = false;
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