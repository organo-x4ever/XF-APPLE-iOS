﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.organo.x4ever.Globals;
using com.organo.x4ever.Handler;
using com.organo.x4ever.Helpers;
using com.organo.x4ever.Localization;
using com.organo.x4ever.Models.Authentication;
using com.organo.x4ever.Pages;
using com.organo.x4ever.Services;
using com.organo.x4ever.Statics;
using com.organo.x4ever.ViewModels.Base;
using Xamarin.Forms;

namespace com.organo.x4ever.ViewModels.Menu
{
    public class MenuGridViewModel : BaseViewModel
    {
        private readonly IHelper _helper;

        public MenuGridViewModel(INavigation navigation = null) : base(navigation)
        {
            _helper = DependencyService.Get<IHelper>();
            User = App.CurrentUser.UserInfo;
        }

        public async Task GetProfilePhoto()
        {
            try
            {
                if (string.IsNullOrEmpty(App.CurrentUser.UserInfo.ProfileImage))
                {
                    var response = await DependencyService.Get<IMetaPivotService>().GetMetaAsync();
                    App.CurrentUser.UserInfo.ProfileImage = response?.ProfilePhoto;
                    User = App.CurrentUser.UserInfo;
                }

                ProfileImagePath = User.ProfileImage;
            }
            catch (Exception ex)
            {
                new ExceptionHandler(typeof(MenuPageViewModel).FullName, ex);
            }
        }

        public async Task BindMenuData()
        {
            float height = 30, width = 30;
            var iconSize = App.Configuration.GetImageSizeByID(ImageIdentity.MENU_ITEM_ICON);
            if (iconSize != null)
            {
                height = iconSize.Height;
                width = iconSize.Width;
            }

            var menuItems = await DependencyService.Get<IMenuServices>().GetByApplicationAsync();
            MenuItems = (from m in menuItems
                select new HomeMenuItem
                {
                    MenuTitle = _helper.GetResource(m.MenuTitle),
                    MenuType = (MenuType) Enum.Parse(typeof(MenuType), m.MenuType),
                    MenuIcon = m.MenuIcon != null ? _helper.GetResource(m.MenuIcon) : "",
                    IconStyle = IconStyle,
                    IconSource = m.MenuIcon != null
                        ? ImageResizer.ResizeImage(_helper.GetResource(m.MenuIcon), iconSize)
                        : null,
                    IconHeight = height,
                    IconWidth = width,
                    IsIconVisible = m.MenuIconVisible,
                    TextStyle = (MenuType) Enum.Parse(typeof(MenuType), m.MenuType) == MenuType.MyProfile
                        ? SelectedStyle
                        : DefaultStyle,
                    IsSelected = (MenuType) Enum.Parse(typeof(MenuType), m.MenuType) == MenuType.MyProfile,
                    ItemPadding = new Thickness(15, 5, 0, 5)
                }).ToList();
        }

        public Style DefaultStyle => (Style) App.CurrentApp.Resources["labelStyleMenuItem"];
        public Style SelectedStyle => (Style) App.CurrentApp.Resources["labelStyleMenuItemHighlight"];
        public Style IconStyle => (Style) App.CurrentApp.Resources["imageIconMenuItem"];
        private UserInfo _user;
        public const string UserPropertyName = "User";

        public UserInfo User
        {
            get { return _user; }
            set { SetProperty(ref _user, value, UserPropertyName); }
        }

        private List<HomeMenuItem> _menuItems;
        public const string MenuItemsPropertyName = "MenuItems";

        public List<HomeMenuItem> MenuItems
        {
            get { return _menuItems; }
            set { SetProperty(ref _menuItems, value, MenuItemsPropertyName); }
        }

        private string DefaultImage => TextResources.ImageNotAvailable;

        private string profileImagePath;
        public const string ProfileImagePathPropertyName = "ProfileImagePath";

        public string ProfileImagePath
        {
            private get => profileImagePath;
            set
            {
                var photo = string.IsNullOrEmpty(value) ? null : value;
                SetProperty(ref profileImagePath, photo ?? TextResources.ImageNotAvailable,
                    ProfileImagePathPropertyName, ChangeProfileImagePath);
            }
        }

        private void ChangeProfileImagePath()
        {
            var imageSize = App.Configuration.GetImageSizeByID(ImageIdentity.MENU_PAGE_USER_IMAGE);
            if (imageSize != null)
            {
                ProfileImageHeight = imageSize.Height;
                ProfileImageWidth = imageSize.Width;
            }

            ProfileImageSource = ProfileImagePath.Contains(DefaultImage)
                ? ImageResizer.ResizeImage(ProfileImagePath, imageSize)
                : DependencyService.Get<IHelper>().GetFileUri(ProfileImagePath,
                    ProfileImagePath.Contains("http") ? FileType.None : FileType.User);
        }

        private ImageSource profileImageSource;
        public const string ProfileImageSourcePropertyName = "ProfileImageSource";

        public ImageSource ProfileImageSource
        {
            get { return profileImageSource; }
            set { SetProperty(ref profileImageSource, value, ProfileImageSourcePropertyName); }
        }

        private float profileImageHeight;
        public const string ProfileImageHeightPropertyName = "ProfileImageHeight";

        public float ProfileImageHeight
        {
            get { return profileImageHeight; }
            set { SetProperty(ref profileImageHeight, value, ProfileImageHeightPropertyName); }
        }

        private float profileImageWidth;
        public const string ProfileImageWidthPropertyName = "ProfileImageWidth";

        public float ProfileImageWidth
        {
            get { return profileImageWidth; }
            set { SetProperty(ref profileImageWidth, value, ProfileImageWidthPropertyName); }
        }

        public string ApplicationVersion =>
            string.Format(TextResources.AppVersion, App.Configuration.AppConfig.ApplicationVersion);
    }
}