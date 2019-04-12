using System;
//using System.Collections;
using System.Collections.Generic;
using System.Linq;
using com.organo.x4ever.Globals;
using com.organo.x4ever.Helpers;
using com.organo.x4ever.Localization;
using com.organo.x4ever.Pages;
using com.organo.x4ever.Statics;
using com.organo.x4ever.ViewModels.Base;
using Plugin.MediaManager;
using Plugin.MediaManager.Abstractions.Enums;
using Plugin.MediaManager.Abstractions.Implementations;
using System.Threading.Tasks;
using System.Windows.Input;
using com.organo.x4ever.Models;
using com.organo.x4ever.Services;
using Plugin.MediaManager.Abstractions;
using Xamarin.Forms;

namespace com.organo.x4ever.ViewModels.Audio
{
    public class AudioManagerViewModel : BaseViewModel
    {
        private IPlaybackController PlaybackController => CrossMediaManager.Current.PlaybackController;
        private readonly ILocalFile _localFile;
        public AudioManagerViewModel(INavigation navigation = null) : base(navigation)
        {
            _localFile = DependencyService.Get<ILocalFile>();
            _currentTime = DateTime.Now;
            tapAttempt = 0;
            IsDeveloper = false;
            SetPageImageSize();
            MediaFiles = new List<MediaFile>();
            CurrentMediaFile = new MediaFile();
            PlayButton = ImageResizer.ResizeImage(TextResources.icon_media_play, ButtonImageSize);
            PauseButton = ImageResizer.ResizeImage(TextResources.icon_media_pause, ButtonImageSize);
            StopButton = ImageResizer.ResizeImage(TextResources.icon_media_stop, ButtonImageSize);
            NextButton = ImageResizer.ResizeImage(TextResources.icon_media_next, ButtonImageSize);
            PreviousButton = ImageResizer.ResizeImage(TextResources.icon_media_previous, ButtonImageSize);
            PlayPauseButton = PlayButton;
            IsPlaying = false;
            IsPause = false;
            IsMediaExists = false;
            CurrentSongIndex = 0;
            CrossMediaManager.Current.PlayingChanged += async (sender, e) =>
            {
                CurrentPosition = e.Progress;
                CurrentTimer = e.Position.ToString(@"hh\:mm\:ss");
                TotalTimer = e.Duration.ToString(@"hh\:mm\:ss");
                double TOLERANCE = 0;
                if (Math.Abs(e.Progress - 1) < TOLERANCE)
                    await PlayCurrent(NextAsync());
            };
        }

        public void GetDirectoryAsync()
        {
           GetFilesAsync();
        }

        private List<MediaFile> GetFilesAsync()
        {
            var mediaFiles = new List<MediaFile>();
            StackLayout stackLayout = new StackLayout();
            var fileDetails = _localFile.UpdatePlayListAsync();
            var messages = _localFile.Messages;
            if (fileDetails == null || fileDetails.Count == 0)
            {
                messages.Add("FileDetails Count: #" + fileDetails?.Count);
            }

            if (messages.Count > 0)
            {
                foreach (var message in messages)
                {
                    stackLayout.Children.Add(new Label()
                    {
                        FormattedText = new FormattedString() {Spans = {new Span() {Text = message}}}
                    });
                }
            }

            //if (fileDetails == null || fileDetails.Count == 0)
            //{
            //var fileDetail = await _localFile.GetLogicalDrivesAsync();
            //messages = _localFile.Messages;
            //if (fileDetail == null || fileDetail.Count == 0)
            //{
            //    messages.Add("FileDetail Count: #" + fileDetail?.Count);
            //}

            //if (messages.Count > 0)
            //{
            //    foreach (var message in messages)
            //    {
            //        stackLayout.Children.Add(new Label()
            //        {
            //            FormattedText = new FormattedString() {Spans = {new Span() {Text = message}}}
            //        });
            //    }
            //}

            //}
            MessageContent = stackLayout;

            if (fileDetails == null
                || fileDetails.Count == 0
                || !fileDetails.Any(f => f.Type.EndsWith(FileType)))
                SetActivityResource(showError: true, errorMessage: TextResources.NoFileOrPermission);
            else
            {
                foreach (var content in fileDetails.Where(f => f.Type.EndsWith(FileType)).OrderBy(f => f.Name))
                {
                    mediaFiles.Add(new MediaFile()
                    {
                        Url = content.Path,
                        Type = MediaFileType.Audio,
                        MetadataExtracted = false,
                        Availability = ResourceAvailability.Local,
                    });
                }

                //foreach (var content in fileDetail.Where(f => f.Type.EndsWith(FileType)).OrderBy(f => f.Name))
                //{
                //    mediaFiles.Add(new MediaFile()
                //    {
                //        Url = content.Path,
                //        Type = MediaFileType.Audio,
                //        MetadataExtracted = false,
                //        Availability = ResourceAvailability.Local,
                //    });
                //}

                MediaFiles = mediaFiles;
            }

            IsMediaExists = MediaFiles.Count > 0;
            return MediaFiles;
        }
        
