
using System.Collections.Generic;

namespace com.organo.x4ever
{
    public interface IMusicDictionary
    {
        List<string> Messages { get; set; }
        bool MediaLibraryAuthorized { get; }
        bool MediaLibraryDenied { get; }
        bool MediaLibraryRestricted { get; }
        bool MediaLibraryNotDetermined { get; }
        List<MediaItem> GetSongs();
        Dictionary<string, List<MediaItem>> GetSongsByAlbum();
        Dictionary<string, List<MediaItem>> GetSongsByArtist();
    }
}