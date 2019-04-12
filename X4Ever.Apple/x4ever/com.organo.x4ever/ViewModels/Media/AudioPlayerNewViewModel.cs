using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using com.organo.xchallenge.Globals;
using com.organo.xchallenge.Helpers;
using com.organo.xchallenge.Localization;
using com.organo.xchallenge.Pages;
using com.organo.xchallenge.Permissions;
using com.organo.xchallenge.Services;
using com.organo.xchallenge.Statics;
using com.organo.xchallenge.ViewModels.Base;
using Plugin.MediaManager;
using Plugin.MediaManager.Abstractions.Enums;
using Plugin.MediaManager.Abstractions.Implementations;
using Xamarin.Forms;
using System.Diagnostics;

namespace com.organo.xchallenge.ViewModels.Media
{
    public class AudioPlayerNewViewModel : BaseViewModel
    {
        private readonly ILocalFile _localFile;
        private IDevicePermissionServices _devicePermissionServices;

        public AudioPlayerNewViewModel(INavigation navigation = null) : base(navigation)
        {
            this.SetPageImageSize();
            this.PlayButton = TextResources.icon_media_play;
            this.PauseButton = TextResources.icon_media_pause;
            this.StopButton = TextResources.icon_media_stop;
            this.NextButton = TextResources.icon_media_next;
            this.PreviousButton = TextResources.icon_media_previous;
            this.PlayPauseButton = this.PlayButton;

            VolumeLabel = "Volume (0-" + CrossMediaManager.Current.VolumeManager.MaxVolume + ")";
            SetVolumeBtn = "Set Volume";

            VolumeEntryChange();
            CrossMediaManager.Current.VolumeManager.Mute = true;
            Mute();

            this.MediaContents = new List<MediaFile>();
            this._localFile = DependencyService.Get<ILocalFile>();
            _devicePermissionServices = DependencyService.Get<IDevicePermissionServices>();
            //CrossMediaManager.Current.AudioPlayer.OnFinishedPlaying = () =>
            //{
            //    //this.IsStopped = true;
            //    //this.PlayPauseButton = this.PlayButton;
            //    this.NextCommand();
            //};
            this.IsEditable = false;
            CrossMediaManager.Current.StatusChanged += AudioPlayer_StatusChanged;
        }

        public void AudioPlayer_StatusChanged(object sender,
            Plugin.MediaManager.Abstractions.EventArguments.StatusChangedEventArgs e)
        {
            //SliderValue=e. e.Status
            Debug.WriteLine($"MediaManager Status: {e.Status}");
        }

        public async Task GetAsync()
        {
            if (await _devicePermissionServices.RequestReadStoragePermission())
            {
                this.MediaContents = new List<MediaFile>();
                var mediaContents = new List<MediaFile>();
                var fileDetails = await _localFile.UpdatePlayListAsync();
                if (fileDetails == null || fileDetails.Count == 0 && !fileDetails.Any(f => f.Type.EndsWith(FileType)))
                    this.SetActivityResource(true, false, false, true, string.Empty, string.Empty,
                        TextResources.NoFileOrPermission);
                else
                {
                    int count = 0;
                    foreach (var content in fileDetails?.Where(f => f.Type.EndsWith(FileType)).OrderBy(f => f.Name))
                    {
                        count++;
                        mediaContents.Add(new MediaFile()
                        {
                            Url = content.Path,
                            Type = MediaFileType.Audio,
                            MetadataExtracted = false,
                            Availability = ResourceAvailability.Remote,
                        });
                    }

                    this.MediaContents = mediaContents;
                    CrossMediaManager.Current.MediaQueue.Repeat = RepeatType.RepeatOne;
                    this.IsEditable = this.MediaContents.Count > 0;
                    if (!this.IsEditable)
                    {
                        this.MessageText = TextResources.NoRecordToProcess;
                        this.IsMessage = true;
                    }

                    //this.PlaySong();
                    this.ErrorMessage = string.Empty;
                    this.IsError = false;
                }
            }
            else
            {
                this.ErrorMessage = TextResources.MessagePermissionReadStorageRequired;
                this.IsError = true;
            }
        }

        private string FileType => "MP3";

        private RootPage root;
        public const string RootPropertyName = "Root";

        public RootPage Root
        {
            get { return root; }
            set { SetProperty(ref root, value, RootPropertyName); }
        }

        private List<MediaFile> _mediaContents;
        public const string MediaContentsPropertyName = "MediaContents";

        public List<Plugin.MediaManager.Abstractions.Implementations.MediaFile> MediaContents
        {
            get { return _mediaContents; }
            set { SetProperty(ref _mediaContents, value, MediaContentsPropertyName); }
        }

