using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Homeserver_GDrive.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;

namespace Homeserver_GDrive.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    static string[] Scopes = { DriveService.Scope.Drive };
    static string ApplicationName = "penthouse-gdrive";

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        GoogleCredential credential;
        using (FileStream stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
        }
        // Create Drive API service.
        DriveService driveService = new DriveService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName
        });

        AboutResource.GetRequest aboutUser = driveService.About.Get();
        aboutUser.Fields = "kind,user,storageQuota";
        Google.Apis.Drive.v3.Data.About userInfo = aboutUser.Execute();

        return View(new HomeViewModel() { ServiceAccountName = userInfo.User.DisplayName });
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
