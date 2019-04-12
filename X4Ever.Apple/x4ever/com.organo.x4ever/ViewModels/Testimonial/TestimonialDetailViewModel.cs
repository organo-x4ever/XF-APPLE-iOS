
using com.organo.x4ever.Globals;
using com.organo.x4ever.Statics;
using com.organo.x4ever.ViewModels.Base;
using Plugin.MediaManager;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Enums;
using Plugin.MediaManager.Abstractions.Implementations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace com.organo.x4ever.ViewModels.Testimonial
{
    public class TestimonialDetailViewModel : BaseViewModel
    {
        private IPlaybackController PlaybackController => CrossMediaManager.Current.PlaybackController;
        private readonly IHelper _helper;

        public TestimonialDetailViewModel(INavigation navigation = null) : base(navigation)
        {
            _helper = DependencyService.Get<IHelper>();
            Source = string.Empty;
        }

        public async void OnLoad()
        {
            if (Testimonial.IsVideoExists)
                await Page_Load();
        }

        async Task<bool> Page_Load()
        {
            List<MediaFile> mediaFiles = new List<MediaFile>();
            Source = _helper.GetFilePath(Testimonial.VideoUrl, FileType.TestimonialVideo);
            mediaFiles.Add(new MediaFile()
            {
                Url = Source,
                Type = MediaFileType.Video,
                MetadataExtracted = false,
                Availability = ResourceAvailability.Remote,
            });
            CrossMediaManager.Current.MediaQueue.Repeat = RepeatType.RepeatOne;
            await CrossMediaManager.Current.Play(mediaFiles);
            return true;
        }

        private Models.Testimonial _testimonial;
        public const string TestimonialPropertyName = "Testimonial";

        public Models.Testimonial Testimonial
        {
            get { return _testimonial; }
            set { SetProperty(ref _testimonial, value, TestimonialPropertyName); }
        }

        private string _source;
        public const string SourcePropertyName = "Source";

        public string Source
        {
            get { return _source; }
            set { SetProperty(ref _source, value, SourcePropertyName); }
        }

        public async void StopPlayer()
        {
            try
            {
                await PlaybackController.Stop();
            }
            catch (Exception)
            {
                // Commented
            }
        }

        private ICommand _closeCommand;

        public ICommand CloseCommand
        {
            get
            {
                return _closeCommand ?? (_closeCommand = new Command(
                           async (obj) => { await PopModalAsync(); }));
            }
        }
    }
}