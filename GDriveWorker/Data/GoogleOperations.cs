using GDriveWorker.Domain;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;

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
            if (!string.IsNullOrEmpty(foundFolder.Files[0].Id))
            {
                folderID = foundFolder.Files[0].Id;
            }

            return folderID;
        }

        public string CreatFolder(string folderID, string folderName)
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
