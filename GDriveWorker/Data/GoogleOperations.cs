﻿using GDriveWorker.Domain;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;

namespace GDriveWorker.Data
{
    public class GoogleOperations : IGoogleOperations
    {
        private static readonly string[] Scopes = { DriveService.Scope.Drive };
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;
        private readonly ISQLiteDB _sqLiteDB;

        public GoogleOperations(IMemoryCache memoryCache, IConfiguration configuration, ISQLiteDB sqLiteDB)
        {
            _memoryCache = memoryCache;
            _configuration = configuration;
            _sqLiteDB = sqLiteDB;
        }

        public Google.Apis.Drive.v3.Data.About GetUserInfo()
        {
            if (!_memoryCache.TryGetValue("RUN_AS_USER", out Google.Apis.Drive.v3.Data.About userInfo))
            {
                DriveService driveService = SALogin();

                AboutResource.GetRequest aboutUser = driveService.About.Get();
                aboutUser.Fields = "kind,user,storageQuota";
                userInfo = aboutUser.Execute();

                _memoryCache.Set("RUN_AS_USER", userInfo, TimeSpan.FromDays(1));
            }

            return userInfo;
        }

        public string FindFolderID(string folderName, string parentID = "")
        {
            string folderID = string.Empty;

            try
            {
                DriveService driveService = SALogin();

                FilesResource.ListRequest folder = driveService.Files.List();
                folder.Q = $"name = '{folderName}' and mimeType = 'application/vnd.google-apps.folder'";
                if (!string.IsNullOrWhiteSpace(parentID))
                {
                    folder.Q += $" and '{parentID}' in parents";
                }
                Google.Apis.Drive.v3.Data.FileList foundFolder = folder.Execute();


                Google.Apis.Drive.v3.Data.File? firstFolder = foundFolder.Files.FirstOrDefault();
                if (firstFolder is not null)
                {
                    folderID = firstFolder.Id;
                }
            }
            catch (Exception e)
            {
                string errMessage = e.Message.Length > 400 ? e.Message.Substring(0, 400) : e.Message;
                _sqLiteDB.InsertErrorRecord($"{MethodBase.GetCurrentMethod().Name} - {folderName}:{parentID} -- {errMessage}", DateTime.Now.ToString());
            }

            return folderID;
        }

        public string FindFileID(string fileName, string parentID)
        {
            string fileID = string.Empty;

            try
            {
                DriveService driveService = SALogin();

                FilesResource.ListRequest file = driveService.Files.List();
                file.Q = $"name = '{fileName}' and mimeType != 'application/vnd.google-apps.folder' and '{parentID}' in parents";
                Google.Apis.Drive.v3.Data.FileList foundFile = file.Execute();


                Google.Apis.Drive.v3.Data.File? firstFile = foundFile.Files.FirstOrDefault();
                if (firstFile is not null)
                {
                    fileID = firstFile.Id;
                }
            }
            catch (Exception e)
            {
                string errMessage = e.Message.Length > 400 ? e.Message.Substring(0, 400) : e.Message;
                _sqLiteDB.InsertErrorRecord($"{MethodBase.GetCurrentMethod().Name} - {fileName}:{parentID} -- {errMessage}", DateTime.Now.ToString());
            }

            return fileID;
        }

        public List<Google.Apis.Drive.v3.Data.File> FindAllFolders(string parentID, string nextPageToken = "")
        {
            List<Google.Apis.Drive.v3.Data.File> allFolders = new List<Google.Apis.Drive.v3.Data.File>();

            try
            {
                DriveService driveService = SALogin();

                FilesResource.ListRequest folders = driveService.Files.List();
                folders.Q = $"mimeType = 'application/vnd.google-apps.folder' and '{parentID}' in parents";
                if (!string.IsNullOrWhiteSpace(nextPageToken))
                {
                    folders.PageToken = nextPageToken;
                }
                Google.Apis.Drive.v3.Data.FileList fileList = folders.Execute();

                allFolders = fileList.Files.ToList();

                if (!string.IsNullOrWhiteSpace(fileList.NextPageToken))
                {
                    allFolders.AddRange(FindAllFiles(parentID, fileList.NextPageToken));
                }
            }
            catch (Exception e)
            {
                string errMessage = e.Message.Length > 400 ? e.Message.Substring(0, 400) : e.Message;
                _sqLiteDB.InsertErrorRecord($"{MethodBase.GetCurrentMethod().Name} - {parentID}:{nextPageToken} -- {errMessage}", DateTime.Now.ToString());
            }

            return allFolders;
        }

        public List<Google.Apis.Drive.v3.Data.File> FindAllFiles(string parentID, string nextPageToken = "")
        {
            List<Google.Apis.Drive.v3.Data.File> allFiles = new List<Google.Apis.Drive.v3.Data.File>();

            try
            {
                DriveService driveService = SALogin();

                FilesResource.ListRequest files = driveService.Files.List();
                files.Q = $"mimeType != 'application/vnd.google-apps.folder' and '{parentID}' in parents";
                if (!string.IsNullOrWhiteSpace(nextPageToken))
                {
                    files.PageToken = nextPageToken;
                }
                Google.Apis.Drive.v3.Data.FileList fileList = files.Execute();

                allFiles = fileList.Files.ToList();

                if (!string.IsNullOrWhiteSpace(fileList.NextPageToken))
                {
                    allFiles.AddRange(FindAllFiles(parentID, fileList.NextPageToken));
                }
            }
            catch (Exception e)
            {
                string errMessage = e.Message.Length > 400 ? e.Message.Substring(0, 400) : e.Message;
                _sqLiteDB.InsertErrorRecord($"{MethodBase.GetCurrentMethod().Name} - {parentID}:{nextPageToken} -- {errMessage}", DateTime.Now.ToString());
            }

            return allFiles;
        }

