﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;


using AVFoundation;
using AudioToolbox;
using MediaPlayer;
using CoreMedia;
using CoreFoundation;
using System.Net;
using System.IO;
using ObjCRuntime;
using System.Diagnostics;

namespace com.organo.x4ever
{
    public class MyMusicPlayer : NSObject
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

        public AVPlayer avPlayer { get; private set; }
        float SEEK_RATE = 10.0f;
        AVPlayerItem item;
        AVPlayerItem streamingItem;
        bool streamingItemPaused;
        static MyMusicPlayer myMusicPlayer;
        NSObject timeObserver;

        #endregion

        #region - Public properties

        public Song currentSong { get; set; }

        public float Rate
        {
            get { return avPlayer != null ? avPlayer.Rate : 0.0f; }
        }

        #endregion

        #region - Constructors

        private MyMusicPlayer()
        {
            initSession();
        }

        public static MyMusicPlayer GetInstance()
        {
            if (MyMusicPlayer.myMusicPlayer == null)
                MyMusicPlayer.myMusicPlayer = new MyMusicPlayer();
            return MyMusicPlayer.myMusicPlayer;
        }

        // Initialize audio session
        void initSession()
        {
            streamingItemPaused = false;

            avPlayer = new AVPlayer();
            AVAudioSession avSession = AVAudioSession.SharedInstance();

            avSession.SetCategory(AVAudioSessionCategory.Playback);

            NSError activationError = null;
            avSession.SetActive(true, out activationError);
            if (activationError != null)
                Console.WriteLine("Could not activate audio session {0}", activationError.LocalizedDescription);
            avPlayer.ActionAtItemEnd = AVPlayerActionAtItemEnd.None;
            timeObserver =
                avPlayer.AddPeriodicTimeObserver(CMTime.FromSeconds(5.0, 1), DispatchQueue.MainQueue, ObserveTime);
        }

        public void ObserveTime(CMTime time)
        {
            Console.WriteLine("Seconds: {0}, Value: {1}", time.Seconds, time.Value);

            EventArgs args = new EventArgs();
            if (time.Seconds >= avPlayer.CurrentItem.Duration.Seconds - 1.0)
            {
                OnEndReached(args);
            }
            else if (avPlayer.Rate > 1.0f && time.Seconds >= avPlayer.CurrentItem.Duration.Seconds - 6.0)
            {
                avPlayer.Rate = 1.0f;
                OnEndReached(args);
            }
            else if (avPlayer.Rate < 0 && time.Seconds <= 6.0)
            {
                avPlayer.Rate = 1.0f;
                OnStartReached(args);
            }
        }

        #endregion

        #region - Public methods

        // Play song from persistentSongID
        public void playSong(Song song)
        {
            currentSong = song;
            if (song.streamingURL == null)
            {
                MusicQuery musicQuery = new MusicQuery();
                MPMediaItem mediaItem = musicQuery.queryForSongWithId(song.songID);
                if (mediaItem != null)
                {
                    NSUrl Url = mediaItem.AssetURL;
                    item = AVPlayerItem.FromUrl(Url);
                    if (item != null)
                    {
                        this.avPlayer.ReplaceCurrentItemWithPlayerItem(item);
                    }

                    MPNowPlayingInfo np = new MPNowPlayingInfo();
                    SetNowPlayingInfo(song, np);
                    this.play();
                }
            }
            else
            {
                NSUrl nsUrl = NSUrl.FromString(song.streamingURL);
                MyMusicPlayer.myMusicPlayer?.streamingItem?.RemoveObserver(MyMusicPlayer.myMusicPlayer, "status");
                streamingItem = AVPlayerItem.FromUrl(nsUrl);
                streamingItem.AddObserver(this, new NSString("status"), NSKeyValueObservingOptions.New,
                    avPlayer.Handle);
                avPlayer.ReplaceCurrentItemWithPlayerItem(streamingItem);
                streamingItemPaused = false;
                //NSNotificationCenter.DefaultCenter.AddObserver(this, new Selector("playerItemDidReachEnd:"), AVPlayerItem.DidPlayToEndTimeNotification, streamingItem);
            }
        }

        public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
        {
            if (keyPath.ToString() == "status")
            {
                Console.WriteLine("Status Observed Method {0}", avPlayer.Status);
                if (avPlayer.Status == AVPlayerStatus.ReadyToPlay)
                {
                    if (currentSong != null)
                    {
                        currentSong.duration = streamingItem.Duration.Seconds;
                        MPNowPlayingInfo np = new MPNowPlayingInfo();
                        SetNowPlayingInfo(currentSong, np);
                        if (!streamingItemPaused)
                            this.play();

                        OnReadyToPlay(new EventArgs());
                    }
                }
                else if (avPlayer.Status == AVPlayerStatus.Failed)
                {
                    Console.WriteLine("Stream Failed");
                }
            }
        }

        public void pause()
        {
            if (currentSong.streamingURL != null)
                streamingItemPaused = true;
            this.avPlayer.Pause();
        }

        public void play()
        {
            if (currentSong.streamingURL != null)
                streamingItemPaused = false;
            this.avPlayer.Play();
        }

        // Handle control events from lock or control screen
        public void RemoteControlReceived(UIEvent theEvent)
        {
            MPNowPlayingInfo np = new MPNowPlayingInfo();
            if (theEvent.Subtype == UIEventSubtype.RemoteControlPause)
            {
                pause();
            }
            else if (theEvent.Subtype == UIEventSubtype.RemoteControlPlay)
            {
                play();
            }
            else if (theEvent.Subtype == UIEventSubtype.RemoteControlBeginSeekingForward)
            {
                avPlayer.Rate = SEEK_RATE;
                np.PlaybackRate = SEEK_RATE;
            }
            else if (theEvent.Subtype == UIEventSubtype.RemoteControlEndSeekingForward)
            {
                avPlayer.Rate = 1.0f;
                np.PlaybackRate = 1.0f;
            }
            else if (theEvent.Subtype == UIEventSubtype.RemoteControlBeginSeekingBackward)
            {
                avPlayer.Rate = -SEEK_RATE;
                np.PlaybackRate = -SEEK_RATE;
            }
            else if (theEvent.Subtype == UIEventSubtype.RemoteControlEndSeekingBackward)
            {
                avPlayer.Rate = 1.0f;
                np.PlaybackRate = 1.0f;
            }

            np.ElapsedPlaybackTime = avPlayer.CurrentTime.Seconds;
            SetNowPlayingInfo(currentSong, np);
        }

        #endregion

        #region - Helper methods

        void SetNowPlayingInfo(Song song, MPNowPlayingInfo np)
        {
            // Pass song info to the lockscreen/control screen
            np.AlbumTitle = song.album;
            np.Artist = song.artist;
            np.Title = song.song;
            if (streamingItem != null)
                np.PersistentID = song.songID;
            if (song.artwork != null)
                np.Artwork = song.artwork;
            np.PlaybackDuration = song.duration;
            MPNowPlayingInfoCenter.DefaultCenter.NowPlaying = np;
        }

        #endregion
    }
}