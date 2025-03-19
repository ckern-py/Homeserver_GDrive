using GDriveWorker.Domain;
using Homeserver_GDrive.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Homeserver_GDrive.Controllers;

public class UploadRecordsController : Controller
{
    private readonly ILogger<UploadRecordsController> _logger;
    private readonly ISQLiteDB _liteDB;
    private readonly IGoogleOperations _googleOperations;

    public UploadRecordsController(ILogger<UploadRecordsController> logger, ISQLiteDB liteDB, IGoogleOperations googleOperations)
    {
        _logger = logger;
        _liteDB = liteDB;
        _googleOperations = googleOperations;
    }

    public IActionResult Index()
    {
        Google.Apis.Drive.v3.Data.About userInfo = _googleOperations.GetUserInfo();

        UploadrRecordsViewModel viewInfo = new UploadrRecordsViewModel()
        {
            ServiceAccountName = userInfo.User.DisplayName,
            UploadInfo = _liteDB.LastUploadRecords(20),
            UploadCount = _liteDB.CountUploadRecords()
        };

        return View(viewInfo);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
