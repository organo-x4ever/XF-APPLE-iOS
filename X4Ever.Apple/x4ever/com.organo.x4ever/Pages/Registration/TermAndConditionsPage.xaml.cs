using com.organo.x4ever.Pages.Base;
using com.organo.x4ever.Statics;
using com.organo.x4ever.ViewModels.Registration;
using Xamarin.Forms;

namespace com.organo.x4ever.Pages.Registration
{
    public partial class TermAndConditionsPage : TermAndConditionsPageXaml
    {
        private TermAndConditionViewModel _model;

        public TermAndConditionsPage()
        {
            InitializeComponent();
            this.Init();
        }

        public sealed override async void Init(object obj = null)
        {
           await App.Configuration.InitialAsync(this);
            NavigationPage.SetHasNavigationBar(this, false);
            _model = new TermAndConditionViewModel(App.CurrentApp.MainPage.Navigation);
            BindingContext = _model;
        }
    }

    public abstract class TermAndConditionsPageXaml : ModelBoundContentPage<TermAndConditionViewModel> { }
}