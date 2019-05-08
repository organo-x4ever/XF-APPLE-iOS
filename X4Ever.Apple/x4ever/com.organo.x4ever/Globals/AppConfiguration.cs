﻿
using com.organo.x4ever.Helpers;
using com.organo.x4ever.Localization;
using com.organo.x4ever.Models;
using com.organo.x4ever.Models.User;
using com.organo.x4ever.Notification;
using com.organo.x4ever.Services;
using com.organo.x4ever.Statics;
using Connectivity.Plugin;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.organo.x4ever.ViewModels.Profile;
using Xamarin.Forms;

namespace com.organo.x4ever.Globals
{
    public class AppConfiguration : IAppConfiguration
    {
        /// <summary>
        /// Object Instances
        /// </summary>
        private readonly IConfigFetcher _configFetcher;
        private readonly ISecureStorage _secureStorage;
        private readonly ILocalize _localize;
        private readonly ISetDeviceProperty _deviceProperty;
        private readonly IHelper _helper;

        /// <summary>
        /// Properties
        /// </summary>
        public string BackgroundImage { get; set; }
        public Color BackgroundColor { get; set; }
        public Color StatusBarColor { get; set; }
        public bool IsFullScreenMode { get; set; }
        public ActivityType ActivityType { get; set; }
        public CultureInfo LanguageInfo { get; set; }
        public bool IsConnected { get; set; }
        public bool IsAnimationAllowed { get; set; }
        public UserSetting UserSetting { get; set; }
        public AppConfig AppConfig { get; set; }
        public bool IsMenuLoaded { get; set; }
        public List<ImageSize> ImageSizes { get; set; }
        private string _userToken { get; set; }
        public string UserToken
        {
            get
            {
                if (string.IsNullOrEmpty(_userToken)) _userToken = GetUserToken();
                return _userToken;
            }
        }

        public bool IsProfileEditAllowed { get; set; }
        
        public AppConfiguration()
        {
            AppConfig = new AppConfig();
            ImageSizes = new List<ImageSize>();
            _helper = new Helper();
            _configFetcher = DependencyService.Get<IConfigFetcher>();
            _secureStorage = DependencyService.Get<ISecureStorage>();
            _localize = DependencyService.Get<ILocalize>();
            _deviceProperty = DependencyService.Get<ISetDeviceProperty>();
            BackgroundColor = Palette._MainBackground;
            StatusBarColor = Palette._MainAccent;
            BackgroundImage = TextResources.icon_background_blur;
            IsFullScreenMode = true;
            ActivityType = ActivityType.NONE;
            IsConnected = false;
            IsAnimationAllowed = true;
            InitialTasks();
        }

        private async void InitialTasks()
        {
            AppConfig = await _configFetcher.GetAsync();
            AppConfig.ApplicationVersion = DependencyService.Get<IAppVersionProvider>().Version;
            GetImageSizes(AppConfig);
            await GetConnectionInfoAsync();
            await InitAsync();
            SetBarColor();
        }

        public async Task InitAsync()
        {
            IsMenuLoaded = false;
            GetUserToken();
            await GetLanguageAsync();
            await GetWeightVolumeAsync();
        }

        public void Initial(Page page, bool showBackgroundImage = false)
        {
            SetBarColor();
            page.BackgroundColor = BackgroundColor;
            if (showBackgroundImage)
                page.BackgroundImage = BackgroundImage;
        }

        public async Task InitialAsync(Page page, bool showBackgroundImage = false)
        {
            await Task.Run(() =>
            {
                SetBarColor();
                page.BackgroundColor = BackgroundColor;
                if (showBackgroundImage)
                    page.BackgroundImage = BackgroundImage;
            });
        }

        public void Initial(Page page, Color backgroundColor, bool showBackgroundImage = false)
        {
            SetBarColor();
            page.BackgroundColor = backgroundColor;
            if (showBackgroundImage)
                page.BackgroundImage = BackgroundImage;
        }

        public async Task InitialAsync(Page page, Color backgroundColor,
            bool showBackgroundImage = false)
        {
            await Task.Run(() =>
            {
                SetBarColor();
                page.BackgroundColor = backgroundColor;
                if (showBackgroundImage)
                    page.BackgroundImage = BackgroundImage;
            });
        }

        private void SetBarColor() => DependencyService.Get<ISetDeviceProperty>()
            .SetStatusBarColor(StatusBarColor, IsFullScreenMode);
        
