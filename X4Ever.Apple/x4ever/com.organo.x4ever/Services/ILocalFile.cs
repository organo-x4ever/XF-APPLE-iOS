using com.organo.x4ever.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace com.organo.x4ever.Services
{
    public interface ILocalFile
    {
        List<string> Messages { get; set; }
        string DownloadDirectoryPath();
        List<FileDetail> Files { get; set; }

        List<FileDetail> UpdatePlayListAsync();
        List<FileDetail> GetLogicalDrivesAsync();

        void CreateTestFileAsync();

        Task TestCodeAsync();

        List<FileDetail> GetLocalFilesAsync();
    }
}