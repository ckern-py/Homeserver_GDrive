using GDriveWorker.Domain;
using GDriveWorker.Metadata;
using System.Data.SQLite;

namespace GDriveWorker.Data
{
    public class SQLiteDB : ISQLiteDB
    {
        private static readonly string _sqliteConnection = "Data Source=/../config/gdrive.db; Version = 3;";

        public static void InitializeDB()
        {
            using (SQLiteConnection localDBConn = new SQLiteConnection(_sqliteConnection))
            {
                localDBConn.Open();

                SQLiteCommand uploadsTableCmd = localDBConn.CreateCommand();
                uploadsTableCmd.CommandText = "CREATE TABLE IF NOT EXISTS [FileUploads](FileName VARCHAR(200), UploadDT VARCHAR(100))";
                uploadsTableCmd.ExecuteNonQuery();

                SQLiteCommand downloadsTableCmd = localDBConn.CreateCommand();
                downloadsTableCmd.CommandText = "CREATE TABLE IF NOT EXISTS [FileDownloads](FileName VARCHAR(200), DownloadDT VARCHAR(100))";
                downloadsTableCmd.ExecuteNonQuery();

                SQLiteCommand infoTableCmd = localDBConn.CreateCommand();
                infoTableCmd.CommandText = "CREATE TABLE IF NOT EXISTS [Information](InfoMessage VARCHAR(500), InfoDT VARCHAR(100))";
                infoTableCmd.ExecuteNonQuery();

                SQLiteCommand errorTableCmd = localDBConn.CreateCommand();
                errorTableCmd.CommandText = "CREATE TABLE IF NOT EXISTS [Errors](Error VARCHAR(500), ErrorDT VARCHAR(100))";
                errorTableCmd.ExecuteNonQuery();
            }
        }

        public DateTime GetFileUploadTime(string fileName)
        {
            using (SQLiteConnection fileUploadConnection = new SQLiteConnection(_sqliteConnection))
            {
                fileUploadConnection.Open();

                SQLiteCommand lastUploadTime = fileUploadConnection.CreateCommand();

                lastUploadTime.CommandText = "SELECT UploadDT FROM [FileUploads] WHERE FileName = @fileName ORDER BY ROWID DESC LIMIT 1";
                lastUploadTime.Parameters.AddWithValue("@fileName", fileName);

                SQLiteDataReader sqliteDatareader = lastUploadTime.ExecuteReader();
                DateTime lastUpload = DateTime.MinValue;
                while (sqliteDatareader.Read())
                {
                    lastUpload = Convert.ToDateTime(sqliteDatareader.GetString(0));
                }

                return lastUpload;
            }
        }

        public List<BasicTableInfo> LastUploadRecords(int uploadAmount = 5)
        {
            using (SQLiteConnection lastUploadsConnection = new SQLiteConnection(_sqliteConnection))
            {
                lastUploadsConnection.Open();

                SQLiteCommand lastFiveCmd = lastUploadsConnection.CreateCommand();

                lastFiveCmd.CommandText = "SELECT * FROM [FileUploads] ORDER BY ROWID DESC LIMIT @uploadAmount";
                lastFiveCmd.Parameters.AddWithValue("@uploadAmount", uploadAmount);

                SQLiteDataReader sqliteDatareader = lastFiveCmd.ExecuteReader();
                List<BasicTableInfo> fileList = new List<BasicTableInfo>();
                while (sqliteDatareader.Read())
                {
                    BasicTableInfo info = new BasicTableInfo()
                    {
                        TableMessage = sqliteDatareader.GetString(0),
                        MessageDT = sqliteDatareader.GetString(1)
                    };
                    fileList.Add(info);
                }

                return fileList;
            }
        }

        public List<BasicTableInfo> LastDownloadRecords(int downloadAmount = 5)
        {
            using (SQLiteConnection lastUploadsConnection = new SQLiteConnection(_sqliteConnection))
            {
                lastUploadsConnection.Open();

                SQLiteCommand lastFiveCmd = lastUploadsConnection.CreateCommand();

                lastFiveCmd.CommandText = "SELECT * FROM [FileDownloads] ORDER BY ROWID DESC LIMIT @downloadAmount";
                lastFiveCmd.Parameters.AddWithValue("@downloadAmount", downloadAmount);

                SQLiteDataReader sqliteDatareader = lastFiveCmd.ExecuteReader();
                List<BasicTableInfo> fileList = new List<BasicTableInfo>();
                while (sqliteDatareader.Read())
                {
                    BasicTableInfo info = new BasicTableInfo()
                    {
                        TableMessage = sqliteDatareader.GetString(0),
                        MessageDT = sqliteDatareader.GetString(1)
                    };
                    fileList.Add(info);
                }

                return fileList;
            }
        }

