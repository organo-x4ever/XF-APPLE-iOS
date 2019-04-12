using System;
using com.organo.x4ever.Localization;

namespace com.organo.x4ever.Models.User
{
    public class UserMeta
    {
        public bool IsOGXProductPurchased { get; set; }
        public short Age { get; set; }
        public string Gender { get; set; }
        public double WeightToLose { get; set; }
        public string WhyJoiningChallenge { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string ProfilePhoto { get; set; }
        public DateTime ModifyDate { get; set; }
        public double WeightGoal { get; set; }
        public string WeightGoalDisplay => WeightGoal + App.Configuration.AppConfig.DefaultWeightVolume;
        public string WeightToLoseDisplay => WeightToLose + App.Configuration.AppConfig.DefaultWeightVolume;
        public string TargetDurationDisplay => "";

        public string ProfilePhotoWithUrl =>
            ProfilePhoto != null ? App.Configuration.AppConfig.BaseUrl + "" + ProfilePhoto : "";

        public string ModifyDateDisplay =>
            string.Format(TextResources.DateDisplayFormat, ModifyDate); // "Sunday, March 9, 2008"
    }
}