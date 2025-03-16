using GDriveWorker.Metadata;

namespace GDriveWorker.Domain
{
    public interface ISQLiteDB
    {
        List<UploadInfo> LastFiveUploads();
        int InsertUploadRecord(string fileName, string uploadDT);
        int DeleteOldRecords();
        int CountRecords();
    }
}