        public async Task GetActivityAsync(string action = "")
        {
            await Task.Run(() =>
            {
                if (action != null && action.Trim().Length > 0)
                    if (action == ActivityType.WEIGHT_SUBMISSION_REQUIRED.ToString())
                        ActivityType = ActivityType.WEIGHT_SUBMISSION_REQUIRED;
            });
        }
        
        public async Task SetUserConfigurationAsync(string token, string languageCode, string weightVolume)
        {
            await SetUserTokenAsync(token);
            if (!string.IsNullOrEmpty(languageCode))
                await SetUserLanguageAsync(languageCode);
            if (!string.IsNullOrEmpty(weightVolume))
                await SetWeightVolumeAsync(weightVolume);
        }

        public async Task SetUserLanguageAsync(string languageCode)
        {
            if (!string.IsNullOrEmpty(languageCode))
                _secureStorage.StoreByte(StorageConstants.KEY_USER_LANGUAGE, Encoding.UTF8.GetBytes(languageCode));
            await GetLanguageAsync();
        }

        private async Task GetLanguageAsync()
        {
            await Task.Run(() =>
            {
                var data = _secureStorage.Retrieve<byte[]>(StorageConstants.KEY_USER_LANGUAGE);
                var language = data != null ? Encoding.UTF8.GetString(data, 0, data.Length) : "";
                if (!string.IsNullOrEmpty(language))
                    AppConfig.DefaultLanguage = language;
                else
                {
                    if (!string.IsNullOrEmpty(AppConfig.DefaultLanguage))
                        AppConfig.DefaultLanguage = _localize.GetLanguage(AppConfig?.DefaultLanguage);
                    else
                        AppConfig.DefaultLanguage = _localize.GetLanguage();
                }

                LanguageInfo = _localize.GetCurrentCultureInfo(AppConfig.DefaultLanguage);
                TextResources.Culture = LanguageInfo;
            });
        }

        public async Task SetWeightVolumeAsync(string weightVolume)
        {
            if (weightVolume != null)
                _secureStorage.StoreByte(StorageConstants.KEY_USER_WEIGHT_VOLUME, Encoding.UTF8.GetBytes(weightVolume));
            await GetWeightVolumeAsync();
        }

        private async Task GetWeightVolumeAsync()
        {
            var weightVolume = "";
            await Task.Run(() =>
            {
                var data = _secureStorage.Retrieve<byte[]>(StorageConstants.KEY_USER_WEIGHT_VOLUME);
                if (data != null) weightVolume = Encoding.UTF8.GetString(data, 0, data.Length);
                if (!string.IsNullOrEmpty(weightVolume))
                    AppConfig.DefaultWeightVolume = weightVolume;
            });
        }

        public async Task GetConnectionInfoAsync() => await Task.Run(() => { GetConnectionInfo(); });

        public void GetConnectionInfo()
        {
            IsConnected = CrossConnectivity.Current.IsConnected;
        }
        
        public async Task SetUserTokenAsync(string token) => await Task.Run(async () =>
        {
            await DeleteUserTokenAsync();
            _secureStorage.StoreByte(StorageConstants.KEY_VAULT_TOKEN_ID, Encoding.UTF8.GetBytes(token));
        });

        public string GetUserToken()
        {
            var data = _secureStorage.Retrieve<byte[]>(StorageConstants.KEY_VAULT_TOKEN_ID);
            return data != null ? Encoding.UTF8.GetString(data, 0, data.Length) : "";
        }

        public async Task<string> GetUserTokenAsync() => await Task.Factory.StartNew(() => { return UserToken; });

        public async Task<bool> IsUserTokenExistsAsync() => !string.IsNullOrEmpty(await GetUserTokenAsync());

        public async Task DeleteUserTokenAsync() => await Task.Run(() =>
        {
            _secureStorage.Delete(StorageConstants.KEY_VAULT_TOKEN_ID);
            _userToken = string.Empty;
        });
        
