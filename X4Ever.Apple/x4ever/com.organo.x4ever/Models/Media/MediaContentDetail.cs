﻿using System;
using System.Collections.Generic;
using com.organo.x4ever.Localization;
using Xamarin.Forms;

namespace com.organo.x4ever.Models.Media
{
    public class MediaContentDetail
    {
        public MediaContentDetail()
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

            CategoryTitle = string.Empty;
            CategoryDescription = string.Empty;
            MediaTypeTitle = string.Empty;
            MediaTypeShortTitle = string.Empty;

            WorkoutLevel = string.Empty;
            WorkoutWeek = string.Empty;
            WorkoutDay = string.Empty;
            Active = false;
            IsHeader = false;
        }

        public int ID { get; set; }
        public short MediaCategoryID { get; set; }
        public short MediaTypeID { get; set; }
        public string MediaTitle { get; set; }
        public string MediaUrl { get; set; }
        public string SetsAndRepeats { get; set; }
        public string TotalDuration { get; set; }
        private string _previewImageUrl { get; set; }

        public string PreviewImageUrl
        {
            get => !string.IsNullOrEmpty(_previewImageUrl) ? _previewImageUrl : TextResources.ImageNotAvailable;
            set => _previewImageUrl = value;
        }

        public short DisplaySequence { get; set; }
        public DateTime CreateDate { get; set; }
        public string MediaDescription { get; set; }
        public bool IsPlayingNow { get; set; }
        public string CategoryTitle { get; set; }
        public string CategoryDescription { get; set; }
        public string MediaTypeTitle { get; set; }
        public string MediaTypeShortTitle { get; set; }

        public string WorkoutLevel { get; set; }
        public string WorkoutWeek { get; set; }
        public string WorkoutDay { get; set; }
        public bool Active { get; set; }

        public bool IsHeader { get; set; }
        public bool IsHeaderReverse => IsHeader == false;
        public Style TextStyle { get; set; }
    }

    public class WorkoutLevel
    {
        public int LevelSequence { get; set; }
        public string LevelDisplay { get; set; }
        public bool Selected { get; set; }
        public List<WorkoutWeek> WorkoutWeeks { get; set; }
    }

    public class WorkoutWeek
    {
        public string WeekSequence { get; set; }
        public string WeekDisplay { get; set; }
        public bool Selected { get; set; }
        public List<WorkoutDay> WorkoutDays { get; set; }
    }

    public class WorkoutDay
    {
        public string DaySequence { get; set; }
        public string DayDisplay { get; set; }
        public bool Selected { get; set; }
        public List<MediaContentDetail> MediaContentDetails { get; set; }
    }
}