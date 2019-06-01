using com.organo.x4ever.Helpers;
using com.organo.x4ever.Localization;
using com.organo.x4ever.MediaManager;
using com.organo.x4ever.Models;
using com.organo.x4ever.Statics;
using com.organo.x4ever.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using com.organo.x4ever.Handler;
using com.organo.x4ever.Services;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace com.organo.x4ever.ViewModels.Media
{
    public class AudioPlayerViewModel : BaseViewModel
    {
        private IAudioPlayerManager _audioPlayerManager;
        private readonly IMusicDictionary _musicDictionary;
        private static short timerSeconds = 1;
        private bool _songReloaded = false;

        public AudioPlayerViewModel(INavigation navigation = null) : base(navigation)
        {
            SetActivityResource(showEditable: false, showBusy: true, busyMessage: TextResources.ProcessingPleaseWait);
            _audioPlayerManager = DependencyService.Get<IAudioPlayerManager>();
            _musicDictionary = DependencyService.Get<IMusicDictionary>();
            if (!_musicDictionary.Authorized())
            {
                var message = "";
                _musicDictionary.Messages.ForEach((msg) => { message += msg + "\n"; });
                SetActivityResource(showMessage: true, message: message);
            }
            SetPageImageSize();
            MusicFiles = new List<MediaItem>();
            AllMusicFiles = new List<MediaItem>();
            PlaylistMusicFiles = new List<MediaItem>();
            CurrentMusicFile = new MediaItem();
            PlayButton = ImageResizer.ResizeImage(TextResources.icon_media_play, ButtonImageSize);
            //PauseButton = ImageResizer.ResizeImage(TextResources.icon_media_pause, ButtonImageSize);
            StopButton = ImageResizer.ResizeImage(TextResources.icon_media_stop, ButtonImageSize);
            NextButton = ImageResizer.ResizeImage(TextResources.icon_media_next, ButtonImageSize);
            PreviousButton = ImageResizer.ResizeImage(TextResources.icon_media_previous, ButtonImageSize);
            //ForwardButton = ImageResizer.ResizeImage(TextResources.icon_media_forward, ButtonImageSize);
            //BackwardButton = ImageResizer.ResizeImage(TextResources.icon_media_backward, ButtonImageSize);
            PlayPauseButton = PlayButton;
            NowPlayingButton = PlayButton;

            ChecklistImage = ChecklistDefaultImage;
            SortImage = SortDefaultImage;
            PlaylistTextStyle = PlaylistTextStyleDefault;
            PlaylistSortBy = PlaylistSortList.Title;

            IsPlaying = false;
            IsPause = false;
            IsMediaExists = false;
            CurrentSongIndex = 0;
            IsLoopStarted = false;
            IsPermissionGranted = false;
        }

        private void CurrentPlay()
        {
            if (IsLoopStarted) return;
            var seconds = TimeSpan.FromSeconds(timerSeconds);
            Device.StartTimer(seconds, () =>
            {
                IsLoopStarted = true;
                PlayerLoop().GetAwaiter();
                return IsLoopStarted;
            });
        }

        private async Task PlayerLoop()
        {
            if (IsPlaying && CurrentPlayer.IsPlaying)
            {
                CurrentPosition = (CurrentPlayer.CurrentPosition * 100) / CurrentMusicFile._Duration;
                CurrentTimer = TimeSpan.FromSeconds(CurrentPlayer.CurrentPosition).ToString(TimeStyle);
                if (CurrentPlayer.IsPlaying && CurrentPlayer.CurrentPosition >= CurrentMusicFile._Duration)
                    await PlayCurrent(Next());
            }
        }

        public async Task GetFilesAsync()
        {
            try
            {
                await Task.Factory.StartNew(async () =>
                {
                    await SetSongs();
                    _audioPlayerManager.CurrentPlayer.ReadyToPlay += (sender, e) => { CurrentPlay(); };
                    _audioPlayerManager.CurrentPlayer.StartReached += async (sender, e) =>
                    {
                        await PlayCurrent(Previous());
                    };
                    _audioPlayerManager.CurrentPlayer.EndReached += async (sender, e) => { await PlayCurrent(Next()); };
                    SetActivityResource();
                });
            }
            catch (Exception ex)
            {
                new ExceptionHandler(nameof(AudioPlayerViewModel), ex);
            }
        }

        private async Task SetSongs()
        {
            MusicFiles = new List<MediaItem>();
            AllMusicFiles = new List<MediaItem>();
            ClearMessage();
            _musicDictionary.Messages = new List<string>();
            var musicFiles = _musicDictionary.GetSongs();
            AllMusicFiles = musicFiles?.Select(music =>
            {
                music._Duration = music.Duration;
                music._DurationTimeSpan = TimeSpan.FromSeconds(music.Duration)
                    .ToString(music.Duration > (60 * 60) ? @"hh\:mm\:ss" : @"mm\:ss");
                return music;
            }).OrderBy(m => m.Title).ToList();

            IsMediaExists = AllMusicFiles.Count > 0;
            MusicFiles = AllMusicFiles;
            IsPermissionGranted = IsMediaExists && _musicDictionary.MediaLibraryAuthorized;
            SortOrderBy(PlaylistSortBy);
            if (MusicFiles.Count == 0)
            {
                if (!_songReloaded)
                {
                    _songReloaded = true;
                    SetActivityResource(false, showBusy: true, busyMessage: "Reloading . . .");
                    await Task.Delay(TimeSpan.FromMilliseconds(1500));
                    //await SetSongs();
                    RefreshCommand.Execute(null);
                }
                else
                    SetActivityResource();
            }
            else
                SetActivityResource();
        }

        private IAudioPlayer _currentPlayer;

        private IAudioPlayer CurrentPlayer
        {
            get => _currentPlayer;
            set => _currentPlayer = value;
        }

        private void AddMessage(string text)
        {
            var messages = new List<Message>();
            Messages.ForEach((m) =>
            {
                messages.Add(m);
            });
            messages.Add(new Message() {Text = text});
            Messages = new List<Message>();
            Messages = messages;
            IsMessageExists = Messages.Count() > 0;
        }

        private void ClearMessage()
        {
            Messages = new List<Message>();
            IsMessageExists = Messages.Count() > 0;
        }

        private List<Message> _messages;
        public const string MessagesPropertyName = "Messages";

        public List<Message> Messages
        {
            get => _messages;
            set => SetProperty(ref _messages, value, MessagesPropertyName);
        }

        public class Message
        {
            public string Text { get; set; }
        }

        private bool _isMessageExists;
        public const string IsMessageExistsPropertyName = "IsMessageExists";

        public bool IsMessageExists
        {
            get => _isMessageExists;
            set => SetProperty(ref _isMessageExists, value, IsMessageExistsPropertyName);
        }

        private LayoutOptions _messageLayout;
        public const string MessageLayoutPropertyName = "MessageLayout";

        public LayoutOptions MessageLayout
        {
            get => _messageLayout;
            set => SetProperty(ref _messageLayout, value, MessageLayoutPropertyName);
        }

        private bool IsLoopStarted { get; set; }
        private bool IsDurationLong { get; set; }
        private string TimeStyle { get; set; }

        private bool _isPermissionGranted;
        public const string IsPermissionGrantedPropertyName = "IsPermissionGranted";

        public bool IsPermissionGranted
        {
            get { return _isPermissionGranted; }
            set { SetProperty(ref _isPermissionGranted, value, IsPermissionGrantedPropertyName); }
        }

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


        private string _timeSplitor;
        public const string TimeSplitorPropertyName = "TimeSplitor";

        public string TimeSplitor
        {
            get { return _timeSplitor; }
            set { SetProperty(ref _timeSplitor, value, TimeSplitorPropertyName); }
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

        //private ImageSource _pauseButton;
        //public string PauseButtonPropertyName = "PauseButton";

        //public ImageSource PauseButton
        //{
        //    get { return _pauseButton; }
        //    set { SetProperty(ref _pauseButton, value, PauseButtonPropertyName); }
        //}

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

        private ImageSource _forwardButton;
        public string ForwardButtonPropertyName = "ForwardButton";

        public ImageSource ForwardButton
        {
            get { return _forwardButton; }
            set { SetProperty(ref _forwardButton, value, ForwardButtonPropertyName); }
        }

        private ImageSource _backwardButton;
        public string BackwardButtonPropertyName = "BackwardButton";

        public ImageSource BackwardButton
        {
            get { return _backwardButton; }
            set { SetProperty(ref _backwardButton, value, BackwardButtonPropertyName); }
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

        private ImageSource _nowPlayingButton;
        public string NowPlayingButtonPropertyName = "NowPlayingButton";

        public ImageSource NowPlayingButton
        {
            get { return _nowPlayingButton; }
            set { SetProperty(ref _nowPlayingButton, value, NowPlayingButtonPropertyName); }
        }

        private bool _isChecklistSelected;
        public const string IsChecklistSelectedPropertyName = "IsChecklistSelected";

        public bool IsChecklistSelected
        {
            get => _isChecklistSelected;
            set => SetProperty(ref _isChecklistSelected, value, IsChecklistSelectedPropertyName);
        }

        public ImageSource ChecklistSelectedImage =>
            ImageResizer.ResizeImage(ImageConstants.ICON_CHECK_LIST_24x24, 24, 24);

        public ImageSource ChecklistDefaultImage =>
            ImageResizer.ResizeImage(ImageConstants.ICON_CHECK_LIST_LIGHT_24x24, 24, 24);

        private ImageSource _checklistImage;
        public const string ChecklistImagePropertyName = "ChecklistImage";

        public ImageSource ChecklistImage
        {
            get => _checklistImage;
            set => SetProperty(ref _checklistImage, value, ChecklistImagePropertyName);
        }

        public ImageSource SortSelectedImage => ImageResizer.ResizeImage(ImageConstants.ICON_SORT_24x24, 24, 24);
        public ImageSource SortDefaultImage => ImageResizer.ResizeImage(ImageConstants.ICON_SORT_LIGHT_24x24, 24, 24);

        private ImageSource _sortImage;
        public const string SortImagePropertyName = "SortImage";

        public ImageSource SortImage
        {
            get => _sortImage;
            set => SetProperty(ref _sortImage, value, SortImagePropertyName);
        }

        private string _sortBy;
        public const string SortByPropertyName = "SortBy";

        public string SortBy
        {
            get => _sortBy;
            set => SetProperty(ref _sortBy, value, SortByPropertyName);
        }

        private PlaylistSortList _playlistSortBy;
        public const string PlaylistSortByPropertyName = "PlaylistSortBy";

        public PlaylistSortList PlaylistSortBy
        {
            get => _playlistSortBy;
            set => SetProperty(ref _playlistSortBy, value, PlaylistSortByPropertyName);
        }

        private Style _playlistTextStyle;
        public const string PlaylistTextStylePropertyName = "PlaylistTextStyle";

        public Style PlaylistTextStyle
        {
            get => _playlistTextStyle;
            set => SetProperty(ref _playlistTextStyle, value, PlaylistTextStylePropertyName);
        }

        public Style PlaylistTextStyleDefault => (Style) App.CurrentApp.Resources["labelStyleInfoCheck"];
        public Style PlaylistTextStyleSelected => (Style) App.CurrentApp.Resources["labelStyleInfoCheckHighlight"];

        private ImageSize ButtonImageSize { get; set; }

        private void SetPageImageSize()
        {
            ButtonImageSize = App.Configuration.GetImageSizeByID(ImageIdentity.AUDIO_PLAYER_PAGE_COMMAND_IMAGE);
            if (ButtonImageSize != null)
            {
                AudioCommandImageHeight = ButtonImageSize.Height * 2;
                AudioCommandImageWidth = ButtonImageSize.Width * 2;
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
                PlayPauseButton = StopButton;
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
        
        private bool _isRefreshing;
        public const string IsRefreshingPropertyName = "IsRefreshing";

        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set { SetProperty(ref _isRefreshing, value, IsRefreshingPropertyName); }
        }

        public async Task PlayCurrent(int songIndex)
        {
            try
            {
                await Task.Run(() =>
                {
                    CurrentSongIndex = songIndex;
                    IsPlaying = true;
                    MusicFiles = MusicFiles.Select(m =>
                    {
                        m.IsPlayNow = false;
                        m.TextColor = Palette._LightGrayD;
                        return m;
                    }).ToList();

                    var currentMusicFile = MusicFiles[songIndex];
                    currentMusicFile.IsPlayNow = true;
                    currentMusicFile.TextColor = Palette._MainAccent;
                    CurrentMusicFile = currentMusicFile;
                    MediaTitle = CurrentMusicFile.Title;
                    _audioPlayerManager.CurrentPlayer.PlaySong(CurrentMusicFile);
                    CurrentPlayer = _audioPlayerManager.CurrentPlayer;
                    IsDurationLong = CurrentMusicFile.Duration > (60 * 60);
                    TimeStyle = IsDurationLong ? @"hh\:mm\:ss" : @"mm\:ss";
                    TotalTimer = TimeSpan.FromSeconds(CurrentMusicFile.Duration).ToString(TimeStyle);
                });
            }
            catch (Exception ex)
            {
                //Error
                _ = ex;
            }
        }

        private int Next()
        {
            if (MusicFiles.Count() > (CurrentSongIndex + 1))
                CurrentSongIndex++;
            else
                CurrentSongIndex = 0;
            return CurrentSongIndex;
        }

        private int Previous()
        {
            if (CurrentSongIndex > 0)
                CurrentSongIndex--;
            else
                CurrentSongIndex = MusicFiles.Count - 1;
            return CurrentSongIndex;
        }

        public List<MediaItem> AllMusicFiles { get; set; }
        public List<MediaItem> PlaylistMusicFiles { get; set; }

        private List<MediaItem> _musicFiles;
        public const string MusicFilesPropertyName = "MusicFiles";

        public List<MediaItem> MusicFiles
        {
            get { return _musicFiles; }
            set { SetProperty(ref _musicFiles, value, MusicFilesPropertyName); }
        }

        private MediaItem _currentMusicFile;
        public const string CurrentMusicFilePropertyName = "CurrentMusicFile";

        public MediaItem CurrentMusicFile
        {
            get { return _currentMusicFile; }
            set { SetProperty(ref _currentMusicFile, value, CurrentMusicFilePropertyName); }
        }

        private ICommand _refreshCommand;

        public ICommand RefreshCommand => _refreshCommand ?? (_refreshCommand = new Command(async (obj) =>
        {
            IsRefreshing = true;
            await SetSongs();
            IsRefreshing = false;
        }));

        private ICommand _playCommand;

        public ICommand PlayCommand => _playCommand ?? (_playCommand = new Command(async (obj) =>
        {
            if (MusicFiles.Count == 0)
                await SetSongs();
            if (MusicFiles.Count == 0) return;
            IsPlaying = IsPlaying == false;
            if (IsPlaying)
            {
                //if (IsPause)
                await PlayCurrent(CurrentSongIndex);
                //IsPause = false;
            }
            else
            {
                //IsPause = true;
                IsPlaying = false;
                StopAsync();
                CurrentTimer = TimeSpan.FromSeconds(0).ToString(TimeStyle);
                TimeSplitor = "";
                //CurrentPlayer.Pause();
            }
        }));

        private ICommand _stopCommand;
        public ICommand StopCommand => _stopCommand ?? (_stopCommand = new Command(() =>
        {
            IsPlaying = false;
            StopAsync();
            CurrentTimer = TimeSpan.FromSeconds(0).ToString(TimeStyle);
            TimeSplitor = "";
        }));

        public void StopAsync()
        {
            try
            {
                IsLoopStarted = false;
                CurrentPosition = 0;
                CurrentPlayer.Pause();
            }
            catch
            {
                // Commented
            }
        }

        private ICommand _nextCommand;
        public ICommand NextCommand => _nextCommand ?? (_nextCommand = new Command(async () => { await PlayCurrent(Next()); }));

        private ICommand _previousCommand;
        public ICommand PreviousCommand => _previousCommand ?? (_previousCommand = new Command(async () => { await PlayCurrent(Previous()); }));

        //private ICommand _forwardCommand;

        //public ICommand ForwardCommand
        //{
        //    get
        //    {
        //        return _forwardCommand ?? (_forwardCommand = new Command(async () =>
        //        {
        //            if (_audioPlayerManager.CurrentPlayer.Duration >
        //                _audioPlayerManager.CurrentPlayer.CurrentPosition + 10)
        //                _audioPlayerManager.CurrentPlayer.Seek(_audioPlayerManager.CurrentPlayer.CurrentPosition + 10);
        //            else
        //                await PlayCurrent(Next());
        //        }));
        //    }
        //}

        //private ICommand _backwardCommand;

        //public ICommand BackwardCommand
        //{
        //    get
        //    {
        //        return _backwardCommand ?? (_backwardCommand = new Command(() =>
        //        {
        //            if (_audioPlayerManager.CurrentPlayer.CurrentPosition - 10 > 0)
        //                _audioPlayerManager.CurrentPlayer.Seek(_audioPlayerManager.CurrentPlayer.CurrentPosition - 10);
        //            else
        //                _audioPlayerManager.CurrentPlayer.Seek(0);
        //        }));
        //    }
        //}

        private ICommand _checklistImageCommand;
        public ICommand ChecklistImageCommand => _checklistImageCommand ?? (_checklistImageCommand = new Command(() =>
        {
            IsChecklistSelected = !IsChecklistSelected;
            if (IsChecklistSelected)
            {
                ChecklistImage = ChecklistSelectedImage;
                PlaylistTextStyle = PlaylistTextStyleSelected;
                MusicFiles = new List<MediaItem>();
                MusicFiles = AllMusicFiles.Select(m =>
                {
                    m.IsPlayNow = false;
                    m.IsPlaylistSelected = PlaylistMusicFiles.Any(t =>
                        t.AlbumPersistentID == m.AlbumPersistentID && t.SongID == m.SongID &&
                        t.Title == m.Title && t.Album == m.Album && t.Artist == m.Artist);
                    m.TextColor = m.IsPlaylistSelected ? Palette._LightGrayD : Palette._ButtonBackgroundGray;
                    return m;
                }).ToList();
            }
            else
            {
                ChecklistImage = ChecklistDefaultImage;
                PlaylistTextStyle = PlaylistTextStyleDefault;
                MusicFiles = new List<MediaItem>();
                if (PlaylistMusicFiles.Count > 0)
                    MusicFiles = PlaylistMusicFiles;
                else
                    MusicFiles = AllMusicFiles.Select(m =>
                    {
                        m.IsPlayNow = false;
                        m.IsPlaylistSelected = true;
                        m.TextColor = m.IsPlaylistSelected ? Palette._LightGrayD : Palette._ButtonBackgroundGray;
                        return m;
                    }).ToList();
            }
        }));

        public Action DisplaySortByListAction { get; set; }
        public SortDirection SortDirect { get; set; }

        private ICommand _sortCommand;
        public ICommand SortCommand => _sortCommand ?? (_sortCommand = new Command(() => { DisplaySortByListAction?.Invoke(); }));

        public void SortOrderBy(PlaylistSortList sort)
        {
            switch (sort)
            {
                case PlaylistSortList.Album:
                    if (SortBy != PlaylistSortList.Album.ToString() || SortDirect != SortDirection.Asc)
                    {
                        MusicFiles = MusicFiles.OrderBy(m => m.Album).ToList();
                        SortDirect = SortDirection.Asc;
                    }
                    else
                    {
                        SortDirect = SortDirection.Desc;
                        MusicFiles = MusicFiles.OrderByDescending(m => m.Album).ToList();
                    }

                    SortBy = PlaylistSortList.Album.ToString();
                    break;
                case PlaylistSortList.Artist:
                    if (SortBy != PlaylistSortList.Artist.ToString() || SortDirect != SortDirection.Asc)
                    {
                        MusicFiles = MusicFiles.OrderBy(m => m.Artist).ToList();
                        SortDirect = SortDirection.Asc;
                    }
                    else
                    {
                        SortDirect = SortDirection.Desc;
                        MusicFiles = MusicFiles.OrderByDescending(m => m.Artist).ToList();
                    }

                    SortBy = PlaylistSortList.Artist.ToString();
                    break;
                case PlaylistSortList.Duration:
                    if (SortBy != PlaylistSortList.Duration.ToString() || SortDirect != SortDirection.Asc)
                    {
                        MusicFiles = MusicFiles.OrderBy(m => m.Duration).ToList();
                        SortDirect = SortDirection.Asc;
                    }
                    else
                    {
                        SortDirect = SortDirection.Desc;
                        MusicFiles = MusicFiles.OrderByDescending(m => m.Duration).ToList();
                    }

                    SortBy = PlaylistSortList.Duration.ToString();
                    break;
                default:
                    if (SortBy != PlaylistSortList.Title.ToString() || SortDirect != SortDirection.Asc)
                    {
                        MusicFiles = MusicFiles.OrderBy(m => m.Title).ToList();
                        SortDirect = SortDirection.Asc;
                    }
                    else
                    {
                        SortDirect = SortDirection.Desc;
                        MusicFiles = MusicFiles.OrderByDescending(m => m.Title).ToList();
                    }

                    SortBy = PlaylistSortList.Title.ToString();
                    break;
            }
        }
    }

    public enum PlaylistSortList
    {
        Title,
        Album,
        Artist,
        Duration
    }

    public enum SortDirection
    {
        Asc,
        Desc
    }
}