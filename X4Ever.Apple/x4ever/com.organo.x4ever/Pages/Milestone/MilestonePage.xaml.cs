using com.organo.xchallenge.Pages.Base;
using com.organo.xchallenge.ViewModels.Milestone;
using com.organo.xchallenge.ViewModels.Profile;
using System;
using System.Linq;
using com.organo.xchallenge.Globals;
using com.organo.xchallenge.Localization;
using com.organo.xchallenge.Statics;
using Xamarin.Forms;

namespace com.organo.xchallenge.Pages.Milestone
{
    public partial class MilestonePage : MilestonePageXaml
    {
        private readonly MilestoneViewModel _model;

        public MilestonePage(RootPage root, MyProfileViewModel profileViewModel)
        {
            try
            {
                InitializeComponent();

                _model = new MilestoneViewModel(App.CurrentApp.MainPage.Navigation)
                {
                    Root = root,
                    ProfileViewModel = profileViewModel
                };
                this.Init();
                //Device.BeginInvokeOnMainThread(async () =>
                //{
                //    // Works
                //    await DisplayAlert("Testing!", "Some text", "OK");

                //    // Does not work
                //    await DisplayActionSheet("Test", "Cancel", "Destroy", new[] {"1", "2"});
                //});
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }

        public sealed override async void Init(object obj = null)
        {
            await App.Configuration.InitialAsync(this);
            NavigationPage.SetHasNavigationBar(this, false);
            BindingContext = _model;
            var imageSize =
                App.Configuration.ImageSizes.FirstOrDefault(s =>
                    s.ImageID == ImageIdentity.MILESTONE_ACHEIVEMENT_BADGE_ICON);
            if (imageSize != null)
            {
                ImageBadgeAchieved.MinimumHeightRequest = ImageBadgeAchieved.HeightRequest = imageSize.Height;
                ImageBadgeAchieved.MinimumWidthRequest = imageSize.Width;
            }
        }
    }

    public abstract class MilestonePageXaml : ModelBoundContentPage<MilestoneViewModel>
    {
    }
}