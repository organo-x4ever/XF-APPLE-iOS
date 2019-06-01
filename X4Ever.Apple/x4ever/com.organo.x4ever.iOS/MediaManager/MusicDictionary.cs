using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using com.organo.x4ever;
using com.organo.x4ever.ios.Extensions;
using CoreGraphics;
using Foundation;
using Intents;
using MediaPlayer;
using Plugin.Media.Abstractions;
using UIKit;
using Xamarin.Forms;
using static System.Environment;

[assembly:Dependency(typeof(MusicDictionary))]

namespace com.organo.x4ever
{
    public class MusicDictionary : IMusicDictionary
    {
        public List<string> Messages { get; set; }

        public bool MediaLibraryAuthorized =>
            MPMediaLibrary.AuthorizationStatus == MPMediaLibraryAuthorizationStatus.Authorized;

        public bool MediaLibraryDenied =>
            MPMediaLibrary.AuthorizationStatus == MPMediaLibraryAuthorizationStatus.Denied;

        public bool MediaLibraryRestricted =>
            MPMediaLibrary.AuthorizationStatus == MPMediaLibraryAuthorizationStatus.Restricted;

        public bool MediaLibraryNotDetermined =>
            MPMediaLibrary.AuthorizationStatus == MPMediaLibraryAuthorizationStatus.NotDetermined;

        public MusicDictionary()
        {
            Messages = new List<string>();
        }

        public bool Authorized()
        {
            if (MPMediaLibrary.AuthorizationStatus != MPMediaLibraryAuthorizationStatus.Authorized)
            {
                MPMediaLibrary.RequestAuthorization(status =>
                {
                    if ((MPMediaLibraryAuthorizationStatus) status != MPMediaLibraryAuthorizationStatus.Authorized)
                    {
                        Messages.Add(
                            "You must authorize the X4Ever to play music. To allow go Settings -> X4Ever then enable Media & Apple Music");
                    }
                });
            }

            return MPMediaLibrary.AuthorizationStatus == MPMediaLibraryAuthorizationStatus.Authorized;
        }

        public Dictionary<string, List<MediaItem>> GetSongsByAlbum()
        {
            Dictionary<string, List<MediaItem>> albumSongs = new Dictionary<string, List<MediaItem>>();
            try
            {
                MPMediaQuery mediaQuery = MPMediaQuery.AlbumsQuery;
                MPMediaItemCollection[] songsByAlbum = mediaQuery.Collections;
                List<MediaItem> songs;
                foreach (MPMediaItemCollection album in songsByAlbum)
                {
                    MPMediaItem[] songItems = album.Items;
                    string albumName = "";
                    songs = new List<MediaItem>();
                    foreach (MPMediaItem songItem in songItems)
                    {
                        // Create a new song type and add the info from this song to it
                        MediaItem song = new MediaItem();
                        try
                        {
                            if (albumName == "")
                                albumName = songItem.AlbumTitle;
                            SetMediaItem(ref song, songItem);
                            songs.Add(song);
                        }
                        catch (Exception ex)
                        {
                            Messages.Add(GetExceptionDetail(ex));
                        }
                    }

                    if (!albumSongs.ContainsKey(albumName))
                        albumSongs.Add(albumName, songs);
                    else
                    {
                        List<MediaItem> temp = null;
                        albumSongs.TryGetValue(albumName, out temp);
                        if (temp != null)
                            temp.AddRange(songs);
                    }
                }
            }
            catch (Exception ex)
            {
                Messages.Add(GetExceptionDetail(ex));
                _ = ex;
            }

            return albumSongs;
        }