        private async void GetImageSizes(AppConfig appConfig)
        {
            await Task.Factory.StartNew(() =>
            {
                SetImageSizes(ImageIdentity.TOP_BAR_LOGO, appConfig.TOP_BAR_LOGO, TextResources.logo_transparent);
                SetImageSizes(ImageIdentity.TOP_BAR_MENU, appConfig.TOP_BAR_MENU, TextResources.icon_menu);
                SetImageSizes(ImageIdentity.TOP_BAR_SETTING, appConfig.TOP_BAR_SETTING, ImageConstants.ICON_PROFILE_SETTINGS);
                SetImageSizes(ImageIdentity.TOP_BAR_CLOSE, appConfig.TOP_BAR_CLOSE, TextResources.icon_close);
                SetImageSizes(ImageIdentity.MAIN_PAGE_LOGO, appConfig.MAIN_PAGE_LOGO, TextResources.logo_page);
                SetImageSizes(ImageIdentity.MAIN_PAGE_XCHALLENGE_LOGO, appConfig.MAIN_PAGE_XCHALLENGE_LOGO, TextResources.logo_challenge);
                SetImageSizes(ImageIdentity.MENU_PAGE_USER_IMAGE, appConfig.MENU_PAGE_USER_IMAGE, null, true);
                SetImageSizes(ImageIdentity.USER_PROFILE_BADGE_ICON, appConfig.USER_PROFILE_BADGE_ICON, null, true);
                SetImageSizes(ImageIdentity.MILESTONE_ACHEIVEMENT_BADGE_ICON, appConfig.MILESTONE_ACHEIVEMENT_BADGE_ICON, null, true);
                SetImageSizes(ImageIdentity.BADGE_HINT_WINDOW, appConfig.BADGE_HINT_WINDOW);
                SetImageSizes(ImageIdentity.BADGE_HINT_WINDOW_CLOSE, appConfig.BADGE_HINT_WINDOW_CLOSE);
                SetImageSizes(ImageIdentity.BADGE_HINT_ICON, appConfig.BADGE_HINT_ICON);
                SetImageSizes(ImageIdentity.ENTRY_EMAIL_ICON, appConfig.ENTRY_EMAIL_ICON, TextResources.icon_email);
                SetImageSizes(ImageIdentity.ENTRY_LOCK_ICON, appConfig.ENTRY_LOCK_ICON, TextResources.icon_lock);
                SetImageSizes(ImageIdentity.COUNTRY_FLAG_ICON, appConfig.COUNTRY_FLAG_ICON);
                SetImageSizes(ImageIdentity.MENU_ITEM_ICON, appConfig.MENU_ITEM_ICON);
                SetImageSizes(ImageIdentity.GENDER_IMAGE, appConfig.GENDER_IMAGE);
                SetImageSizes(ImageIdentity.UPLOAD_CAMERA_IMAGE, appConfig.UPLOAD_CAMERA_IMAGE);
                SetImageSizes(ImageIdentity.MEAL_PLAN_PAGE_MEAL_IMAGE, appConfig.MEAL_PLAN_PAGE_MEAL_IMAGE);
                SetImageSizes(ImageIdentity.MEAL_PLAN_PAGE_MEAL_HEADER, appConfig.MEAL_PLAN_PAGE_MEAL_HEADER);
                SetImageSizes(ImageIdentity.AUDIO_PLAYER_PAGE_COMMAND_IMAGE, appConfig.AUDIO_PLAYER_PAGE_COMMAND_IMAGE);
                SetImageSizes(ImageIdentity.VIDEO_PLAYER_PAGE_COMMAND_IMAGE, appConfig.VIDEO_PLAYER_PAGE_COMMAND_IMAGE);
                SetImageSizes(ImageIdentity.VIDEO_PLAYER_PAGE_EXPAND_LIST_IMAGE, appConfig.VIDEO_PLAYER_PAGE_EXPAND_LIST_IMAGE);
                SetImageSizes(ImageIdentity.VIDEO_PLAYER_PAGE_NOTE_PLAY_IMAGE, appConfig.VIDEO_PLAYER_PAGE_NOTE_PLAY_IMAGE);
                SetImageSizes(ImageIdentity.PICTURE_GALLERY_IMAGE, appConfig.PICTURE_GALLERY_IMAGE);
                SetImageSizes(ImageIdentity.USER_SETTING_TAB_ICON, appConfig.USER_SETTING_TAB_ICON);
                SetImageSizes(ImageIdentity.CHECKBOX_ICON, appConfig.CHECKBOX_ICON);
                SetImageSizes(ImageIdentity.TESTIMONIAL_PERSON_IMAGE, appConfig.TESTIMONIAL_PERSON_IMAGE);
                SetImageSizes(ImageIdentity.WORKOUT_OPTIONS_IMAGE, appConfig.WORKOUT_OPTIONS_IMAGE);
                SetImageSizes(ImageIdentity.WORKOUT_EXPAND_COLLAPSE_IMAGE, appConfig.WORKOUT_EXPAND_COLLAPSE_IMAGE);
                SetImageSizes(ImageIdentity.WORKOUT_PLAY_ICON, appConfig.WORKOUT_PLAY_ICON);
                SetImageSizes(ImageIdentity.WORKOUT_VIDEO_WINDOW, appConfig.WORKOUT_VIDEO_WINDOW);
                SetImageSizes(ImageIdentity.PAGE_IMAGE_SIGN_UP, appConfig.PAGE_IMAGE_SIGN_UP);
                SetImageSizes(ImageIdentity.PAGE_IMAGE_PRODUCTS, appConfig.PAGE_IMAGE_PRODUCTS);
                SetImageSizes(ImageIdentity.PAGE_IMAGE_EARN_REWARDS, appConfig.PAGE_IMAGE_EARN_REWARDS);
                SetImageSizes(ImageIdentity.PAGE_IMAGE_LOSE_WEIGHT, appConfig.PAGE_IMAGE_LOSE_WEIGHT);
                SetImageSizes(ImageIdentity.PAGE_IMAGE_T_SHIRT, appConfig.PAGE_IMAGE_T_SHIRT);
                SetImageSizes(ImageIdentity.PAGE_IMAGE_T_SHIRTS_BUNDLE, appConfig.PAGE_IMAGE_T_SHIRTS_BUNDLE);
                SetImageSizes(ImageIdentity.PAGE_IMAGE_LINE, appConfig.PAGE_IMAGE_LINE);
                SetImageSizes(ImageIdentity.PAGE_IMAGE_BULLET, appConfig.PAGE_IMAGE_BULLET);
                SetImageSizes(ImageIdentity.IMAGE_EYE_PASSWORD, appConfig.IMAGE_EYE_PASSWORD);
            });
        }

