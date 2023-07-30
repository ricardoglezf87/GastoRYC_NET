using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace GARCA.DAO.Managers
{
    public class RycContextManager
    {
        private RycContext GetContext()
        {
            return new RycContext();
        }

        public void MigrateDataBase()
        {
            GetContext().Database.Migrate();
        }
        
        public void MakeBackup()
        {
            var path = string.Empty;
            var nameDdbb = string.Empty;

            path =  "Data\\";

            if (!Directory.Exists(path + "Backup\\"))
            {
                Directory.CreateDirectory(path + "Backup\\");
            }

#if DEBUG
            nameDdbb = "rycBBDD_PRE.db";
#else
            nameDDBB = "rycBBDD.db";
#endif

            if (File.Exists(path + nameDdbb))
            {
                File.Copy(path + nameDdbb, path + "Backup\\" +
                    nameDdbb + "." + DateTime.Now.Ticks.ToString() + ".bk", true);
            }
        }
    }
}
