using GDriveWorker.Metadata;

namespace Homeserver_GDrive.Models
{
    public class ErrorRecordsViewModel
    {
        public string ServiceAccountName { get; set; } = "";
        public int ErrorCount { get; set; } = 0;
        public List<BasicTableInfo> ErrorInfo { get; set; }
    }
}