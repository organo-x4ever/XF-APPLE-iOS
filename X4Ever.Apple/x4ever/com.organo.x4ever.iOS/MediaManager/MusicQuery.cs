﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.organo.x4ever;
using Foundation;
using MediaPlayer;
using UIKit;
using Xamarin.Forms;

[assembly:Dependency(typeof(MusicQuery))]
namespace com.organo.x4ever
{
    public class MusicQuery: IMusicQuery
    {

        public MusicQuery()
        {
        }

        public void GetQuery()
        {
            queryForSongs();
        }

        // Get the songs on the device
        public Dictionary<string, List<Song>> queryForSongs()
        {
            MPMediaQuery query = MPMediaQuery.AlbumsQuery;
            /*
                TigerMending album (12 missing on 5s) Picked up in app on 4 (and iPad Air 2!!) but not on 5s… not filtered out, just not picked up by app????
                Casey James (“Let’s do…"Missing on 4) <<<<<<<<<<<< filtered out as they should be as they ARE icloud items (not on computer or device)
                Israel K (2 extra versions on 5s) <<<<<<<<<<<<<<<<<
                Muse (2 extra “Hysteria” and “Time is running out” on 5s) <<<<<<<<<<<<
                Owsley (“Undone" missing on 4) <<<<<<<<<<<<<<<<<<<
                Radiohead (6 “Nude” single and stems missing on 4) <<<<<<<<<<<<<<<
                U2 (1 “Vertigo” extra on 5s) <<<<<<<<<<<<<<<<<<<
            */

            //MPMediaPropertyPredicate filter = MPMediaPropertyPredicate.PredicateWithValue(NSNumber.FromBoolean(false), MPMediaItem.IsCompilationProperty);
            //query.AddFilterPredicate(filter);

            MPMediaItemCollection[] songsByArtist = query.Collections;
            

            Dictionary<string, List<Song>> artistSongs = new Dictionary<string, List<Song>>();
            List<Song> songs;

            foreach (MPMediaItemCollection album in songsByArtist)
            {
                MPMediaItem[] albumSongs = album.Items;
                string artistName = "";
                songs = new List<Song>();
                foreach (MPMediaItem songMediumItem in albumSongs)
                {
                    // Create a new song type and add the info from this song to it
                    Song song = new Song();
                    song.album = songMediumItem.AlbumTitle.ToString();
                    song.artist = songMediumItem.Artist.ToString();
                    if (artistName == "")
                        artistName = song.artist;
                    song.song = songMediumItem.Title.ToString();
                    song.songID = songMediumItem.PersistentID;
                    song.artwork = songMediumItem.Artwork;
                    song.duration = songMediumItem.PlaybackDuration;

                    // Add the song to the list
                    songs.Add(song);
                }

                /* The reason Tigermending was not getting picked up is that it was deleivered
                 * by the iPhone 5s MediaQuery separately from the rest of the Carina ROund albums
                 * So without the below else clause, It was not added to the existing Carina Round song list
                 * This is good to do anyway, so be it. 
                */
                if (!artistSongs.ContainsKey(artistName))
                    artistSongs.Add(artistName, songs);
                else
                {
                    List<Song> temp = null;
                    artistSongs.TryGetValue(artistName, out temp);
                    if (temp != null)
                        temp.AddRange(songs);
                }
            }

            return artistSongs;
        }

        // Get a song with a particular id
        public MPMediaItem queryForSongWithId(ulong songPersistenceId)
        {
            MPMediaPropertyPredicate mediaItemPersistenceIdPredicate =
                MPMediaPropertyPredicate.PredicateWithValue(new NSNumber(songPersistenceId),
                    MPMediaItem.PersistentIDProperty);

            MPMediaQuery songQuery = new MPMediaQuery();
            songQuery.AddFilterPredicate(mediaItemPersistenceIdPredicate);

            var items = songQuery.Items;

            return items[items.Length - 1];
        }
    }
}