        public List<BasicTableInfo> LastInformationRecords(int infoAmount = 5)
        {
            using (SQLiteConnection lastInfoConnection = new SQLiteConnection(_sqliteConnection))
            {
                lastInfoConnection.Open();

                SQLiteCommand lastFiveCmd = lastInfoConnection.CreateCommand();

                lastFiveCmd.CommandText = "SELECT * FROM [Information] ORDER BY ROWID DESC LIMIT @infoAmount";
                lastFiveCmd.Parameters.AddWithValue("@infoAmount", infoAmount);

                SQLiteDataReader sqliteDatareader = lastFiveCmd.ExecuteReader();
                List<BasicTableInfo> fileList = new List<BasicTableInfo>();
                while (sqliteDatareader.Read())
                {
                    BasicTableInfo info = new BasicTableInfo()
                    {
                        TableMessage = sqliteDatareader.GetString(0),
                        MessageDT = sqliteDatareader.GetString(1)
                    };
                    fileList.Add(info);
                }

                return fileList;
            }
        }

        public List<BasicTableInfo> LastErrorRecords(int errorAmount = 5)
        {
            using (SQLiteConnection lastErrorsConnection = new SQLiteConnection(_sqliteConnection))
            {
                lastErrorsConnection.Open();

                SQLiteCommand lastFiveCmd = lastErrorsConnection.CreateCommand();

                lastFiveCmd.CommandText = "SELECT * FROM [Errors] ORDER BY ROWID DESC LIMIT @errorAmount";
                lastFiveCmd.Parameters.AddWithValue("@errorAmount", errorAmount);

                SQLiteDataReader sqliteDatareader = lastFiveCmd.ExecuteReader();
                List<BasicTableInfo> fileList = new List<BasicTableInfo>();
                while (sqliteDatareader.Read())
                {
                    BasicTableInfo info = new BasicTableInfo()
                    {
                        TableMessage = sqliteDatareader.GetString(0),
                        MessageDT = sqliteDatareader.GetString(1)
                    };
                    fileList.Add(info);
                }

                return fileList;
            }
        }

        public int InsertUploadRecord(string fileName, string uploadDT)
        {
            using (SQLiteConnection newInsertConn = new SQLiteConnection(_sqliteConnection))
            {
                newInsertConn.Open();

                SQLiteCommand insertComand = newInsertConn.CreateCommand();

                insertComand.CommandText = "INSERT INTO [FileUploads](FileName, UploadDT) VALUES(@fileName, @uploadDT);";
                insertComand.Parameters.AddWithValue("@fileName", fileName);
                insertComand.Parameters.AddWithValue("@uploadDT", uploadDT);

                int records = insertComand.ExecuteNonQuery();
                return records;
            }
        }
        public int InsertDownloadRecord(string fileName, string downloadDT)
        {
            using (SQLiteConnection newInsertConn = new SQLiteConnection(_sqliteConnection))
            {
                newInsertConn.Open();

                SQLiteCommand insertComand = newInsertConn.CreateCommand();
                insertComand.CommandText = "INSERT INTO [FileDownloads](FileName, DownloadDT) VALUES(@fileName, @downloadDT);";
                insertComand.Parameters.AddWithValue("@fileName", fileName);
                insertComand.Parameters.AddWithValue("@downloadDT", downloadDT);

                int records = insertComand.ExecuteNonQuery();
                return records;
            }
        }

        public int InsertInformationdRecord(string infoMessage, string infoDT)
        {
            using (SQLiteConnection newInsertConn = new SQLiteConnection(_sqliteConnection))
            {
                newInsertConn.Open();

                SQLiteCommand insertComand = newInsertConn.CreateCommand();

                insertComand.CommandText = "INSERT INTO [Information](InfoMessage, InfoDT) VALUES(@infoMessage, @infoDT);";
                insertComand.Parameters.AddWithValue("@infoMessage", infoMessage);
                insertComand.Parameters.AddWithValue("@infoDT", infoDT);

                int records = insertComand.ExecuteNonQuery();
                return records;
            }
        }

