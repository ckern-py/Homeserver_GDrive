using GDriveWorker.Metadata;

namespace Homeserver_GDrive.Models
{
    public class InfoRecordsViewModel
    {
        public string ServiceAccountName { get; set; } = "";
        public int InfoCount { get; set; } = 0;
        public List<BasicTableInfo> InfoInfo { get; set; }
    }
}