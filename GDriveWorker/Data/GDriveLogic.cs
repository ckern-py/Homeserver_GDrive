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

        public string UploadFiles(string location)
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
                string fileStatus = _googleOperation.UploadFile(file, parentFolderID);
            };

            string[] dir = Directory.GetDirectories(location);

            //if folder not exist create, else skip


            return "OK";
        }
    }
}
