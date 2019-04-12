using com.organo.x4ever.ios.Services;
using com.organo.x4ever.Models;
using com.organo.x4ever.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using com.organo.x4ever.Extensions;
using Foundation;
using Newtonsoft.Json;
using Xamarin.Forms;

[assembly: Dependency(typeof(LocalFile))]

namespace com.organo.x4ever.ios.Services
{
    public class LocalFile : ILocalFile
    {
        public List<string> Messages { get; set; }
        public List<FileDetail> Files { get; set; }
        private NSUrl[] nsUrls;
        private string[] levelDirs, extensions;

        public LocalFile()
        {
            Files = new List<FileDetail>();
            extensions = new string[]
            {
                "AIFF",
                "WAV",
                "AAX",
                "AAX+",
                "AA",
                "MP3",
                "ALAC",
                "AAC",
                "HE"
            };
            levelDirs = new string[]
            {
                "library",
                "users",
                "home",
                "usr",
                "audio",
                "files",
                "itunes",
                "media",
                "music",
                "quicktime",
                "sound",
                "video",
                "documents",
                "download",
                "podcast",
                "podcasts"
            };
        }

        public List<FileDetail> UpdatePlayListAsync()
        {
            var root = Path.IsPathRooted("/");

            Messages = new List<string>();
            Messages.Add("###Playlist");
            var i = 1;
            Files = new List<FileDetail>();
            var home = "";
            do
            {
                try
                {
                    home = NextDirectoryPath(ref i);
                    Messages.Add(i.ToString() + ". " + home);
                }
                catch (System.Exception ex)
                {
                    Messages.Add("E. " + i.ToString() + ". " + ex.Message);
                }
                SubDirectories(home);
            } while (i > 0);

            return Files.ToList();
        }


        public List<FileDetail> GetLogicalDrivesAsync()
        {
            Messages = new List<string>();
            Messages.Add("###LogicalDrives");
            Files = new List<FileDetail>();
            var directories = Environment.GetLogicalDrives();
            foreach (var directory in directories)
            {
                SubDirectories(directory);
            }

            return Files.ToList();
        }

