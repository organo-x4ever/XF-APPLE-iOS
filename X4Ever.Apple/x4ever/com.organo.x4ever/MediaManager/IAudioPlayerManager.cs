using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.organo.x4ever.MediaManager
{
    public interface IAudioPlayerManager
    {
        IAudioPlayer CurrentPlayer { get; }
        void PlayPreviousSong();
        void PlayNextSong();
        void Load();
    }
}