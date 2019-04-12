
using com.organo.x4ever.Extensions;
using com.organo.x4ever.Localization;
using com.organo.x4ever.Pages.Base;
using com.organo.x4ever.Services;
using com.organo.x4ever.Statics;
using com.organo.x4ever.ViewModels.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using com.organo.x4ever.Globals;
using com.organo.x4ever.Helpers;
using Xamarin.Forms;
using System.Threading.Tasks;
using com.organo.x4ever.Handler;
using TextResources = com.organo.x4ever.Localization.TextResources;

namespace com.organo.x4ever.Pages
{
    public partial class MenuPage : MenuPageXaml
    {
        private IMedia _media;
        private IMetaPivotService _metaPivotService;
        private MenuPageViewModel _model;
        private IHelper _helper;

        public MenuPage(RootPage root)
        {
            try
            {
                InitializeComponent();
                Init(root);
            }
            catch (Exception ex)
            {
                new ExceptionHandler(TAG, ex);
            }
        }

        public sealed override async void Init(object obj)
        {
            _model = new MenuPageViewModel(Navigation)
            {
                Title = TextResources.x4ever,
                Subtitle = TextResources.x4ever,
                Icon = TextResources.icon_menu,
                Root = (RootPage) obj,
                MenuBindCallback = MenuBind
            };
            BindingContext = _model;
            _helper = DependencyService.Get<IHelper>();
            await _model.GetMenuData();
            await _model.GetProfilePhoto();
            _metaPivotService = DependencyService.Get<IMetaPivotService>();
            _media = DependencyService.Get<Globals.IMedia>();
        }

        private void MenuBind()
        {
            if (_model.MenuItems?.Count > 0)
            {
                ListViewMenu.SelectedItem = _model.MenuItems.FirstOrDefault(m => m.IsSelected);
                ListViewMenu.ItemSelected += async (sender, e) =>
                {
                    if (ListViewMenu.SelectedItem != null)
                    {
                        var menuItem = (HomeMenuItem) ListViewMenu.SelectedItem;
                        if (!menuItem.IsSelected)
                        {
                            await _model.Root.NavigateAsync(menuItem.MenuType);
                            foreach (var mi in _model.MenuItems)
                            {
                                mi.IsSelected = false;
                                mi.TextStyle = _model.DefaultStyle;
                            }

                            var menu = _model.MenuItems.Find(t => t.MenuType == menuItem.MenuType);
                            menu.IsSelected = true;
                            menu.TextStyle = _model.SelectedStyle;
                        }

                        ListViewMenu.SelectedItem = null;
                    }
                };
                App.Configuration.IsMenuLoaded = true;
            }
        }

        private async void ChangeProfilePhoto(object sender, EventArgs args)
        {
            var result = await DisplayActionSheet(TextResources.ChooseOption, TextResources.Cancel, null,
                new string[] {TextResources.PickFromGallery, TextResources.TakeFromCamera});

            if (result != null)
            {
                if (result == "Cancel")
                    return;
                if (result == TextResources.PickFromGallery)
                {
                    _media.Refresh();
                    var mediaFile = await _media.PickPhotoAsync();
                    if (mediaFile == null)
                    {
                        _model.SetActivityResource(showError: true, errorMessage: _media.Message);
                        return;
                    }

                    await Task.Run(() => { _model.SetActivityResource(false, true); });
                    var response = await _media.UploadPhotoAsync(mediaFile);
                    if (!response)
                    {
                        _model.SetActivityResource(true, false, showError: true, errorMessage: _media.Message);
                        return;
                    }
                }
                else if (result == TextResources.TakeFromCamera)
                {
                    _media.Refresh();
                    var mediaFile = await _media.TakePhotoAsync();
                    if (mediaFile == null)
                    {
                        _model.SetActivityResource(showError: true, errorMessage: _media.Message);
                        return;
                    }

                    await Task.Run(() => { _model.SetActivityResource(false, true); });
                    var response = await _media.UploadPhotoAsync(mediaFile);
                    if (!response)
                    {
                        _model.SetActivityResource(true, false, showError: true, errorMessage: _media.Message);
                        return;
                    }
                }
                _model.SetActivityResource();

                if (!string.IsNullOrEmpty(_media.FileName))
                {
                    var profileImage = _metaPivotService.AddMeta(_media.FileName,
                        MetaConstants.PROFILE_PHOTO.ToCapital(), MetaConstants.PROFILE_PHOTO,
                        MetaConstants.PROFILE_PHOTO);
                    var response = await _metaPivotService.SaveMetaAsync(profileImage);
                    if (response != null && response.Contains(HttpConstants.SUCCESS))
                    {
                        _model.User.ProfileImage = _media.FileName;
                        App.CurrentUser.UserInfo = _model.User;
                        _model.ProfileImagePath = _model.User.ProfileImage;
                        _model.SetActivityResource(showMessage: true,
                            message: TextResources.ChangeProfilePhoto + " " + TextResources.Change + " " +
                                     TextResources.Success);
                    }
                }
                else
                    _model.SetActivityResource(showError: true, errorMessage: _media.Message);
            }
        }
    }

    public abstract class MenuPageXaml : ModelBoundContentPage<MenuPageViewModel>
    {
    }
}