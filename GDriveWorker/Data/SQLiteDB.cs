using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDriveWorker.Domain;
using GDriveWorker.Metadata;

namespace GDriveWorker.Data
{
    public class SQLiteDB : ISQLiteDB
    {
        public List<UploadInfo> LastFiveUploads()
        {
            List<UploadInfo> fileList = new List<UploadInfo>();
            SQLiteConnection lastFiveConn;
            lastFiveConn = CreateConnection();

            SQLiteDataReader sqliteDatareader;
            SQLiteCommand lastFiveCmd;
            lastFiveCmd = lastFiveConn.CreateCommand();
            lastFiveCmd.CommandText = "SELECT * FROM FileUploads ORDER BY UploadDT DESC LIMIT 5";

            sqliteDatareader = lastFiveCmd.ExecuteReader();
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
            SQLiteConnection newInsertConn;
            newInsertConn = CreateConnection();

            SQLiteCommand insertComand;
            insertComand = newInsertConn.CreateCommand();
            insertComand.CommandText = $"INSERT INTO FileUploads(FileName, UploadDT) VALUES('{fileName}','{uploadDT}'); ";
            int records = insertComand.ExecuteNonQuery();
            newInsertConn.Close();
            return records;
        }

        private static SQLiteConnection CreateConnection()
        {
            SQLiteConnection uploadDBSQLiteConn;
            // Create a new database connection:
            uploadDBSQLiteConn = new SQLiteConnection("Data Source=upload.db; Version = 3; New = True; Compress = True; ");
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
