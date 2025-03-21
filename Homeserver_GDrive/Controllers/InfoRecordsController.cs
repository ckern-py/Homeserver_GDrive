using GDriveWorker.Domain;
using Homeserver_GDrive.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Homeserver_GDrive.Controllers;

public class InfoRecordsController : Controller
{
    private readonly ISQLiteDB _liteDB;
    private readonly IGoogleOperations _googleOperations;
    private readonly IConfiguration _configuration;

    public InfoRecordsController(ISQLiteDB liteDB, IGoogleOperations googleOperations, IConfiguration configuration)
    {
        _liteDB = liteDB;
        _googleOperations = googleOperations;
        _configuration = configuration;
    }

    public IActionResult Index()
    {
        Google.Apis.Drive.v3.Data.About userInfo = _googleOperations.GetUserInfo();

        int infoRecordCount = Convert.ToInt32(_configuration["AppSettings:DetailInfoRecordsCount"]);
        InfoRecordsViewModel viewInfo = new InfoRecordsViewModel()
        {
            ServiceAccountName = userInfo.User.DisplayName,
            InfoInfo = _liteDB.LastInformationRecords(infoRecordCount),
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