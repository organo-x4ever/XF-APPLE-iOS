using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.organo.x4ever.Helpers;
using Foundation;
using UIKit;
using Xamarin.Forms;
using System.Drawing;
using System.IO;
using CoreGraphics;
using System.Threading.Tasks;
using com.organo.x4ever.Globals;
using com.organo.x4ever.Handler;
using com.organo.x4ever.ios;
using com.organo.x4ever.Models;

[assembly:Dependency(typeof(ImageResizerHelpers))]

namespace com.organo.x4ever.ios
{
    public class ImageResizerHelpers : IImageResizerHelpers
    {
        UIKit.UIImage resizedImage;

        private Type assembly
        {
            get { return this.GetType(); }
        }

        private string ResourceName
        {
            get { return assembly.Namespace?.ToLower() + ".Resources."; }
        }

        public byte[] ResizeImage(byte[] imageData, float width, float height)
        {
            UIImage originalImage = ImageFromByteArray(imageData);
            UIImageOrientation orientation = originalImage.Orientation;

            //create a 24bit RGB image
            using (CGBitmapContext context = new CGBitmapContext(IntPtr.Zero,
                (int) width, (int) height, 8,
                4 * (int) width, CGColorSpace.CreateDeviceRGB(),
                CGImageAlphaInfo.PremultipliedFirst))
            {

                RectangleF imageRect = new RectangleF(0, 0, width, height);

                // draw the image
                context.DrawImage(imageRect, originalImage.CGImage);

                UIKit.UIImage resizedImage = UIKit.UIImage.FromImage(context.ToImage(), 0, orientation);

                // save the image as a jpeg
                return resizedImage.AsPNG().ToArray();
            }
        }

        public byte[] ResizeImage(string fileName, float width, float height)
        {
            var resource = ResourceName + fileName;
            byte[] imageData;

            Stream stream = assembly.Assembly.GetManifestResourceStream(resource);
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                imageData = ms.ToArray();
            }

            return this.ResizeImage(imageData, width, height);
        }

        public async Task<byte[]> ResizeImageAsync(byte[] imageData, float width, float height)
        {
            await Task.Run(() =>
            {
                UIImage originalImage = ImageFromByteArray(imageData);
                UIImageOrientation orientation = originalImage.Orientation;


                //create a 24bit RGB image
                using (CGBitmapContext context = new CGBitmapContext(IntPtr.Zero,
                    (int) width, (int) height, 8,
                    4 * (int) width, CGColorSpace.CreateDeviceRGB(),
                    CGImageAlphaInfo.PremultipliedFirst))
                {

                    RectangleF imageRect = new RectangleF(0, 0, width, height);

                    // draw the image
                    context.DrawImage(imageRect, originalImage.CGImage);

                    resizedImage = UIKit.UIImage.FromImage(context.ToImage(), 0, orientation);
                }
            });
            // save the image as a jpeg
            return resizedImage.AsPNG().ToArray();
        }

        public async Task<byte[]> ResizeImageAsync(string fileName, float width, float height)
        {
            var assembly = this.GetType();
            var resource = ResourceName + fileName;
            byte[] imageData;
            Stream stream = assembly.Assembly.GetManifestResourceStream(resource);
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                imageData = ms.ToArray();
            }

