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
            string topLevelFolder = _configuration["AppSettings:TopLevelGDriveUploadFolder"] ?? "";
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
            string allDownloadFolders = _configuration["AppSettings:TopLevelGDriveDownloadFolder"] ?? "";
            string[] foldersArray = allDownloadFolders.Split(',');

            foreach (string topLevelFolder in foldersArray)
            {
                string currentFolder = topLevelFolder;
                string currentFolderID = string.Empty;

                if (topLevelFolder.StartsWith("ID:"))
                {
                    currentFolderID = topLevelFolder.Remove(0, 3);
                    string foldderName = _googleOperation.GetFileByID(currentFolderID);
                    if (string.IsNullOrWhiteSpace(foldderName))
                    {
                        _logger.LogInformation("Could not find folder with ID {folderID}", currentFolderID);
                        _sqliteDB.InsertInformationdRecord($"Could not find folder with ID {currentFolderID}", DateTime.Now.ToString());
                        continue;
                    }
                    currentFolder = foldderName;
                }
                else
                {
                    currentFolderID = _googleOperation.FindFolderID(currentFolder);

                    if (string.IsNullOrWhiteSpace(currentFolderID))
                    {
                        _logger.LogInformation("Could not find parent folder {parent}", currentFolder);
                        _sqliteDB.InsertInformationdRecord($"Could not find parent folder {currentFolder}", DateTime.Now.ToString());
                        continue;
                    }
                }


                string combinedDownloadPath = Path.Combine(location, currentFolder);
                if (!Directory.Exists(combinedDownloadPath))
                {
                    _sqliteDB.InsertInformationdRecord($"Local folder {RemovePathBeginning(combinedDownloadPath)} not found, creating it", DateTime.Now.ToString());
                    Directory.CreateDirectory(combinedDownloadPath);
                    _sqliteDB.InsertDownloadRecord(RemovePathBeginning(combinedDownloadPath), DateTime.Now.ToString());
                }

                DownloadAllFiles(combinedDownloadPath, currentFolderID);

                DownloadFolder(combinedDownloadPath, currentFolderID);
            }
        }
        private void DownloadFolder(string location, string parentFolderID)
        {
            List<Google.Apis.Drive.v3.Data.File> folderList = _googleOperation.FindAllFolders(parentFolderID);

            foreach (Google.Apis.Drive.v3.Data.File googleFolders in folderList)
            {
                string combinedFolderPath = Path.Combine(location, googleFolders.Name);
                if (!Directory.Exists(combinedFolderPath))
                {
                    _sqliteDB.InsertInformationdRecord($"Local folder {RemovePathBeginning(combinedFolderPath)} not found, creating it", DateTime.Now.ToString());
                    Directory.CreateDirectory(combinedFolderPath);
                    _sqliteDB.InsertDownloadRecord(RemovePathBeginning(combinedFolderPath), DateTime.Now.ToString());
                }

                DownloadAllFiles(combinedFolderPath, googleFolders.Id);
                DownloadFolder(combinedFolderPath, googleFolders.Id);
            }
        }

        private void DownloadAllFiles(string location, string parentFolderID)
        {
            bool filesTask = Convert.ToBoolean(_configuration["AppSettings:TaskForFileDownload"] ?? "true");
            if (filesTask)
            {
                Task.Run(() => DownloadFiles(location, parentFolderID));
            }
            else
            {
                DownloadFiles(location, parentFolderID);
            }
        }

        private void DownloadFiles(string location, string parentFolderID)
        {
            List<Google.Apis.Drive.v3.Data.File> filesList = _googleOperation.FindAllFiles(parentFolderID);

            foreach (Google.Apis.Drive.v3.Data.File googleFile in filesList)
            {
                string combinedFilePath = Path.Combine(location, googleFile.Name);
                if (!File.Exists(combinedFilePath))
                {
                    _sqliteDB.InsertInformationdRecord($"Local file {RemovePathBeginning(combinedFilePath)} not found, downloading it", DateTime.Now.ToString());

                    try
                    {
                        MemoryStream stream = _googleOperation.DownloadFile(googleFile.Id);
                        using (FileStream fileStream = new FileStream(combinedFilePath, FileMode.Create, FileAccess.Write))
                        {
                            stream.WriteTo(fileStream);
                        }
                        _sqliteDB.InsertDownloadRecord(RemovePathBeginning(combinedFilePath), DateTime.Now.ToString());
                    }
                    catch (Exception e)
                    {
                        _sqliteDB.InsertErrorRecord($"Failed to download: {e.Message}", DateTime.Now.ToString());
                    }
                }
                else
                {
                    _sqliteDB.InsertInformationdRecord($"Local file {RemovePathBeginning(combinedFilePath)} found, not downloading", DateTime.Now.ToString());
                }
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