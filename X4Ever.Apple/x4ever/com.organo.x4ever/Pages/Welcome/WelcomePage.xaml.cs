using com.organo.x4ever.Globals;
using com.organo.x4ever.Pages.Base;
using com.organo.x4ever.Statics;
using com.organo.x4ever.ViewModels.Welcome;
using System;
using Xamarin.Forms;

namespace com.organo.x4ever.Pages.Welcome
{
    public partial class WelcomePage : WelcomePageXaml
    {
        private WelcomeViewModel _model;

        public WelcomePage()
        {
            try
            {
                InitializeComponent();
                WriteLog.Write("WelcomePage()");
                Init();
            }
            catch (Exception exception)
            {
                WriteLog.Write("Exception: " + exception.Message);
                //throw new NotImplementedException(exception.Message);
            }
        }

        public sealed override async void Init(object obj = null)
        {
            WriteLog.Write("WelcomePage #Init()");
            App.Configuration.Initial(this);
            NavigationPage.SetHasNavigationBar(this, false);
            _model = new WelcomeViewModel();
            BindingContext = _model;
            await _model.OnLoad();
        }

        protected override void OnDisappearing()
        {
            if (_model.MediaFiles.Count > 0)
                _model.StopPlayer();
            base.OnDisappearing();
        }
    }

    public abstract class WelcomePageXaml : ModelBoundContentPage<WelcomeViewModel>
    {
    }
}