        private void SubDirectories(string directory)
        {
            try
            {
                if (directory != null && directory.Trim().Length > 0)
                {
                    var directoryInfo = new DirectoryInfo(directory);
                    if (directoryInfo.Exists)
                    {
                        try
                        {
                            var tempFiles = directoryInfo.GetFiles();
                            Messages.Add("File Count: #" + tempFiles.Length);
                            if (tempFiles.Length > 0)
                            {
                                foreach (var file in tempFiles)
                                {
                                    Messages.Add("File: #" + file.Name);
                                    if (file.Name.IsExtension(extensions))
                                    {
                                        var filePath = AddFile(file) ?? null;
                                        if (filePath != null)
                                            Files.Add(filePath);
                                    }
                                }
                            }

                            var directories = MainDirectoies(directoryInfo);
                            foreach (var dirInfo in directories)
                                SubDirectories(dirInfo.FullName);
                        }
                        catch (System.Exception exception)
                        {
                            Messages.Add("E. {" + directory + "}: " + exception.Message);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Messages.Add("E. " + ex.Message);
            }
        }

        private DirectoryInfo[] MainDirectoies(DirectoryInfo directoryInfo)
        {
            var directories = directoryInfo.GetDirectories();
            if (DateTime.Now.Minute > 0 && DateTime.Now.Minute <= 30)
                return directories.Where(d => levelDirs.Contains(d.Name.ToLower().Trim())).ToArray();
            else
                return directories;
        }

        private string NextDirectoryPath(ref int i)
        {
            string path = "";
            switch (i)
            {
                case 1:
                    path = Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic);
                    break;
                case 2:
                    path = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
                    break;
                case 3:
                    path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    break;
                case 4:
                    path = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
                    break;
                case 5:
                    path = Environment.GetFolderPath(Environment.SpecialFolder.CommonVideos);
                    break;
                case 6:
                    nsUrls = NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.MusicDirectory,
                        NSSearchPathDomain.User);
                    if (nsUrls.Length > 0)
                        path = nsUrls[0].Path;
                    break;
                case 7:
                    nsUrls = NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.AllLibrariesDirectory,
                        NSSearchPathDomain.User);
                    if (nsUrls.Length > 0)
                        path = nsUrls[0].Path;
                    break;
                case 8:
                    nsUrls = NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.SharedPublicDirectory,
                        NSSearchPathDomain.User);
                    if (nsUrls.Length > 0)
                        path = nsUrls[0].Path;
                    break;
                case 9:
                    nsUrls = NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.UserDirectory,
                        NSSearchPathDomain.User);
                    if (nsUrls.Length > 0)
                        path = nsUrls[0].Path;
                    break;
                case 10:
                    nsUrls = NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.DownloadsDirectory,
                        NSSearchPathDomain.User);
                    if (nsUrls.Length > 0)
                        path = nsUrls[0].Path;
                    break;
                case 11:
                    nsUrls = NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.DocumentDirectory,
                        NSSearchPathDomain.User);
                    if (nsUrls.Length > 0)
                        path = nsUrls[0].Path;
                    break;
                default:
                    i = 0;
                    break;
            }

            if (i > 0)
                i++;
            return path;
        }

        public FileDetail AddFile(FileInfo file)
        {
            if (file != null)
            {
                try
                {
                    var url = new NSUrl(file.FullName, false);
                    if (url != null)
                    {
                        var path = url.Path;
                        return new FileDetail()
                        {
                            Name = file.Name,
                            Path = path,
                            IsDirectory = false,
                            IsFile = true,
                        };
                    }
                }
                catch (System.Exception)
                {
                    //
                }
            }

            return null;
        }

        public void CreateTestFileAsync()
        {
            // Create a new record
            var account = new Account()
            {
                Email = "newuser@xamarin.com",
                Active = true,
                CreatedDate = new DateTime(2018, 7, 6, 9, 29, 0, DateTimeKind.Utc),
                Roles = new List<string> { "User", "Admin" }
            };

            // Serialize object
            var json = JsonConvert.SerializeObject(account, Newtonsoft.Json.Formatting.Indented);

            // Save to file
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var filename = Path.Combine(documents, "account.json");
            File.WriteAllText(filename, json);


            /******* TO DELETE FILE *******/
            //var home = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //var directory = new DirectoryInfo(home);

            //var tempFiles = directory.GetFiles();

            //if (tempFiles.Length > 0)
            //{
            //    foreach (var file in tempFiles)
            //    {
            //        AddFile(file);
            //        file.Delete();
            //    }
            //}
        }

        private string GetDirectoryPath(Environment.SpecialFolder folder = Environment.SpecialFolder.MyDocuments)
        {
            return Environment.GetFolderPath(folder);
            //var nsUrls = NSFileManager.DefaultManager.GetUrls(directory, NSSearchPathDomain.User);
            //if (nsUrls.Length > 0)
            //    path =  nsUrls[0].Path;
            //path =  "";
        }

        public async Task TestCodeAsync()
        {
            await Task.Run(() =>
            {
                var nsUrls = NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.LibraryDirectory,
                    NSSearchPathDomain.User);
                foreach (var nsurl in nsUrls)
                {
                    var path = nsurl.Path;
                }
            });
        }

        public List<FileDetail> GetLocalFilesAsync()
        {
            Messages = new List<string>();
            Files = new List<FileDetail>();
            var directories = Directory.EnumerateDirectories("./");
            foreach (var directory in directories)
            {
                Files.Add(new FileDetail()
                {
                    Path = directory,
                    Name = directory,
                    IsFile = false,
                    IsDirectory = true,
                    Parent = null
                });
                Messages.Add(directory);
                Console.WriteLine(directory);
            }

            return Files;
        }

        public string DownloadDirectoryPath()
        {
            return GetDirectoryPath(Environment.SpecialFolder.MyDocuments);
        }

        [Preserve]
        public class Account
        {
            #region Computed Properties
            public string Email { get; set; }
            public bool Active { get; set; }
            public DateTime CreatedDate { get; set; }
            public List<string> Roles { get; set; }
            #endregion

            #region Constructors
            public Account()
            {

            }
            #endregion
        }
    }
}