            return await this.ResizeImageAsync(imageData, width, height);
        }

        public byte[] ResizeImage(byte[] imageData, ImageSize imageSize)
        {
            UIImage originalImage = ImageFromByteArray(imageData);
            UIImageOrientation orientation = originalImage.Orientation;

            //create a 24bit RGB image
            using (CGBitmapContext context = new CGBitmapContext(IntPtr.Zero,
                (int) imageSize.Width, (int) imageSize.Height, 8,
                4 * (int) imageSize.Width, CGColorSpace.CreateDeviceRGB(),
                CGImageAlphaInfo.PremultipliedFirst))
            {

                RectangleF imageRect = new RectangleF(0, 0, imageSize.Width, imageSize.Height);

                // draw the image
                context.DrawImage(imageRect, originalImage.CGImage);

                UIKit.UIImage resizedImage = UIKit.UIImage.FromImage(context.ToImage(), 0, orientation);

                // save the image as a jpeg
                return resizedImage.AsPNG().ToArray();
            }
        }

        public byte[] ResizeImage(string fileName, ImageSize imageSize)
        {
            var assembly = this.GetType();
            var resource = ResourceName + fileName;
            byte[] imageData;

            Stream stream = assembly.Assembly.GetManifestResourceStream(resource);
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                imageData = ms.ToArray();
            }

            return this.ResizeImage(imageData, imageSize.Width, imageSize.Height);
        }

        public async Task<byte[]> ResizeImageAsync(byte[] imageData, ImageSize imageSize)
        {
            await Task.Run(() =>
            {
                UIImage originalImage = ImageFromByteArray(imageData);
                UIImageOrientation orientation = originalImage.Orientation;


                //create a 24bit RGB image
                using (CGBitmapContext context = new CGBitmapContext(IntPtr.Zero,
                    (int) imageSize.Width, (int) imageSize.Height, 8,
                    4 * (int) imageSize.Width, CGColorSpace.CreateDeviceRGB(),
                    CGImageAlphaInfo.PremultipliedFirst))
                {

                    RectangleF imageRect = new RectangleF(0, 0, imageSize.Width, imageSize.Height);

                    // draw the image
                    context.DrawImage(imageRect, originalImage.CGImage);

                    resizedImage = UIKit.UIImage.FromImage(context.ToImage(), 0, orientation);
                }
            });
            // save the image as a jpeg
            return resizedImage.AsPNG().ToArray();
        }

        public async Task<byte[]> ResizeImageAsync(string fileName, ImageSize imageSize)
        {
            var assembly = this.GetType();
            var resource = ResourceName + fileName;
            byte[] imageData;
            Stream stream = assembly.Assembly.GetManifestResourceStream(resource);
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                imageData = ms.ToArray();
            }

            return await this.ResizeImageAsync(imageData, imageSize.Width, imageSize.Height);
        }

        public byte[] ResizeImage(ImageSize imageSize)
        {
            if (imageSize.ImageName == null)
                return null;
            var assembly = this.GetType();
            var resource = ResourceName + imageSize.ImageName;
            byte[] imageData;
            Stream stream = null;
            bool exists = true;

            stream = assembly.Assembly.GetManifestResourceStream(resource);
            if (stream == null)
            {
                exists = false;
                new ExceptionHandler(typeof(ImageResizerHelpers).FullName,
                    "Change image " + imageSize.ImageName + " property Build Action to Embedded Resource");
            }

            if (!exists)
            {
                resource = ResourceName + "no.png";
                stream = assembly.Assembly.GetManifestResourceStream(resource);
            }

            using (MemoryStream ms = new MemoryStream())
            {
                stream?.CopyTo(ms);
                imageData = ms.ToArray();
            }

            return this.ResizeImage(imageData, imageSize.Width, imageSize.Height);
        }

        public async Task<byte[]> ResizeImageAsync(ImageSize imageSize)
        {
            var assembly = this.GetType();
            var resource = ResourceName + imageSize.ImageName;
            byte[] imageData;
            Stream stream = assembly.Assembly.GetManifestResourceStream(resource);
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                imageData = ms.ToArray();
            }

            return await this.ResizeImageAsync(imageData, imageSize.Width, imageSize.Height);
        }

        UIKit.UIImage ImageFromByteArray(byte[] data)
        {
            if (data == null)
            {
                return null;
            }

            UIKit.UIImage image;
            try
            {
                image = new UIKit.UIImage(Foundation.NSData.FromArray(data));
            }
            catch (System.Exception e)
            {
                Console.WriteLine("Image load failed: " + e.Message);
                return null;
            }

            return image;
        }

        public byte[] ResizeImageFromRemote(ImageSize imageSize)
        {
            return this.ResizeImage(this.ImageToBytes(imageSize), imageSize);
        }

        public async Task<byte[]> ResizeImageFromRemoteAsync(ImageSize imageSize)
        {
            return await this.ResizeImageAsync(await ImageToBytesAsync(imageSize), imageSize);
        }

        public byte[] ImageToBytes(ImageSize imageSize)
        {
            byte[] imageData = null;
            FileInfo fileInfo = new FileInfo(imageSize.ImageName);
            long imageFileLength = fileInfo.Length;
            FileStream fs = new FileStream(imageSize.ImageName, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            imageData = br.ReadBytes((int) imageFileLength);
            return imageData;
        }

        public async Task<byte[]> ImageToBytesAsync(ImageSize imageSize)
        {
            return await Task.Factory.StartNew(() =>
            {
                byte[] imageData = null;
                FileInfo fileInfo = new FileInfo(imageSize.ImageName);
                long imageFileLength = fileInfo.Length;
                FileStream fs = new FileStream(imageSize.ImageName, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                imageData = br.ReadBytes((int) imageFileLength);
                return imageData;
            });
        }
    }
}