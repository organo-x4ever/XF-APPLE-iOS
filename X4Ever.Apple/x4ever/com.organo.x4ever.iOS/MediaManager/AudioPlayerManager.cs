using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.organo.x4ever;
using com.organo.x4ever.MediaManager;
using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.Forms;

[assembly:Dependency(typeof(AudioPlayerManager))]
namespace com.organo.x4ever
{
    public class AudioPlayerManager : IAudioPlayerManager
    {
        static Lazy<IAudioPlayer> Implementation = new Lazy<IAudioPlayer>(() => CreateAudioPlayer(),
            System.Threading.LazyThreadSafetyMode.PublicationOnly);

        private static bool Initialzed { get; set; } = false;

        private IAudioPlayer Current
        {
            get
            {
                var ret = Implementation.Value;
                if (ret == null)
                {
                    throw NotImplementedInReferenceAssembly();
                }

                Initialize();
                return ret;
            }
        }

        private void Initialize()
        {
            if (!Initialzed || musicPlayer == null)
            {
                Song = new MediaItem();
                CurrentSongList = new List<MediaItem>();
                Initialzed = true;
                // Create and initialize music player
                musicPlayer = AudioPlayerImplementation.GetInstance();
                musicPlayer.EndReached -= MusicPlayer_PlayNextSong;
                musicPlayer.StartReached -= MusicPlayer_PlayPrevSong;
                musicPlayer.ReadyToPlay -= MusicPlayer_EnablePlayPauseButton;
                musicPlayer.EndReached += MusicPlayer_PlayNextSong;
                musicPlayer.StartReached += MusicPlayer_PlayPrevSong;
                musicPlayer.ReadyToPlay += MusicPlayer_EnablePlayPauseButton;
            }
        }

        public IAudioPlayer CurrentPlayer => Current;

        private AudioPlayerImplementation musicPlayer;

        private const string messageNoStream = "No Stream for current file";

        

        #region - instance variables

        // Currently selected song
        public MediaItem Song { get; set; }

        // Current list of songs
        private List<MediaItem> CurrentSongList { get; set; }
        public int CurrentSongIndex;
        public int CurrentSongCount;

        #endregion

        public void Load()
        {
            var audioManager = CurrentPlayer;
            //base.ViewDidAppear(animated);
            // Play song (and load all songs by artist to player queue)
            if (musicPlayer.CurrentSong != Song)
                musicPlayer.PlaySong(Song);
            //SetPrevNextButtonStatus();
            //SetPlayPauseButtonStatus();

            //this.BecomeFirstResponder();
        }

        void MusicPlayer_PlayNextSong(object sender, EventArgs e)
        {
            PlayNextSong();
            Current.PlayNextAction();
        }

        void MusicPlayer_PlayPrevSong(object sender, EventArgs e)
        {
            PlayPreviousSong();
            Current.PlayPreviousAction();
        }

        void MusicPlayer_EnablePlayPauseButton(object sender, EventArgs e)
        {

            Current.ReadyToPlayAction();
        }

        //public void ViewWillDisappear(bool animated)
        //{
        //    //base.ViewWillDisappear(animated);

        //    // Unregister for control events
        //    UIApplication.SharedApplication.EndReceivingRemoteControlEvents();
        //    //this.ResignFirstResponder();
        //    //this.NavigationController.NavigationBar.BackgroundColor = UIColor.White;
        //    //this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
        //    //this.NavigationController.NavigationBar.TintColor = systemNavBarTintColor;
        //}

        public bool CanBecomeFirstResponder
        {
            get { return true; }
        }

        public bool ShouldAutorotate()
        {
            return false;
        }

        #region - Handle events from outside the app

        [Export("resumeFromBackground")]
        public void resumeFromBackGround()
        {
            CurrentSongIndex = CurrentSongList.FindIndex(s => s.ArtistPersistentID == Song.ArtistPersistentID);
            DisplaySongInfo();
        }

        // Forward remote control received events, from the lock or control screen, to the music player.
        public void RemoteControlReceived(UIEvent theEvent)
        {
            if (theEvent.Subtype == UIEventSubtype.RemoteControlPreviousTrack)
            {
                PlayPreviousSong();
            }
            else if (theEvent.Subtype == UIEventSubtype.RemoteControlNextTrack)
            {
                PlayNextSong();
            }
            else
            {
                musicPlayer.RemoteControlReceived(theEvent);
            }
        }

        #endregion

        #region - handle in-app playback controls

        // In app play/pause button clicked
        public void PlayPause()
        {
            if (CurrentPlayer.Rate > 0.0f)
            {
                CurrentPlayer.Pause();
            }
            else
            {
                CurrentPlayer.Play();
            }
        }

        #endregion

        #region - Class helper methods

        public void PlayPreviousSong()
        {
            Current.ErrorMessage = string.Empty;
            if (CurrentSongIndex > 0)
            {
                CurrentSongIndex--;
                Song = CurrentSongList[CurrentSongIndex];
                musicPlayer.PlaySong(Song);
                DisplaySongInfo();
                if (Song.SongURL != null)
                {
                    Current.ErrorMessage = messageNoStream;
                }
            }
        }

        public void PlayNextSong()
        {
            Current.ErrorMessage = string.Empty;
            if (CurrentSongIndex + 1 < CurrentSongCount)
            {
                CurrentSongIndex++;
                Song = CurrentSongList[CurrentSongIndex];
                musicPlayer.PlaySong(Song);
                DisplaySongInfo();
                if (Song.SongURL != null)
                {
                    Current.ErrorMessage = messageNoStream;
                }
            }
        }

        public void DisplaySongInfo()
        {
            MusicQuery query = new MusicQuery();
            // Display info for current song as might've changed
            Current.ArtistName = Song.Artist;
            Current.ArtistName = Song.Album;
            Current.SongTitle = Song.Title;
            Current.SongURL = Song.SongURL;
            if (Song.Artwork != null)
                Current.Image = Song.Artwork;
            Current.SongDetail = String.Format($"Playing song {CurrentSongIndex + 1} of {CurrentSongCount}");
        }

        //void setPrevNextButtonStatus()
        //{
        //if (CurrentSongIndex == 0)
        //{
        //    PrevBtn.UserInteractionEnabled = false;
        //    PrevBtn.TintColor = UIColor.DarkGray;
        //}
        //else
        //{
        //    PrevBtn.UserInteractionEnabled = true;
        //    PrevBtn.TintColor = UIColor.Blue;
        //}

        //if (CurrentSongIndex + 1 == CurrentSongCount)
        //{
        //    NextBtn.UserInteractionEnabled = false;
        //    NextBtn.TintColor = UIColor.DarkGray;
        //}
        //else
        //{
        //    NextBtn.UserInteractionEnabled = true;
        //    NextBtn.TintColor = UIColor.Blue;
        //}
        //}

        #endregion




















        ///<Summary>
        /// Create a new AudioPlayer object
        ///</Summary>
        public static IAudioPlayer CreateAudioPlayer()
        {
#if NETSTANDARD1_0
            return null;
#else
            return new AudioPlayerImplementation();
#endif
        }

        internal static Exception NotImplementedInReferenceAssembly()
        {
            return new NotImplementedException(
                "This functionality is not implemented in the .NET standard version of this assembly. Reference the NuGet package from your platform-specific (head) application project in order to reference the platform-specific implementation.");
        }
    }
}