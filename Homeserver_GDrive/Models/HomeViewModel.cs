using GDriveWorker.Metadata;

namespace Homeserver_GDrive.Models
{
    public class HomeViewModel
    {
        public string ServiceAccountName { get; set; } = "";
        public int RecordCount { get; set; } = 0;
        public List<UploadInfo> UploadInfo { get; set; }
    }
}
