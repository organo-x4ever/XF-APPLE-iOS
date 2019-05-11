
using com.organo.x4ever.Statics;
using System;
using Xamarin.Forms;

namespace com.organo.x4ever
{
    public class MediaItem
    {
        public string Artist { get; set; }

        // AlbumTitle
        public string Album { get; set; }

        // AssetURL
        public Uri SongURL { get; set; }

        // PersistentID
        public ulong SongID { get; set; }

        // PlaybackDuration
        public double Duration { get; set; }
        public ImageSource Artwork { get; set; }
        public string AlbumArtist { get; set; }
        public ulong AlbumArtistPersistentID { get; set; }
        public ulong AlbumPersistentID { get; set; }
        public int AlbumTrackCount { get; set; }
        public int AlbumTrackNumber { get; set; }
        public ulong ArtistPersistentID { get; set; }
        public Uri AssetURL { get; set; }
        public uint BeatsPerMinute { get; set; }
        public double BookmarkTime { get; set; }
        public string Comments { get; set; }
        public string Composer { get; set; }
        public ulong ComposerPersistentID { get; set; }
        public DateTime DateAdded { get; set; }
        public int DiscCount { get; set; }
        public int DiscNumber { get; set; }
        public string Genre { get; set; }
        public ulong GenrePersistentID { get; set; }
        public bool HasProtectedAsset { get; set; }
        public bool IsCloudItem { get; set; }
        public bool IsCompilation { get; set; }
        public bool IsExplicitItem { get; set; }
        public DateTime LastPlayedDate { get; set; }
        public string Lyrics { get; set; }
        public string MediaType { get; set; }
        public int PlayCount { get; set; }
        public string PlaybackStoreID { get; set; }
        public ulong PodcastPersistentID { get; set; }
        public string PodcastTitle { get; set; }
        public uint Rating { get; set; }
        public DateTime ReleaseDate { get; set; }

        public int SkipCount { get; set; }

        // Song
        public string Title { get; set; }
        public string UserGrouping { get; set; }


        public double _Duration { get; set; }
        public DateTime _Date { get; set; }
        public int _Track { get; set; }
        public long _Year { get; set; }
        public string _DurationTimeSpan { get; set; }
        public bool IsPlayNow { get; set; }
        public Color TextColor { get; set; }
        public bool IsPlaylistSelected { get; set; }

        public MediaItem()
        {
            Artist = string.Empty;
            Album = string.Empty;
            SongURL = null;
            SongID = 0;
            Duration = 0;
            Artwork = null;
            AlbumArtist = string.Empty;
            AlbumArtistPersistentID = 0;
            AlbumPersistentID = 0;
            AlbumTrackCount = 0;
            AlbumTrackNumber = 0;
            ArtistPersistentID = 0;
            AssetURL = null;
            BeatsPerMinute = 0;
            BookmarkTime = 0;
            Comments = string.Empty;
            Composer = string.Empty;
            ComposerPersistentID = 0;
            DateAdded = new DateTime(1900, 1, 1);
            DiscCount = 0;
            DiscNumber = 0;
            Genre = string.Empty;
            GenrePersistentID = 0;
            HasProtectedAsset = false;
            IsCloudItem = false;
            IsCompilation = false;
            IsExplicitItem = false;
            LastPlayedDate = new DateTime(1900, 1, 1);
            Lyrics = string.Empty;
            MediaType = string.Empty;
            PlayCount = 0;
            PlaybackStoreID = string.Empty;
            PodcastPersistentID = 0;
            PodcastTitle = string.Empty;
            Rating = 0;
            ReleaseDate = new DateTime(1900, 1, 1);
            SkipCount = 0;
            Title = string.Empty;
            UserGrouping = string.Empty;

            _Duration = 0;
            _Date = new DateTime(1900, 1, 1);
            _Track = 0;
            _Year = 1900;
            _DurationTimeSpan = string.Empty;
            IsPlayNow = false;
            TextColor = Palette._LightGrayD;
            IsPlaylistSelected = true;
        }
    }
}