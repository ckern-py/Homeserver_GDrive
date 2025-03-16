using GDriveWorker;
using GDriveWorker.Data;
using GDriveWorker.Domain;

namespace Homeserver_GDrive
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            SQLiteDB.InitializeDB();

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSingleton<ISQLiteDB, SQLiteDB>();
            builder.Services.AddScoped<IGoogleOperations, GoogleOperations>();
            //builder.Services.AddSingleton<IHostedService>(sp => new Worker(sp.GetService<ILogger<Worker>>(), 10000, sp.GetService<ISQLiteDB>()));
            //builder.Services.AddSingleton<IHostedService>(sp => new Worker(sp.GetService<ILogger<Worker>>(), 5000));
            builder.Services.AddHostedService<UploadService>();
            builder.Services.AddHostedService<DBMaintenance>();

            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
