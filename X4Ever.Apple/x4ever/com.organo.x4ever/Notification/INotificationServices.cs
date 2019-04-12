using com.organo.x4ever.Localization;

namespace com.organo.x4ever.Notification
{
    public interface INotificationServices
    {
        void SendNotification(string requestID, string title, string subTitle, string body, int badge);

        void RemoveNotification(string requestID);

        void RemoveNotification(string[] requests);

        void SendExceptionMessage(string requestID, string title, string subTitle, string body, int badge);
    }

    public enum ActivityType
    {
        NONE = 0,
        WEIGHT_SUBMISSION_REQUIRED = 1,
        EXCEPTION_OCCURRED = 2
    }
}