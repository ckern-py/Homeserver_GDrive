using GDriveWorker.Domain;
using Homeserver_GDrive.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Homeserver_GDrive.Controllers;

public class ErrorRecordsController : Controller
{
    private readonly ISQLiteDB _liteDB;
    private readonly IGoogleOperations _googleOperations;
    private readonly IConfiguration _configuration;

    public ErrorRecordsController(ISQLiteDB liteDB, IGoogleOperations googleOperations, IConfiguration configuration)
    {
        _liteDB = liteDB;
        _googleOperations = googleOperations;
        _configuration = configuration;
    }

    public IActionResult Index()
    {
        Google.Apis.Drive.v3.Data.About userInfo = _googleOperations.GetUserInfo();

        int errorRecordCount = Convert.ToInt32(_configuration["AppSettings:DetailErrorRecordsCount"]);
        ErrorRecordsViewModel viewInfo = new ErrorRecordsViewModel()
        {
            ServiceAccountName = userInfo.User.DisplayName,
            ErrorInfo = _liteDB.LastErrorRecords(errorRecordCount),
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
