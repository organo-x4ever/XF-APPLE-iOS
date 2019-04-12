using com.organo.x4ever.Extensions;
using com.organo.x4ever.Globals;
using com.organo.x4ever.Localization;
using com.organo.x4ever.Models;
using com.organo.x4ever.Models.Validation;
using com.organo.x4ever.Pages.Base;
using com.organo.x4ever.Permissions;
using com.organo.x4ever.Services;
using com.organo.x4ever.Statics;
using com.organo.x4ever.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using com.organo.x4ever.Utilities;
using Xamarin.Forms;

namespace com.organo.x4ever.Pages.Account
{
    public partial class UploadPhotoPage : UploadPhotoPageXaml
    {
        private UploadPhotoViewModel _model;
        private readonly IMedia _media;
        private readonly UserFirstUpdate _user;
        private readonly ITrackerPivotService _trackerPivotService;
        private readonly IHelper _helper;

        public UploadPhotoPage(UserFirstUpdate user, bool error = false, string message = "")
        {
            try
            {
                InitializeComponent();
                _user = user;
                _trackerPivotService = DependencyService.Get<ITrackerPivotService>();
                _media = DependencyService.Get<IMedia>();
                _helper = DependencyService.Get<IHelper>();
                Init();
                if (error)
                    _model.SetActivityResource(showError: true,
                        errorMessage: string.IsNullOrWhiteSpace(message.Trim())
                            ? message.Trim()
                            : _helper.ReturnMessage(message));
            }
            catch (Exception ex)
            {
                ClientService.WriteLog(null, ex).GetAwaiter();
            }
        }

        public sealed override void Init(object obj = null)
        {
            App.Configuration.Initial(this);
            NavigationPage.SetHasNavigationBar(this, false);
            _model = new UploadPhotoViewModel();
            BindingContext = _model;
            Initialization();
        }

        private void Initialization()
        {
            if (_user.UserTrackers != null && _user.UserTrackers.Count > 0)
            {
                var frontPath = _user.UserTrackers.Get(TrackerEnum.frontimage);
                if (!string.IsNullOrEmpty(frontPath))
                    _model.ImageFront = frontPath;

                var sidePath = _user.UserTrackers.Get(TrackerEnum.frontimage);
                if (!string.IsNullOrEmpty(sidePath))
                    _model.ImageSide = sidePath;
            }

            var tapImageFront = new TapGestureRecognizer()
            {
                Command = new Command(async (obj) => { await UploadImageAsync(ImageSide.FRONT); })
            };
            imageFront.GestureRecognizers.Add(tapImageFront);

            var tapImageSide = new TapGestureRecognizer()
            {
                Command = new Command(async (obj) => { await UploadImageAsync(ImageSide.SIDE); })
            };
            imageSide.GestureRecognizers.Add(tapImageSide);

            buttonSubmit.Clicked += async (sender, e) => { await SubmitAsync(); };
        }

        private async Task UploadImageAsync(ImageSide side)
        {
            if (side == ImageSide.SIDE)
                _model.ImageSide = _model.ImageDefault;
            else
                _model.ImageFront = _model.ImageDefault;

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
                    {
                        _model.SetActivityResource(showError: true, errorMessage: _media.Message);
                        return;
                    }

                    await Task.Run(() => { _model.SetActivityResource(false, true); });
                    var response = await _media.UploadPhotoAsync(mediaFile);
                    if (!response)
                    {
                        _model.SetActivityResource(true, false, showError: true, errorMessage: _media.Message);
                        return;
                    }

                    _model.SetActivityResource();
                    if (_media.FileName != null)
                    {
                        if (side == ImageSide.SIDE)
                            _model.ImageSide = _media.FileName;
                        else
                            _model.ImageFront = _media.FileName;
                        (side == ImageSide.SIDE ? imageSide : imageFront).Source =
                            ImageSource.FromStream(() => mediaFile.GetStream());
                    }
                    else
                        _model.SetActivityResource(showError: true,
                            errorMessage: _media.Message);
                }
                else if (result == TextResources.TakeFromCamera)
                {
                    var mediaFile = await _media.TakePhotoAsync();
                    if (mediaFile == null)
                        _model.SetActivityResource(showError: true, errorMessage: _media.Message);
                    else
                    {
                        await Task.Run(() => { _model.SetActivityResource(false, true); });
                        var response = await _media.UploadPhotoAsync(mediaFile);
                        if (!response)
                        {
                            _model.SetActivityResource(true, false, showError: true, errorMessage: _media.Message);
                            return;
                        }

                        _model.SetActivityResource();
                        if (!string.IsNullOrEmpty(_media.FileName))
                        {
                            if (side == ImageSide.SIDE)
                                _model.ImageSide = _media.FileName;
                            else
                                _model.ImageFront = _media.FileName;
                            (side == ImageSide.SIDE ? imageSide : imageFront).Source =
                                ImageSource.FromStream(() => mediaFile.GetStream());
                        }
                        else
                            _model.SetActivityResource(showError: true,
                                errorMessage: _media.Message);
                    }
                }
            }
        }

        private List<Models.User.Tracker> _trackers;

        private async Task SubmitAsync()
        {
            _model.SetActivityResource(false, true);
            if (Validate())
            {
                _trackers = new List<Models.User.Tracker>();
                _trackers.Add(_trackerPivotService.AddTracker(TrackerConstants.FRONT_IMAGE, _model.ImageFront));
                _trackers.Add(_trackerPivotService.AddTracker(TrackerConstants.SIDE_IMAGE, _model.ImageSide));
                foreach (var tracker in _trackers)
                {
                    tracker.RevisionNumber = "10000";
                    _user.UserTrackers.Add(tracker);
                }

                if (await _trackerPivotService.SaveTrackerStep3Async(_trackers, true))
                    App.GoToAccountPage(true);
                else
                    _model.SetActivityResource(showError: true,
                        errorMessage: _helper.ReturnMessage(_trackerPivotService.Message));
            }
        }

        private bool Validate()
        {
            ValidationErrors validationErrors = new ValidationErrors();
#if DEBUG
            if (string.IsNullOrEmpty(_model.ImageFront) || _model.ImageFront == _model.ImageDefault)
                _model.ImageFront = "Uploads/no.png";
            if (string.IsNullOrEmpty(_model.ImageSide) || _model.ImageSide == _model.ImageDefault)
                _model.ImageSide = "Uploads/no.png";
#endif
            if (_model.ImageFront == _model.ImageDefault)
                validationErrors.Add(string.Format(TextResources.Required_IsMandatory, TextResources.FrontPhoto));
            if (_model.ImageSide == _model.ImageDefault)
                validationErrors.Add(string.Format(TextResources.Required_IsMandatory, TextResources.SidePhoto));

            if (validationErrors.Count() > 0)
                _model.SetActivityResource(showError: true, errorMessage: validationErrors.Show(CommonConstants.SPACE));
            return validationErrors.Count() == 0;
        }
    }

    public abstract class UploadPhotoPageXaml : ModelBoundContentPage<UploadPhotoViewModel>
    {
    }
}