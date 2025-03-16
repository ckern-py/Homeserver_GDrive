using GDriveWorker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDriveWorker.Data
{
    public class GDriveLogic : IGDriveLogic
    {
        private string gDriveUploadFolder = "HomeServer_GDrive";
        private readonly IGoogleOperations _googleOperation;
        private readonly ILogger<GDriveLogic> _logger;

        public GDriveLogic (ILogger<GDriveLogic> logger, IGoogleOperations googleOperations)
        {
            _logger = logger;
            _googleOperation = googleOperations;
        }

        public string UploadFiles()
        {
            string parentFolderID = _googleOperation.FindFolderID(gDriveUploadFolder);

            if (string.IsNullOrEmpty(parentFolderID))
            {
                _logger.LogInformation("Could not find parent folder {parent}", gDriveUploadFolder);
                return "";
            }

            string[] dir = Directory.GetDirectories("/../media/");
            
            //if folder not exist create, else skip

            
            return "OK";
        }
    }
}
