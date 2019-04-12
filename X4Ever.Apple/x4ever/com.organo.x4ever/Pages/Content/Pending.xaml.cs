using com.organo.x4ever.Pages.Base;
using com.organo.x4ever.ViewModels.Content;
using System;

namespace com.organo.x4ever.Pages.Content
{
    public partial class Pending : PendingXaml
    {
        private ContentViewModel _model;

        public Pending(RootPage root)
        {
            try
            {
                InitializeComponent();
                _model = new ContentViewModel()
                {
                    Root = root
                };
                this.Init();
            }
            catch (Exception ex)
            {
                var m = ex;
            }
        }

        public sealed override async void Init(object obj = null)
        {
            await App.Configuration.InitialAsync(this);
            App.Configuration.IsFullScreenMode = false;
            BindingContext = this._model;
        }
    }

    public abstract class PendingXaml : ModelBoundContentPage<ContentViewModel> { }
}