using GDriveWorker;
using GDriveWorker.Data;
using GDriveWorker.Domain;
using Microsoft.Extensions.DependencyInjection;
using System.Data.SQLite;

namespace Homeserver_GDrive
{
    public class Program
    {
        public static void Main(string[] args)
        {
            SQLiteConnection localDBConn;
            localDBConn = CreateConnection();
            CreateTable(localDBConn);

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSingleton<ISQLiteDB, SQLiteDB>();
            builder.Services.AddSingleton<IHostedService>(sp => new Worker(sp.GetService<ILogger<Worker>>(), 10000, sp.GetService<ISQLiteDB>()));
            //builder.Services.AddSingleton<IHostedService>(sp => new Worker(sp.GetService<ILogger<Worker>>(), 5000));
            //builder.Services.AddHostedService<Worker>();

            var app = builder.Build();

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

        static SQLiteConnection CreateConnection()
        {
            SQLiteConnection sqliteConn;
            // Create a new database connection:
            sqliteConn = new SQLiteConnection("Data Source=upload.db; Version = 3; New = True; Compress = True; ");
            // Open the connection:
            try
            {
                sqliteConn.Open();
            }
            catch (Exception ex)
            {

            }
            return sqliteConn;
        }

        static void CreateTable(SQLiteConnection conn)
        {
            SQLiteCommand newTableCmd;
            string createTable = "CREATE TABLE IF NOT EXISTS [FileUploads](FileName VARCHAR(200), UploadDT VARCHAR(100))";
            newTableCmd = conn.CreateCommand();
            newTableCmd.CommandText = createTable;
            newTableCmd.ExecuteNonQuery();
        }
    }
}
