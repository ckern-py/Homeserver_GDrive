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

        public GDriveLogic(ILogger<GDriveLogic> logger, IGoogleOperations googleOperations, ISQLiteDB sqliteDB, IConfiguration configuration)
        {
            _logger = logger;
            _googleOperation = googleOperations;
            _sqliteDB = sqliteDB;
            _configuration = configuration;
        }

        public void UploadMediaDirectory(string location)
        {
            string topLevelFolder = _configuration["AppSettings:TopLevelGDriveFolder"];
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
                    _sqliteDB.InsertInformationdRecord($"File {file} not found, uploading it", DateTime.Now.ToString());
                    fileStatus = _googleOperation.UploadFile(file, parentFolderID);
                }
                else
                {
                    _sqliteDB.InsertInformationdRecord($"File {file} found, updating it", DateTime.Now.ToString());
                    fileStatus = _googleOperation.UpdateFile(file, fileID);
                }

                if (fileStatus.Status == UploadStatus.Completed)
                {
                    _sqliteDB.InsertUploadRecord(file, DateTime.Now.ToString());
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
                    _sqliteDB.InsertInformationdRecord($"Folder {directory} not found, creating it", DateTime.Now.ToString());
                    folderID = _googleOperation.CreateFolder(justFolder, parentFolderID);
                    _sqliteDB.InsertUploadRecord(directory, DateTime.Now.ToString());
                }

                UploadFiles(directory, folderID);
                UploadFolder(directory, folderID);
            }
        }
    }
}
