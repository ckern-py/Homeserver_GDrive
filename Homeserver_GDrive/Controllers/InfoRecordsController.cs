using GDriveWorker.Domain;
using Homeserver_GDrive.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Homeserver_GDrive.Controllers;

public class InfoRecordsController : Controller
{
    private readonly ILogger<InfoRecordsController> _logger;
    private readonly ISQLiteDB _liteDB;
    private readonly IGoogleOperations _googleOperations;

    public InfoRecordsController(ILogger<InfoRecordsController> logger, ISQLiteDB liteDB, IGoogleOperations googleOperations)
    {
        _logger = logger;
        _liteDB = liteDB;
        _googleOperations = googleOperations;
    }

    public IActionResult Index()
    {
        Google.Apis.Drive.v3.Data.About userInfo = _googleOperations.GetUserInfo();

        InfoRecordsViewModel viewInfo = new InfoRecordsViewModel()
        {
            ServiceAccountName = userInfo.User.DisplayName,
            InfoInfo = _liteDB.LastInformationRecords(20),
            InfoCount = _liteDB.CountInfoRecords()
        };

        return View(viewInfo);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
