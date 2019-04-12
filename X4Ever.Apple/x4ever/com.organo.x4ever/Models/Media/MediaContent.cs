using System;
using com.organo.x4ever.Statics;
using Xamarin.Forms;

namespace com.organo.x4ever.Models.Media
{
    public class MediaFile
    {
        public MediaFile()
        {
            ID = 0;
            MediaCategoryID = 0;
            MediaTypeID = 0;
            MediaTitle = string.Empty;
            MediaUrl = string.Empty;
            SetsAndRepeats = string.Empty;
            TotalDuration = string.Empty;
            PreviewImageUrl = string.Empty;
            DisplaySequence = 0;
            CreateDate = new DateTime();
            MediaDescription = string.Empty;
            IsPlayingNow = false;
            MediaTitleColor = Palette._TitleTexts;
        }

        public int ID { get; set; }
        public short MediaCategoryID { get; set; }
        public short MediaTypeID { get; set; }
        public string MediaTitle { get; set; }
        public Color MediaTitleColor { get; set; }
        public string MediaUrl { get; set; }
        public string SetsAndRepeats { get; set; }
        public string TotalDuration { get; set; }
        public string PreviewImageUrl { get; set; }
        public short DisplaySequence { get; set; }
        public DateTime CreateDate { get; set; }
        public string MediaDescription { get; set; }
        public bool IsPlayingNow { get; set; }
        public string WorkoutLevel { get; set; }
        public short WorkoutWeek { get; set; }
        public short WorkoutDay { get; set; }
        public bool Active { get; set; }
    }
}