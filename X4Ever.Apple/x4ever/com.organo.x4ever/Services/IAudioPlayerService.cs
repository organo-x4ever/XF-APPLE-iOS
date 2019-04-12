﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.organo.x4ever.Services
{
    public interface IAudioPlayerService
    {
        int Duration { get; }
        int CurrentPosition { get; }
        float Volume { get; set; }
        bool CanSeek { get; }
        bool IsPlaying { get; set; }
        string PlayNow { get; set; }

        void SetVolume(float volume);

        void Play(string pathToAudioFile);

        void Play();

        void Pause();

        void Stop();

        void SeekTo(int seekValue);

        int GetTrackDuration(string pathToAudioFile);

        Action OnFinishedPlaying { get; set; }
    }
}