using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using com.organo.x4ever.Notification;
using com.organo.x4ever.ios.Notification;
using Xamarin.Forms;
using UserNotifications;

[assembly: Dependency(typeof(NotificationServices))]

namespace com.organo.x4ever.ios.Notification
{
    public class NotificationServices : INotificationServices
    {
        // LINK : https://docs.microsoft.com/en-us/xamarin/ios/platform/user-notifications/enhanced-user-notifications?tabs=vswin

        public void RemoveNotification(string requestID)
        {
            // ID of Notification to be removed
            var requests = new string[] {requestID};

            // Remove notification from system
            UNUserNotificationCenter.Current.RemovePendingNotificationRequests(requests);
        }

        public void RemoveNotification(string[] requests)
        {
            // Remove notifications from system
            UNUserNotificationCenter.Current.RemovePendingNotificationRequests(requests);
        }

        public void SendExceptionMessage(string requestID, string title, string subTitle, string body, int badge)
        {

            #region SCHEDULE NOTIFICATION

            // ************** SCHEDULE NOTIFICATION ************** //

            //---- create the notification
            UILocalNotification notification = new UILocalNotification();

            //---- set the fire date (the date time in which it will fire)
            var fireDate = DateTime.Now.AddSeconds(10);
            notification.FireDate = (NSDate)fireDate;

            //---- configure the alert stuff
            notification.AlertTitle = title;
            notification.AlertAction = "View Alert";
            notification.AlertBody = body;

            //---- modify the badge
            notification.ApplicationIconBadgeNumber = badge;

            //---- set the sound to be the default sound
            notification.SoundName = UILocalNotification.DefaultSoundName;

            //notification.UserInfo = new NSDictionary();
            //notification.UserInfo[new NSString("Message")] = new NSString("Your 1 minute notification has fired!");

            //---- schedule it
            UIApplication.SharedApplication.ScheduleLocalNotification(notification);

            #endregion SCHEDULE NOTIFICATION
        }

        public void SendNotification(string requestID, string title, string subTitle, string body, int badge)
        {
            //// Rebuild notification
            //var content = new UNMutableNotificationContent()
            //{
            //    Title = title,
            //    Subtitle = subTitle,
            //    Body = body,
            //    Badge = badge,
            //    Sound = UNNotificationSound.Default
            //};

            //// New trigger time
            //var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(10, false);

            //// ID of Notification to be sent
            //var request = UNNotificationRequest.FromIdentifier(requestID, content, trigger);

            //// Add to system to modify existing Notification
            //UNUserNotificationCenter.Current.AddNotificationRequest(request, (err) =>
            //{
            //    if (err != null)
            //    {
            //        // Do something with error...
            //    }
            //});

            #region SCHEDULE NOTIFICATION

            // ************** SCHEDULE NOTIFICATION ************** //

            //---- create the notification
            UILocalNotification notification = new UILocalNotification();

            //---- set the fire date (the date time in which it will fire)
            var fireDate = DateTime.Now.AddSeconds(10);
            notification.FireDate = (NSDate)fireDate;

            //---- configure the alert stuff
            notification.AlertTitle = title;
            notification.AlertAction = "View Alert";
            notification.AlertBody = body;

            //---- modify the badge
            notification.ApplicationIconBadgeNumber = badge;

            //---- set the sound to be the default sound
            notification.SoundName = UILocalNotification.DefaultSoundName;

            //notification.UserInfo = new NSDictionary();
            //notification.UserInfo[new NSString("Message")] = new NSString("Your 1 minute notification has fired!");

            //---- schedule it
            UIApplication.SharedApplication.ScheduleLocalNotification(notification);

            #endregion SCHEDULE NOTIFICATION
        }
    }
}