using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.organo.x4ever.Models.Notifications
{
    public class UserNotificationSetting
    {
        public UserNotificationSetting()
        {
            IsGeneralMessage = false;
            Intimation = false;
            IsPromotional = false;
            IsSpecialOffer = false;
            IsVersionUpdate = false;
            IsWeightSubmitReminder = false;
        }

        public bool IsGeneralMessage { get; set; }
        public bool Intimation { get; set; }
        public bool IsPromotional { get; set; }
        public bool IsSpecialOffer { get; set; }
        public bool IsVersionUpdate { get; set; }
        public bool IsWeightSubmitReminder { get; set; }
    }
}