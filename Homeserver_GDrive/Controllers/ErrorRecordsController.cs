using GDriveWorker.Domain;
using Homeserver_GDrive.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Homeserver_GDrive.Controllers;

public class ErrorRecordsController : Controller
{
    private readonly ILogger<ErrorRecordsController> _logger;
    private readonly ISQLiteDB _liteDB;
    private readonly IGoogleOperations _googleOperations;

    public ErrorRecordsController(ILogger<ErrorRecordsController> logger, ISQLiteDB liteDB, IGoogleOperations googleOperations)
    {
        _logger = logger;
        _liteDB = liteDB;
        _googleOperations = googleOperations;
    }

    public IActionResult Index()
    {
        Google.Apis.Drive.v3.Data.About userInfo = _googleOperations.GetUserInfo();

        ErrorRecordsViewModel viewInfo = new ErrorRecordsViewModel()
        {
            ServiceAccountName = userInfo.User.DisplayName,
            ErrorInfo = _liteDB.LastErrorRecords(20),
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
