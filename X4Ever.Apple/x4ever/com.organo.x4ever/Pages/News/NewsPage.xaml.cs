using com.organo.x4ever.Pages.Base;
using com.organo.x4ever.ViewModels.News;
using System;
using System.Threading.Tasks;
using com.organo.x4ever.Controls;
using com.organo.x4ever.Handler;
using Xamarin.Forms;

namespace com.organo.x4ever.Pages.News
{
    public partial class NewsPage : NewsPageXaml
    {
        private NewsViewModel _model;

        public NewsPage(RootPage root)
        {
            try
            {
                InitializeComponent();
                _model = new NewsViewModel()
                {
                    Root = root
                };
                Init();
            }
            catch (Exception ex)
            {
                new ExceptionHandler(TAG, ex);
            }
        }

        public sealed override async void Init(object obj = null)
        {
            BindingContext = _model;
            SetGridNews();
            await App.Configuration.InitialAsync(this);
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void SetGridNews()
        {
            GridNews.Source = await _model.GetAsync();
        }
    }

    public abstract class NewsPageXaml : ModelBoundContentPage<NewsViewModel>
    {
    }
}