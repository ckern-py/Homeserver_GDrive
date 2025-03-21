using GDriveWorker.Metadata;

namespace Homeserver_GDrive.Models
{
    public class UploadrRecordsViewModel
    {
        public string ServiceAccountName { get; set; } = "";
        public int UploadCount { get; set; } = 0;
        public List<BasicTableInfo> UploadInfo { get; set; }
    }
}