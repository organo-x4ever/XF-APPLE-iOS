using com.organo.x4ever.Pages.Base;
using com.organo.x4ever.ViewModels.Storage;
using System;
using Xamarin.Forms;

namespace com.organo.x4ever.Pages.Storage
{
    public partial class FileExplorerPage : FileExplorerPageXaml
    {
        private FileExplorerViewModel _model;

        public FileExplorerPage(RootPage root)
        {
            try
            {
                InitializeComponent();
                this._model = new FileExplorerViewModel();
                this._model.Root = root;
            }
            catch (Exception exception)
            {
                throw new NotImplementedException(exception.Message);
            }
        }

        public sealed override async void Init(object obj = null)
        {
            await App.Configuration.InitialAsync(this);
            NavigationPage.SetHasNavigationBar(this, false);

            BindingContext = _model;
            _model.GetFiles();
        }

        private void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            this._model.Root.IsPresented = this._model.Root.IsPresented == false;
        }
    }

    public abstract class FileExplorerPageXaml : ModelBoundContentPage<FileExplorerViewModel>
    {
    }
}