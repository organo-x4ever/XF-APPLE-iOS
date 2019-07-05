using com.organo.x4ever.Pages.Base;
using com.organo.x4ever.ViewModels.MealPlan;
using System;
using Xamarin.Forms;

namespace com.organo.x4ever.Pages.MealPlan
{
    public partial class MealPlanPage : MealPlanPageXaml
    {
        private MealPlanViewModel _model;

        public MealPlanPage(RootPage root)
        {
            try
            {
                InitializeComponent();
                _model = new MealPlanViewModel(App.CurrentApp.MainPage.Navigation)
                {
                    Root = root,
                    BindDataSourceAction = () =>
                    {
                        AccordionMain.DataSource = _model.AccordionSources;
                        AccordionMain.DataBind();
                    },
                };
                Init();
            }
            catch (Exception exception)
            {
                var msg = exception;
            }
        }

        public sealed override async void Init(object obj = null)
        {
            await App.Configuration.InitialAsync(this);
            NavigationPage.SetHasNavigationBar(this, false);

            BindingContext = _model;
            AccordionMain.FirstExpaned = true;
            Page_Load();
        }

        protected async void Page_Load()
        {
            await _model.UpdateMealOptionSelected(MealOptionSelected.FullMeals);
        }
    }

    public abstract class MealPlanPageXaml : ModelBoundContentPage<MealPlanViewModel>
    {
    }
}