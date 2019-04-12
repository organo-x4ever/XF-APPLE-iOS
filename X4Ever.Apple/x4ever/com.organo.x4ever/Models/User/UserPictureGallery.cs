using System;
using System.Collections.Generic;
using com.organo.x4ever.Localization;

namespace com.organo.x4ever.Models.User
{
    public class UserPicture
    {
        public string FrontImage { get; set; }
        public string SideImage { get; set; }

        public string FrontImageWithUrl =>
            FrontImage != null ? App.Configuration.AppConfig.BaseUrl + "" + FrontImage : "";

        public string SideImageWithUrl => SideImage != null ? App.Configuration.AppConfig.BaseUrl + "" + SideImage : "";
    }

    public class UserPictureGallery
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public DateTime ModifyDate => new DateTime(Year, Month, Day);

        public string ModifyDateDisplay =>
            string.Format(TextResources.DateDisplayFormat, ModifyDate); // "Sunday, March 9, 2008"

        public List<UserPicture> UserPictures { get; set; }
    }
}