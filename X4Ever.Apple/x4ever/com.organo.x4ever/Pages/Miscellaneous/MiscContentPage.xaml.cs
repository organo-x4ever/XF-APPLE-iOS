
using com.organo.x4ever.Pages.Base;
using com.organo.x4ever.ViewModels.Miscellaneous;
using com.organo.x4ever.Statics;
using System;
using com.organo.x4ever.Controls;
using Xamarin.Forms;
using com.organo.x4ever.Handler;
using com.organo.x4ever.Services;

namespace com.organo.x4ever.Pages.Miscellaneous
{
    public partial class MiscContentPage : MiscContentPageXaml
    {
        private readonly MiscContentViewModel _model;
        public MiscContentPage(RootPage root)
        {
            try
            {
                InitializeComponent();
                _model = new MiscContentViewModel() { Root = root };
                Init();
            }
            catch (Exception ex)
            {
                new ExceptionHandler(TAG, ClientService.GetExceptionDetail(ex));
            }
        }

        public override async void Init(object obj = null)
        {
            await App.Configuration.InitialAsync(this);
            NavigationPage.SetHasNavigationBar(this, false);
            BindingContext = _model;
            _model.WebUri = await _model.GetLink();
            contentView.Content = new HybridWebView()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Margin = new Thickness(0, -6, 0, 0),
                BackgroundColor = Palette._MainBackground,
                Uri = _model.WebUri + $"?token={App.Configuration.UserToken}"
            };
        }
    }

    public abstract class MiscContentPageXaml : ModelBoundContentPage<MiscContentViewModel> { }
}