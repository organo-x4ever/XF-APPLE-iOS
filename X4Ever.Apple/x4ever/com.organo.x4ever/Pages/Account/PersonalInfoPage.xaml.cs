using com.organo.x4ever.Extensions;
using com.organo.x4ever.Localization;
using com.organo.x4ever.Models;
using com.organo.x4ever.Models.Validation;
using com.organo.x4ever.Pages.Base;
using com.organo.x4ever.Services;
using com.organo.x4ever.Statics;
using com.organo.x4ever.ViewModels.Account;
using System;
using System.Linq;
using System.Threading.Tasks;
using com.organo.x4ever.Converters;
using com.organo.x4ever.Globals;
using Xamarin.Forms;

namespace com.organo.x4ever.Pages.Account
{
    public partial class PersonalInfoPage : PersonalInfoPageXaml
    {
        private PersonalInfoViewModel _model;
        private UserFirstUpdate _user;
        private readonly IMetaPivotService _metaPivotService;
        private readonly ITrackerPivotService _trackerPivotService;
        private readonly IHelper _helper;
        private readonly PoundToKiligramConverter _converter = new PoundToKiligramConverter();

        public PersonalInfoPage(UserFirstUpdate user)
        {
            InitializeComponent();
            _user = user;
            _metaPivotService = DependencyService.Get<IMetaPivotService>();
            _trackerPivotService = DependencyService.Get<ITrackerPivotService>();
            _helper = DependencyService.Get<IHelper>();
            Init();
        }

        public sealed override async void Init(object obj = null)
        {
            await App.Configuration.InitialAsync(this);
            NavigationPage.SetHasNavigationBar(this, false);
            _model = new PersonalInfoViewModel()
            {
                SliderAgeModel = sliderAge,
                SliderCurrentWeightModel = sliderCurrentWeight,
                SliderWeightLossGoalModel = sliderWeightLossGoal
            };
            _model.SetSliders();
            BindingContext = _model;

            if (_user?.UserMetas?.Count > 0)
            {
                if (short.TryParse(_user.UserMetas.ToList().Get(MetaEnum.age), out short age) && age > 0)
                    _model.AgeValue = age;

                if (double.TryParse(_user.UserMetas.ToList().Get(MetaEnum.weighttolose),
                        out double weightLossGoal) && weightLossGoal > 0)
                    _model.WeightLossGoalValue = _converter.DisplayWeightVolume(weightLossGoal);
            }

            if (_user?.UserTrackers?.Count > 0)
            {
                if (double.TryParse(_user.UserTrackers.ToList().Get(TrackerEnum.currentweight),
                    out double weight))
                    _model.CurrentWeightValue = _converter.DisplayWeightVolume(weight);
            }

            buttonNext.Clicked += async (sender, e) => { await NextStepAsync(); };
        }

        private async Task NextStepAsync()
        {
            _model.SetActivityResource(false, true, busyMessage: TextResources.ProcessingPleaseWait);

            if (Validate())
            {
                _user.UserMetas.Add(_metaPivotService.AddMeta(_model.AgeValue.ToString(), MetaConstants.AGE,
                    MetaConstants.AGE, MetaConstants.LABEL));
                
                var tracker = _trackerPivotService.AddTracker(TrackerConstants.CURRENT_WEIGHT,
                    _model.CurrentWeightValue.ToString());
                tracker.RevisionNumber = "10000";
                _user.UserTrackers.Add(tracker);
                
                tracker = _trackerPivotService.AddTracker(TrackerConstants.CURRENT_WEIGHT_UI,
                    _model.CurrentWeightValue.ToString());
                tracker.RevisionNumber = "10000";
                _user.UserTrackers.Add(tracker);

                tracker = _trackerPivotService.AddTracker(TrackerConstants.WEIGHT_VOLUME_TYPE,
                    App.Configuration.AppConfig.DefaultWeightVolume);
                tracker.RevisionNumber = "10000";
                _user.UserTrackers.Add(tracker);

                _user.UserMetas.Add(_metaPivotService.AddMeta(_model.WeightLossGoalValue.ToString(),
                    MetaConstants.WEIGHT_LOSS_GOAL, MetaConstants.WEIGHT_LOSS_GOAL, MetaConstants.LABEL));
                
                _user.UserMetas.Add(_metaPivotService.AddMeta(_model.WeightLossGoalValue.ToString(),
                    MetaConstants.WEIGHT_LOSS_GOAL_UI, MetaConstants.WEIGHT_LOSS_GOAL_UI, MetaConstants.LABEL));

                _user.UserMetas.Add(_metaPivotService.AddMeta(App.Configuration.AppConfig.DefaultWeightVolume,
                    MetaConstants.WEIGHT_VOLUME_TYPE, MetaConstants.WEIGHT_VOLUME_TYPE, MetaConstants.LABEL));

                var response = await _metaPivotService.SaveMetaStep2Async(_user.UserMetas);
                if (response)
                {
                    var result = await _trackerPivotService.SaveTrackerStep3Async(_user.UserTrackers);
                    if (result)
                        App.CurrentApp.MainPage = new AddressPage(_user);
                    else
                        _model.SetActivityResource(showError: true,
                            errorMessage: _helper.ReturnMessage(_trackerPivotService.Message));
                }
                else
                    _model.SetActivityResource(showError: true,
                        errorMessage: _helper.ReturnMessage(_metaPivotService.Message));
            }
        }

        private bool Validate()
        {
            ValidationErrors validationErrors = new ValidationErrors();
            if (_model.AgeValue == 0)
                validationErrors.Add(string.Format(TextResources.Required_IsMandatory, TextResources.YourAge));
            else if (_model.AgeValue < App.Configuration.AppConfig.MINIMUM_AGE)
                validationErrors.Add(string.Format(TextResources.Validation_MustBeMoreThan, TextResources.YourAge,
                    App.Configuration.AppConfig.MINIMUM_AGE));

            if (_model.CurrentWeightValue == 0)
                validationErrors.Add(string.Format(TextResources.Required_IsMandatory,
                    TextResources.YourCurrentWeight));
            else if (_model.CurrentWeightValue <
                     _converter.DisplayWeightVolume(App.Configuration.AppConfig.MINIMUM_CURRENT_WEIGHT_KG))
                validationErrors.Add(string.Format(TextResources.Validation_MustBeMoreThan,
                    TextResources.YourCurrentWeight,
                    _converter.DisplayWeightVolume(App.Configuration.AppConfig.MINIMUM_CURRENT_WEIGHT_KG)));

            if (_model.WeightLossGoalValue == 0)
                validationErrors.Add(
                    string.Format(TextResources.Required_IsMandatory, TextResources.WeightLossGoal));
            else if (_model.WeightLossGoalValue <
                     _converter.DisplayWeightVolume(App.Configuration.AppConfig.MINIMUM_WEIGHT_LOSE_KG))
                validationErrors.Add(string.Format(TextResources.Validation_MustBeMoreThan,
                    TextResources.WeightLossGoal,
                    _converter.DisplayWeightVolume(App.Configuration.AppConfig.MINIMUM_WEIGHT_LOSE_KG)));
            if (validationErrors.Count() > 0)
                _model.SetActivityResource(showError: true,
                    errorMessage: validationErrors.Count() > 2
                        ? TextResources.Required_AllInputs
                        : validationErrors.Show(CommonConstants.SPACE));
            return validationErrors.Count() == 0;
        }
    }

    public abstract class PersonalInfoPageXaml : ModelBoundContentPage<PersonalInfoViewModel>
    {
    }
}