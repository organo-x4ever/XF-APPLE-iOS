using com.organo.x4ever.Pages.Base;
using com.organo.x4ever.ViewModels.Profile;
using System;
using com.organo.x4ever.Globals;
using com.organo.x4ever.Localization;
using Xamarin.Forms;

namespace com.organo.x4ever.Pages.Profile
{
    public partial class PictureGalleryPage : PictureGalleryPageXaml
    {
        private MyProfileViewModel _model;

        public PictureGalleryPage(MyProfileViewModel model)
        {
            try
            {
                InitializeComponent();
                _model = model;
                Init();
            }
            catch (Exception ex)
            {
                DependencyService.Get<IMessage>().AlertAsync(TextResources.Alert,
                    ex.InnerException != null ? ex.InnerException.Message : ex.Message, TextResources.Ok);
            }
        }

        public sealed override async void Init(object obj = null)
        {
            await App.Configuration.InitialAsync(this);
            NavigationPage.SetHasNavigationBar(this, false);

            _model.Navigation = App.CurrentApp.MainPage.Navigation;
            BindingContext = _model;
            ListViewGallery.ItemSelected += (sender, e) => ListViewGallery.SelectedItem = null;
        }
    }

    public abstract class PictureGalleryPageXaml : ModelBoundContentPage<MyProfileViewModel> { }
}