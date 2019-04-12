using com.organo.x4ever.ViewModels.Base;
using System.Windows.Input;
using com.organo.x4ever.Statics;
using Xamarin.Forms;

namespace com.organo.x4ever.ViewModels.Testimonial
{
    public class TestimonialPhotoViewModel : BaseViewModel
    {
        public TestimonialPhotoViewModel(INavigation navigation = null) : base(navigation)
        {
            //this.SetPageImageSize();
        }

        private Models.Testimonial _testimonial;
        public const string TestimonialPropertyName = "Testimonial";

        public Models.Testimonial Testimonial
        {
            get { return _testimonial; }
            set { SetProperty(ref _testimonial, value, TestimonialPropertyName); }
        }

        private ICommand _closeCommand;

        public ICommand CloseCommand
        {
            get
            {
                return _closeCommand ?? (_closeCommand = new Command(
                           async (obj) => { await this.PopModalAsync(); }));
            }
        }
        
        //private void SetPageImageSize()
        //{
        //    var imageSize = App.Configuration.GetImageSizeByID(ImageIdentity.TESTIMONIAL_PERSON_IMAGE);
        //    if (imageSize != null)
        //    {
        //        this.PersonImageHeight = imageSize.Height;
        //        this.PersonImageWidth = imageSize.Width;
        //    }
        //}

        //private float personImageHeight;
        //public const string PersonImageHeightPropertyName = "PersonImageHeight";

        //public float PersonImageHeight
        //{
        //    get { return personImageHeight; }
        //    set { SetProperty(ref personImageHeight, value, PersonImageHeightPropertyName); }
        //}

        //private float personImageWidth;
        //public const string PersonImageWidthPropertyName = "PersonImageWidth";

        //public float PersonImageWidth
        //{
        //    get { return personImageWidth; }
        //    set { SetProperty(ref personImageWidth, value, PersonImageWidthPropertyName); }
        //}
    }
}