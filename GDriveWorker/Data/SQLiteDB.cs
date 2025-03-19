using GDriveWorker.Domain;
using GDriveWorker.Metadata;
using System.Data.SQLite;

namespace GDriveWorker.Data
{
    public class SQLiteDB : ISQLiteDB
    {
        public static void InitializeDB()
        {
            SQLiteConnection localDBConn = CreateConnection();

            SQLiteCommand uploadsTableCmd = localDBConn.CreateCommand();
            uploadsTableCmd.CommandText = "CREATE TABLE IF NOT EXISTS [FileUploads](FileName VARCHAR(200), UploadDT VARCHAR(100))";
            uploadsTableCmd.ExecuteNonQuery();

            SQLiteCommand errorTableCmd = localDBConn.CreateCommand();
            errorTableCmd.CommandText = "CREATE TABLE IF NOT EXISTS [Errors](Error VARCHAR(500), ErrorDT VARCHAR(100))";
            errorTableCmd.ExecuteNonQuery();

            SQLiteCommand infoTableCmd = localDBConn.CreateCommand();
            infoTableCmd.CommandText = "CREATE TABLE IF NOT EXISTS [Information](InfoMessage VARCHAR(500), InfoDT VARCHAR(100))";
            infoTableCmd.ExecuteNonQuery();
        }

        public List<BasicTableInfo> LastUploadRecords(int uploadAmount = 5)
        {
            List<BasicTableInfo> fileList = new List<BasicTableInfo>();

            SQLiteConnection lastFiveConn = CreateConnection();

            SQLiteCommand lastFiveCmd = lastFiveConn.CreateCommand();
            lastFiveCmd.CommandText = $"SELECT * FROM [FileUploads] ORDER BY ROWID DESC LIMIT {uploadAmount}";

            SQLiteDataReader sqliteDatareader = lastFiveCmd.ExecuteReader();
            while (sqliteDatareader.Read())
            {
                BasicTableInfo info = new BasicTableInfo()
                {
                    TableMessage = sqliteDatareader.GetString(0),
                    MessageDT = sqliteDatareader.GetString(1)
                };
                fileList.Add(info);
            }
            lastFiveConn.Close();

            return fileList;
        }

        public List<BasicTableInfo> LastInformationRecords(int infoAmount = 5)
        {
            List<BasicTableInfo> fileList = new List<BasicTableInfo>();

            SQLiteConnection lastFiveConn = CreateConnection();

            SQLiteCommand lastFiveCmd = lastFiveConn.CreateCommand();
            lastFiveCmd.CommandText = $"SELECT * FROM [Information] ORDER BY ROWID DESC LIMIT {infoAmount}";

            SQLiteDataReader sqliteDatareader = lastFiveCmd.ExecuteReader();
            while (sqliteDatareader.Read())
            {
                BasicTableInfo info = new BasicTableInfo()
                {
                    TableMessage = sqliteDatareader.GetString(0),
                    MessageDT = sqliteDatareader.GetString(1)
                };
                fileList.Add(info);
            }
            lastFiveConn.Close();

            return fileList;
        }

        public List<BasicTableInfo> LastErrorRecords(int errorAmount = 5)
        {
            List<BasicTableInfo> fileList = new List<BasicTableInfo>();

            SQLiteConnection lastFiveConn = CreateConnection();

            SQLiteCommand lastFiveCmd = lastFiveConn.CreateCommand();
            lastFiveCmd.CommandText = $"SELECT * FROM [Errors] ORDER BY ROWID DESC LIMIT {errorAmount}";

            SQLiteDataReader sqliteDatareader = lastFiveCmd.ExecuteReader();
            while (sqliteDatareader.Read())
            {
                BasicTableInfo info = new BasicTableInfo()
                {
                    TableMessage = sqliteDatareader.GetString(0),
                    MessageDT = sqliteDatareader.GetString(1)
                };
                fileList.Add(info);
            }
            lastFiveConn.Close();

            return fileList;
        }

        public int InsertUploadRecord(string fileName, string uploadDT)
        {
            SQLiteConnection newInsertConn = CreateConnection();

            SQLiteCommand insertComand = newInsertConn.CreateCommand();
            insertComand.CommandText = $"INSERT INTO [FileUploads](FileName, UploadDT) VALUES('{fileName}','{uploadDT}'); ";
            int records = insertComand.ExecuteNonQuery();
            newInsertConn.Close();
            return records;
        }

