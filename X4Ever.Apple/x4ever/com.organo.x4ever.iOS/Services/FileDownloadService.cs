using com.organo.x4ever.ios.Services;
using com.organo.x4ever.Services;
using Foundation;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using com.organo.x4ever.Notification;
using Xamarin.Forms;
using com.organo.x4ever.Handler;

[assembly: Dependency(typeof(FileDownloadService))]

namespace com.organo.x4ever.ios.Services
{
    public class FileDownloadService : IFileDownloadService
    {
        private readonly WebClient _webClient;

        public FileDownloadService()
        {
            _webClient = new WebClient();
        }

        public async Task<bool> DownloadFileAsync(Uri fileUri, string fileName)
        {
            try
            {
                _webClient.DownloadFileAsync(fileUri, await GetFileAsync(fileName));
                return true;
            }
            catch (System.Exception ex)
            {
                var exceptionHandler = new ExceptionHandler(typeof(FileDownloadService).FullName, ex);
            }

            return false;
        }

        public async Task<bool> DownloadFileAsync(string fileUri, string fileName)
        {
            try
            {
                _webClient.DownloadFile(fileUri, await GetFileAsync(fileName));
                return true;
            }
            catch (System.Exception ex)
            {
                var exceptionHandler = new ExceptionHandler(typeof(FileDownloadService).FullName, ex);
            }

            return false;
        }

        public string GetFile(string fileName)
        {
            return Path.Combine(DependencyService.Get<ILocalFile>().DownloadDirectoryPath(), fileName);
        }

        public async Task<string> GetFileAsync(string fileName)
        {
            return await Task.Factory.StartNew(() => GetFile(fileName));
        }

        public async Task<bool> RemoveFileAsync(string fileName)
        {
            try
            {
                var filePathCombine = await GetFileAsync(fileName);
                NSFileManager manager = new NSFileManager();
                bool removed = false;
                if (manager.FileExists(filePathCombine))
                    removed = manager.Remove(new NSUrl(filePathCombine, false), out _);
                return removed;
            }
            catch (System.Exception ex)
            {
                // Notification : LOCAL PUSH NOTIFICATION
                //DependencyService.Get<INotificationServices>().SendNotification(
                //    ActivityType.EXCEPTION_OCCURRED.ToString(),
                //    "EXCEPTION OCCURRED", "File:1",
                //    ex.Message,
                //    1001);
                var exceptionHandler = new ExceptionHandler(typeof(FileDownloadService).FullName, ex);
            }

            return false;
        }
    }
}