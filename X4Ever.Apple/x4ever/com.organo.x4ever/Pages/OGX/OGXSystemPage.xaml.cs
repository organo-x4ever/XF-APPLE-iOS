using com.organo.x4ever.Pages.Base;
using com.organo.x4ever.ViewModels.OGX;
using System;
using System.Threading.Tasks;
using com.organo.x4ever.Globals;
using com.organo.x4ever.Services;
using com.organo.x4ever.Statics;
using Xamarin.Forms;
using com.organo.x4ever.Controls;

namespace com.organo.x4ever.Pages.OGX
{
    public partial class OgxSystemPage : OgxSystemXaml
    {
        private OGXViewModel model;
        private IHelper _helper;
        private IFileDownloadService _fileDownloadService;

        public OgxSystemPage(RootPage root)
        {
            try
            {
                InitializeComponent();
                model = new OGXViewModel()
                {
                    Root = root
                };
                Init();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public sealed override async void Init(object obj = null)
        {
            _helper = DependencyService.Get<IHelper>();
            _fileDownloadService = DependencyService.Get<IFileDownloadService>();
            App.Configuration.Initial(this);
            NavigationPage.SetHasNavigationBar(this, false);
            BindingContext = model;
            if (await model.LoadPageAsync())
            {
                var fullPath = _helper.GetFilePath(model.FileUri, FileType.Document);
                if (await _fileDownloadService.DownloadFileAsync(fullPath, model.FileUri))
                {
                    stackLayoutContent.Children.Add(new CustomWebView()
                    {
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        Uri = model.FileUri
                    });
                    await RemoveFile();
                }
            }
        }

        async Task RemoveFile()
        {
            await Task.Delay(3000);
            await _fileDownloadService.RemoveFileAsync(model.FileUri);
        }
    }

    public abstract class OgxSystemXaml : ModelBoundContentPage<OGXViewModel>
    {
    }
}