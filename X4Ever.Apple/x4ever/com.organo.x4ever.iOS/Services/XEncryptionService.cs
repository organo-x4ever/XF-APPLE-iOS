using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using com.organo.x4ever.ios.Services;
using com.organo.x4ever.Services;
using Foundation;
using UIKit;
using Xamarin.Forms;

[assembly:Dependency(typeof(XEncryptionService))]

namespace com.organo.x4ever.ios.Services
{
    public class XEncryptionService : IXEncryptionService
    {
        private const string EncryptionSalt = "n73bc47sr19vw";
        private const string EncryptionPassword = "7passwordn73bc47sr19vw";

        public string Encrypt(string value)
        {
            var salt = Encoding.ASCII.GetBytes(EncryptionSalt);
            var key = new Rfc2898DeriveBytes(EncryptionPassword, salt);

            var algorithm = new RijndaelManaged();
            var bytesForKey = algorithm.KeySize / 8;
            var bytesForIv = algorithm.BlockSize / 8;
            algorithm.Key = key.GetBytes(bytesForKey);
            algorithm.IV = key.GetBytes(bytesForIv);

            byte[] encryptedBytes;
            using (var encryptor = algorithm.CreateEncryptor(algorithm.Key, algorithm.IV))
            {
                var bytesToEncrypt = Encoding.UTF8.GetBytes(value);
                encryptedBytes = InMemoryCrypt(bytesToEncrypt, encryptor);
            }

            return Convert.ToBase64String(encryptedBytes);
        }

        private static byte[] InMemoryCrypt(byte[] data, ICryptoTransform transform)
        {
            var memory = new MemoryStream();
            using (Stream stream = new CryptoStream(memory, transform, CryptoStreamMode.Write))
            {
                stream.Write(data, 0, data.Length);
            }

            return memory.ToArray();
        }

        public string Decrypt(string value)
        {
            var salt = Encoding.ASCII.GetBytes(EncryptionSalt);
            var key = new Rfc2898DeriveBytes(EncryptionPassword, salt);

            var algorithm = new RijndaelManaged();
            var bytesForKey = algorithm.KeySize / 8;
            var bytesForIv = algorithm.BlockSize / 8;
            algorithm.Key = key.GetBytes(bytesForKey);
            algorithm.IV = key.GetBytes(bytesForIv);

            byte[] descryptedBytes;
            using (var decryptor = algorithm.CreateDecryptor(algorithm.Key, algorithm.IV))
            {
                var encryptedBytes = Convert.FromBase64String(value);
                descryptedBytes = InMemoryCrypt(encryptedBytes, decryptor);
            }

            return Encoding.UTF8.GetString(descryptedBytes);
        }
    }
}