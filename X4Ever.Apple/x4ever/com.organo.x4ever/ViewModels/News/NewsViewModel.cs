using com.organo.x4ever.Models.News;
using com.organo.x4ever.Pages;
using com.organo.x4ever.Services;
using com.organo.x4ever.ViewModels.Base;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using com.organo.x4ever.Globals;
using com.organo.x4ever.Helpers;
using com.organo.x4ever.Statics;
using Xamarin.Forms;

namespace com.organo.x4ever.ViewModels.News
{
    public class NewsViewModel : BaseViewModel
    {
        private readonly INewsService _newsService;

        public NewsViewModel(INavigation navigation = null) : base(navigation)
        {
            _newsService = DependencyService.Get<INewsService>();
        }

        public async Task<List<NewsModel>> GetAsync()
        {
            var newsList = await _newsService.GetByLanguage(App.Configuration.AppConfig.DefaultLanguage, true);
            return (from n in newsList
                select new NewsModel()
                {
                    Active = n.Active,
                    ApplicationId = n.ApplicationId,
                    Body = n.Body,
                    Header = n.Header,
                    ID = n.ID,
                    LanguageCode = n.LanguageCode,
                    ModifiedBy = n.ModifiedBy,
                    ModifyDate = n.ModifyDate,
                    NewsImage = n.NewsImage,
                    NewsImagePosition = n.NewsImagePosition,
                    NewsImageSource = DependencyService.Get<IHelper>().GetFileUri(n.NewsImage, FileType.None),
                    PostDate = n.PostDate,
                    PostedBy = n.PostedBy
                }).OrderByDescending(n => n.PostDate).ToList();
        }
    }
}