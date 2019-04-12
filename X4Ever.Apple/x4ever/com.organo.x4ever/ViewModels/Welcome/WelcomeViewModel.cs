using com.organo.x4ever.Globals;
using com.organo.x4ever.Localization;
using com.organo.x4ever.Services;
using com.organo.x4ever.Statics;
using com.organo.x4ever.ViewModels.Base;
using Plugin.MediaManager;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Enums;
using Plugin.MediaManager.Abstractions.Implementations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace com.organo.x4ever.ViewModels.Welcome
{
    public class WelcomeViewModel : BaseViewModel
    {
        private IPlaybackController PlaybackController => CrossMediaManager.Current.PlaybackController;

        public WelcomeViewModel(INavigation navigation = null) : base(navigation)
        {
            Opacity = 0;
            VideoSource = "";
            SkipText = string.Empty;
            VideoTransparentColor = Palette._VideoTransparent;
            IsSkipVisible = false;
            MediaFiles = new List<MediaFile>();
        }

        public async Task OnLoad()
        {
            await Task.Run(() => { SetActivityResource(false, true); });
            await App.Configuration.InitAsync();

            SetActivityResource();
            VideoSource = DependencyService.Get<IHelper>()
                .GetFilePath(App.Configuration.AppConfig.WelcomeVideoUrl, FileType.Video);
            IsSkipVisible = await Page_Load();
            int.TryParse("0", out var vol);
            CrossMediaManager.Current.VolumeManager.CurrentVolume = vol;
            CrossMediaManager.Current.VolumeManager.Mute = true;
            await ShowButtonAsync();
            if (!string.IsNullOrEmpty(await App.Configuration.GetUserToken()))
                await App.LogoutAsync();
        }

        async Task<bool> Page_Load()
        {
            MediaFiles.Add(new MediaFile()
            {
                Url = VideoSource,
                Type = MediaFileType.Video,
                MetadataExtracted = false,
                Availability = ResourceAvailability.Remote,
            });
            CrossMediaManager.Current.MediaQueue.Repeat = RepeatType.RepeatOne;
            await CrossMediaManager.Current.Play(MediaFiles);
            SkipText = TextResources.Skip;
            await Task.Delay(4000);

            return true;
        }

        public async void StopPlayer()
        {
            try
            {
                await PlaybackController.Stop();
            }
            catch (Exception)
            {
                // Commented
            }
        }

        private async Task ShowButtonAsync()
        {
            if (IsSkipVisible)
            {
                await Task.Run(async () =>
                {
                    Opacity = .1;
                    while (Opacity < 1)
                    {
                        await Task.Delay(100);
                        await UpdateOpacity();
                    }
                });
            }
        }

        private async Task UpdateOpacity()
        {
            await Task.Run(() => { Opacity = Opacity + .1; });
        }

        public List<MediaFile> MediaFiles { get; set; }

        private string _videSource;
        const string VideoSourcePropertyName = "VideoSource";

        public string VideoSource
        {
            get { return _videSource; }
            set { SetProperty(ref _videSource, value, VideoSourcePropertyName); }
        }

        private string _skipText;
        public const string SkipTextPropertyName = "SkipText";

        public string SkipText
        {
            get { return _skipText; }
            set { SetProperty(ref _skipText, value, SkipTextPropertyName); }
        }

        private bool _isSkipVisible;
        public const string IsSkipVisiblePropertyName = "IsSkipVisible";

        public bool IsSkipVisible
        {
            get { return _isSkipVisible; }
            set { SetProperty(ref _isSkipVisible, value, IsSkipVisiblePropertyName); }
        }

        private Color _videoTransparentColor;
        public const string VideoTransparentColorPropertyName = "VideoTransparentColor";

        public Color VideoTransparentColor
        {
            get { return _videoTransparentColor; }
            set { SetProperty(ref _videoTransparentColor, value, VideoTransparentColorPropertyName); }
        }

        private ICommand _skipCommand;

        public ICommand SkipCommand
        {
            get
            {
                return _skipCommand ?? (_skipCommand = new Command(
                           (obj) => { App.GoToAccountPage(); }));
            }
        }

        private double _opacity;
        public const string OpacityPropertyName = "Opacity";

        public double Opacity
        {
            get { return _opacity; }
            set { SetProperty(ref _opacity, value, OpacityPropertyName); }
        }
    }
}