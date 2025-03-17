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
            string parentFolderID = _googleOperation.FindFolderID(gDriveUploadFolder);

            if (string.IsNullOrEmpty(parentFolderID))
            {
                _logger.LogInformation("Could not find parent folder {parent}", gDriveUploadFolder);
                return "";
            }

            string[] files = Directory.GetFiles(location);
            foreach (string file in files)
            {
                //does file exist? if so update else upload
                string fileStatus = string.Empty;
                string fileID = _googleOperation.FindFileID(file, parentFolderID);
                if (string.IsNullOrWhiteSpace(fileID))
                {
                    fileStatus = _googleOperation.UploadFile(file, parentFolderID);
                }
                else
                {
                    fileStatus = _googleOperation.UpdateFile(file, parentFolderID, fileID);
                }
                    
            };

            string[] dir = Directory.GetDirectories(location);

            //if folder not exist create, else skip


            return "OK";
        }
    }
}
