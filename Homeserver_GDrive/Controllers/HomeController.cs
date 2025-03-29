using GDriveWorker.Domain;
using Homeserver_GDrive.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Homeserver_GDrive.Controllers;

public class HomeController : Controller
{
    private readonly ISQLiteDB _liteDB;
    private readonly IGoogleOperations _googleOperations;

    public HomeController(ISQLiteDB liteDB, IGoogleOperations googleOperations)
    {
        _liteDB = liteDB;
        _googleOperations = googleOperations;
    }

    public IActionResult Index()
    {
        Google.Apis.Drive.v3.Data.About userInfo = _googleOperations.GetUserInfo();

        HomeViewModel viewInfo = new HomeViewModel()
        {
            ServiceAccountName = userInfo.User.DisplayName,
            UploadInfo = _liteDB.LastUploadRecords(),
            UploadCount = _liteDB.CountUploadRecords(),
            DownloadInfo = _liteDB.LastDownloadRecords(),
            DownloadCount = _liteDB.CountDownloadRecords(),
            InfoInfo = _liteDB.LastInformationRecords(),
            InfoCount = _liteDB.CountInfoRecords(),
            ErrorInfo = _liteDB.LastErrorRecords(),
            ErrorCount = _liteDB.CountErrorRecords()
        };

        return View(viewInfo);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}