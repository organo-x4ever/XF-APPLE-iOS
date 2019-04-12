using com.organo.x4ever.Pages.Base;
using com.organo.x4ever.ViewModels.Testimonial;
using Xamarin.Forms;

namespace com.organo.x4ever.Pages.Testimonial
{
    public partial class TestimonialPhotoPage : TestimonialPhotoPageXaml
    {
        private TestimonialPhotoViewModel _model;

        public TestimonialPhotoPage(Models.Testimonial testimonial)
        {
            InitializeComponent();
            _model = new TestimonialPhotoViewModel(App.CurrentApp.MainPage.Navigation)
            {
                Testimonial = testimonial
            };
            Init();
        }

        public sealed override async void Init(object obj = null)
        {
            await App.Configuration.InitialAsync(this);
            NavigationPage.SetHasNavigationBar(this, false);

            BindingContext = this._model;
        }
    }

    public abstract class TestimonialPhotoPageXaml : ModelBoundContentPage<TestimonialPhotoViewModel>
    {
    }
}