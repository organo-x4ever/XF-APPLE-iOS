using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.organo.x4ever.Handler;
using com.organo.x4ever.Pages.Base;
using com.organo.x4ever.ViewModels.Audio;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.MediaManager.Abstractions.Implementations;

namespace com.organo.x4ever.Pages.Audio
{
    public partial class AudioManagerPage : AudioManagerXaml
    {
        private AudioManagerViewModel _model;

        public AudioManagerPage(RootPage rootPage)
        {
            try
            {
                InitializeComponent();
                Init(rootPage);
            }
            catch (Exception ex)
            {
                new ExceptionHandler(TAG, ex);
            }
        }

        public sealed override void Init(object obj)
        {
            App.Configuration.Initial(this);
            _model = new AudioManagerViewModel()
            {
                Root = (RootPage) obj
            };
            BindingContext = _model;
            _model.GetDirectoryAsync();
        }

        private async void ListView_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                var selectedContent = (MediaFile) e.SelectedItem;
                _model.CurrentMediaFile = selectedContent;
                int index = this._model.MediaFiles.FindIndex(m => m == selectedContent && m == selectedContent);
                await _model.PlayCurrent(index);
            }

            ListViewPlayer.SelectedItem = null;
        }

        protected override void OnDisappearing()
        {
            _model.StopAsync();
            base.OnDisappearing();
        }
    }

    public abstract class AudioManagerXaml : ModelBoundContentPage<AudioManagerViewModel>
    {

    }
}