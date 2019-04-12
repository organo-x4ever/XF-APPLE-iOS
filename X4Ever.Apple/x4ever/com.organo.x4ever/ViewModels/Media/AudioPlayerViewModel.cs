using com.organo.xchallenge.Localization;
using com.organo.xchallenge.Pages;
using com.organo.xchallenge.Permissions;
using com.organo.xchallenge.Services;
using com.organo.xchallenge.Statics;
using com.organo.xchallenge.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using com.organo.xchallenge.Globals;
using com.organo.xchallenge.Helpers;
using Plugin.MediaManager;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Enums;
using Plugin.MediaManager.Abstractions.Implementations;
using Xamarin.Forms;

namespace com.organo.xchallenge.ViewModels.Media
{
    public class AudioPlayerViewModel : BaseViewModel
    {
        private readonly ILocalFile _localFile;
        private IDevicePermissionServices _devicePermissionServices;
        
        public AudioPlayerViewModel(INavigation navigation = null) : base(navigation)
        {
            this.SetPageImageSize();
            this.PlayButton = TextResources.icon_media_play;
            this.PauseButton = TextResources.icon_media_pause;
            this.StopButton = TextResources.icon_media_stop;
            this.NextButton = TextResources.icon_media_next;
            this.PreviousButton = TextResources.icon_media_previous;
            this.PlayPauseButton = this.PlayButton;
            this.IsPaused = false;
            this.IsPlaying = false;
            this.MediaContents = new List<MediaFile>();
            this.CurrentSongIndex = -1;
            this._localFile = DependencyService.Get<ILocalFile>();
            _devicePermissionServices = DependencyService.Get<IDevicePermissionServices>();
            CrossMediaManager.Current.AudioPlayer.StatusChanged += AudioPlayer_StatusChanged;
            //CrossMediaManager.Current.AudioPlayer.OnFinishedPlaying = () =>
            //{
            //    //this.IsStopped = true;
            //    //this.PlayPauseButton = this.PlayButton;
            //    this.NextCommand();
            //};
            this.IsStopped = true;
            this.IsEditable = false;
        }

