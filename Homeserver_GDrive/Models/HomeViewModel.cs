using GDriveWorker.Metadata;

namespace Homeserver_GDrive.Models
{
    public class HomeViewModel
    {
        public string ServiceAccountName { get; set; } = "";
        public int recordCount { get; set; } = 0;
        public List<UploadInfo> uploadInfo { get; set; }
    }
}
