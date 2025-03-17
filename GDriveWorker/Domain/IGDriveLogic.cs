using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDriveWorker.Domain
{
    public interface IGDriveLogic
    {
        string UploadMediaDirectory(string location);
    }
}
