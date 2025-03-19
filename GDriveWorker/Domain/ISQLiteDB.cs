using GDriveWorker.Metadata;

namespace GDriveWorker.Domain
{
    public interface ISQLiteDB
    {
        List<BasicTableInfo> LastFiveUploads();
        List<BasicTableInfo> LastFiveInformation();
        List<BasicTableInfo> LastFiveErrors();
        int InsertUploadRecord(string fileName, string uploadDT);
        int InsertInformationdRecord(string infoMessage, string infoDT);
        int InsertErrorRecord(string errorMessage, string errorDT);
        int DeleteOldFileUploadsRecords();
        int DeleteOldInformationRecords();
        int DeleteOldErrorsRecords();
        int CountRecords();
    }
}
