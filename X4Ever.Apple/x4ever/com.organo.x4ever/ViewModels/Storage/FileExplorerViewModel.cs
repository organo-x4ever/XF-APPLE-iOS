using com.organo.x4ever.Models;
using com.organo.x4ever.Pages;
using com.organo.x4ever.Services;
using com.organo.x4ever.ViewModels.Base;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace com.organo.x4ever.ViewModels.Storage
{
    public class FileExplorerViewModel : BaseViewModel
    {
        private readonly ILocalFile _localFile;

        public FileExplorerViewModel(INavigation navigation = null) : base(navigation)
        {
            _localFile = DependencyService.Get<ILocalFile>();
        }

        public void GetFiles()
        {
            this.FileDetails = new List<FileDetail>();
            var files = _localFile.UpdatePlayListAsync();
            List<FileDetail> fileDetails = files;
            this.FileDetails = (from f in fileDetails
                                    //where f.Type == this.FileType
                                orderby f.Parent, f.Path, f.Name
                                select f).ToList();
        }

        private string FileType => "XML";

        private List<FileDetail> _fileDetail;
        public const string FileDetailPropertyName = "FileDetails";

        public List<FileDetail> FileDetails
        {
            get { return _fileDetail; }
            set { SetProperty(ref _fileDetail, value, FileDetailPropertyName); }
        }
    }
}