        private void SetImageSizes(string id, string dimensions, string path = null, bool isDynamic = false)
        {
            if (dimensions != null && dimensions.Contains("*"))
            {
                var splits = dimensions.Split('*');
                if (splits != null && splits.Length > 1)
                {
                    int.TryParse(splits[0], out var width);
                    int.TryParse(splits[1], out var height);
                    var imageSize = new ImageSize()
                    {
                        ImageID = id,
                        ImageName = path,
                        Height = height,
                        Width = width,
                        IsDynamic = isDynamic
                    };
                    ImageSizes.Add(imageSize);
                    if (width == 0 || width == 99 || height == 0 || height == 99)
                        ShowImageMessage(id);
                }
                else
                    ShowImageMessage(id);
            }
            else
                ShowImageMessage(id);
        }

        private async void ShowImageMessage(string id)
        {
            await DependencyService.Get<IMessage>().AlertAsync(TextResources.Alert,
                "Image ID '" + id + "' has an error", TextResources.Ok, TextResources.Cancel);
        }
        
        private IntervalPeriodType GetIntervalPeriodType(string intervalType)
        {
            if (intervalType.ToLower().Contains("year"))
                return IntervalPeriodType.Years;
            else if (intervalType.ToLower().Contains("month"))
                return IntervalPeriodType.Months;
            else if (intervalType.ToLower().Contains("day"))
                return IntervalPeriodType.Days;
            else if (intervalType.ToLower().Contains("hour"))
                return IntervalPeriodType.Hours;
            else if (intervalType.ToLower().Contains("minute"))
                return IntervalPeriodType.Minutes;
            return IntervalPeriodType.Days;
        }

        public async Task<ImageSize> GetImageSizeByIDAsync(string imageIdentity)
        {
            return await Task.Factory.StartNew(() => GetImageSizeByID(imageIdentity));
        }

        public ImageSize GetImageSizeByID(string imageIdentity)
        {
            return App.Configuration.ImageSizes.FirstOrDefault(s => s.ImageID.ToLower() == imageIdentity.ToLower());
        }

        public async Task SetImageAsync(string imageIdentity, string badgeImage)
        {
            await Task.Run(async () =>
            {
                var imageSize = await GetImageSizeByIDAsync(imageIdentity);
                if (imageSize != null)
                    imageSize.ImageName = badgeImage;
            });
        }

