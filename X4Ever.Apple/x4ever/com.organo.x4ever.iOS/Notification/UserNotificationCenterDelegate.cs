using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using UserNotifications;

namespace com.organo.x4ever.ios.Notification
{
    public class UserNotificationCenterDelegate : UNUserNotificationCenterDelegate
    {
        #region Constructors

        public UserNotificationCenterDelegate()
        {

        }

        #endregion

        #region Override Methods

        public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification,
            Action<UNNotificationPresentationOptions> completionHandler)
        {

            // Do something with the notification
            //Console.Write("Active Notification: {0}", notification);

            // Tell system to display the notification anyway or use
            // 'None' to say we have handled the display locally.
            completionHandler(UNNotificationPresentationOptions.Alert);
            //base.WillPresentNotification(center, notification, completionHandler);
        }

        #endregion
    }
}