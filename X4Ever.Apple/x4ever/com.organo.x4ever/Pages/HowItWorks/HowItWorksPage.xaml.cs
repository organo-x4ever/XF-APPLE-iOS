using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.organo.x4ever.Pages.Base;
using com.organo.x4ever.ViewModels.HowItWorks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace com.organo.x4ever.Pages.HowItWorks
{
    public partial class HowItWorksPage : HowItWorksXamlPage
    {
        private HowItWorksViewModel _model;

        public HowItWorksPage(RootPage root)
        {
            InitializeComponent();
            Init(root);
        }

        public override void Init(object obj)
        {
            App.Configuration.Initial(this);
            NavigationPage.SetHasNavigationBar(this, false);
            _model = new HowItWorksViewModel()
            {
                Root = (RootPage) obj
            };
            BindingContext = _model;
            _model.LoadAsync();
        }
    }

    public abstract class HowItWorksXamlPage : ModelBoundContentPage<HowItWorksViewModel>
    {
    }
}