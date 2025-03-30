using GDriveWorker.Domain;
using Homeserver_GDrive.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Homeserver_GDrive.Controllers;

public class DownloadRecordsController : Controller
{
    private readonly ISQLiteDB _liteDB;
    private readonly IGoogleOperations _googleOperations;
    private readonly IConfiguration _configuration;

    public DownloadRecordsController(ISQLiteDB liteDB, IGoogleOperations googleOperations, IConfiguration configuration)
    {
        _liteDB = liteDB;
        _googleOperations = googleOperations;
        _configuration = configuration;
    }

    public IActionResult Index()
    {
        Google.Apis.Drive.v3.Data.About userInfo = _googleOperations.GetUserInfo();

        int uploadRecordCount = Convert.ToInt32(_configuration["AppSettings:DetailDownloadRecordsCount"]);
        DownloadRecordsViewModel viewInfo = new DownloadRecordsViewModel()
        {
            ServiceAccountName = userInfo.User.DisplayName,
            DownloadInfo = _liteDB.LastDownloadRecords(uploadRecordCount),
            DownloadCount = _liteDB.CountDownloadRecords()
        };

        return View(viewInfo);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}