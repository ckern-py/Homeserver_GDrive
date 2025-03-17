using GDriveWorker.Domain;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;

namespace GDriveWorker.Data
{
    public class GoogleOperations : IGoogleOperations
    {
        private static readonly string[] Scopes = { DriveService.Scope.Drive };
        private static readonly string ApplicationName = "penthouse-gdrive";

        public Google.Apis.Drive.v3.Data.About GetUserInfo()
        {
            DriveService driveService = SALogin();

            AboutResource.GetRequest aboutUser = driveService.About.Get();
            aboutUser.Fields = "kind,user,storageQuota";
            return aboutUser.Execute();
        }

        public string FindFolderID(string folderName)
        {
            DriveService driveService = SALogin();

            FilesResource.ListRequest folder = driveService.Files.List();
            folder.Q = $"name = '{folderName}' and mimeType = 'application/vnd.google-apps.folder'";
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

        public string CreateFolder(string folderID, string folderName)
        {
            DriveService driveService = SALogin();

            Google.Apis.Drive.v3.Data.File newFolder = new Google.Apis.Drive.v3.Data.File()
            {
                Kind = "drive#file",
                MimeType = "application/vnd.google-apps.folder",
                Id = folderID,
                Name = folderName
            };

            FilesResource.CreateRequest createRequest = driveService.Files.Create(newFolder);
            Google.Apis.Drive.v3.Data.File folder = createRequest.Execute();
            return folder.Id;
        }

        public string UploadFile(string fileLocation, string parent)
        {
            DriveService driveService = SALogin();

            IUploadProgress uploadProgress;

            using (FileStream uploadStream = File.OpenRead(fileLocation))
            {
                Google.Apis.Drive.v3.Data.File driveFile = new Google.Apis.Drive.v3.Data.File
                {
                    Name = Path.GetFileName(fileLocation),
                    MimeType = "application/vnd.google-apps.document",
                    Parents = new List<string>() { parent }
                };

                FilesResource.CreateMediaUpload uploadRequest = driveService.Files.Create(driveFile, uploadStream, "text/plain");

                uploadProgress = uploadRequest.Upload();
            }
            return uploadProgress.Status.ToString();
        }

        public string UpdateFile(string fileLocation, string parent, string fileID)
        {
            DriveService driveService = SALogin();

            IUploadProgress uploadProgress;

            using (FileStream uploadStream = File.OpenRead(fileLocation))
            {
                Google.Apis.Drive.v3.Data.File driveFile = new Google.Apis.Drive.v3.Data.File
                {
                    Name = Path.GetFileName(fileLocation),
                    MimeType = "application/vnd.google-apps.document",
                    Parents = new List<string>() { parent }
                };

                FilesResource.UpdateMediaUpload updateRequest = driveService.Files.Update(driveFile, fileID, uploadStream, "text/plain");

                uploadProgress = updateRequest.Upload();
            }
            return uploadProgress.Status.ToString();
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
    }
}
