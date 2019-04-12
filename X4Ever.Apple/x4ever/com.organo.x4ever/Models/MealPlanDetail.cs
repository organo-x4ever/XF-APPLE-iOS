using System;
using System.Collections.Generic;

namespace com.organo.x4ever.Models
{
    public class MealPlanDetail
    {
        public short ID { get; set; }
        public string LanguageCode { get; set; }
        public string MealTitle { get; set; }
        public string MealTitleCompare => MealTitle.Replace(" ", "").Replace("-", "").Replace("_", "");
        public string MealPlanPhoto { get; set; }
        public short DisplaySequence { get; set; }
        public bool Active { get; set; }
        public DateTime ModifyDate { get; set; }
        public string ModifiedBy { get; set; }
        public string ViewType { get; set; }
        public List<MealPlanOptionDetail> MealPlanOptionDetails { get; set; }
    }
}