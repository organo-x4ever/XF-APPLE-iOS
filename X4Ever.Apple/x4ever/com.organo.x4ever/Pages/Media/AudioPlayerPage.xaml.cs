
using com.organo.xchallenge.Pages.Base;
using com.organo.xchallenge.ViewModels.Media;
using Plugin.MediaManager.Abstractions.Implementations;
using System;
using Xamarin.Forms;

namespace com.organo.xchallenge.Pages.Media
{
    public partial class AudioPlayerPage : AudioPlayerPageXaml
    {
        private AudioPlayerViewModel _model;

        public AudioPlayerPage(RootPage root)
        {
            try
            {
                InitializeComponent();
                this._model = new AudioPlayerViewModel();
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

            this._model.TimerDisplaying = () => { Device.StartTimer(TimeSpan.FromSeconds(0.5), UpdatePosition); };
            BindingContext = this._model;
            await this._model.GetAsync();
            this.InitControls();
        }

        private void InitControls()
        {
            SliderPosition.ValueChanged += SliderPostionValueChanged;

            SliderPosition.Minimum = 0;
            SliderPosition.IsEnabled = this._model.CanSeek;
        }

        private void SliderPostionValueChanged(object sender, ValueChangedEventArgs e)
        {
            if ((int) SliderPosition.Value != this._model.Duration)
                this._model.SeekTo((int) SliderPosition.Value);
        }

        private bool UpdatePosition()
        {
            if (this._model.IsPlaying && this._model.Duration >= this._model.CurrentPosition)
            {
                this._model.ProgressTime = this._model.ConvertTImeToDisplay(this._model.CurrentPosition);
                //var totalDuration = this._model.MediaContentCurrent.TotalDuration;
                //if (totalDuration == null || totalDuration.Trim().Length == 0)
                //{
                //    this._model.MediaContentCurrent.TotalDuration =
                //        this._model.ConvertTImeToDisplay(this._model.Duration);
                //}
                //this._model.TotalTime = this._model.MediaContentCurrent.TotalDuration;
                SliderPosition.ValueChanged -= SliderPostionValueChanged;
                SliderPosition.Value = this._model.CurrentPosition;
                SliderPosition.ValueChanged += SliderPostionValueChanged;
                SliderPosition.Maximum = this._model.Duration;
            }

            return this._model.IsPlaying;
        }

        protected void PlayPauseClicked(Object sender, System.EventArgs e)
        {
            this._model.PlayPauseCommand();
        }

        protected void PreviousClicked(Object sender, System.EventArgs e)
        {
            this._model.PreviousCommand();
        }

        protected void NextClicked(Object sender, System.EventArgs e)
        {
            this._model.NextCommand();
        }

        protected void StopClicked(Object sender, System.EventArgs e)
        {
            SliderPosition.Value = SliderPosition.Minimum;
            this._model.Stop();
        }

        private void ListView_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var selectedContent = (MediaFile) e.SelectedItem;
            this._model.MediaContentCurrent = selectedContent;
            int index = this._model.MediaContents.FindIndex(m => m == selectedContent && m == selectedContent);
            this._model.IndexedCommand(index);
        }
    }

    public abstract class AudioPlayerPageXaml : ModelBoundContentPage<AudioPlayerViewModel>
    {
    }
}