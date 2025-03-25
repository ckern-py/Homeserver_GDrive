namespace GDriveWorker.Domain
{
    public interface IGDriveLogic
    {
        void UploadMediaDirectory(string location);
        void DownloadMediaDirectory(string location);
    }
}