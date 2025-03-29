using GDriveWorker.Domain;
using Google.Apis.Upload;

namespace GDriveWorker.Data
{
    public class GDriveLogic : IGDriveLogic
    {
        private readonly IGoogleOperations _googleOperation;
        private readonly ILogger<GDriveLogic> _logger;
        private readonly ISQLiteDB _sqliteDB;
        private readonly IConfiguration _configuration;

        private static readonly string uploadLocation = "/../media/upload";
        private static readonly string downloadLocation = "/../media/download";

        public GDriveLogic(ILogger<GDriveLogic> logger, IGoogleOperations googleOperations, ISQLiteDB sqliteDB, IConfiguration configuration)
        {
            _logger = logger;
            _googleOperation = googleOperations;
            _sqliteDB = sqliteDB;
            _configuration = configuration;
        }

        public void UploadMediaDirectory(string location)
        {
            string topLevelFolder = _configuration["AppSettings:TopLevelGDriveUploadFolder"];
            string parentFolderID = _googleOperation.FindFolderID(topLevelFolder);

            if (string.IsNullOrWhiteSpace(parentFolderID))
            {
                _logger.LogInformation("Could not find parent folder {parent}", topLevelFolder);
                _sqliteDB.InsertInformationdRecord($"Could not find parent folder {topLevelFolder}", DateTime.Now.ToString());
                return;
            }

            UploadFiles(location, parentFolderID);

            UploadFolder(location, parentFolderID);
        }

        private void UploadFiles(string location, string parentFolderID)
        {
            string[] files = Directory.GetFiles(location);
            foreach (string file in files)
            {
                IUploadProgress fileStatus;
                string fileID = _googleOperation.FindFileID(Path.GetFileName(file), parentFolderID);

                if (string.IsNullOrWhiteSpace(fileID))
                {
                    _sqliteDB.InsertInformationdRecord($"Drive file {RemovePathBeginning(file)} not found, uploading it", DateTime.Now.ToString());
                    fileStatus = _googleOperation.UploadFile(file, parentFolderID);
                }
                else
                {
                    DateTime lastWrite = File.GetLastWriteTime(file);
                    DateTime lastUpload = _sqliteDB.GetFileUploadTime(file);
                    if (lastWrite > lastUpload)
                    {
                        _sqliteDB.InsertInformationdRecord($"Drive file {RemovePathBeginning(file)} found, updating it", DateTime.Now.ToString());
                        fileStatus = _googleOperation.UpdateFile(file, fileID);
                    }
                    else
                    {
                        _sqliteDB.InsertInformationdRecord($"Drive file {RemovePathBeginning(file)} has no changes, not updating", DateTime.Now.ToString());
                        continue;
                    }
                }

                if (fileStatus.Status == UploadStatus.Completed)
                {
                    _sqliteDB.InsertUploadRecord(RemovePathBeginning(file), DateTime.Now.ToString());
                }
                else
                {
                    _sqliteDB.InsertErrorRecord(fileStatus.Exception.ToString(), DateTime.Now.ToString());
                }
            }
        }

        private void UploadFolder(string location, string parentFolderID)
        {
            string[] dir = Directory.GetDirectories(location);

            foreach (string directory in dir)
            {
                string justFolder = new DirectoryInfo(directory).Name;
                string folderID = _googleOperation.FindFolderID(justFolder, parentFolderID);
                if (string.IsNullOrWhiteSpace(folderID))
                {
                    _sqliteDB.InsertInformationdRecord($"Drive folder {RemovePathBeginning(directory)} not found, creating it", DateTime.Now.ToString());
                    folderID = _googleOperation.CreateFolder(justFolder, parentFolderID);
                    _sqliteDB.InsertUploadRecord(RemovePathBeginning(directory), DateTime.Now.ToString());
                }

                UploadFiles(directory, folderID);
                UploadFolder(directory, folderID);
            }
        }
        
        public void DownloadMediaDirectory(string location)
        {
            string topLevelFolder = _configuration["AppSettings:TopLevelGDriveDownloadFolder"];
            string parentFolderID = _googleOperation.FindFolderID(topLevelFolder);

            if (string.IsNullOrWhiteSpace(parentFolderID))
            {
                _logger.LogInformation("Could not find parent folder {parent}", topLevelFolder);
                _sqliteDB.InsertInformationdRecord($"Could not find parent folder {topLevelFolder}", DateTime.Now.ToString());
                return;
            }

            DownloadFiles(location, parentFolderID);

            DownloadFolder(location, parentFolderID);
        }

        private void DownloadFiles(string location, string parentFolderID)
        {
            List<Google.Apis.Drive.v3.Data.File> filesList = _googleOperation.FindAllFiles(parentFolderID);

            foreach (Google.Apis.Drive.v3.Data.File googleFile in filesList)
            {
                string combinedPath = Path.Combine(location, googleFile.Name);
                if (!File.Exists(combinedPath))
                {
                    _sqliteDB.InsertInformationdRecord($"Local file {RemovePathBeginning(combinedPath)} not found, downloading it", DateTime.Now.ToString());

                    try
                    {
                        MemoryStream stream = _googleOperation.DownloadFile(googleFile.Id);
                        using (FileStream fileStream = new FileStream(combinedPath, FileMode.Create, FileAccess.Write))
                        {
                            stream.WriteTo(fileStream);
                        }
                        _sqliteDB.InsertDownloadRecord(RemovePathBeginning(combinedPath), DateTime.Now.ToString());
                    }
                    catch(Exception e)
                    {
                        _sqliteDB.InsertErrorRecord($"Failed to download: {e.Message}", DateTime.Now.ToString());
                    }                    
                }
                else
                {
                    _sqliteDB.InsertInformationdRecord($"Local file {RemovePathBeginning(combinedPath)} found, not downloading", DateTime.Now.ToString());
                }
            }
        }

        private void DownloadFolder(string location, string parentFolderID)
        {
            List<Google.Apis.Drive.v3.Data.File> folderList = _googleOperation.FindAllFolders(parentFolderID);

            foreach (Google.Apis.Drive.v3.Data.File googleFolders in folderList)
            {
                string combinedPath = Path.Combine(location, googleFolders.Name);
                if (!Directory.Exists(combinedPath))
                {
                    _sqliteDB.InsertInformationdRecord($"Local folder {RemovePathBeginning(combinedPath)} not found, creating it", DateTime.Now.ToString());
                    Directory.CreateDirectory(combinedPath);
                    _sqliteDB.InsertDownloadRecord(RemovePathBeginning(combinedPath), DateTime.Now.ToString());
                }

                DownloadFiles(combinedPath, googleFolders.Id);
                DownloadFolder(combinedPath, googleFolders.Id);
            }
        }

        private static string RemovePathBeginning(string location)
        {
            if (location.Contains(uploadLocation))
            {
                location = location.Remove(0, uploadLocation.Length);
            }
            else if (location.Contains(downloadLocation))
            {
                location = location.Remove(0, downloadLocation.Length);
            }

            return location;
        }
    }
}