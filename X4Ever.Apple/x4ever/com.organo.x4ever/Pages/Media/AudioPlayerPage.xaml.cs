using System;
using System.Collections.Generic;
using System.Linq;
using com.organo.x4ever.Handler;
using com.organo.x4ever.Localization;
using com.organo.x4ever.Pages.Base;
using com.organo.x4ever.Statics;
using com.organo.x4ever.Utilities;
using com.organo.x4ever.ViewModels.Media;
using Xamarin.Forms;

namespace com.organo.x4ever.Pages.Media
{
    public partial class AudioPlayerPage : AudioPlayerPageXaml
    {
        private AudioPlayerViewModel _model;

        public AudioPlayerPage(RootPage rootPage)
        {
            try
            {
                InitializeComponent();
                Init(rootPage);
            }
            catch (Exception ex)
            {
                var exceptionHandler = new ExceptionHandler(TAG, ex);
            }
        }

        public override async void Init(object obj = null)
        {
            await App.Configuration.InitialAsync(this);
            _model = new AudioPlayerViewModel()
            {
                Root = (RootPage) obj,
                DisplaySortByListAction = DisplaySortByList
            };
            BindingContext = _model;
            await _model.GetFilesAsync();
        }

        private async void ListView_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                if (_model.IsChecklistSelected)
                {
                    var musicFile = ((MediaItem) e.SelectedItem);
                    musicFile.IsPlaylistSelected = true;
                    musicFile.TextColor = musicFile.IsPlaylistSelected
                        ? Palette._LightGrayD
                        : Palette._ButtonBackgroundGray;

                    var playlistMusicFile = _model.PlaylistMusicFiles.Find(m =>
                        m.Title == musicFile.Title && m.Album == musicFile.Album && m.Artist == musicFile.Artist);
                    if (playlistMusicFile == null)
                        _model.PlaylistMusicFiles.Add(musicFile);
                    else
                        _model.PlaylistMusicFiles.Remove(playlistMusicFile);

                    _model.MusicFiles = _model.MusicFiles.Select(m =>
                    {
                        m.IsPlaylistSelected = _model.PlaylistMusicFiles.Any(t =>
                            t.AlbumPersistentID == m.AlbumPersistentID && t.SongID == m.SongID &&
                            t.Title == m.Title && t.Album == m.Album && t.Artist == m.Artist);
                        m.TextColor = m.IsPlaylistSelected ? Palette._LightGrayD : Palette._ButtonBackgroundGray;
                        return m;
                    }).ToList();
                }
                else if (!((MediaItem) e.SelectedItem).IsPlaylistSelected)
                {
                    _model.SetActivityResource(showMessage: true, message: "Song does not exist in playlist");
                    return;
                }
                else if (_model.CurrentMusicFile != (MediaItem) e.SelectedItem)
                {
                    var selectedContent = (MediaItem) e.SelectedItem;
                    _model.CurrentMusicFile = selectedContent;
                    int index = _model.MusicFiles.FindIndex(m => m == selectedContent && m == selectedContent);
                    await _model.PlayCurrent(index);
                }
            }

            ListViewPlayer.SelectedItem = null;
        }

        public async void DisplaySortByList()
        {
            var sortLists = EnumUtil.GetValues<PlaylistSortList>();
            var list = new List<string>();
            foreach (var sortList in sortLists)
            {
                list.Add(sortList.ToString());
            }

            var result =
                await DisplayActionSheet(TextResources.SortBy, TextResources.Cancel, null, list.ToArray());
            if (result != null && result != TextResources.Cancel)
            {
                _model.PlaylistSortBy = (PlaylistSortList) Enum.Parse(typeof(PlaylistSortList), result.ToString());
                _model.SortOrderBy(_model.PlaylistSortBy);
            }
        }

        protected override void OnDisappearing()
        {
            _model?.StopAsync();
            base.OnDisappearing();
        }

    }

    public abstract class AudioPlayerPageXaml : ModelBoundContentPage<AudioPlayerViewModel>
    {
    }
}