        public int InsertInformationdRecord(string infoMessage, string infoDT)
        {
            SQLiteConnection newInsertConn = CreateConnection();

            SQLiteCommand insertComand = newInsertConn.CreateCommand();
            insertComand.CommandText = $"INSERT INTO [Information](InfoMessage, InfoDT) VALUES('{infoMessage}','{infoDT}'); ";
            int records = insertComand.ExecuteNonQuery();
            newInsertConn.Close();
            return records;
        }

        public int InsertErrorRecord(string errorMessage, string errorDT)
        {
            SQLiteConnection newInsertConn = CreateConnection();

            SQLiteCommand insertComand = newInsertConn.CreateCommand();
            insertComand.CommandText = $"INSERT INTO [Errors](Error, ErrorDT) VALUES('{errorMessage}','{errorDT}'); ";
            int records = insertComand.ExecuteNonQuery();
            newInsertConn.Close();
            return records;
        }

        public int DeleteOldFileUploadsRecords()
        {
            SQLiteConnection newDeleteConn = CreateConnection();

            SQLiteCommand deleteComand = newDeleteConn.CreateCommand();
            deleteComand.CommandText = $"DELETE FROM FileUploads WHERE ROWID NOT IN (SELECT ROWID FROM FileUploads ORDER BY ROWID DESC LIMIT 50)";
            int records = deleteComand.ExecuteNonQuery();
            newDeleteConn.Close();
            return records;
        }

        public int DeleteOldInformationRecords()
        {
            SQLiteConnection newDeleteConn = CreateConnection();

            SQLiteCommand deleteComand = newDeleteConn.CreateCommand();
            deleteComand.CommandText = $"DELETE FROM [Information] WHERE ROWID NOT IN (SELECT ROWID FROM [Information] ORDER BY ROWID DESC LIMIT 50)";
            int records = deleteComand.ExecuteNonQuery();
            newDeleteConn.Close();
            return records;
        }

        public int DeleteOldErrorsRecords()
        {
            SQLiteConnection newDeleteConn = CreateConnection();

            SQLiteCommand deleteComand = newDeleteConn.CreateCommand();
            deleteComand.CommandText = $"DELETE FROM [Errors] WHERE ROWID NOT IN (SELECT ROWID FROM [Errors] ORDER BY ROWID DESC LIMIT 50)";
            int records = deleteComand.ExecuteNonQuery();
            newDeleteConn.Close();
            return records;
        }

        public int CountUploadRecords()
        {
            SQLiteConnection newCountConn = CreateConnection();

            SQLiteCommand countComand = newCountConn.CreateCommand();
            countComand.CommandText = $"SELECT COUNT(*) FROM [FileUploads]";
            int records = Convert.ToInt32(countComand.ExecuteScalar());
            newCountConn.Close();
            return records;
        }

        public int CountInfoRecords()
        {
            SQLiteConnection newCountConn = CreateConnection();

            SQLiteCommand countComand = newCountConn.CreateCommand();
            countComand.CommandText = $"SELECT COUNT(*) FROM [Information]";
            int records = Convert.ToInt32(countComand.ExecuteScalar());
            newCountConn.Close();
            return records;
        }

        public int CountErrorRecords()
        {
            SQLiteConnection newCountConn = CreateConnection();

            SQLiteCommand countComand = newCountConn.CreateCommand();
            countComand.CommandText = $"SELECT COUNT(*) FROM [Errors]";
            int records = Convert.ToInt32(countComand.ExecuteScalar());
            newCountConn.Close();
            return records;
        }

        private static SQLiteConnection CreateConnection()
        {
            //SQLiteConnection uploadDBSQLiteConn = new SQLiteConnection("Data Source=/../config/upload.db; Version = 3;");
            SQLiteConnection uploadDBSQLiteConn = new SQLiteConnection("Data Source=upload.db; Version = 3;");

            if (uploadDBSQLiteConn.State != System.Data.ConnectionState.Open)
            {
                try
                {
                    uploadDBSQLiteConn.Open();
                }
                catch (Exception ex)
                {

                }
            }

            return uploadDBSQLiteConn;
        }
    }
}
