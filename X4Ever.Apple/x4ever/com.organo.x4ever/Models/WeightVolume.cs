namespace com.organo.x4ever.Models
{
    public sealed class WeightVolume
    {
        public WeightVolume()
        {
            ID = 0;
            ApplicationID = 0;
            VolumeCode = string.Empty;
            VolumeName = string.Empty;
        }

        public int ID { get; set; }
        public int ApplicationID { get; set; }
        public string VolumeCode { get; set; }
        public string VolumeName { get; set; }
        public string DisplayVolume => VolumeName + " (" + VolumeCode + ")";
    }
}