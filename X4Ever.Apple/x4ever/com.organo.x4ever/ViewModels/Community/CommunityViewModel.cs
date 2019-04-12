using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using com.organo.x4ever.Globals;
using com.organo.x4ever.Localization;
using com.organo.x4ever.Models;
using com.organo.x4ever.Pages;
using com.organo.x4ever.Services;
using com.organo.x4ever.Statics;
using com.organo.x4ever.ViewModels.Base;
using Xamarin.Forms;

namespace com.organo.x4ever.ViewModels.Community
{
    public class CommunityViewModel : BaseViewModel
    {
        public CommunityViewModel(INavigation navigation = null) : base(navigation)
        {
            this.PageBackgroundImage = DependencyService.Get<IHelper>()
                .GetFileUri(TextResources.image_community_background, FileType.Image);
            LoadContent();
        }

        private async void LoadContent() =>
            ApplicationSetting = await DependencyService.Get<IApplicationSettingService>().GetAsync();

        private ImageSource _pageBackgroundImage;
        public const string PageBackgroundImagePropertyName = "PageBackgroundImage";

        public ImageSource PageBackgroundImage
        {
            get { return _pageBackgroundImage; }
            set { SetProperty(ref _pageBackgroundImage, value, PageBackgroundImagePropertyName); }
        }
        
        private ApplicationSetting _applicationSetting;
        public const string ApplicationSettingPropertyName = "ApplicationSetting";

        public ApplicationSetting ApplicationSetting
        {
            get { return _applicationSetting; }
            set { SetProperty(ref _applicationSetting, value, ApplicationSettingPropertyName); }
        }

        private string FacebookUrl => ApplicationSetting?.CommunityFacebookUrl;
        private string InstagramUrl => ApplicationSetting?.CommunityInstagramUrl;

        
        private ICommand _facebookCommand;

        public ICommand FacebookCommand
        {
            get
            {
                return _facebookCommand ?? (_facebookCommand = new Command((obj) =>
                {
                    Device.OpenUri(new Uri(FacebookUrl));
                }));
            }
        }

        private ICommand _instagramCommand;

        public ICommand InstagramCommand
        {
            get
            {
                return _instagramCommand ?? (_instagramCommand = new Command((obj) =>
                {
                    Device.OpenUri(new Uri(InstagramUrl));
                }));
            }
        }
    }
}