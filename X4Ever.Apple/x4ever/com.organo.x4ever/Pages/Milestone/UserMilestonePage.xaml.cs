
using com.organo.x4ever.Converters;
using com.organo.x4ever.Extensions;
using com.organo.x4ever.Globals;
using com.organo.x4ever.Localization;
using com.organo.x4ever.Pages.Base;
using com.organo.x4ever.Services;
using com.organo.x4ever.Utilities;
using com.organo.x4ever.ViewModels.Profile;
using System;
using System.Linq;
using System.Threading.Tasks;
using com.organo.x4ever.ViewModels.Milestone;
using Xamarin.Forms;

namespace com.organo.x4ever.Pages.Milestone
{
    public partial class UserMilestonePage : UserMilestonePageXaml
    {
        private readonly UserMilestoneViewModel _model;
        private readonly IMedia _media;
        private readonly PoundToKiligramConverter _converter = new PoundToKiligramConverter();

        public UserMilestonePage(RootPage root, MyProfileViewModel profileViewModel)
        {
            try
            {
                InitializeComponent();
                App.Configuration.Initial(this);
                NavigationPage.SetHasNavigationBar(this, false);
                _media = DependencyService.Get<IMedia>();
                _model = new UserMilestoneViewModel(App.CurrentApp.MainPage.Navigation)
                {
                    Root = root,
                    ProfileViewModel = profileViewModel,
                    // CHANGED
                    SliderCurrentWeight = sliderCurrentWeight,
                    WeightLossGoal = profileViewModel.YourGoal,
                    UserTrackers = profileViewModel.UserTrackers.OrderBy(t => t.ModifyDate).ToList(),
                    UserMetas = profileViewModel.UserDetail.MetaPivot
                };
                BindingContext = _model;
                _model.GetUserTracker();
                _model.ChangeSliderValue(0);
                Init();
            }
            catch (Exception ex)
            {
                ClientService.WriteLog(null, ex, true).GetAwaiter();
            }
        }

        public sealed override async void Init(object obj = null)
        {
            try
            {
                entryTShirtSize.Focused += async (sender, e) =>
                {
                    entryTShirtSize.Unfocus();
                    var shirtSizes = _model.GetTShirtSizeList();
                    var result = await DisplayActionSheet(TextResources.SelectTShirtSize, TextResources.Cancel, null,
                        shirtSizes.ToArray());
                    if (result != null && !result.Contains("Cancel"))
                    {
                        _model.TShirtSize = result.ToString();
                        entryAboutJourney.Focus();
                    }
                };
                Device.BeginInvokeOnMainThread(() =>
                {
                    var tapMale = new TapGestureRecognizer()
                    {
                        Command = new Command(_model.Male_Selected)
                    };
                    ImageMale.GestureRecognizers.Add(tapMale);
                    LabelMale.GestureRecognizers.Add(tapMale);

                    var tapFemale = new TapGestureRecognizer()
                    {
                        Command = new Command(_model.Female_Selected)
                    };
                    ImageFemale.GestureRecognizers.Add(tapFemale);
                    LabelFemale.GestureRecognizers.Add(tapFemale);

                    var tapImageFront = new TapGestureRecognizer()
                    {
                        Command = new Command(async (e) => { await UploadImageAsync(ImageSide.FRONT); })
                    };
                    imageFront.GestureRecognizers.Add(tapImageFront);

                    var tapImageSide = new TapGestureRecognizer()
                    {
                        Command = new Command(async (e) => { await UploadImageAsync(ImageSide.SIDE); })
                    };
                    imageSide.GestureRecognizers.Add(tapImageSide);

                    //    _model.ViewComponents.Add(sliderCurrentWeight);
                    //    _model.ViewComponents.Add(imageFront);
                    //    _model.ViewComponents.Add(imageSide);
                    //    _model.ViewComponents.Add(ImageMale);
                    //    _model.ViewComponents.Add(LabelMale);
                    //    _model.ViewComponents.Add(ImageFemale);
                    //    _model.ViewComponents.Add(LabelFemale);
                    //    _model.ViewComponents.Add(entryTShirtSize);
                    //    _model.ViewComponents.Add(pickerTShirtSize);
                    //    _model.ViewComponents.Add(entryAboutJourney);
                });
            }
            catch (Exception ex)
            {
                await ClientService.WriteLog(null, ex, true);
            }
        }

        private async Task UploadImageAsync(ImageSide side)
        {
            if (side == ImageSide.SIDE)
                _model.ImageSide = _model.ImageDefault;
            else
                _model.ImageFront = _model.ImageDefault;
            var localMessage = "";
            Device.BeginInvokeOnMainThread(async () =>
            {
                var result = await DisplayActionSheet(TextResources.ChooseOption, TextResources.Cancel, null,
                    new string[] {TextResources.PickFromGallery, TextResources.TakeFromCamera});

                if (result != null)
                {
                    if (result == "Cancel")
                        return;
                    _media.Refresh();
                    if (result == TextResources.PickFromGallery)
                    {
                        var mediaFile = await _media.PickPhotoAsync();
                        if (mediaFile == null)
                            localMessage = _media.Message;
                        else
                        {
                            await Task.Run(() => { _model.SetActivityResource(false, true); });
                            var response = await _media.UploadPhotoAsync(mediaFile);
                            if (response)
                            {
                                if (side == ImageSide.SIDE)
                                    _model.ImageSide = _media.FileName;
                                else
                                    _model.ImageFront = _media.FileName;
                                (side == ImageSide.SIDE ? imageSide : imageFront).Source =
                                    ImageSource.FromStream(() => { return mediaFile.GetStream(); });
                            }
                            else
                                localMessage = _media.Message;

                            _model.SetActivityResource();
                        }
                    }
                    else if (result == TextResources.TakeFromCamera)
                    {
                        var mediaFile = await _media.TakePhotoAsync();
                        if (mediaFile == null)
                            localMessage = _media.Message;
                        else
                        {
                            await Task.Run(() => { _model.SetActivityResource(false, true); });
                            var response = await _media.UploadPhotoAsync(mediaFile);
                            if (response)
                            {
                                if (side == ImageSide.SIDE)
                                    _model.ImageSide = _media.FileName;
                                else
                                    _model.ImageFront = _media.FileName;
                                (side == ImageSide.SIDE ? imageSide : imageFront).Source =
                                    ImageSource.FromStream(() => { return mediaFile.GetStream(); });
                            }
                            else
                                localMessage = _media.Message;

                            _model.SetActivityResource();
                        }
                    }
                }

                if (!string.IsNullOrEmpty(localMessage))
                    _model.SetActivityResource(showError: true,
                        errorMessage: localMessage);
            });
            await Task.Delay(1);
        }
    }

    public abstract class UserMilestonePageXaml : ModelBoundContentPage<UserMilestoneViewModel>
    {
    }
}