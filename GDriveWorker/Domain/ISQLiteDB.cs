using GDriveWorker.Metadata;

namespace GDriveWorker.Domain
{
    public interface ISQLiteDB
    {
        List<BasicTableInfo> LastUploadRecords(int uploadAmount = 5);
        List<BasicTableInfo> LastInformationRecords(int infoAmount = 5);
        List<BasicTableInfo> LastErrorRecords(int errorAmount = 5);
        int InsertUploadRecord(string fileName, string uploadDT);
        int InsertInformationdRecord(string infoMessage, string infoDT);
        int InsertErrorRecord(string errorMessage, string errorDT);
        int DeleteOldFileUploadsRecords();
        int DeleteOldInformationRecords();
        int DeleteOldErrorsRecords();
        int CountUploadRecords();
        int CountInfoRecords();
        int CountErrorRecords();
    }
}
