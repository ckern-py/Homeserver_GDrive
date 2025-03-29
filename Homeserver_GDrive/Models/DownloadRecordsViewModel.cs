using GDriveWorker.Metadata;

namespace Homeserver_GDrive.Models
{
    public class DownloadRecordsViewModel
    {
        public string ServiceAccountName { get; set; } = "";
        public int DownloadCount { get; set; } = 0;
        public List<BasicTableInfo> DownloadInfo { get; set; }
    }
}