﻿using Google.Apis.Upload;

namespace GDriveWorker.Domain
{
    public interface IGoogleOperations
    {
        Google.Apis.Drive.v3.Data.About GetUserInfo();
        string FindFolderID(string folderName, string parentID = "");
        string FindFileID(string fileName, string parentID);
        string GetFileByID(string fileID);
        string CreateFolder(string folderName, string parentID, string folderID = "");
        IUploadProgress UploadFile(string fileLocation, string parent);
        IUploadProgress UpdateFile(string fileLocation, string fileID);
        List<Google.Apis.Drive.v3.Data.File> FindAllFolders(string parentID, string nextPageToken = "");
        List<Google.Apis.Drive.v3.Data.File> FindAllFiles(string parentID, string nextPageToken = "");
        MemoryStream DownloadFile(string fileID);
    }
}