        private View messageContent;
        public const string MessageContentPropertyName = "MessageContent";

        public View MessageContent
        {
            get { return messageContent; }
            set { SetProperty(ref messageContent, value, MessageContentPropertyName); }
        }

        private bool _developer;
        public const string IsDeveloperPropertyName = "IsDeveloper";

        public bool IsDeveloper
        {
            get { return _developer; }
            set { SetProperty(ref _developer, value, IsDeveloperPropertyName); }
        }

        private short timeDelay => 2000;
        private ICommand _developerViewCommand;
        private DateTime _currentTime;
        private short tapAttempt;

        public ICommand DeveloperViewCommand
        {
            get
            {
                return _developerViewCommand ?? (_developerViewCommand = new Command((obj) =>
                {
                    if (tapAttempt == 0)
                        _currentTime = DateTime.Now;
                    if (_currentTime.AddMilliseconds(timeDelay) >= DateTime.Now)
                        tapAttempt++;
                    else
                    {
                        tapAttempt = 0;
                        _currentTime = DateTime.Now;
                    }

                    IsDeveloper = tapAttempt == 3;
                }));
            }
        }

        private bool _showPlayer;
        public const string ShowPlayerPropertyName = "ShowPlayer";

        public bool ShowPlayer
        {
            get { return _showPlayer; }
            set { SetProperty(ref _showPlayer, value, ShowPlayerPropertyName); }
        }

        private string FileType => "MP3";

        private double _currentPosition;
        public const string CurrentPositionPropertyName = "CurrentPosition";

        public double CurrentPosition
        {
            get { return _currentPosition; }
            set { SetProperty(ref _currentPosition, value, CurrentPositionPropertyName); }
        }

        private string _currentTimer;
        public const string CurrentTimerPropertyName = "CurrentTimer";

        public string CurrentTimer
        {
            get { return _currentTimer; }
            set { SetProperty(ref _currentTimer, value, CurrentTimerPropertyName); }
        }

        private string _totalTimer;
        public const string TotalTimerPropertyName = "TotalTimer";

        public string TotalTimer
        {
            get { return _totalTimer; }
            set { SetProperty(ref _totalTimer, value, TotalTimerPropertyName); }
        }

        private string _mediaTitle;
        public const string MediaTitlePropertyName = "MediaTitle";

        public string MediaTitle
        {
            get { return _mediaTitle; }
            set { SetProperty(ref _mediaTitle, value, MediaTitlePropertyName); }
        }

        private ImageSource _playButton;
        public string PlayButtonPropertyName = "PlayButton";

        public ImageSource PlayButton
        {
            get { return _playButton; }
            set { SetProperty(ref _playButton, value, PlayButtonPropertyName); }
        }

        private ImageSource _pauseButton;
        public string PauseButtonPropertyName = "PauseButton";

        public ImageSource PauseButton
        {
            get { return _pauseButton; }
            set { SetProperty(ref _pauseButton, value, PauseButtonPropertyName); }
        }

        private ImageSource _playPauseButton;
        public string PlayPauseButtonPropertyName = "PlayPauseButton";

        public ImageSource PlayPauseButton
        {
            get { return _playPauseButton; }
            set { SetProperty(ref _playPauseButton, value, PlayPauseButtonPropertyName); }
        }

        private ImageSource _stopButton;
        public string StopButtonPropertyName = "StopButton";

        public ImageSource StopButton
        {
            get { return _stopButton; }
            set { SetProperty(ref _stopButton, value, StopButtonPropertyName); }
        }

        private ImageSource _nextButton;
        public string NextButtonPropertyName = "NextButton";

        public ImageSource NextButton
        {
            get { return _nextButton; }
            set { SetProperty(ref _nextButton, value, NextButtonPropertyName); }
        }

        private ImageSource _previousButton;
        public string PreviousButtonPropertyName = "PreviousButton";

        public ImageSource PreviousButton
        {
            get { return _previousButton; }
            set { SetProperty(ref _previousButton, value, PreviousButtonPropertyName); }
        }

        private ImageSize ButtonImageSize { get; set; }

        private void SetPageImageSize()
        {
            ButtonImageSize = App.Configuration.GetImageSizeByID(ImageIdentity.AUDIO_PLAYER_PAGE_COMMAND_IMAGE);
            if (ButtonImageSize != null)
            {
                AudioCommandImageHeight = ButtonImageSize.Height;
                AudioCommandImageWidth = ButtonImageSize.Width;
            }
        }

        private float _audioCommandImageHeight;
        public const string AudioCommandImageHeightPropertyName = "AudioCommandImageHeight";