        public List<MediaItem> GetSongs()
        {
            List<MediaItem> songs = new List<MediaItem>();
            try
            {
                MPMediaQuery mediaQuery = new MPMediaQuery();
                MPMediaItem[] songItems = mediaQuery.Items;
                foreach (var songItem in songItems)
                {
                    // Create a new song type and add the info from this song to it
                    MediaItem song = new MediaItem();
                    try
                    {
                        SetMediaItem(ref song, songItem);
                        songs.Add(song);
                    }
                    catch (Exception ex)
                    {
                        Messages.Add(GetExceptionDetail(ex));
                    }
                }
                
                #region

                //return SetMediaItem(mediaQuery.Items);

                //List<MediaItem> SetMediaItem(MPMediaItem[] songItems)
                //{
                //    return (from songItem in songItems
                //        select new MediaItem()
                //        {
                //            Title = songItem.Title,
                //            Album = songItem.AlbumTitle,

                //            Artist = songItem.Artist,
                //            SongID = songItem.PersistentID,

                //            Artwork = songItem.Artwork != null
                //                ? ImageSource.FromStream(() =>
                //                    songItem.Artwork.ImageWithSize(new CGSize(115.0f, 115.0f)).AsPNG().AsStream())
                //                : null,
                //            Duration = songItem.PlaybackDuration,
                //            SongURL = songItem.AssetURL,
                //            AlbumArtist = songItem.AlbumArtist,
                //            AlbumArtistPersistentID = songItem.AlbumArtistPersistentID,
                //            AlbumPersistentID = songItem.AlbumPersistentID,

                //            AlbumTrackCount = songItem.AlbumTrackCount,
                //            AlbumTrackNumber = songItem.AlbumTrackNumber,
                //            ArtistPersistentID = songItem.ArtistPersistentID,
                //            AssetURL = songItem.AssetURL,
                //            BeatsPerMinute = songItem.BeatsPerMinute,
                //            BookmarkTime = songItem.BookmarkTime,
                //            Comments = songItem.Comments,
                //            Composer = songItem.Composer,
                //            ComposerPersistentID = songItem.ComposerPersistentID,

                //            DateAdded = songItem.DateAdded != null
                //                ? songItem.DateAdded.ToDateTime()
                //                : new DateTime(1900, 1, 1),
                //            DiscCount = songItem.DiscCount,
                //            DiscNumber = songItem.DiscNumber,
                //            Genre = songItem.Genre,
                //            GenrePersistentID = songItem.GenrePersistentID,
                //            HasProtectedAsset = songItem.HasProtectedAsset,
                //            IsCloudItem = songItem.IsCloudItem,
                //            IsCompilation = songItem.IsCompilation,
                //            IsExplicitItem = songItem.IsExplicitItem,

                //            LastPlayedDate = songItem.LastPlayedDate != null
                //                ? songItem.LastPlayedDate.ToDateTime()
                //                : new DateTime(1900, 1, 1),
                //            Lyrics = songItem.Lyrics,
                //            MediaType = songItem.MediaType.ToString(),
                //            PlayCount = songItem.PlayCount,
                //            PlaybackStoreID = songItem.PlaybackStoreID,
                //            PodcastPersistentID = songItem.PodcastPersistentID,

                //            PodcastTitle = songItem.PodcastTitle,
                //            Rating = songItem.Rating,
                //            ReleaseDate = songItem.ReleaseDate != null
                //                ? songItem.ReleaseDate.ToDateTime()
                //                : new DateTime(1900, 1, 1),
                //            SkipCount = songItem.SkipCount,
                //            UserGrouping = songItem.UserGrouping,
                //        }).ToList();
                //}

                #endregion
            }
            catch (Exception ex)
            {
                Messages.Add(GetExceptionDetail(ex));
                _ = ex;
            }

            return songs;
        }

        public Dictionary<string, List<MediaItem>> GetSongsByArtist()
        {
            Dictionary<string, List<MediaItem>> artistSongs = new Dictionary<string, List<MediaItem>>();
            try
            {
                MusicQuery musicQuery = new MusicQuery();
                var artistSongs1 = musicQuery.queryForSongs();

                MPMediaQuery mediaQuery = MPMediaQuery.ArtistsQuery;
                MPMediaItemCollection[] songsByArtist = mediaQuery.Collections;
                List<MediaItem> songs;
                foreach (MPMediaItemCollection artist in songsByArtist)
                {
                    MPMediaItem[] songItems = artist.Items;
                    string artistName = "";
                    songs = new List<MediaItem>();
                    foreach (MPMediaItem songItem in songItems)
                    {
                        // Create a new song type and add the info from this song to it
                        MediaItem song = new MediaItem();
                        try
                        {
                            if (artistName == "")
                                artistName = songItem.Artist;
                            SetMediaItem(ref song, songItem);
                            songs.Add(song);
                        }
                        catch (Exception ex)
                        {
                            Messages.Add(GetExceptionDetail(ex));
                        }
                    }

                    if (!artistSongs.ContainsKey(artistName))
                        artistSongs.Add(artistName, songs);
                    else
                    {
                        List<MediaItem> temp = null;
                        artistSongs.TryGetValue(artistName, out temp);
                        if (temp != null)
                            temp.AddRange(songs);
                    }
                }
            }
            catch (Exception ex)
            {
                Messages.Add(GetExceptionDetail(ex));
                _ = ex;
            }

            return artistSongs;
        }


