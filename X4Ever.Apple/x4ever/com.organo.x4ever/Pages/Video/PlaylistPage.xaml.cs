using com.organo.x4ever.Controls;
using com.organo.x4ever.Localization;
using com.organo.x4ever.Pages.Base;
using com.organo.x4ever.Statics;
using com.organo.x4ever.ViewModels.Video;
using Plugin.MediaManager.Forms;
using System;
using System.Threading.Tasks;
using com.organo.x4ever.Handler;
using com.organo.x4ever.Helpers;
using Xamarin.Forms;

namespace com.organo.x4ever.Pages.Video
{
    public partial class PlaylistPage : PlaylistPageXaml
    {
        private readonly PlaylistViewModel _model;
        private PopupLayout popupLayout;
        private readonly IDeviceInfo _deviceInfo;

        public PlaylistPage(RootPage root)
        {
            try
            {
                InitializeComponent();
                _deviceInfo = DependencyService.Get<IDeviceInfo>();
                _model = new PlaylistViewModel(App.CurrentApp.MainPage.Navigation)
                {
                    Root = root,
                    BindDataSourceAction = () =>
                    {
                        AccordionMain.DataSource = this._model.AccordionSources;
                        AccordionMain.DataBind();
                    },
                    PopupAction = OpenPopupWindow,
                    ClosePopupAction = CloseWindow
                };
                this.Init();
            }
            catch (Exception ex)
            {
                new ExceptionHandler(TAG, ex);
            }
        }

        public sealed override async void Init(object obj = null)
        {
            App.Configuration.Initial(this);
            NavigationPage.SetHasNavigationBar(this, false);

            BindingContext = this._model;
            AccordionMain.FirstExpaned = true;
            await this.Page_Load();
        }

        private async Task Page_Load()
        {
            await this._model.UpdateButtonSelected(ButtonSelected.Beginner);
        }

        public async void OpenPopupWindow()
        {
            var imageSizeWindow = await App.Configuration.GetImageSizeByIDAsync(ImageIdentity.WORKOUT_VIDEO_WINDOW);
            int height = 340, width = 360;
            if (imageSizeWindow != null)
            {
                height = (int)imageSizeWindow.Height;
                width = (int)imageSizeWindow.Width;
            }

            if (_deviceInfo.WidthPixels != 0)
            {
                width = _deviceInfo.WidthPixels;
                width -= 20;
                height = width - 10;
                //height = _deviceInfo.HeightPixels;
            }

            this.CloseWindow();
            popupLayout = this.Content as PopupLayout;
            var stackLayout = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Center,
                //BackgroundColor = Palette._MainAccent,
                BackgroundColor = Palette._Transparent,
                Orientation = StackOrientation.Vertical,
                HeightRequest = height,
                WidthRequest = width
            };

            StackLayout stackLayoutTitle = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.End,
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Palette._Transparent,
            };

            //Label labelTitle = new Label()
            //{
            //    Text = this._model.CurrentMediaContent.MediaTitle,
            //    LineBreakMode = LineBreakMode.TailTruncation,
            //    Style = (Style) App.CurrentApp.Resources["labelStyleInfoHeading"],
            //    HorizontalOptions = LayoutOptions.Start,
            //    Margin = new Thickness(3, 0, 0, 0)
            //};
            //Label labelSets = new Label()
            //{
            //    Text = (!string.IsNullOrEmpty(_model.CurrentMediaContent.SetsAndRepeats)
            //        ? " [" + this._model.CurrentMediaContent.SetsAndRepeats + "]"
            //        : ""),
            //    LineBreakMode = LineBreakMode.TailTruncation,
            //    Style = (Style) App.CurrentApp.Resources["labelStyleInfoHeading"],
            //    HorizontalOptions = LayoutOptions.StartAndExpand,
            //};

            //var imageSize = App.Configuration.GetImageSizeByID(ImageIdentity.TOP_BAR_CLOSE);
            //Image imageClose = new Image()
            //{
            //    Source = ImageResizer.ResizeImage(TextResources.icon_close, imageSize),
            //    Style = (Style) App.CurrentApp.Resources["imagePopupClose"],
            //    //Margin = new Thickness(0, 2, 5, 2),
            //    Margin = new Thickness(0, 0, 0, 5),
            //    HorizontalOptions = LayoutOptions.Center
            //};

            var closeImageSize = App.Configuration.GetImageSizeByID(ImageIdentity.BADGE_HINT_WINDOW_CLOSE);
            var closeImage = new Image()
            {
                Source = ImageResizer.ResizeImage(TextResources.icon_BadgeCloseCircle, closeImageSize),
                Style = (Style) App.CurrentApp.Resources["imageBadgeHintClose"],
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(0),
                WidthRequest = 55,
                HeightRequest = 55
            };
            
            GestureRecognizer gestureRecognizer = new TapGestureRecognizer()
            {
                Command = new Command(CloseWindow)
            };
            closeImage.GestureRecognizers.Add(gestureRecognizer);
            //if (imageSize != null)
            //{
            //    imageClose.HeightRequest = imageSize.Height;
            //    imageClose.WidthRequest = imageSize.Width;
            //}

            //stackLayoutTitle.Children.Add(labelTitle);
            //stackLayoutTitle.Children.Add(labelSets);
            stackLayoutTitle.Children.Add(closeImage);

            var videoView = new VideoView()
            {
                HeightRequest = height,
                WidthRequest = width,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                BackgroundColor = Palette._Transparent
            };
            videoView.SetBinding(VideoView.SourceProperty, new Binding("Source", BindingMode.OneWay, null, null, "{0}"));

            stackLayout.Children.Add(stackLayoutTitle);
            stackLayout.Children.Add(videoView);
            popupLayout.ShowPopup(stackLayout);
            this._model.UpdateCurrentMedia();
        }

        public void CloseWindow()
        {
            popupLayout = this.Content as PopupLayout;
            if (popupLayout.IsPopupActive)
            {
                _model.StopPlayer();
                this._model.IsPlaying = false;
                popupLayout.DismissPopup();
            }
        }

        protected override void OnDisappearing()
        {
            _model.StopPlayer();
            base.OnDisappearing();
        }
    }

    public abstract class PlaylistPageXaml : ModelBoundContentPage<PlaylistViewModel>
    {
    }
}