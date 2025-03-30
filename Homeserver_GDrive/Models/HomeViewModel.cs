using GDriveWorker.Metadata;

namespace Homeserver_GDrive.Models
{
    public class HomeViewModel
    {
        public string ServiceAccountName { get; set; } = "";
        public int UploadCount { get; set; } = 0;
        public int DownloadCount { get; set; } = 0;
        public int InfoCount { get; set; } = 0;
        public int ErrorCount { get; set; } = 0;
        public List<BasicTableInfo> UploadInfo { get; set; }
        public List<BasicTableInfo> DownloadInfo { get; set; }
        public List<BasicTableInfo> InfoInfo { get; set; }
        public List<BasicTableInfo> ErrorInfo { get; set; }
    }
}