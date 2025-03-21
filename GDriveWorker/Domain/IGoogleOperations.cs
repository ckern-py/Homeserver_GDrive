using Google.Apis.Upload;

namespace GDriveWorker.Domain
{
    public interface IGoogleOperations
    {
        Google.Apis.Drive.v3.Data.About GetUserInfo();
        string FindFolderID(string folderName, string parentID = "");
        string FindFileID(string fileName, string parentID);
        string CreateFolder(string folderName, string parentID, string folderID = "");
        IUploadProgress UploadFile(string fileLocation, string parent);
        IUploadProgress UpdateFile(string fileLocation, string fileID);
    }
}