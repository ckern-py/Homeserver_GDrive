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

            SQLiteCommand newTableCmd;
            string createTable = "CREATE TABLE IF NOT EXISTS [FileUploads](FileName VARCHAR(200), UploadDT VARCHAR(100))";
            newTableCmd = localDBConn.CreateCommand();
            newTableCmd.CommandText = createTable;
            newTableCmd.ExecuteNonQuery();
        }

        public List<UploadInfo> LastFiveUploads()
        {
            List<UploadInfo> fileList = new List<UploadInfo>();

            SQLiteConnection lastFiveConn = CreateConnection();

            SQLiteCommand lastFiveCmd = lastFiveConn.CreateCommand();
            lastFiveCmd.CommandText = "SELECT * FROM FileUploads ORDER BY UploadDT DESC LIMIT 5";

            SQLiteDataReader sqliteDatareader = lastFiveCmd.ExecuteReader();
            while (sqliteDatareader.Read())
            {
                UploadInfo info = new UploadInfo()
                {
                    FileName = sqliteDatareader.GetString(0),
                    UploadDT = sqliteDatareader.GetString(1)
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
            insertComand.CommandText = $"INSERT INTO FileUploads(FileName, UploadDT) VALUES('{fileName}','{uploadDT}'); ";
            int records = insertComand.ExecuteNonQuery();
            newInsertConn.Close();
            return records;
        }

        public int DeleteOldRecords()
        {
            SQLiteConnection newDeleteConn = CreateConnection();

            SQLiteCommand deleteComand = newDeleteConn.CreateCommand();
            //deleteComand.CommandText = $"INSERT INTO FileUploads(FileName, UploadDT) VALUES('{fileName}','{uploadDT}'); ";
            deleteComand.CommandText = $"DELETE FROM FileUploads WHERE UploadDT NOT IN (SELECT UploadDT FROM FileUploads ORDER BY UploadDT DESC LIMIT 50)";
            int records = deleteComand.ExecuteNonQuery();
            newDeleteConn.Close();
            return records;

        }

        public int CountRecords()
        {
            SQLiteConnection newCountConn = CreateConnection();

            SQLiteCommand countComand = newCountConn.CreateCommand();
            countComand.CommandText = $"SELECT COUNT(*) FROM FileUploads";
            int records = Convert.ToInt32(countComand.ExecuteScalar());
            newCountConn.Close();
            return records;
        }

        private static SQLiteConnection CreateConnection()
        {
            SQLiteConnection uploadDBSQLiteConn = new SQLiteConnection("Data Source=upload.db; Version = 3; New = True; Compress = True; ");
            // Open the connection:
            try
            {
                uploadDBSQLiteConn.Open();
            }
            catch (Exception ex)
            {

            }
            return uploadDBSQLiteConn;
        }
    }
}
