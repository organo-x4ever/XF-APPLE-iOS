using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.organo.xchallenge.Models.Media;
using com.organo.xchallenge.Pages.Base;
using com.organo.xchallenge.ViewModels.Media;
using Plugin.MediaManager;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace com.organo.xchallenge.Pages.Media
{
    public partial class AudioPlayerNewPage : AudioPlayerNewPageXaml
    {
        private AudioPlayerNewViewModel _model;

        public AudioPlayerNewPage(RootPage root)
        {
            try
            {
                InitializeComponent();
                this._model = new AudioPlayerNewViewModel();
                this._model.Root = root;
                this.Init();
            }
            catch (Exception exception)
            {
                var msg = exception;
            }
        }

        public sealed override async void Init(object obj = null)
        {
            await App.Configuration.InitialAsync(this);
            NavigationPage.SetHasNavigationBar(this, false);
            BindingContext = this._model;
            //await this._model.GetAsync();
        }

        protected override void OnDisappearing()
        {
            CrossMediaManager.Current.StatusChanged -= _model.AudioPlayer_StatusChanged;
            base.OnDisappearing();
        }
        private void ListView_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var selectedContent = (MediaFile)e.SelectedItem;
            //this._model.MediaContentCurrent = selectedContent;
            //int index = this._model.MediaContents.FindIndex(m => m == selectedContent && m == selectedContent);
            //this._model.IndexedCommand(index);
        }
    }

    public abstract class AudioPlayerNewPageXaml : ModelBoundContentPage<AudioPlayerNewViewModel>
    {
    }
}