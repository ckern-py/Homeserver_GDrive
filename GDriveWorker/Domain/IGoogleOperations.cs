namespace GDriveWorker.Domain
{
    public interface IGoogleOperations
    {
        Google.Apis.Drive.v3.Data.About GetUserInfo();
        string FindFolderID(string folderName);
        string CreateFolder(string folderID, string folderName);
        string UploadFile(string fileLocation, string parent = "");
    }
}
