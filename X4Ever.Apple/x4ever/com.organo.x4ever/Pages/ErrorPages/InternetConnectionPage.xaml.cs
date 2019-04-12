using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.organo.x4ever.Pages.Base;
using com.organo.x4ever.ViewModels.ErrorPages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace com.organo.x4ever.Pages.ErrorPages
{
    public partial class InternetConnectionPage : InternetConnectionXamlPage
    {
        private InternetConnectionViewModel _model;

        public InternetConnectionPage()
        {
            try
            {
                App.Configuration.Initial(this, false);
                NavigationPage.SetHasNavigationBar(this, false);
                this._model = new InternetConnectionViewModel();
                InitializeComponent();
                BindingContext = this._model;
            }
            catch (Exception)
            {
                //
            }
        }

        public sealed override void Init(object obj = null)
        {
            
        }
    }

    public abstract class InternetConnectionXamlPage : ModelBoundContentPage<InternetConnectionViewModel>
    {
    }
}