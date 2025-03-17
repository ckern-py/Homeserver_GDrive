using GDriveWorker.Domain;

namespace GDriveWorker.Data
{
    public class GDriveLogic : IGDriveLogic
    {
        private string gDriveUploadFolder = "HomeServer_GDrive";
        private readonly IGoogleOperations _googleOperation;
        private readonly ILogger<GDriveLogic> _logger;

        public GDriveLogic(ILogger<GDriveLogic> logger, IGoogleOperations googleOperations)
        {
            _logger = logger;
            _googleOperation = googleOperations;
        }

        public string UploadMediaDirectory(string location)
        {
            string parentFolderID = _googleOperation.FindFirstFolderID(gDriveUploadFolder);

            if (string.IsNullOrWhiteSpace(parentFolderID))
            {
                _logger.LogInformation("Could not find parent folder {parent}", gDriveUploadFolder);
                return "";
            }

            string[] files = Directory.GetFiles(location);
            foreach (string file in files)
            {
                //does file exist? if so update else upload
                //TODO:add errors to error table
                string fileStatus = string.Empty;
                string fileID = _googleOperation.FindFileID(Path.GetFileName(file), parentFolderID);
                if (string.IsNullOrWhiteSpace(fileID))
                {
                    fileStatus = _googleOperation.UploadFile(file, parentFolderID);
                }
                else
                {
                    fileStatus = _googleOperation.UpdateFile(file, fileID);
                }
            }

            string[] dir = Directory.GetDirectories(location);

            foreach (string directory in dir)
            {
                string justFolder = new DirectoryInfo(directory).Name;
                string folderID = _googleOperation.FindFolderID(justFolder, parentFolderID);
                if (string.IsNullOrWhiteSpace(folderID))
                {
                    _googleOperation.CreateFolder(justFolder, parentFolderID);
                }
            }

            //if folder not exist create, else skip

            return "OK";
        }
    }
}
