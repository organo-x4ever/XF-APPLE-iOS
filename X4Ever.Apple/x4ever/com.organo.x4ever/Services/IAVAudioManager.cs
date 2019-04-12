using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.organo.x4ever.Services
{
    public interface IAVAudioManager
    {
        #region Computed Properties

        float AVMusicVolume { get; set; }
        bool MusicOn { get; set; }
        float MusicVolume { get; set; }

        bool EffectsOn { get; set; }
        float EffectsVolume { get; set; }

        string FileType { get; set; }

        #endregion

        #region Public Methods

        void ActivateAudioSession();

        void DeactivateAudioSession();

        void ReactivateAudioSession();

        void PlayMusic(string filename);

        void StopMusic();

        void SuspendMusic();

        void RestartMusic();

        void PlaySound(string filename);

        #endregion
    }
}