        private MediaFile _mediaContentCurrent;
        public const string MediaContentCurrentPropertyName = "MediaContentCurrent";

        public MediaFile MediaContentCurrent
        {
            get { return _mediaContentCurrent; }
            set { SetProperty(ref _mediaContentCurrent, value, MediaContentCurrentPropertyName); }
        }

        private string volumeLabel;
        public const string VolumeLabelPropertyName = "VolumeLabel";

        public string VolumeLabel
        {
            get { return volumeLabel; }
            set { SetProperty(ref volumeLabel, value, VolumeLabelPropertyName); }
        }

        private short volumeEntry;
        public const string VolumeEntryPropertyName = "VolumeEntry";

        public short VolumeEntry
        {
            get { return volumeEntry; }
            set { SetProperty(ref volumeEntry, value, VolumeEntryPropertyName, VolumeEntryChange); }
        }

        private void VolumeEntryChange()
        {
            int.TryParse(this.VolumeEntry.ToString(), out var vol);
            CrossMediaManager.Current.VolumeManager.CurrentVolume = vol;
        }

        private string setVolumeBtn;
        public const string SetVolumeBtnPropertyName = "SetVolumeBtn";

        public string SetVolumeBtn
        {
            get { return setVolumeBtn; }
            set { SetProperty(ref setVolumeBtn, value, SetVolumeBtnPropertyName); }
        }

        private string mutedBtn;
        public const string MutedBtnPropertyName = "MutedBtn";

        public string MutedBtn
        {
            get { return mutedBtn; }
            set { SetProperty(ref mutedBtn, value, MutedBtnPropertyName); }
        }

        private string _playPauseButton;
        public string PlayPauseButtonPropertyName = "PlayPauseButton";

        public string PlayPauseButton
        {
            get { return _playPauseButton; }
            set { SetProperty(ref _playPauseButton, value, PlayPauseButtonPropertyName, ChangePlayPauseButton); }
        }

        private void ChangePlayPauseButton()
        {
            this.PlayPauseButtonSource = ImageResizer.ResizeImage(this.PlayPauseButton, this.ButtonImageSize);
        }

        private ImageSource _playPauseButtonSource;
        public string PlayPauseButtonSourcePropertyName = "PlayPauseButtonSource";

        public ImageSource PlayPauseButtonSource
        {
            get { return _playPauseButtonSource; }
            set { SetProperty(ref _playPauseButtonSource, value, PlayPauseButtonSourcePropertyName); }
        }

        private string _playButton;
        public string PlayButtonPropertyName = "PlayButton";

        public string PlayButton
        {
            get { return _playButton; }
            set { SetProperty(ref _playButton, value, PlayButtonPropertyName, ChangePlayButton); }
        }

        private void ChangePlayButton()
        {
            this.PlayButtonSource = ImageResizer.ResizeImage(this.PlayButton, this.ButtonImageSize);
        }

        private ImageSource _playButtonSource;
        public string PlayButtonSourcePropertyName = "PlayButtonSource";

        public ImageSource PlayButtonSource
        {
            get { return _playButtonSource; }
            set { SetProperty(ref _playButtonSource, value, PlayButtonSourcePropertyName); }
        }

        private string _pauseButton;
        public string PauseButtonPropertyName = "PauseButton";

        public string PauseButton
        {
            get { return _pauseButton; }
            set { SetProperty(ref _pauseButton, value, PauseButtonPropertyName, ChangePauseButton); }
        }

        private void ChangePauseButton()
        {
            this.PauseButtonSource = ImageResizer.ResizeImage(this.PauseButton, this.ButtonImageSize);
        }

        private ImageSource _pauseButtonSource;
        public string PauseButtonSourcePropertyName = "PauseButtonSource";

        public ImageSource PauseButtonSource
        {
            get { return _pauseButtonSource; }
            set { SetProperty(ref _pauseButtonSource, value, PauseButtonSourcePropertyName); }
        }

        private string _stopButton;
        public string StopButtonPropertyName = "StopButton";

        public string StopButton
        {
            get { return _stopButton; }
            set { SetProperty(ref _stopButton, value, StopButtonPropertyName, ChangeStopButton); }
        }

        private void ChangeStopButton()
        {
            this.StopButtonSource = ImageResizer.ResizeImage(this.StopButton, this.ButtonImageSize);
        }

        private ImageSource _stopButtonSource;
        public string StopButtonSourcePropertyName = "StopButtonSource";

        public ImageSource StopButtonSource
        {
            get { return _stopButtonSource; }
            set { SetProperty(ref _stopButtonSource, value, StopButtonSourcePropertyName); }
        }

        private string _nextButton;
        public string NextButtonPropertyName = "NextButton";

