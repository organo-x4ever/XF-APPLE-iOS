namespace com.organo.x4ever.Models
{
    public sealed class ApplicationLanguage
    {
        public ApplicationLanguage()
        {
            ApplicationID = 0;
            CountryID = 0;
            CountryCode = string.Empty;
            CountryName = string.Empty;
            CountryFlag = string.Empty;
            LanguageID = 0;
            LanguageCode = string.Empty;
            LanguageName = string.Empty;
            LanguageNameEnglish = string.Empty;

            DisplayLanguageCode = string.Empty;
            DisplayCountryLanguage = string.Empty;
            IsSelected = false;
        }

        public int ApplicationID { get; set; }
        public int CountryID { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string CountryFlag { get; set; }
        public int LanguageID { get; set; }
        public string LanguageCode { get; set; }
        public string LanguageName { get; set; }
        public string LanguageNameEnglish { get; set; }
        public string DisplayLanguageCode { get; set; }
        public string DisplayCountryLanguage { get; set; }
        public bool IsSelected { get; set; }
    }

    public class ApplicationLanguageRequest
    {
        public ApplicationLanguageRequest()
        {
            LanguageCode = string.Empty;
            LanguageName = string.Empty;
        }

        public string LanguageCode { get; set; }
        public string LanguageName { get; set; }
    }

    public class UserWeightVolume
    {
        public UserWeightVolume()
        {
            WeightVolume = string.Empty;
        }

        public string WeightVolume { get; set; }
    }
}