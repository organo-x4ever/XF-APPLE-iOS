using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace com.organo.x4ever.Extensions
{
    public static class ImageToByte
    {
        //public byte[] ToBytes(this Xamarin.Forms.Image image)
        //{
        //    using (var ms = new MemoryStream())
        //    {
        //        image.Save(ms, image.RawFormat);
        //        return ms.ToArray();
        //    }
        //}

        //public static byte[] ReadImageFile(string imageLocation)
        //{
        //    byte[] imageData = null;
        //    File fileInfo = new File(imageLocation);
        //    long imageFileLength = fileInfo.Length;
        //    FileStream fs = new FileStream(imageLocation, FileMode.Open, FileAccess.Read);
        //    BinaryReader br = new BinaryReader(fs);
        //    imageData = br.ReadBytes((int)imageFileLength);
        //    return imageData;
        //}
    }
}