using GDriveWorker.Domain;
using Homeserver_GDrive.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Homeserver_GDrive.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ISQLiteDB _liteDB;
    private readonly IGoogleOperations _googleOperations;

    public HomeController(ILogger<HomeController> logger, ISQLiteDB liteDB, IGoogleOperations googleOperations)
    {
        _logger = logger;
        _liteDB = liteDB;
        _googleOperations = googleOperations;
    }

    public IActionResult Index()
    {
        Google.Apis.Drive.v3.Data.About userInfo = _googleOperations.GetUserInfo();

        HomeViewModel viewInfo = new HomeViewModel()
        {
            ServiceAccountName = userInfo.User.DisplayName,
            UploadInfo = _liteDB.LastFiveUploads(),
            RecordCount = _liteDB.CountRecords()
        };

        return View(viewInfo);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
