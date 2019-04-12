namespace com.organo.x4ever.Models
{
    public sealed class MilestonePercentage
    {
        public short ID { get; set; }

        public string LanguageCode { get; set; }

        public string MilestoneTitle { get; set; }

        public string MilestoneSubTitle { get; set; }

        public short TargetValue { get; set; }

        public short TargetPercentValue { get; set; }

        public bool IsPercent { get; set; }

        public string AchievedMessage { get; set; }

        public string AchievementIcon { get; set; }

        public string AchievementGiftImage { get; set; }

        public bool Active { get; set; }
    }
}