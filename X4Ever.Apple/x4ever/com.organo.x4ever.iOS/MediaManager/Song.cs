using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using MediaPlayer;
using UIKit;

namespace com.organo.x4ever
{
    public class Song
    {
        public string artist { get; set; }
        public string album { get; set; }
        public string song { get; set; }
        public string streamingURL { get; set; }
        public ulong songID { get; set; }
        public double duration { get; set; }
        public MPMediaItemArtwork artwork { get; set; }

        public Song()
        {
        }
    }
}