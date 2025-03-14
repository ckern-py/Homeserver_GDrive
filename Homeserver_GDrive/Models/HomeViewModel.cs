using GDriveWorker.Metadata;

namespace Homeserver_GDrive.Models
{
    public class HomeViewModel
    {
        public string ServiceAccountName { get; set; } = "";
        public List<UploadInfo> uploadInfo { get; set; }
    }
}
