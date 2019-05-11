﻿// Loosely based on this guide: http://www.sagorin.org/ios-playing-audio-in-background-audio/
// and sample in Obj-C: https://github.com/jsagorin/iOSBackgroundAudio

using System;
using CoreGraphics;
using System.Collections.Generic;

using Foundation;
using UIKit;
using MediaPlayer;
using ObjCRuntime;

namespace com.organo.x4ever
{
    public partial class DetailViewController : UIViewController
    {
        #region - instance variables

        // Currently selected song
        public Song song { get; set; }

        // The Music Player
        public MyMusicPlayer musicPlayer { get; set; }

        // Current list of songs
        public List<Song> currentSongList { get; set; }
        public int currentSongIndex;
        public int currentSongCount;
        UIColor systemNavBarTintColor;

        #endregion

        #region - Constructors

        public DetailViewController(IntPtr handle) : base(handle)
        {
        }

        #endregion

        #region - View Controller overrides

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Create and initialize music player
            musicPlayer = MyMusicPlayer.GetInstance();
            musicPlayer.EndReached -= MusicPlayer_PlayNextSong;
            musicPlayer.StartReached -= MusicPlayer_PlayPrevSong;
            musicPlayer.ReadyToPlay -= MusicPlayer_EnablePlayPauseButton;
            musicPlayer.EndReached += MusicPlayer_PlayNextSong;
            musicPlayer.StartReached += MusicPlayer_PlayPrevSong;
            musicPlayer.ReadyToPlay += MusicPlayer_EnablePlayPauseButton;

            // set up handler for when app resumes from background
            NSNotificationCenter.DefaultCenter.AddObserver(this, new Selector("resumeFromBackground"),
                UIApplication.DidBecomeActiveNotification, null);
            // Register for receiving controls from lock screen and controlscreen
            UIApplication.SharedApplication.BeginReceivingRemoteControlEvents();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (song.streamingURL != null)
            {
                currentSongList = Songs.GetStreamingSongs();
                currentSongCount = currentSongList.Count;
                currentSongIndex = Songs.GetIndexOfStreamingSong(song);
            }
            else
            {
                if (Songs.searching)
                {
                    currentSongList = Songs.GetSearchedSongsByArtist(song.artist);
                    currentSongCount = currentSongList.Count;
                    currentSongIndex = Songs.GetSearchedIndexOfSongByArtist(song);
                }
                else
                {
                    currentSongList = Songs.GetSongsByArtist(song.artist);
                    currentSongCount = currentSongList.Count;
                    currentSongIndex = Songs.GetIndexOfSongByArtist(song);
                }
            }

            DisplaySongInfo();
            this.NavigationController.NavigationBar.BackgroundColor = UIColor.DarkGray;
            this.NavigationController.NavigationBar.BarTintColor = UIColor.DarkGray;
            systemNavBarTintColor = this.NavigationController.NavigationBar.TintColor;
            this.NavigationController.NavigationBar.TintColor = UIColor.LightGray;

            if (musicPlayer.currentSong == null)
            {
                actIndView.StartAnimating();
                actIndView.Hidden = false;
                playPause.UserInteractionEnabled = false;
                playPause.TintColor = UIColor.DarkGray;
            }

            if (song.streamingURL == null || musicPlayer.avPlayer.CurrentItem != null)
            {
                actIndView.Hidden = true;
                actIndView.StopAnimating();
                playPause.UserInteractionEnabled = true;
                playPause.TintColor = UIColor.Blue;
            }
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            // Play song (and load all songs by artist to player queue)
            if (musicPlayer.currentSong != song)
                musicPlayer.playSong(song);
            setPrevNextButtonStatus();
            setPlayPauseButtonStatus();

            this.BecomeFirstResponder();
        }

        void MusicPlayer_PlayNextSong(object sender, EventArgs e)
        {
            PlayNextSong();
        }

        void MusicPlayer_PlayPrevSong(object sender, EventArgs e)
        {
            PlayPrevSong();
        }

        void MusicPlayer_EnablePlayPauseButton(object sender, EventArgs e)
        {
            EnablePlayPauseButton();
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            // Unregister for control events
            UIApplication.SharedApplication.EndReceivingRemoteControlEvents();
            this.ResignFirstResponder();
            this.NavigationController.NavigationBar.BackgroundColor = UIColor.White;
            this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
            this.NavigationController.NavigationBar.TintColor = systemNavBarTintColor;
        }

