using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDriveWorker.Metadata;

namespace GDriveWorker.Domain
{
    public interface ISQLiteDB
    {
        List<UploadInfo> LastFiveUploads();
        int InsertUploadRecord(string fileName, string uploadDT);
        int DeleteOldRecords();
        int CountRecords();
    }
}
