using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using com.organo.x4ever.Globals;
using com.organo.x4ever.Localization;
using com.organo.x4ever.Pages;
using com.organo.x4ever.Services;
using com.organo.x4ever.Statics;
using com.organo.x4ever.ViewModels.Base;
using Xamarin.Forms;

namespace com.organo.x4ever.ViewModels.OGX
{
    public class OGXViewModel : BaseViewModel
    {
        private readonly IFileService _fileService;

        public OGXViewModel(INavigation navigation = null) : base(navigation)
        {
            _fileService = DependencyService.Get<IFileService>();
        }

        public async Task<bool> LoadPageAsync()
        {
            var response = await _fileService.GetFileAsync(TextResources.OGX_Content_FilePath);
            if (response != null)
                FileUri = response;
            return !string.IsNullOrEmpty(FileUri);
        }

        private string _fileUri;
        public const string FileUriPropertyName = "FileUri";

        public string FileUri
        {
            get { return _fileUri; }
            set { SetProperty(ref _fileUri, value, FileUriPropertyName); }
        }
    }
}