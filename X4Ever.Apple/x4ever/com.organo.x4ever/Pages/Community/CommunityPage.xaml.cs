using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.organo.x4ever.Pages.Base;
using com.organo.x4ever.ViewModels.Community;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace com.organo.x4ever.Pages.Community
{
    public partial class CommunityPage : CommunityPageXaml
    {
        private CommunityViewModel _model;

        public CommunityPage(RootPage root)
        {
            try
            {
                InitializeComponent();
                _model = new CommunityViewModel()
                {
                    Root = root
                };
                Init();
            }
            catch (Exception)
            {
                //
            }
        }

        public sealed override void Init(object obj = null)
        {
            App.Configuration.Initial(this);
            NavigationPage.SetHasNavigationBar(this, false);
            BindingContext = _model;
        }
    }

    public abstract class CommunityPageXaml : ModelBoundContentPage<CommunityViewModel>
    {
    }
}