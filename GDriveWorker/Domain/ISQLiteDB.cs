using GDriveWorker.Metadata;

namespace GDriveWorker.Domain
{
    public interface ISQLiteDB
    {
        DateTime GetFileUploadTime(string fileName);
        List<BasicTableInfo> LastUploadRecords(int uploadAmount = 5);
        List<BasicTableInfo> LastDownloadRecords(int uploadAmount = 5);
        List<BasicTableInfo> LastInformationRecords(int infoAmount = 5);
        List<BasicTableInfo> LastErrorRecords(int errorAmount = 5);
        int InsertUploadRecord(string fileName, string uploadDT);
        int InsertDownloadRecord(string fileName, string downloadDT);
        int InsertInformationdRecord(string infoMessage, string infoDT);
        int InsertErrorRecord(string errorMessage, string errorDT);
        int DeleteOldFileUploadsRecords();
        int DeleteOldFileDownloadsRecords();
        int DeleteOldInformationRecords();
        int DeleteOldErrorsRecords();
        int CountUploadRecords();
        int CountDownloadRecords();
        int CountInfoRecords();
        int CountErrorRecords();
    }
}