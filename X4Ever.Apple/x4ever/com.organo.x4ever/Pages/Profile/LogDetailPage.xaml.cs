using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using com.organo.x4ever.Models.User;
using com.organo.x4ever.Pages.Base;
using com.organo.x4ever.ViewModels.Profile;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace com.organo.x4ever.Pages.Profile
{
    public partial class LogDetailPage : LogDetailPageXaml
    {
        private MyProfileViewModel _model;

        public LogDetailPage(MyProfileViewModel model)
        {
            InitializeComponent();
            App.Configuration.InitialAsync(this);
            NavigationPage.SetHasNavigationBar(this, false);
            _model = model;
            Init();
        }

        public sealed override void Init(object obj = null)
        {
            _model.Navigation = App.CurrentApp.MainPage.Navigation;
            BindingContext = _model;
            ListViewTrackers.ItemSelected += (sender, e) =>
            {
                if (_model.UserDetail.IsDownloadAllowed)
                {
                    if (ListViewTrackers.SelectedItem != null)
                        DownloadImages((TrackerPivot) e.SelectedItem);
                }

                ListViewTrackers.SelectedItem = null;
            };
        }

        private async void DownloadImages(TrackerPivot tracker)
        {
            await Task.Factory.StartNew(() => { Device.OpenUri(new Uri(tracker.FrontImageWithUrl)); });
            await Task.Delay(TimeSpan.FromSeconds(2));
            await Task.Factory.StartNew(() => { Device.OpenUri(new Uri(tracker.SideImageWithUrl)); });
        }
    }

    public abstract class LogDetailPageXaml : ModelBoundContentPage<MyProfileViewModel>
    {
    }
}