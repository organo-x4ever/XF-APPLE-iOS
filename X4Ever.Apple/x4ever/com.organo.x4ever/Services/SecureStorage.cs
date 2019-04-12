using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.organo.x4ever.Globals;
using com.organo.x4ever.Services;
using PCLStorage;
using Xamarin.Forms;

//[assembly:Dependency(typeof(SecureStorage))]

namespace com.organo.x4ever.Services
{
    public class SecureStorage //: ISecureStorage
    {
        private readonly IHelper helper;
        public SecureStorage()
        {
            helper = DependencyService.Get<IHelper>();
        }
        public string FolderName { get; set; }
        public string FileName { get; set; }

        public Task<bool> Contains(string key)
        {
            throw new NotImplementedException();
        }

        public Task Delete(string key)
        {
            throw new NotImplementedException();
        }

        public async Task<string> Retrieve(string key)
        {
            var data = await RetrieveByte(key);
            return Encoding.UTF8.GetString(data, 0, data.Length);
        }

        public Task<byte[]> RetrieveByte(string key)
        {
            throw new NotImplementedException();
        }

        public async Task Store(string key, byte[] dataBytes)
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFolder folder = await rootFolder.CreateFolderAsync(FolderName,
                CreationCollisionOption.OpenIfExists);
            IFile file = await folder.CreateFileAsync(FileName,
                CreationCollisionOption.ReplaceExisting);
            await file.WriteAllTextAsync(dataBytes.ToString());
        }

        public async Task Store(string key, string data)
        {
            await this.Store(key, Encoding.UTF8.GetBytes(data));
        }
    }
}