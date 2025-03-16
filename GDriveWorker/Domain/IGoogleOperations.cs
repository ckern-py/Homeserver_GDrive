namespace GDriveWorker.Domain
{
    public interface IGoogleOperations
    {
        Google.Apis.Drive.v3.Data.About GetUserInfo();
        string FindFolderID(string folderName);
        string CreatFolder(string folderID, string folderName);
    }
}
