using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Xamarin.Forms;
using ObjCRuntime;
using AVFoundation;
using MediaPlayer;

using AudioToolbox;
using CoreMedia;
using CoreFoundation;
using System.Net;
using System.IO;
using System.Diagnostics;
using com.organo.x4ever;

[assembly:Dependency(typeof(AudioPlayerImplementation))]
namespace com.organo.x4ever
{
    public class AudioPlayerImplementation : NSObject, IAudioPlayer
    {
        #region - EventHandlers

        public event EventHandler EndReached;
        public event EventHandler StartReached;
        public event EventHandler ReadyToPlay;

        protected virtual void OnEndReached(EventArgs e)
        {
            EventHandler handler = EndReached;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnStartReached(EventArgs e)
        {
            EventHandler handler = StartReached;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnReadyToPlay(EventArgs e)
        {
            EventHandler handler = ReadyToPlay;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region - Private instance variables

        public AVPlayer Player { get; private set; }
        private float SEEK_RATE = 10.0f;
        private AVPlayerItem item;
        private AVPlayerItem streamingItem;
        public bool StreamingItemPaused { get; set; }
        static AudioPlayerImplementation myMusicPlayer;
        private NSObject timeObserver;
        private IMusicDictionary _musicDictionary;

        #endregion

        #region - Public properties

        public MediaItem CurrentSong { get; set; }

        public float Rate
        {
            get { return Player != null ? Player.Rate : 0.0f; }
        }

        
        private ImageSource _image;

        public ImageSource Image
        {
            get => _image;
            set => _image = value;
        }

        private bool _isPlaying;

        public bool IsPlaying
        {
            get => !StreamingItemPaused; //Player != null ? (Player.CurrentTime.Seconds > 0) : false;
        }

        private string _errorMessage;

        public string ErrorMessage
        {
            get => _errorMessage;
            set => _errorMessage = value;
        }

        public double Duration => CurrentSong.Duration; //Player.CurrentItem.Duration.Seconds;

        public double CurrentPosition => Player.CurrentTime.Seconds;

        #endregion

        #region - Constructors

        public AudioPlayerImplementation()
        {
            _musicDictionary = DependencyService.Get<IMusicDictionary>();
            initSession();
        }

        // Initialize audio session
        void initSession()
        {
            StreamingItemPaused = false;

            Player = new AVPlayer();
            AVAudioSession avSession = AVAudioSession.SharedInstance();

            avSession.SetCategory(AVAudioSessionCategory.Playback);

            NSError activationError = null;
            avSession.SetActive(true, out activationError);
            if (activationError != null)
                Console.WriteLine("Could not activate audio session {0}", activationError.LocalizedDescription);
            Player.ActionAtItemEnd = AVPlayerActionAtItemEnd.None;
            timeObserver =
                Player.AddPeriodicTimeObserver(CMTime.FromSeconds(5.0, 1), DispatchQueue.MainQueue, ObserveTime);
        }

        public void ObserveTime(CMTime time)
        {
            Console.WriteLine("Seconds: {0}, Value: {1}", time.Seconds, time.Value);

            EventArgs args = new EventArgs();
            if (time.Seconds >= Player.CurrentItem.Duration.Seconds - 1.0)
            {
                OnEndReached(args);
            }
            else if (Player.Rate > 1.0f && time.Seconds >= Player.CurrentItem.Duration.Seconds - 6.0)
            {
                Player.Rate = 1.0f;
                OnEndReached(args);
            }
            else if (Player.Rate < 0 && time.Seconds <= 6.0)
            {
                Player.Rate = 1.0f;
                OnStartReached(args);
            }
        }

        #endregion

        #region - Public methods

        // Play song from persistentSongID
        public void PlaySong(MediaItem song)
        {
            CurrentSong = song;

            if (song != null)
            {
                NSUrl Url = song.SongURL;
                item = AVPlayerItem.FromUrl(Url);
                if (item != null)
                {
                    this.Player.ReplaceCurrentItemWithPlayerItem(item);
                }

                MPNowPlayingInfo np = new MPNowPlayingInfo();
                SetNowPlayingInfo(song, np);
                this.Play();

                AudioPlayerImplementation.myMusicPlayer?.streamingItem?.RemoveObserver(
                    AudioPlayerImplementation.myMusicPlayer, "status");
                streamingItem = AVPlayerItem.FromUrl(Url);
                streamingItem.AddObserver(this, new NSString("status"), NSKeyValueObservingOptions.New,
                    Player.Handle);
                Player.ReplaceCurrentItemWithPlayerItem(streamingItem);
                StreamingItemPaused = false;

                // WAS COMMENTED
                NSNotificationCenter.DefaultCenter.AddObserver(this, new Selector("playerItemDidReachEnd:"),
                    AVPlayerItem.DidPlayToEndTimeNotification, streamingItem);
            }
        }

        public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
        {
            if (keyPath.ToString() == "status")
            {
                Console.WriteLine("Status Observed Method {0}", Player.Status);
                if (Player.Status == AVPlayerStatus.ReadyToPlay)
                {
                    if (CurrentSong != null)
                    {
                        CurrentSong.Duration = streamingItem.Duration.Seconds;
                        MPNowPlayingInfo np = new MPNowPlayingInfo();
                        SetNowPlayingInfo(CurrentSong, np);
                        if (!StreamingItemPaused)
                            this.Play();

                        OnReadyToPlay(new EventArgs());
                    }
                }
                else if (Player.Status == AVPlayerStatus.Failed)
                {
                    Console.WriteLine("Stream Failed");
                }
            }
        }

        public void Pause()
        {
            StreamingItemPaused = true;
            this.Player.Pause();
        }

        public void Play()
        {
            StreamingItemPaused = false;
            this.Player.Play();
        }

        // Handle control events from lock or control screen
        public void RemoteControlReceived(UIEvent theEvent)
        {
            MPNowPlayingInfo np = new MPNowPlayingInfo();
            if (theEvent.Subtype == UIEventSubtype.RemoteControlPause)
            {
                Pause();
            }
            else if (theEvent.Subtype == UIEventSubtype.RemoteControlPlay)
            {
                Play();
            }
            else if (theEvent.Subtype == UIEventSubtype.RemoteControlBeginSeekingForward)
            {
                Player.Rate = SEEK_RATE;
                np.PlaybackRate = SEEK_RATE;
            }
            else if (theEvent.Subtype == UIEventSubtype.RemoteControlEndSeekingForward)
            {
                Player.Rate = 1.0f;
                np.PlaybackRate = 1.0f;
            }
            else if (theEvent.Subtype == UIEventSubtype.RemoteControlBeginSeekingBackward)
            {
                Player.Rate = -SEEK_RATE;
                np.PlaybackRate = -SEEK_RATE;
            }
            else if (theEvent.Subtype == UIEventSubtype.RemoteControlEndSeekingBackward)
            {
                Player.Rate = 1.0f;
                np.PlaybackRate = 1.0f;
            }

            np.ElapsedPlaybackTime = Player.CurrentTime.Seconds;
            SetNowPlayingInfo(CurrentSong, np);
        }

        #endregion

        #region - Helper methods

        void SetNowPlayingInfo(MediaItem song, MPNowPlayingInfo np)
        {
            MusicQuery query = new MusicQuery();
            // Pass song info to the lockscreen/control screen
            np.AlbumTitle = song.Album;
            np.Artist = song.Artist;
            np.Title = song.Title;
            if (streamingItem != null)
                np.PersistentID = song.SongID;
            if (song.Artwork != null)
                np.Artwork = query.queryForSongWithId(song.SongID)?.Artwork;
            np.PlaybackDuration = song.Duration;
            MPNowPlayingInfoCenter.DefaultCenter.NowPlaying = np;
        }

        #endregion

        bool isDisposed = false;

        ///<Summary>
        /// Dispose AudioPlayer and release resources
        ///</Summary>
        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed || Player == null)
                return;

            //if (disposing)
            //    DeletePlayer();

            isDisposed = true;
        }

        ~AudioPlayerImplementation()
        {
            Dispose(false);
        }

        ///<Summary>
        /// Dispose AudioPlayer and release resources
        ///</Summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }
    }
}