        public string NextButton
        {
            get { return _nextButton; }
            set { SetProperty(ref _nextButton, value, NextButtonPropertyName, ChangeNextButton); }
        }

        private void ChangeNextButton()
        {
            this.NextButtonSource = ImageResizer.ResizeImage(this.NextButton, this.ButtonImageSize);
        }

        private ImageSource _nextButtonSource;
        public string NextButtonSourcePropertyName = "NextButtonSource";

        public ImageSource NextButtonSource
        {
            get { return _nextButtonSource; }
            set { SetProperty(ref _nextButtonSource, value, NextButtonSourcePropertyName); }
        }

        private string _previousButton;
        public string PreviousButtonPropertyName = "PreviousButton";

        public string PreviousButton
        {
            get { return _previousButton; }
            set { SetProperty(ref _previousButton, value, PreviousButtonPropertyName, ChangePreviousButton); }
        }

        private void ChangePreviousButton()
        {
            this.PreviousButtonSource = ImageResizer.ResizeImage(this.PreviousButton, this.ButtonImageSize);
        }

        private ImageSource _previousButtonSource;
        public string PreviousButtonSourcePropertyName = "PreviousButtonSource";

        public ImageSource PreviousButtonSource
        {
            get { return _previousButtonSource; }
            set { SetProperty(ref _previousButtonSource, value, PreviousButtonSourcePropertyName); }
        }

        private ImageSize ButtonImageSize { get; set; }

        private void SetPageImageSize()
        {
            this.ButtonImageSize = App.Configuration.GetImageSizeByID(ImageIdentity.AUDIO_PLAYER_PAGE_COMMAND_IMAGE);
            if (this.ButtonImageSize != null)
            {
                this.AudioCommandImageHeight = this.ButtonImageSize.Height;
                this.AudioCommandImageWidth = this.ButtonImageSize.Width;
            }
        }

        private double sliderValue;
        public const string SliderValuePropertyName = "SliderValue";

        public double SliderValue
        {
            get { return sliderValue; }
            set { SetProperty(ref sliderValue, value, SliderValuePropertyName); }
        }

        private float audioCommandImageHeight;
        public const string AudioCommandImageHeightPropertyName = "AudioCommandImageHeight";

        public float AudioCommandImageHeight
        {
            get { return audioCommandImageHeight; }
            set { SetProperty(ref audioCommandImageHeight, value, AudioCommandImageHeightPropertyName); }
        }

        private float audioCommandImageWidth;
        public const string AudioCommandImageWidthPropertyName = "AudioCommandImageWidth";

        public float AudioCommandImageWidth
        {
            get { return audioCommandImageWidth; }
            set { SetProperty(ref audioCommandImageWidth, value, AudioCommandImageWidthPropertyName); }
        }

        private ICommand setVolumeBtnCommand;

        public ICommand SetVolumeBtnCommand
        {
            get { return setVolumeBtnCommand ?? (setVolumeBtnCommand = new Command(VolumeEntryChange)); }
        }

        private ICommand mutedBtnCommand;

        public ICommand MutedBtnCommand
        {
            get { return mutedBtnCommand ?? (mutedBtnCommand = new Command(Mute)); }
        }

        private void Mute()
        {
            if (CrossMediaManager.Current.VolumeManager.Mute)
            {
                CrossMediaManager.Current.VolumeManager.Mute = false;
                MutedBtn = "Mute";
            }
            else
            {
                CrossMediaManager.Current.VolumeManager.Mute = true;
                MutedBtn = "Unmute";
            }
        }


        private ICommand playCommand;

        public ICommand PlayCommand
        {
            get
            {
                return playCommand ?? (playCommand = new Command(async () =>
                {
                    var mediaFile = new MediaFile
                    {
                        Type = MediaFileType.Audio,
                        Availability = ResourceAvailability.Remote,
                        Url = "https://audioboom.com/posts/5766044-follow-up-305.mp3",
                        MetadataExtracted = true
                    };
                    await CrossMediaManager.Current.Play(mediaFile);

                    //await CrossMediaManager.Current.Play(this.MediaContents);
                }));
            }
        }

        private ICommand pauseCommand;

        public ICommand PauseCommand
        {
            get { return pauseCommand ?? (pauseCommand = new Command(() => { })); }
        }

        private ICommand stopCommand;

        public ICommand StopCommand
        {
            get
            {
                return stopCommand ??
                       (stopCommand = new Command(async () => { await CrossMediaManager.Current.Stop(); }));
            }
        }

        private ICommand nextCommand;

        public ICommand NextCommand
        {
            get { return nextCommand ?? (nextCommand = new Command(() => { })); }
        }

        private ICommand previousCommand;

        public ICommand PreviousCommand
        {
            get { return previousCommand ?? (previousCommand = new Command(() => { })); }
        }
    }
}