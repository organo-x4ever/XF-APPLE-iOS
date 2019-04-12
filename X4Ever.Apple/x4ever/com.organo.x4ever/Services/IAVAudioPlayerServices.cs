using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.organo.x4ever.Services
{
    public interface IAVAudioPlayerServices
    {
        /// <summary>
        /// The total duration, in seconds, of the sound associated with the audio player.
        /// </summary>
        double Duration { get; }

        /// <summary>
        /// The playback point, in seconds, within the timeline of the sound associated with the audio player.
        /// </summary>
        double CurrentPosition { get; }

        /// <summary>
        /// The time value, in seconds, of the audio output device.
        /// </summary>
        double DeviceCurrentTime { get; set; }

        /// <summary>
        /// The UID of the current audio player.
        /// </summary>
        string CurrentDevice { get; set; }

        /// <summary>
        /// The playback volume for the audio player, ranging from 0.0 through 1.0 on a linear scale.
        /// </summary>
        float Volume { get; set; }

        /// <summary>
        /// The number of times a sound will return to the beginning, upon reaching the end, to repeat playback.
        /// </summary>
        short NumberOfRepeats { get; set; }

        /// <summary>
        /// The delegate object for the audio player.
        /// </summary>
        object Delegate { get; set; }

        /// <summary>
        /// A protocol that allows a delegate to respond to audio interruptions and audio decoding errors, and to the completion of a sound’s playback.
        /// </summary>
        object AVAudioPlayerDelegate { get; set; }

        /// <summary>
        /// A Boolean value that specifies the audio-level metering on/off state for the audio player.
        /// </summary>
        bool CanSeek { get; }

        /// <summary>
        /// The URL for the sound associated with the audio player.
        /// </summary>
        string Url { get; set; }

        /// <summary>
        /// The audio player’s settings dictionary, containing information about the sound associated with the player.
        /// </summary>
        object Settings { get; set; }

        /// <summary>
        /// A Boolean value that indicates whether the audio player is playing (YES) or not (NO).
        /// </summary>
        bool IsPlaying { get; }

        /// <summary>
        /// Plays a sound asynchronously.
        /// </summary>
        void Play();

        /// <summary>
        /// Plays a sound asynchronously, starting at a specified point in the audio output device’s timeline.
        /// </summary>
        void PlayAtTime();

        /// <summary>
        /// Stops playback and undoes the setup needed for playback.
        /// </summary>
        void Stop();

        /// <summary>
        /// Pauses playback; sound remains ready to resume playback from where it left off.
        /// </summary>
        void Pause();

        /// <summary>
        /// Prepares the audio player for playback by preloading its buffers.
        /// </summary>
        void PrepareToPlay();

        /// <summary>
        /// Fades to a new volume over a specific duration.
        /// </summary>
        void SetVolume();
    }
}