        public int InsertErrorRecord(string errorMessage, string errorDT)
        {
            using (SQLiteConnection newInsertConn = new SQLiteConnection(_sqliteConnection))
            {
                newInsertConn.Open();

                SQLiteCommand insertComand = newInsertConn.CreateCommand();

                insertComand.CommandText = "INSERT INTO [Errors](Error, ErrorDT) VALUES(@errorMessage, @errorDT);";
                insertComand.Parameters.AddWithValue("@errorMessage", errorMessage);
                insertComand.Parameters.AddWithValue("@errorDT", errorDT);

                int records = insertComand.ExecuteNonQuery();
                return records;
            }
        }

        public int DeleteOldFileUploadsRecords()
        {
            using (SQLiteConnection newDeleteConn = new SQLiteConnection(_sqliteConnection))
            {
                newDeleteConn.Open();

                SQLiteCommand deleteComand = newDeleteConn.CreateCommand();
                deleteComand.CommandText = $"DELETE FROM FileUploads WHERE ROWID NOT IN (SELECT ROWID FROM FileUploads ORDER BY ROWID DESC LIMIT 50)";
                int records = deleteComand.ExecuteNonQuery();
                return records;
            }
        }

        public int DeleteOldFileDownloadsRecords()
        {
            using (SQLiteConnection newDeleteConn = new SQLiteConnection(_sqliteConnection))
            {
                newDeleteConn.Open();

                SQLiteCommand deleteComand = newDeleteConn.CreateCommand();
                deleteComand.CommandText = $"DELETE FROM FileDownloads WHERE ROWID NOT IN (SELECT ROWID FROM FileDownloads ORDER BY ROWID DESC LIMIT 50)";
                int records = deleteComand.ExecuteNonQuery();
                return records;
            }
        }

        public int DeleteOldInformationRecords()
        {
            using (SQLiteConnection newDeleteConn = new SQLiteConnection(_sqliteConnection))
            {
                newDeleteConn.Open();

                SQLiteCommand deleteComand = newDeleteConn.CreateCommand();
                deleteComand.CommandText = $"DELETE FROM [Information] WHERE ROWID NOT IN (SELECT ROWID FROM [Information] ORDER BY ROWID DESC LIMIT 47)";
                int records = deleteComand.ExecuteNonQuery();
                return records;
            }
        }

        public int DeleteOldErrorsRecords()
        {
            using (SQLiteConnection newDeleteConn = new SQLiteConnection(_sqliteConnection))
            {
                newDeleteConn.Open();

                SQLiteCommand deleteComand = newDeleteConn.CreateCommand();
                deleteComand.CommandText = $"DELETE FROM [Errors] WHERE ROWID NOT IN (SELECT ROWID FROM [Errors] ORDER BY ROWID DESC LIMIT 50)";
                int records = deleteComand.ExecuteNonQuery();
                return records;
            }
        }

        public int CountUploadRecords()
        {
            using (SQLiteConnection countUploadsConnection = new SQLiteConnection(_sqliteConnection))
            {
                countUploadsConnection.Open();

                SQLiteCommand countComand = countUploadsConnection.CreateCommand();
                countComand.CommandText = $"SELECT COUNT(*) FROM [FileUploads]";
                int records = Convert.ToInt32(countComand.ExecuteScalar());
                return records;
            }
        }

        public int CountDownloadRecords()
        {
            using (SQLiteConnection countUploadsConnection = new SQLiteConnection(_sqliteConnection))
            {
                countUploadsConnection.Open();

                SQLiteCommand countComand = countUploadsConnection.CreateCommand();
                countComand.CommandText = $"SELECT COUNT(*) FROM [FileDownloads]";
                int records = Convert.ToInt32(countComand.ExecuteScalar());
                return records;
            }
        }

        public int CountInfoRecords()
        {
            using (SQLiteConnection countInfoConnection = new SQLiteConnection(_sqliteConnection))
            {
                countInfoConnection.Open();

                SQLiteCommand countComand = countInfoConnection.CreateCommand();
                countComand.CommandText = $"SELECT COUNT(*) FROM [Information]";
                int records = Convert.ToInt32(countComand.ExecuteScalar());
                return records;
            }
        }

        public int CountErrorRecords()
        {
            using (SQLiteConnection countErrorsConnection = new SQLiteConnection(_sqliteConnection))
            {
                countErrorsConnection.Open();

                SQLiteCommand countComand = countErrorsConnection.CreateCommand();
                countComand.CommandText = $"SELECT COUNT(*) FROM [Errors]";
                int records = Convert.ToInt32(countComand.ExecuteScalar());
                return records;
            }
        }
    }
}