        public override bool CanBecomeFirstResponder
        {
            get { return true; }
        }

        public override bool ShouldAutorotate()
        {
            return false;
        }

        #endregion

        #region - Handle events from outside the app

        [Export("resumeFromBackground")]
        public void resumeFromBackGround()
        {
            setPlayPauseButtonStatus();
            if (song.streamingURL == null)
            {
                currentSongIndex = Songs.GetIndexOfSongByArtist(song);
                DisplaySongInfo();
            }
        }

        // Forward remote control received events, from the lock or control screen, to the music player.
        public override void RemoteControlReceived(UIEvent theEvent)
        {
            base.RemoteControlReceived(theEvent);
            if (theEvent.Subtype == UIEventSubtype.RemoteControlPreviousTrack)
            {
                PlayPrevSong();
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
        partial void playPauseButtonTapped(UIButton sender)
        {
            if (musicPlayer.Rate > 0.0f)
            {
                musicPlayer.pause();
            }
            else
            {
                musicPlayer.play();
            }

            setPlayPauseButtonStatus();
        }

        partial void prevButtonTapped(UIButton sender)
        {
            PlayPrevSong();
        }

        partial void nextBtnTapped(UIButton sender)
        {
            PlayNextSong();
        }

        #endregion

        #region - Class helper methods

        public void PlayPrevSong()
        {
            if (currentSongIndex > 0)
            {
                currentSongIndex--;
                song = currentSongList[currentSongIndex];
                musicPlayer.playSong(song);
                DisplaySongInfo();
                if (song.streamingURL != null)
                {
                    disableAllButtons();
                }
            }
        }

        public void PlayNextSong()
        {
            if (currentSongIndex + 1 < currentSongCount)
            {
                currentSongIndex++;
                song = currentSongList[currentSongIndex];
                musicPlayer.playSong(song);
                DisplaySongInfo();
                if (song.streamingURL != null)
                {
                    disableAllButtons();
                }
            }
        }

        public void DisplaySongInfo()
        {
            // Display info for current song as might've changed
            artistNameLabel.Text = song.artist;
            albumNameLabel.Text = song.album;
            songTitleLabel.Text = song.song;
            songIdLabel.Text = song.streamingURL == null ? song.songID.ToString() : song.streamingURL;
            if (song.artwork != null)
                artworkView.Image = song.artwork.ImageWithSize(new CGSize(115.0f, 115.0f));
            this.NavigationItem.Title =
                String.Format("Playing song {0} of {1}", currentSongIndex + 1, currentSongCount);
            setPrevNextButtonStatus();
            if (musicPlayer != null)
            {
                playPause.SetTitle(musicPlayer.Rate > 0.0f ? "Pause" : "Play", UIControlState.Normal);
            }
        }

        void setPrevNextButtonStatus()
        {
            if (currentSongIndex == 0)
            {
                prevBtn.UserInteractionEnabled = false;
                prevBtn.TintColor = UIColor.DarkGray;
            }
            else
            {
                prevBtn.UserInteractionEnabled = true;
                prevBtn.TintColor = UIColor.Blue;
            }

            if (currentSongIndex + 1 == currentSongCount)
            {
                nextBtn.UserInteractionEnabled = false;
                nextBtn.TintColor = UIColor.DarkGray;
            }
            else
            {
                nextBtn.UserInteractionEnabled = true;
                nextBtn.TintColor = UIColor.Blue;
            }

        }

        void setPlayPauseButtonStatus()
        {
            if (musicPlayer.Rate > 0.0f)
            {
                playPause.SetTitle("Pause", UIControlState.Normal);
            }
            else
            {
                playPause.SetTitle("Play", UIControlState.Normal);
            }
        }

        public void EnablePlayPauseButton()
        {
            playPause.UserInteractionEnabled = true;
            playPause.TintColor = UIColor.Blue;
            playPause.SetTitle("Pause", UIControlState.Normal);
            actIndView.StopAnimating();
            actIndView.Hidden = true;
            setPrevNextButtonStatus();
        }

        void disableAllButtons()
        {
            prevBtn.UserInteractionEnabled = false;
            prevBtn.TintColor = UIColor.DarkGray;
            nextBtn.UserInteractionEnabled = false;
            nextBtn.TintColor = UIColor.DarkGray;
            actIndView.StartAnimating();
            actIndView.Hidden = false;
            playPause.UserInteractionEnabled = false;
            playPause.TintColor = UIColor.DarkGray;

        }

        #endregion
    }
}