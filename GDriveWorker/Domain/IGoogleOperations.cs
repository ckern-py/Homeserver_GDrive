namespace GDriveWorker.Domain
{
    public interface IGoogleOperations
    {
        Google.Apis.Drive.v3.Data.About GetUserInfo();
    }
}