        public float AudioCommandImageHeight
        {
            get { return _audioCommandImageHeight; }
            set { SetProperty(ref _audioCommandImageHeight, value, AudioCommandImageHeightPropertyName); }
        }

        private float _audioCommandImageWidth;
        public const string AudioCommandImageWidthPropertyName = "AudioCommandImageWidth";

        public float AudioCommandImageWidth
        {
            get { return _audioCommandImageWidth; }
            set { SetProperty(ref _audioCommandImageWidth, value, AudioCommandImageWidthPropertyName); }
        }

        private int _currentSongIndex;
        public const string CurrentSongIndexPropertyName = "CurrentSongIndex";

        public int CurrentSongIndex
        {
            get { return _currentSongIndex; }
            set { SetProperty(ref _currentSongIndex, value, CurrentSongIndexPropertyName); }
        }

        private bool _isMediaExists;
        public const string IsMediaExistsPropertyName = "IsMediaExists";

        public bool IsMediaExists
        {
            get { return _isMediaExists; }
            set { SetProperty(ref _isMediaExists, value, IsMediaExistsPropertyName); }
        }

        private bool _isPlaying;
        public const string IsPlayingPropertyName = "IsPlaying";

        public bool IsPlaying
        {
            get { return _isPlaying; }
            set { SetProperty(ref _isPlaying, value, IsPlayingPropertyName, IsPlayingChange); }
        }

        private void IsPlayingChange()
        {
            if (IsPlaying)
                PlayPauseButton = PauseButton;
            else
                PlayPauseButton = PlayButton;
        }

        private bool _isPause;
        public const string IsPausePropertyName = "IsPause";

        public bool IsPause
        {
            get { return _isPause; }
            set { SetProperty(ref _isPause, value, IsPausePropertyName); }
        }

        public async Task PlayCurrent(int songIndex)
        {
            CurrentSongIndex = songIndex;
            IsPlaying = true;
            CurrentMediaFile = MediaFiles[songIndex];
            var files = CurrentMediaFile.Url.Split('/');
            MediaTitle = files[files.Length - 1];
            await CrossMediaManager.Current.Play(CurrentMediaFile);
        }

        private int NextAsync()
        {
            if (MediaFiles.Count() > (CurrentSongIndex + 1))
                CurrentSongIndex++;
            else
                CurrentSongIndex = 0;
            return CurrentSongIndex;
        }

        private int PreviousAsync()
        {
            if (CurrentSongIndex > 0)
                CurrentSongIndex--;
            else
                CurrentSongIndex = MediaFiles.Count - 1;
            return CurrentSongIndex;
        }

        private List<MediaFile> _mediaFiles;
        public const string MediaFilesPropertyName = "MediaFiles";

        public List<MediaFile> MediaFiles
        {
            get { return _mediaFiles; }
            set { SetProperty(ref _mediaFiles, value, MediaFilesPropertyName); }
        }

        private MediaFile _currentMediaFile;
        public const string CurrentMediaFilePropertyName = "CurrentMediaFile";

        public MediaFile CurrentMediaFile
        {
            get { return _currentMediaFile; }
            set { SetProperty(ref _currentMediaFile, value, CurrentMediaFilePropertyName); }
        }

        private ICommand _playCommand;

        public ICommand PlayCommand
        {
            get
            {
                return _playCommand ?? (_playCommand = new Command(async (obj) =>
                {
                    IsPlaying = IsPlaying == false;
                    if (IsPlaying)
                    {
                        if (IsPause)
                            await CrossMediaManager.Current.PlayByPosition((int) CurrentPosition);
                        else
                            await PlayCurrent(CurrentSongIndex);
                        IsPause = false;
                    }
                    else
                    {
                        IsPause = true;
                        await CrossMediaManager.Current.Pause();
                    }
                }));
            }
        }

        private ICommand _stopCommand;

        public ICommand StopCommand
        {
            get
            {
                return _stopCommand ?? (_stopCommand = new Command(() =>
                {
                    IsPlaying = false;
                    StopAsync();
                    CurrentTimer = TimeSpan.FromSeconds(0).ToString(@"hh\:mm\:ss");
                }));
            }
        }

        public async void StopAsync()
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

        private ICommand _nextCommand;

        public ICommand NextCommand
        {
            get
            {
                return _nextCommand ?? (_nextCommand = new Command(async () => { await PlayCurrent(NextAsync()); }));
            }
        }

        private ICommand _previousCommand;

        public ICommand PreviousCommand
        {
            get
            {
                return _previousCommand ?? (_previousCommand = new Command(async () =>
                {
                    await PlayCurrent(PreviousAsync());
                }));
            }
        }

        //public async Task SeekTo(int seekValue)
        //{
        //    await CrossMediaManager.Current.Seek(new TimeSpan(seekValue));
        //}
    }
}