        private void AudioPlayer_StatusChanged(object sender, Plugin.MediaManager.Abstractions.EventArguments.StatusChangedEventArgs e)
        {
            this.IsStopped = true;
            //this.PlayPauseButton = this.PlayButton;
            this.NextCommand();
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
                    await CrossMediaManager.Current.Play(this.MediaContents);
                    this.IsEditable = this.MediaContents.Count > 0;
                    if (!this.IsEditable)
                    {
                        this.MessageText = TextResources.NoRecordToProcess;
                        this.IsMessage = true;
                    }

                    this.PlaySong();
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

        public int Duration => (int)CrossMediaManager.Current.AudioPlayer.Duration.TotalSeconds;
        public int CurrentPosition => CrossMediaManager.Current.AudioPlayer.Position.Seconds;
        public bool CanSeek => true;

        private float _volume;
        public const string VolumePropertyName = "Volume";

        public float Volume
        {
            get { return _volume; }
            set { SetProperty(ref _volume, value, VolumePropertyName, SetVolume); }
        }

        private bool _isStopped;
        private string IsStoppedPropertyName => "IsStopped";

        public bool IsStopped
        {
            get { return _isStopped; }
            set { SetProperty(ref _isStopped, value, IsStoppedPropertyName, OnPlaying); }
        }

        private bool _isPlaying;
        public const string IsPlayingPropertyName = "IsPlaying";

        public bool IsPlaying
        {
            get { return _isPlaying; }
            set { SetProperty(ref _isPlaying, value, IsPlayingPropertyName); }
        }

        private bool _isPaused;
        public string IsPausedPropertyName => "IsPaused";

        public bool IsPaused
        {
            get { return _isPaused; }
            set { SetProperty(ref _isPaused, value, IsPausedPropertyName, OnPlaying); }
        }

        private bool _showPlaying;
        private string ShowPlayingPropertyName => "ShowPlaying";

        public bool ShowPlaying
        {
            get { return _showPlaying; }
            set { SetProperty(ref _showPlaying, value, ShowPlayingPropertyName); }
        }

        private void OnPlaying()
        {
            this.IsStopped = CrossMediaManager.Current.AudioPlayer.Status == MediaPlayerStatus.Stopped;
            this.ShowPlaying = !this.IsStopped;
            this.IsPlaying  = CrossMediaManager.Current.AudioPlayer.Status == MediaPlayerStatus.Playing;
            TimerDisplaying?.Invoke();
            if (this.IsStopped)
                this.ProgressTime = "00:00";
        }

        public void PlayPauseCommand()
        {
            if (this.IsStopped)
                this.ProgressTime = "00:00";
            if (this.MediaContents.Count == 0)
                return;
            if (this.MediaContentCurrent == null)
                this.MediaContentCurrent = this.MediaContents[0];
            this.PlayPauseButton = this.PlayPauseButton == this.PlayButton ? this.PauseButton : this.PlayButton;
            if (this.PlayPauseButton == this.PauseButton)
            {
                if (this.IsStopped)
                {
                    this.IsStopped = false;
                    CrossMediaManager.Current.AudioPlayer.Play(this.MediaContentCurrent);
                }
                else
                {
                    CrossMediaManager.Current.AudioPlayer.Play();
                }

                this.IsPaused = false;
            }
            else
            {
                this.IsPaused = true;
                CrossMediaManager.Current.AudioPlayer.Pause();
            }
        }

        public Action TimerDisplaying { get; set; }

        public void Stop()
        {
            this.IsStopped = true;
            this.ProgressTime = "00:00";
            CrossMediaManager.Current.AudioPlayer.Stop();
            this.PlayPauseButton = this.PlayButton;
            var mediaContents = this.MediaContents;
            this.MediaContents = new List<MediaFile>();
            foreach (var mediaContent in mediaContents)
            {
                //mediaContent.IsPlayingNow = false;
                //mediaContent.MediaTitleColor = Palette._TitleTexts;
                this.MediaContents.Add(mediaContent);
            }
        }

        public void NextCommand()
        {
            this.IsPaused = false;
            this.CurrentSongIndex += 1;
            if (this.IsPlaying)
            {
                CrossMediaManager.Current.AudioPlayer.Play(this.MediaContentCurrent);
                this.PlayPauseButton = this.PauseButton;
            }
        }

        public void PreviousCommand()
        {
            this.IsPaused = false;
            this.CurrentSongIndex -= 1;
            if (this.IsPlaying)
            {
                CrossMediaManager.Current.AudioPlayer.Play(this.MediaContentCurrent);
                this.PlayPauseButton = this.PauseButton;
            }
        }

        public void IndexedCommand(int index)
        {
            this.IsStopped = false;
            this.IsPaused = false;
            this.CurrentSongIndex = index;
            CrossMediaManager.Current.AudioPlayer.Play(this.MediaContentCurrent);
            this.PlayPauseButton = this.PauseButton;
        }

        public void PlaySong()
        {
            this.ProgressTime = "00:00";
            if (this.MediaContents.Count > 0)
            {
                if (this.CurrentSongIndex < 0)
                    this.CurrentSongIndex = this.MediaContents.Count - 1;
                else if (this.CurrentSongIndex > (this.MediaContents.Count - 1))
                    this.CurrentSongIndex = 0;
                this.MediaContentCurrent = this.MediaContents[this.CurrentSongIndex];
                UpdateMediaContents(this.MediaContentCurrent, this.CurrentSongIndex);
            }
        }

        public void UpdateMediaContents(MediaFile mediaContentCurrent, int rowToUpdateIndex = 100000)
        {
            var mediaContents = this.MediaContents;
            if (rowToUpdateIndex == 100000)
                rowToUpdateIndex = this.MediaContents.FindIndex(m =>
                    m == mediaContentCurrent && m == mediaContentCurrent);
            this.MediaContents = new List<MediaFile>();
            int index = 0;
            foreach (var mediaContent in mediaContents)
            {
                //mediaContent.IsPlayingNow = index == this.CurrentSongIndex;
                if (index == rowToUpdateIndex)
                {
                    //mediaContentCurrent.IsPlayingNow = index == this.CurrentSongIndex;
                    this.MediaContents.Add(mediaContentCurrent);
                }
                else
                    this.MediaContents.Add(mediaContent);

                //mediaContent.MediaTitleColor = mediaContent.IsPlayingNow ? Palette._MainAccent : Palette._TitleTexts;
                index += 1;
            }
        }

        private void SetVolume()
        {
            //CrossMediaManager.Current.AudioPlayer.SetVolume(this.Volume);
        }

        public void SeekTo(int seekValue)
        {
            CrossMediaManager.Current.AudioPlayer.Seek(new TimeSpan(seekValue));
        }

        public string ConvertTImeToDisplay(int milliseconds)
        {
            var times = TimeSpan.FromMilliseconds(milliseconds).ToString().Split('.');
            string[] timeStrings = times[0].ToString().Split(':');
            int.TryParse(timeStrings[0], out int h);
            return (h > 0 ? h.ToString() + ":" : "") + timeStrings[1] + ":" + timeStrings[2];
        }

        private ICommand _showSideMenuCommand;

        public ICommand ShowSideMenuCommand
        {
            get
            {
                return _showSideMenuCommand ?? (_showSideMenuCommand = new Command((obj) =>
                {
                    this.Root.IsPresented = this.Root.IsPresented == false;
                }));
            }
        }

        private string _progressTime;
        public string ProgressTimePropertyName => "ProgressTime";

        public string ProgressTime
        {
            get { return _progressTime; }
            set { SetProperty(ref _progressTime, value, ProgressTimePropertyName); }
        }

        private string _totalTime;
        public string TotalTimePropertyName => "TotalTime";

        public string TotalTime
        {
            get { return _totalTime; }
            set { SetProperty(ref _totalTime, value, TotalTimePropertyName); }
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

        private int _currentSongIndex;
        public const string CurrentSongIndexPropertyName = "CurrentSongIndex";

        public int CurrentSongIndex
        {
            get { return _currentSongIndex; }
            set { SetProperty(ref _currentSongIndex, value, CurrentSongIndexPropertyName, PlaySong); }
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
    }
}