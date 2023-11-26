using Microsoft.Data.Sqlite;
using SQLitePCL;

namespace GARCA.Data.Services
{
    public class RycContextService
    {
        public SqliteConnection getConnection()
        {
            Batteries.Init();
            return new SqliteConnection("Data Source=Data\\rycBBDD_PRE.db");
        }

        public void MakeBackup()
        {
            var path = string.Empty;
            var nameDdbb = string.Empty;

            path = "Data\\";

            if (!Directory.Exists(path + "Backup\\"))
            {
                Directory.CreateDirectory(path + "Backup\\");
            }

#if DEBUG
            nameDdbb = "rycBBDD_PRE.db";
#else
            nameDdbb = "rycBBDD.db";
#endif

            if (File.Exists(path + nameDdbb))
            {
                File.Copy(path + nameDdbb, path + "Backup\\" +
                    nameDdbb + "." + DateTime.Now.Ticks + ".bk", true);
            }
        }
    }
}
