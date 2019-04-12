using com.organo.xchallenge.Pages.Base;
using com.organo.xchallenge.ViewModels.Milestone;
using System;
using com.organo.xchallenge.Extensions;

namespace com.organo.xchallenge.Pages.Milestone
{
    public partial class BasicMilestonePage : BasicMilestonePageXaml
    {
        private MilestoneViewModel _model;

        public BasicMilestonePage(MilestoneViewModel model)
        {
            InitializeComponent();
            this._model = model;
            this.Init();
        }

        public sealed override async void Init(object obj = null)
        {
            BindingContext = this._model;
            sliderCurrentWeight.ValueChanged += (sender, e) =>
            {
                if ((short) e.NewValue < App.Configuration.AppConfig.MINIMUM_WEIGHT_LOSE)
                    sliderCurrentWeight.Value = _model.CurrentWeightValue;
                else
                    _model.CurrentWeightValue = (short) e.NewValue;
            };
            _model.ViewComponents.Add(sliderCurrentWeight);
            sliderCurrentWeight.SetMinValueAsync(App.Configuration.AppConfig.MINIMUM_WEIGHT_LOSE);
        }
    }

    public abstract class BasicMilestonePageXaml : ModelBoundContentPage<MilestoneViewModel>
    {
    }
}