using com.organo.x4ever.Pages.Base;
using com.organo.x4ever.ViewModels.Testimonial;
using System;
using System.Threading.Tasks;
using com.organo.x4ever.Globals;
using com.organo.x4ever.Localization;
using Xamarin.Forms;

namespace com.organo.x4ever.Pages.Testimonial
{
    public partial class TestimonialPage : TestimonialPageXaml
    {
        private TestimonialViewModel _model;

        public TestimonialPage(RootPage rootPage)
        {
            try
            {
                InitializeComponent();
                _model = new TestimonialViewModel(App.CurrentApp.MainPage.Navigation);
                _model.Root = rootPage;
                BindingContext = _model;
                Init();
            }
            catch (Exception ex)
            {
                DependencyService.Get<IMessage>().AlertAsync(TextResources.Alert,
                    ex.InnerException != null ? ex.InnerException.Message : ex.Message, TextResources.Ok);
            }
        }

        public sealed override async void Init(object obj = null)
        {
            await App.Configuration.InitialAsync(this);
            NavigationPage.SetHasNavigationBar(this, false);

            await this._model.GetTestimonialAsync();
            this.ListViewTestimonials.ItemSelected += async (object sender, SelectedItemChangedEventArgs e) =>
            {
                if (e.SelectedItem != null)
                {
                    var content = (Models.Testimonial) e.SelectedItem;
                    if (content.IsVideoExists)
                        await App.CurrentApp.MainPage.Navigation.PushModalAsync(new TestimonialDetailPage(content));
                    else
                        await App.CurrentApp.MainPage.Navigation.PushModalAsync(new TestimonialPhotoPage(content));
                }

                ListViewTestimonials.SelectedItem = null;
            };
        }
    }

    public abstract class TestimonialPageXaml : ModelBoundContentPage<TestimonialViewModel>
    {
    }
}