using System;

namespace com.organo.x4ever.Models
{
    public class MealPlanOptionGridDetail
    {
        public short ID { get; set; }
        public short MealPlanOptionID { get; set; }
        public string LanguageCode { get; set; }
        public string MealOptionVolume { get; set; }
        public string MealOptionVolumeType { get; set; }
        public string MealOptionShakeTitle { get; set; }
        public string MealOptionDetailPhoto { get; set; }
        public bool IsPhotoAvailable => MealOptionDetailPhoto != null;
        public short DisplaySequence { get; set; }
        public bool Active { get; set; }
        public DateTime ModifyDate { get; set; }
        public string ModifiedBy { get; set; }
    }
}