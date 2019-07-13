
namespace com.organo.x4ever.Extensions
{
    public static class DataCleaning
    {
        public static string Clean(this string data)
        {
            try
            {
                data = data.Replace("<string xmlns=\"http://schemas.microsoft.com/2003/10/Serialization/\">", "");
                data = data.Replace("<string xmlns=http://schemas.microsoft.com/2003/10/Serialization/>", "");
                data = data.Replace("</string>", "");
            }
            catch
            {
                //
            }
            return data;
        }
    }
}
