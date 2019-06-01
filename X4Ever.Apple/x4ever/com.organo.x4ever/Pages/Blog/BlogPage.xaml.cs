
using System;
using com.organo.x4ever.Controls;
using com.organo.x4ever.Handler;
using com.organo.x4ever.Pages.Base;
using com.organo.x4ever.Services;
using com.organo.x4ever.Statics;
using com.organo.x4ever.ViewModels.Blog;
using Xamarin.Forms;

namespace com.organo.x4ever.Pages.Blog
{
    public partial class BlogPage : BlogPageXaml
    {
        private readonly BlogViewModel _model;

        public BlogPage(RootPage root)
        {
            try
            {
                InitializeComponent();
                _model = new BlogViewModel() {Root = root};
                Init();
            }
            catch (Exception ex)
            {
                new ExceptionHandler(nameof(BlogPage), ClientService.GetExceptionDetail(ex));
            }
        }

        public override async void Init(object obj = null)
        {
            await App.Configuration.InitialAsync(this);
            NavigationPage.SetHasNavigationBar(this, false);
            BindingContext = _model;
            _model.WebUri = await _model.GetLink();
            contentView.Content = new HybridWebView()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Margin = new Thickness(0, -6, 0, 0),
                BackgroundColor = Palette._MainBackground,
                Uri = _model.WebUri
            };
        }
    }

    public abstract class BlogPageXaml : ModelBoundContentPage<BlogViewModel>
    {
    }
}