        // Get a song with a particular id
        public MPMediaItem QueryForSongBySongID(ulong songPersistenceId)
        {
            MPMediaPropertyPredicate mediaItemPersistenceIdPredicate =
                MPMediaPropertyPredicate.PredicateWithValue(new NSNumber(songPersistenceId),
                    MPMediaItem.PersistentIDProperty);

            MPMediaQuery songQuery = new MPMediaQuery();
            songQuery.AddFilterPredicate(mediaItemPersistenceIdPredicate);

            var items = songQuery.Items;

            return items[items.Length - 1];
        }

        private static string GetExceptionDetail(Exception exception)
        {
            var stringBuilder = new StringBuilder();
            while (exception != null)
            {
                stringBuilder.AppendLine(exception.Message);
                stringBuilder.AppendLine(exception.StackTrace);
                exception = exception.InnerException;
            }

            return stringBuilder.ToString();
        }
        
        private void SetMediaItem(ref MediaItem song,MPMediaItem songItem)
        {
            song.Title = songItem.Title;
            song.Album = songItem.AlbumTitle;

            song.Artist = songItem.Artist;
            song.SongID = songItem.PersistentID;

            song.Artwork = songItem.Artwork != null
                ? ImageSource.FromStream(() =>
                    songItem.Artwork.ImageWithSize(new CGSize(115.0f, 115.0f)).AsPNG().AsStream())
                : null;
            song.Duration = songItem.PlaybackDuration;
            song.SongURL = songItem.AssetURL;
            song.AlbumArtist = songItem.AlbumArtist;
            song.AlbumArtistPersistentID = songItem.AlbumArtistPersistentID;
            song.AlbumPersistentID = songItem.AlbumPersistentID;

            song.AlbumTrackCount = songItem.AlbumTrackCount;
            song.AlbumTrackNumber = songItem.AlbumTrackNumber;
            song.ArtistPersistentID = songItem.ArtistPersistentID;
            song.AssetURL = songItem.AssetURL;
            song.BeatsPerMinute = songItem.BeatsPerMinute;
            song.BookmarkTime = songItem.BookmarkTime;
            song.Comments = songItem.Comments;
            song.Composer = songItem.Composer;
            song.ComposerPersistentID = songItem.ComposerPersistentID;

            song.DateAdded = songItem.DateAdded != null
                ? songItem.DateAdded.ToDateTime()
                : new DateTime(1900, 1, 1);
            song.DiscCount = songItem.DiscCount;
            song.DiscNumber = songItem.DiscNumber;
            song.Genre = songItem.Genre;
            song.GenrePersistentID = songItem.GenrePersistentID;
            song.HasProtectedAsset = songItem.HasProtectedAsset;
            song.IsCloudItem = songItem.IsCloudItem;
            song.IsCompilation = songItem.IsCompilation;
            song.IsExplicitItem = songItem.IsExplicitItem;

            song.LastPlayedDate = songItem.LastPlayedDate != null
                ? songItem.LastPlayedDate.ToDateTime()
                : new DateTime(1900, 1, 1);
            song.Lyrics = songItem.Lyrics;
            song.MediaType = songItem.MediaType.ToString();
            song.PlayCount = songItem.PlayCount;
            song.PlaybackStoreID = songItem.PlaybackStoreID;
            song.PodcastPersistentID = songItem.PodcastPersistentID;

            song.PodcastTitle = songItem.PodcastTitle;
            song.Rating = songItem.Rating;
            song.ReleaseDate = songItem.ReleaseDate != null
                ? songItem.ReleaseDate.ToDateTime()
                : new DateTime(1900, 1, 1);
            song.SkipCount = songItem.SkipCount;
            song.UserGrouping = songItem.UserGrouping;
        }

        //if (mediaItem != null) {
        //    NSUrl Url = mediaItem.AssetURL;
        //    item = AVPlayerItem.FromUrl(Url);					
        //    if (item != null) {
        //        this.avPlayer.ReplaceCurrentItemWithPlayerItem(item);
        //    }					
        //    MPNowPlayingInfo np = new MPNowPlayingInfo();
        //    SetNowPlayingInfo(song, np);					
        //    this.play();					
        //}

        //void MediaPicker(MPMediaPickerController mediaPicker, MPMediaItemCollection mediaItemCollection) {
        //    self.dismissViewControllerAnimated(true, completion: nil)
        //    let selectedSongs = mediaItemCollection
        //    mp.setQueueWithItemCollection(selectedSongs)
        //    mp.play()
        //}

        private Action<MPMediaLibraryAuthorizationStatus> MediaLibraryAuthorizationStatus;
    }
}