        public string GetFileByID(string fileID)
        {
            string foundFileID = string.Empty;

            try
            {
                DriveService driveService = SALogin();

                FilesResource.GetRequest fileRequest = driveService.Files.Get(fileID);
                Google.Apis.Drive.v3.Data.File foundFile = fileRequest.Execute();

                foundFileID = foundFile.Name;
            }
            catch (Exception e)
            {
                string errMessage = e.Message.Length > 400 ? e.Message.Substring(0, 400) : e.Message;
                _sqLiteDB.InsertErrorRecord($"{MethodBase.GetCurrentMethod().Name} - {fileID} -- {errMessage}", DateTime.Now.ToString());
            }

            return foundFileID;
        }

        public string CreateFolder(string folderName, string parentID, string folderID = "")
        {
            string newFolderID = string.Empty;

            try
            {
                DriveService driveService = SALogin();

                Google.Apis.Drive.v3.Data.File newFolder = new Google.Apis.Drive.v3.Data.File()
                {
                    Kind = "drive#file",
                    MimeType = "application/vnd.google-apps.folder",
                    Name = folderName,
                    Parents = new List<string>() { parentID }
                };

                if (!string.IsNullOrWhiteSpace(folderID))
                {
                    newFolder.Id = folderID;
                }

                FilesResource.CreateRequest createRequest = driveService.Files.Create(newFolder);
                Google.Apis.Drive.v3.Data.File folder = createRequest.Execute();
                newFolderID = folder.Id;
            }
            catch (Exception e)
            {
                string errMessage = e.Message.Length > 400 ? e.Message.Substring(0, 400) : e.Message;
                _sqLiteDB.InsertErrorRecord($"{MethodBase.GetCurrentMethod().Name} - {folderName}:{parentID}:{folderID} -- {errMessage}", DateTime.Now.ToString());
            }

            return newFolderID;
        }

        public IUploadProgress UploadFile(string fileLocation, string parent)
        {
            DriveService driveService = SALogin();

            IUploadProgress uploadProgress;
            string justFileName = Path.GetFileName(fileLocation);

            using (FileStream uploadStream = File.OpenRead(fileLocation))
            {
                Google.Apis.Drive.v3.Data.File driveFile = new Google.Apis.Drive.v3.Data.File
                {
                    Name = justFileName,
                    Parents = new List<string>() { parent }
                };

                string contentType = GetMIMEType(Path.GetExtension(justFileName));
                FilesResource.CreateMediaUpload uploadRequest = driveService.Files.Create(driveFile, uploadStream, contentType);

                uploadProgress = uploadRequest.Upload();
            }
            return uploadProgress;
        }

        public IUploadProgress UpdateFile(string fileLocation, string fileID)
        {
            DriveService driveService = SALogin();

            IUploadProgress uploadProgress;
            string justFileName = Path.GetFileName(fileLocation);

            using (FileStream uploadStream = File.OpenRead(fileLocation))
            {
                Google.Apis.Drive.v3.Data.File driveFile = new Google.Apis.Drive.v3.Data.File
                {
                    Name = justFileName
                };

                string contentType = GetMIMEType(Path.GetExtension(justFileName));
                FilesResource.UpdateMediaUpload updateRequest = driveService.Files.Update(driveFile, fileID, uploadStream, contentType);

                uploadProgress = updateRequest.Upload();
            }
            return uploadProgress;
        }

        public MemoryStream DownloadFile(string fileID)
        {
            MemoryStream memoryStream = new MemoryStream();

            try
            {
                DriveService driveService = SALogin();

                FilesResource.GetRequest fileRequest = driveService.Files.Get(fileID);

                fileRequest.Download(memoryStream);
            }
            catch (Exception e)
            {
                string errMessage = e.Message.Length > 400 ? e.Message.Substring(0, 400) : e.Message;
                _sqLiteDB.InsertErrorRecord($"{MethodBase.GetCurrentMethod().Name} - {fileID} -- {errMessage}", DateTime.Now.ToString());
            }

            return memoryStream;
        }

        private DriveService SALogin()
        {
            if (!_memoryCache.TryGetValue("DRIVE_SERVICE", out DriveService driveService))
            {
                GoogleCredential credential;
                using (FileStream stream = new FileStream("/../config/credentials.json", FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
                }

                driveService = new DriveService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = _configuration["AppSettings:GoogleApplicationName"]
                });

                _memoryCache.Set("DRIVE_SERVICE", driveService, TimeSpan.FromHours(1));
            }

            return driveService;
        }

        private static string GetMIMEType(string fileExtension)
        {
            switch (fileExtension)
            {
                case ".txt":
                    return "text/plain";
                case ".yml":
                case ".yaml":
                    return "application/yaml";
                case ".sh":
                    return "text/x-shellscript";
                default:
                    return "application/octet-stream";
            }
        }
    }
}