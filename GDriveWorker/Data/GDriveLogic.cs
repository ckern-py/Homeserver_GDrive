using GDriveWorker.Domain;

namespace GDriveWorker.Data
{
    public class GDriveLogic : IGDriveLogic
    {
        private readonly string gDriveUploadFolder = "HomeServer_GDrive";
        private readonly IGoogleOperations _googleOperation;
        private readonly ILogger<GDriveLogic> _logger;

        public GDriveLogic(ILogger<GDriveLogic> logger, IGoogleOperations googleOperations)
        {
            _logger = logger;
            _googleOperation = googleOperations;
        }

        public void UploadMediaDirectory(string location)
        {
            string parentFolderID = _googleOperation.FindFolderID(gDriveUploadFolder);

            if (string.IsNullOrWhiteSpace(parentFolderID))
            {
                _logger.LogInformation("Could not find parent folder {parent}", gDriveUploadFolder);
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
                //TODO:add errors to error table
                //TODO:add success to upload table
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
        }

        private void UploadFolder(string location, string parentFolderID)
        {
            string[] dir = Directory.GetDirectories(location);
            //TODO:add errors to error table
            //TODO:add success to upload table
            foreach (string directory in dir)
            {
                string justFolder = new DirectoryInfo(directory).Name;
                string folderID = _googleOperation.FindFolderID(justFolder, parentFolderID);
                if (string.IsNullOrWhiteSpace(folderID))
                {
                    folderID = _googleOperation.CreateFolder(justFolder, parentFolderID);
                }

                UploadFiles(directory, folderID);
                UploadFolder(directory, folderID);
            }
        }
    }
}
