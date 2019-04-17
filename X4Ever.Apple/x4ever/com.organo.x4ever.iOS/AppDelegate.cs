
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using com.organo.x4ever.Handler;
using Foundation;
using UIKit;
using com.organo.x4ever.ios.Notification;
using com.organo.x4ever.ios.Renderers;
using com.organo.x4ever.Services;
using com.organo.x4ever.Statics;
using Plugin.MediaManager.Forms.iOS;
using UserNotifications;
using Xamarin.Forms;
using System.Text;

namespace com.organo.x4ever.ios
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        private static readonly string TAG = typeof(AppDelegate).FullName;
        private ISecureStorage _secureStorage;

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            VideoViewRenderer.Init();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
            ImageCircleRenderer.Init();
            _secureStorage = DependencyService.Get<ISecureStorage>();
            try
            {
                // check for a notification
                if (options != null)
                {
                    // check for a local notification
                    if (options.ContainsKey(UIApplication.LaunchOptionsLocalNotificationKey))
                    {
                        if (options[UIApplication.LaunchOptionsLocalNotificationKey] is UILocalNotification
                            localNotification)
                        {
                            var action = localNotification.AlertAction;
                            var body = localNotification.AlertBody;
                            var alert = new UIAlertView()
                            {
                                Title = action,
                                Message = body
                            };
                            alert.AddButton("OK");
                            alert.Show();

                            // reset our badge
                            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
                        }
                    }

                    // check for a remote notification
                    if (options.ContainsKey(UIApplication.LaunchOptionsRemoteNotificationKey))
                    {
                        if (options[UIApplication.LaunchOptionsRemoteNotificationKey] is NSDictionary remoteNotification
                        )
                        {
                            //new UIAlertView(remoteNotification.AlertAction, remoteNotification.AlertBody, null, "OK", null).Show();
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                new ExceptionHandler(TAG, ex);
            }

            try
            {
                // REGISTER : REMOTE PUSH NOTIFICATION
                //#if DEBUG
                //            // NO PUSH NOTIFICATION REGISTRATION FOR DEBUGGING MODE
                //#else
                if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
                {
                    var pushSettings = UIUserNotificationSettings.GetSettingsForTypes(
                        UIUserNotificationType.Alert |
                        UIUserNotificationType.Badge |
                        UIUserNotificationType.Sound,
                        new NSSet());

                    UIApplication.SharedApplication.RegisterUserNotificationSettings(pushSettings);

                    // register for remote notifications
                    UIApplication.SharedApplication.RegisterForRemoteNotifications();
                }
                else
                {
                    //==== register for remote notifications and get the device token
                    // set what kind of notification types we want
                    UIRemoteNotificationType notificationTypes =
                        UIRemoteNotificationType.Alert |
                        UIRemoteNotificationType.Badge |
                        UIRemoteNotificationType.Sound;

                    // register for remote notifications
                    UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(notificationTypes);
                }
                //#endif

                // Watch for notifications while the app is active
                UNUserNotificationCenter.Current.Delegate = new UserNotificationCenterDelegate();

                // Get current notification settings
                UNUserNotificationCenter.Current.GetNotificationSettings((settings) =>
                {
                    var alertsAllowed = (settings.AlertSetting == UNNotificationSetting.Enabled);
                });
            }
            catch (System.Exception ex)
            {
                new ExceptionHandler(TAG, ex);
            }

            LoadApplication(new App());
            return base.FinishedLaunching(app, options);
        }

        // Error/Exception Handling
        private static void TaskSchedulerOnUnobservedTaskException(object sender,
            UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
        {
            try
            {
                var newExc = new System.Exception("TaskSchedulerOnUnobservedTaskException",
                    unobservedTaskExceptionEventArgs.Exception);
                LogUnhandledException(newExc);
            }
            catch
            {
                //
            }
        }

        private static void CurrentDomainOnUnhandledException(object sender,
            UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            try
            {
                var newExc = new System.Exception("CurrentDomainOnUnhandledException",
                    unhandledExceptionEventArgs.ExceptionObject as System.Exception);
                LogUnhandledException(newExc);
            }
            catch
            {
                //
            }
        }

        private static void LogUnhandledException(System.Exception exception)
        {
            //var libraryPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            //// iOS: Environment.SpecialFolder.Resources
            //var errorFilePath = Path.Combine(libraryPath, ErrorFileName);
            //File.WriteAllText(errorFilePath, errorMessage);

            // Log to Android Device Logging.
            //Android.Util.Log.Error("Crash Report", errorMessage);
            // just suppress any error logging exceptions
            CollectionCrashReport(
                $"Time: {DateTime.Now}\r\nError: Unhandled Exception\r\n{exception?.ToString() ?? ""}");
        }

        //// If there is an unhandled exception, the exception information is diplayed 
        //// on screen the next time the app is started (only in debug configuration)
        //private static void DisplayCrashReport()
        //{
        //    const string errorFilename = "Fatal.log";
        //    var libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Resources);
        //    var errorFilePath = Path.Combine(libraryPath, errorFilename);

        //    if (!File.Exists(errorFilePath))
        //    {
        //        return;
        //    }

        //    var errorText = File.ReadAllText(errorFilePath);
        //    var alertView = new UIAlertView("Crash Report", errorText, null, "Close", "Clear")
        //        {UserInteractionEnabled = true};
        //    alertView.Clicked += (sender, args) =>
        //    {
        //        if (args.ButtonIndex != 0)
        //        {
        //            File.Delete(errorFilePath);
        //        }
        //    };
        //    alertView.Show();
        //}
        private static void CollectionCrashReport(string message) => new ExceptionHandler(TAG, message);

        // Error/Exception Handling
        public override void OnActivated(UIApplication uiApplication)
        {
            // reset our badge
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
            base.OnActivated(uiApplication);
        }

        public override void ReceivedLocalNotification(UIApplication application, UILocalNotification notification)
        {
            // show an alert
            var action = notification.AlertAction;
            var body = notification.AlertBody;
            var alert = new UIAlertView()
            {
                Title = action,
                Message = body
            };
            alert.AddButton("OK");
            alert.Show();

            // reset our badge
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
        }

        /// <summary>
        /// The iOS will call the APNS in the background and issue a device token to the device. when that's
        /// accomplished, this method will be called.
        ///
        /// Note: the device token can change, so this needs to register with your server application everytime
        /// this method is invoked, or at a minimum, cache the last token and check for a change.
        /// </summary>
        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            try
            {
                // Get current device token
                var DeviceToken = deviceToken.Description;
                if (!string.IsNullOrWhiteSpace(DeviceToken))
                    DeviceToken = DeviceToken.Trim('<').Trim('>');

                // Get previous device token
                var oldDeviceToken = _secureStorage.RetrieveStringFromBytes(Keys.DEVICE_TOKEN_IDENTITY) ?? "";

                // Has the token changed?
                if (!oldDeviceToken.Equals(DeviceToken))
                {
                    //TODO: Put your own logic here to notify your server that the device token has changed/been created!
                    // Save new device token
                    _secureStorage.StoreBytesFromString(Keys.DEVICE_TOKEN_IDENTITY, DeviceToken);
                }
            }
            catch (System.Exception ex)
            {
                new ExceptionHandler(TAG, ex);
            }
        }

        /// <summary>
        /// Registering for push notifications can fail, for instance, if the device doesn't have network access.
        ///
        /// In this case, this method will be called.
        /// </summary>
        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            //new UIAlertView("Your device is not connected to the internet", error.LocalizedDescription, null, "OK",
            //    null).Show();
            //base.FailedToRegisterForRemoteNotifications(application, error);
            // show an alert
            // Notification : LOCAL PUSH NOTIFICATION
#if LIVE
            var action = "Error registering push notifications";
            var body = error.LocalizedDescription;
            var alert = new UIAlertView()
            {
                Title = action,
                Message = body
            };
            alert.AddButton("OK");
            alert.Show();
#endif
        }
    }
}