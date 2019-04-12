using System;
using com.organo.x4ever.Pages.Base;
using com.organo.x4ever.ViewModels.Testimonial;
using Xamarin.Forms;

namespace com.organo.x4ever.Pages.Testimonial
{
    public partial class TestimonialDetailPage : TestimonialDetailPageXaml
    {
        private TestimonialDetailViewModel _model;

        public TestimonialDetailPage(Models.Testimonial testimonial)
        {
            InitializeComponent();
            _model = new TestimonialDetailViewModel(App.CurrentApp.MainPage.Navigation)
            {
                Testimonial = testimonial
            };
            Init();
        }

        public sealed override async void Init(object obj = null)
        {
            await App.Configuration.InitialAsync(this);
            NavigationPage.SetHasNavigationBar(this, false);
            BindingContext = _model;
            _model.OnLoad();
        }

        protected override void OnDisappearing()
        {
            _model.StopPlayer();
            base.OnDisappearing();
        }
    }

    public abstract class TestimonialDetailPageXaml : ModelBoundContentPage<TestimonialDetailViewModel>
    {
    }
}