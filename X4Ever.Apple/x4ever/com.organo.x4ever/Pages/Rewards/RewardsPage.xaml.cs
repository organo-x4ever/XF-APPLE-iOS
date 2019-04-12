using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.organo.x4ever.Pages.Base;
using com.organo.x4ever.Services;
using com.organo.x4ever.ViewModels.Rewards;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace com.organo.x4ever.Pages.Rewards
{
    public partial class RewardsPage : RewardsXamlPage
    {
        private RewardsViewModel _model;

        public RewardsPage(RootPage root)
        {
            try
            {
                InitializeComponent();
                Init(root);
            }
            catch (Exception)
            {
                //
            }
        }

        public override void Init(object obj)
        {
            App.Configuration.Initial(this);
            NavigationPage.SetHasNavigationBar(this, false);
            _model = new RewardsViewModel()
            {
                Root = (RootPage) obj
            };
            BindingContext = _model;
            _model.LoadAsync();
        }
    }

    public abstract class RewardsXamlPage : ModelBoundContentPage<RewardsViewModel>
    {
    }
}