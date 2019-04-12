using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using Foundation;
using UIKit;
using com.organo.x4ever.ios.Services;
using com.organo.x4ever.Services;
using Newtonsoft.Json;
using System.Threading.Tasks;
using com.organo.x4ever.Handler;

[assembly: Xamarin.Forms.Dependency(typeof(KeyVaultStorage))]

namespace com.organo.x4ever.ios.Services
{
    public class KeyVaultStorage : ISecureStorage
    {
        public void Delete(string key)
        {
            try
            {
                if (!string.IsNullOrEmpty(key))
                {
                    NSUserDefaults.StandardUserDefaults.RemoveObject(key);
                }
            }
            catch (System.Exception ex)
            {
                var exceptionHandler = new ExceptionHandler(typeof(KeyVaultStorage).FullName + ".Delete()", ex);
            }
        }

        public T Retrieve<T>(string key)
        {
            try
            {
                if (!string.IsNullOrEmpty(key))
                {
                    string data = NSUserDefaults.StandardUserDefaults.StringForKey(key);
                    if (!string.IsNullOrEmpty(data))
                        return JsonConvert.DeserializeObject<T>(data);
                }
            }
            catch (System.Exception ex)
            {
                var exceptionHandler = new ExceptionHandler("KeyVaultStorage.cs Retrieve(" + key + ")", ex);
            }

            return JsonConvert.DeserializeObject<T>("");
        }

        public string RetrieveStringFromBytes(string key)
        {
            try
            {
                if (!string.IsNullOrEmpty(key))
                {
                    var data = Retrieve<byte[]>(key);
                    if (data != null)
                        return Encoding.UTF8.GetString(data, 0, data.Length);
                }
            }
            catch (System.Exception ex)
            {
                var exceptionHandler =
                    new ExceptionHandler("KeyVaultStorage.cs RetrieveStringFromBytes(" + key + ")", ex);
            }

            return "";
        }

        public void Store(string key, string data)
        {
            try
            {
                if (!string.IsNullOrEmpty(key))
                {
                    NSUserDefaults defaults = NSUserDefaults.StandardUserDefaults;
                    if (defaults != null)
                    {
                        string value = JsonConvert.SerializeObject(data);
                        if (!string.IsNullOrEmpty(value))
                        {

                            defaults.SetString(value != null ? value : String.Empty, key);
                            defaults.Synchronize();
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                var exceptionHandler = new ExceptionHandler("KeyVaultStorage.cs Store(" + key + ")", ex);
            }
        }

        public void StoreByte(string key, byte[] data)
        {
            try
            {
                if (!string.IsNullOrEmpty(key))
                {
                    NSUserDefaults defaults = NSUserDefaults.StandardUserDefaults;
                    if (defaults != null)
                    {
                        string value = JsonConvert.SerializeObject(data);
                        if (!string.IsNullOrEmpty(value))
                        {
                            defaults.SetString(value != null ? value : String.Empty, key);
                            defaults.Synchronize();
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                var exceptionHandler = new ExceptionHandler("KeyVaultStorage.cs StoreByte(" + key + ")", ex);
            }
        }

        public void StoreBytesFromString(string key, string data)
        {
            try
            {
                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(data))
                    StoreByte(key, Encoding.UTF8.GetBytes(data));
            }
            catch (System.Exception ex)
            {
                var exceptionHandler =
                    new ExceptionHandler("KeyVaultStorage.cs StoreBytesFromString(" + key + "," + data + ")", ex);
            }
        }
    }
}