using GDriveWorker.Domain;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using Microsoft.Extensions.Caching.Memory;

namespace GDriveWorker.Data
{
    public class GoogleOperations : IGoogleOperations
    {
        private static readonly string[] Scopes = { DriveService.Scope.Drive };
        private static readonly string ApplicationName = "penthouse-gdrive";
        private readonly IMemoryCache _memoryCache;

        public GoogleOperations(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
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
            DriveService driveService = SALogin();

            FilesResource.ListRequest folder = driveService.Files.List();
            folder.Q = $"name = '{folderName}' and mimeType = 'application/vnd.google-apps.folder'";
            if (!string.IsNullOrWhiteSpace(parentID))
            {
                folder.Q += $" and '{parentID}' in parents";
            }
            Google.Apis.Drive.v3.Data.FileList foundFolder = folder.Execute();

            string folderID = string.Empty;
            Google.Apis.Drive.v3.Data.File? firstFolder = foundFolder.Files.FirstOrDefault();
            if (firstFolder is not null)
            {
                folderID = firstFolder.Id;
            }

            return folderID;
        }

        public string FindFileID(string fileName, string parentID)
        {
            DriveService driveService = SALogin();

            FilesResource.ListRequest file = driveService.Files.List();
            file.Q = $"name = '{fileName}' and mimeType != 'application/vnd.google-apps.folder' and '{parentID}' in parents";
            Google.Apis.Drive.v3.Data.FileList foundFile = file.Execute();

            string fileID = string.Empty;
            Google.Apis.Drive.v3.Data.File? firstFile = foundFile.Files.FirstOrDefault();
            if (firstFile is not null)
            {
                fileID = firstFile.Id;
            }

            return fileID;
        }

        public string CreateFolder(string folderName, string parentID, string folderID = "")
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
            return folder.Id;
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

        private static DriveService SALogin()
        {
            GoogleCredential credential;
            //FileStream stream = new FileStream("/../config/credentials.json", FileMode.Open, FileAccess.Read)
            using (FileStream stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
            }

            DriveService driveService = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });

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
            ;
        }
    }
}
