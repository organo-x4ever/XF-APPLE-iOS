
using com.organo.x4ever.Models.Youtube;
using com.organo.x4ever.Pages.Base;
using com.organo.x4ever.ViewModels.YouTube;
using System;
using Xamarin.Forms;

namespace com.organo.x4ever.Pages.YouTube
{
    public partial class YoutubeTestimonialPage : YoutubeTestimonialPageXaml
    {
        private YoutubeViewModel _model;

        public YoutubeTestimonialPage(RootPage root)
        {
            try
            {
                InitializeComponent();
                App.Configuration.Initial(this);
                NavigationPage.SetHasNavigationBar(this, false);
                _model = new YoutubeViewModel()
                {
                    Root = root,
                };
                BindingContext = _model;

            }
            catch (Exception)
            {
                //
            }
        }

        public override void Init(object obj)
        {

        }

        private void ListViewOnItemTapped(object sender, ItemTappedEventArgs itemTappedEventArgs)
        {
            if (itemTappedEventArgs.Item != null)
            {
                var youtubeItem = itemTappedEventArgs.Item as YoutubeItem;
                Device.OpenUri(
                    new Uri(string.Format(_model.YoutubeConfiguration.VideoWatchApiUrl, youtubeItem?.VideoId)));
            }
            ListViewYouTube.SelectedItem = null;
        }
    }

    public abstract class YoutubeTestimonialPageXaml : ModelBoundContentPage<YoutubeViewModel>
    {
    }
}