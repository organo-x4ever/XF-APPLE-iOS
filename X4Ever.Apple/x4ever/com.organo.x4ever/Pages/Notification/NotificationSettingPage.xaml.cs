using com.organo.x4ever.Pages.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.organo.x4ever.ViewModels.Notification;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace com.organo.x4ever.Pages.Notification
{
    public partial class NotificationSettingPage : NotificationSettingPageXaml
    {
        private readonly NotificationSettingViewModel _model;

        public NotificationSettingPage()
        {
            try
            {
                InitializeComponent();
                _model = new NotificationSettingViewModel()
                {
                    CreateAllEventAction = CreateEvents
                };
                Init();
            }
            catch (Exception ex)
            {
                _ = ex;
            }
        }

        public sealed override async void Init(object obj = null)
        {
            await App.Configuration.InitialAsync(this);
            NavigationPage.SetHasNavigationBar(this, true);
            BindingContext = _model;
        }

        private async void CreateEvents()
        {
            await Task.Delay(TimeSpan.FromMilliseconds(1000));

            switchWeightSubmitReminder.Toggled -= (sender, e) => { };
            switchWeightSubmitReminder.Toggled += async (sender, e) =>
            {
                await _model.Update(NotifyType.WEIGHT_SUBMIT_REMINDER, e.Value);
            };

            switchGeneralMessage.Toggled -= (sender, e) => { };
            switchGeneralMessage.Toggled += async (sender, e) =>
            {
                await _model.Update(NotifyType.GENERAL_MESSAGE, e.Value);
            };

            switchPromotional.Toggled -= (sender, e) => { };
            switchPromotional.Toggled += async (sender, e) => { await _model.Update(NotifyType.PROMOTIONAL, e.Value); };

            switchSpecialOffer.Toggled -= (sender, e) => { };
            switchSpecialOffer.Toggled += async (sender, e) =>
            {
                await _model.Update(NotifyType.SPECIAL_OFFER, e.Value);
            };

            switchVersionUpdate.Toggled -= (sender, e) => { };
            switchVersionUpdate.Toggled += async (sender, e) =>
            {
                await _model.Update(NotifyType.VERSION_UPDATE, e.Value);
            };
        }
    }

    public abstract class NotificationSettingPageXaml : ModelBoundContentPage<NotificationSettingViewModel>
    {
    }
}