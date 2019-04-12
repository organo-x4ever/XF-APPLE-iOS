using com.organo.x4ever.Localization;
using com.organo.x4ever.Models.Media;
using com.organo.x4ever.Pages;
using com.organo.x4ever.Services;
using com.organo.x4ever.ViewModels.Base;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace com.organo.x4ever.ViewModels.TrainingVideo
{
    public class TrainingVideoViewModel : BaseViewModel
    {
        private readonly IMediaContentService _mediaContentService;

        public TrainingVideoViewModel(INavigation navigation = null) : base(navigation)
        {
            this._mediaContentService = DependencyService.Get<IMediaContentService>();
            this.MediaContents = new List<MediaContentDetail>();
            this.ButtonPlayStop = TextResources.Play;
            this.ButtonNext = TextResources.Next;
            this.ButtonPrevious = TextResources.Previous;
        }

        public async Task OnLoad()
        {
            await this.GetAsync();
        }

        private async Task<List<MediaContentDetail>> GetAsync()
        {
            this.MediaContents = await this._mediaContentService.GetDetailAsync();
            SetCurrentMedia(1);
            return this.MediaContents;
        }

        public void SetCurrentMedia(int currentIndex)
        {
            var current = this.MediaContents.FirstOrDefault(m => m.ID == currentIndex);
            if (current == null)
                this.SetCurrentMedia(1);
            else
            {
                this.CurrentMedia = current;
                this.SetNextMedia(current.ID + 1);
                this.SetPreviousMedia(current.ID - 1);
            }
        }

        private void SetNextMedia(int currentIndex)
        {
            var current = this.MediaContents.Where(m => m.ID != this.CurrentMedia.ID)
                .FirstOrDefault(m => m.ID == currentIndex);
            if (current == null)
            {
                var next = this.MediaContents.LastOrDefault();
                if (next != null && currentIndex >= next.ID)
                    this.SetNextMedia(1);
            }
            else
                this.NextMedia = current;
        }

        private void SetPreviousMedia(int currentIndex)
        {
            var current = this.MediaContents.Where(m => m.ID != this.CurrentMedia.ID)
                .FirstOrDefault(m => m.ID == currentIndex);
            if (current == null)
            {
                if (currentIndex == 0)
                {
                    var previous = this.MediaContents.LastOrDefault();
                    if (previous != null)
                        this.SetPreviousMedia(previous.ID);
                }
                else
                    this.SetPreviousMedia(currentIndex - 1);
            }
            else
                this.PreviousMedia = current;
        }

        public List<MediaContentDetail> MediaContents { get; set; }

        private MediaContentDetail currentMedia;
        public const string CurrentMediaPropertyName = "CurrentMedia";

        public MediaContentDetail CurrentMedia
        {
            get { return this.currentMedia; }
            set { SetProperty(ref currentMedia, value, CurrentMediaPropertyName); }
        }

        private MediaContentDetail previousMedia;
        public const string PreviousMediaPropertyName = "PreviousMedia";

        public MediaContentDetail PreviousMedia
        {
            get { return this.previousMedia; }
            set { SetProperty(ref previousMedia, value, PreviousMediaPropertyName); }
        }

        private MediaContentDetail nextMedia;
        public const string NextMediaPropertyName = "NextMedia";

        public MediaContentDetail NextMedia
        {
            get { return this.nextMedia; }
            set { SetProperty(ref nextMedia, value, NextMediaPropertyName); }
        }

        private string buttonPlayStop;
        public const string ButtonPlayStopPropertyName = "ButtonPlayStop";

        public string ButtonPlayStop
        {
            get { return buttonPlayStop; }
            set { SetProperty(ref buttonPlayStop, value, ButtonPlayStopPropertyName); }
        }

        private string buttonNext;
        public const string ButtonNextPropertyName = "ButtonNext";

        public string ButtonNext
        {
            get { return buttonNext; }
            set { SetProperty(ref buttonNext, value, ButtonNextPropertyName); }
        }

        private string buttonPrevious;
        public const string ButtonPreviousPropertyName = "ButtonPrevious";

        public string ButtonPrevious
        {
            get { return buttonPrevious; }
            set { SetProperty(ref buttonPrevious, value, ButtonPreviousPropertyName); }
        }
    }
}