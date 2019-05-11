﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.IO;

namespace com.organo.x4ever
{
    /// <summary>
    /// Interface for AudioPlayer
    /// </summary>
    public interface IAudioPlayer : IDisposable
    {

        #region Player

        string ErrorMessage { get; set; }
        float Rate { get; }
        string ArtistName { get; set; }
        string AlbumName { get; set; }
        string SongTitle { get; set; }
        Uri SongURL { get; set; }
        ImageSource Image { get; set; }
        string SongDetail { get; set; }

        Action PlayNextAction { get; set; }
        Action PlayPreviousAction { get; set; }
        Action ReadyToPlayAction { get; set; }
        void PlaySong(MediaItem song);
        void Pause();
        void Play();




        #endregion Player


        /////<Summary>
        ///// Raised when audio playback completes successfully 
        /////</Summary>
        //event EventHandler PlaybackEnded;

        ///<Summary>
        /// Length of audio in seconds
        ///</Summary>
        double Duration { get; }

        ///<Summary>
        /// Current position of audio playback in seconds
        ///</Summary>
        double CurrentPosition { get; }

        /////<Summary>
        ///// Playback volume 0 to 1 where 0 is no-sound and 1 is full volume
        /////</Summary>
        //double Volume { get; set; }

        /////<Summary>
        ///// Balance left/right: -1 is 100% left : 0% right, 1 is 100% right : 0% left, 0 is equal volume left/right
        /////</Summary>
        //double Balance { get; set; }

        ///<Summary>
        /// Indicates if the currently loaded audio file is playing
        ///</Summary>
        bool IsPlaying { get; }

        /////<Summary>
        ///// Continously repeats the currently playing sound
        /////</Summary>
        //bool Loop { get; set; }

        /////<Summary>
        ///// Indicates if the position of the loaded audio file can be updated
        /////</Summary>
        //bool CanSeek { get; }

        /////<Summary>
        ///// Load wav or mp3 audio file as a stream
        /////</Summary>
        //bool Load(Stream audioStream);

        /////<Summary>
        ///// Load wav or mp3 audio file from local path
        /////</Summary>
        //bool Load(string fileName, bool fullPath);

        /////<Summary>
        ///// Begin playback or resume if paused
        /////</Summary>
        //void Play();

        /////<Summary>
        ///// Pause playback if playing (does not resume)
        /////</Summary>
        //void Pause();

        /////<Summary>
        ///// Stop playack and set the current position to the beginning
        /////</Summary>
        //void Stop();

        /////<Summary>
        ///// Set the current playback position (in seconds)
        /////</Summary>
        //void Seek(double position);
    }
}