        public void SetImage(string imageIdentity, string badgeImage)
        {
            var imageSize = GetImageSizeByID(imageIdentity);
            if (imageSize != null)
                imageSize.ImageName = badgeImage;
        }

        public string GetApplication()
        {
            return App.CurrentUser?.UserInfo?.UserApplication ?? "";
        }

        public bool IsWelcomeVideoSkipped()
        {
            var data = _secureStorage.Retrieve<byte[]>(StorageConstants.KEY_VAULT_VIDEO_SKIPPED);

            if (data != null)
                return Encoding.UTF8.GetString(data, 0, data.Length)?.Trim().ToLower() ==
                       StorageConstants.KEY_VAULT_SKIP.Trim().ToLower();
            return false;
        }

        public void WelcomeVideoSkipped()
        {
            _secureStorage.StoreByte(StorageConstants.KEY_VAULT_VIDEO_SKIPPED,
                Encoding.UTF8.GetBytes(StorageConstants.KEY_VAULT_SKIP));
        }

        public void DeleteVideoSkipped()
        {
            _secureStorage.Delete(StorageConstants.KEY_VAULT_VIDEO_SKIPPED);
        }

        public bool IsVersionPrompt()
        {
            var data = _secureStorage.Retrieve<byte[]>(StorageConstants.KEY_VAULT_VERSION_PROMPT);
            if (data != null)
            {
                if (Encoding.UTF8.GetString(data, 0, data.Length)?.Trim().ToLower() ==
                    StorageConstants.KEY_VAULT_VERSION.Trim().ToLower())
                {
                    var versionCheckDate =
                        _secureStorage.Retrieve<byte[]>(StorageConstants.KEY_VAULT_VERSION_PROMPT_DATE);
                    if (versionCheckDate != null)
                    {
                        var versionDate = Encoding.UTF8.GetString(versionCheckDate, 0, versionCheckDate.Length)?.Trim();
                        if (DateTime.TryParse(versionDate, out DateTime date))
                            return DateTime.Today >= date;
                    }
                }
            }

            return false;
        }

        public void VersionPrompted()
        {
            _secureStorage.StoreByte(StorageConstants.KEY_VAULT_VERSION_PROMPT,
                Encoding.UTF8.GetBytes(StorageConstants.KEY_VAULT_VERSION));

            _secureStorage.StoreByte(StorageConstants.KEY_VAULT_VERSION_PROMPT_DATE,
                Encoding.UTF8.GetBytes(DateTime.Today.ToString()));
        }

        public void DeleteVersionPrompt()
        {
            _secureStorage.Delete(StorageConstants.KEY_VAULT_VERSION_PROMPT_DATE);
            _secureStorage.Delete(StorageConstants.KEY_VAULT_VERSION_PROMPT);
        }
        
        public void SetUserKey() =>
            _secureStorage.StoreByte(StorageConstants.KEY_USER, Encoding.UTF8.GetBytes(_helper.GetUniqueCode()));

        public string GetUserKey()
        {
            if (App.CurrentUser?.UserInfo != null && !string.IsNullOrEmpty(App.CurrentUser?.UserInfo?.UserKey))
                return App.CurrentUser?.UserInfo?.UserKey;

            var data = _secureStorage.Retrieve<byte[]>(StorageConstants.KEY_USER);
            if (data != null)
                return Encoding.UTF8.GetString(data, 0, data.Length);
            return string.Empty;
        }

        public bool IsUserKeyExists() => !string.IsNullOrEmpty(GetUserKey());

        public void DeleteUserKey() => _secureStorage.Delete(StorageConstants.KEY_USER);

        public void SetUserGraph(ChartType chartType) => _secureStorage.StoreByte(StorageConstants.KEY_USER_GRAPH, Encoding.UTF8.GetBytes(chartType.ToString()));

        public ChartType GetUserGraph()
        {
            var data = _secureStorage.Retrieve<byte[]>(StorageConstants.KEY_USER_GRAPH);
            if (data != null)
                return (ChartType) Enum.Parse(typeof(ChartType), Encoding.UTF8.GetString(data, 0, data.Length));

            return ChartType.LineChart;
        }

        public void DeleteUserGraph() => _secureStorage.Delete(StorageConstants.KEY_USER_GRAPH);
    }
}