using System;
using System.Collections.Generic;

namespace com.organo.x4ever.Models
{
    public class MealPlanOptionDetail
    {
        public short ID { get; set; }
        public short MealPlanID { get; set; }
        public string LanguageCode { get; set; }
        public string MealOptionTitle { get; set; }
        public string MealOptionPhoto { get; set; }
        public string MealOptionSubtitle { get; set; }
        public string MealOptionDesc { get; set; }
        public short DisplaySequence { get; set; }
        public bool Active { get; set; }
        public DateTime ModifyDate { get; set; }
        public string ModifiedBy { get; set; }
        public List<MealPlanOptionGridDetail> MealPlanOptionGridDetails { get; set; }
        public List<MealPlanOptionListDetail> MealPlanOptionListDetails { get; set; }
    }
}