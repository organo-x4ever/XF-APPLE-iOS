﻿
using com.organo.x4ever.ViewModels.Base;
using System.Threading.Tasks;

using Xamarin.Forms;
using System.Text.RegularExpressions;
using com.organo.x4ever.Services;

namespace com.organo.x4ever.ViewModels.Miscellaneous
{
    public class MiscContentViewModel : BaseViewModel
    {
        public MiscContentViewModel(INavigation navigation=null) : base(navigation)
        {

        }
            
        public async Task<string> GetLink() => await DependencyService.Get<IConstantServices>().MoreWebLinks();

        private string _webUri;
        public const string WebUriPropertyName = "WebUri";

        public string WebUri
        {
            get => _webUri;
            set
            {
                var url = Regex.Replace(value, "\"", "");
                SetProperty(ref _webUri, url, WebUriPropertyName);
            }
        }
    }
}