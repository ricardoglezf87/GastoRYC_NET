using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace GARCA.DAO.Managers
{
    public class RYCContextManager
    {
        public RYCContext getContext()
        {
            return new RYCContext();
        }

        public void migrateDataBase()
        {
            getContext().Database.Migrate();
        }
        
        public void makeBackup()
        {
            string path = string.Empty;
            string nameDDBB = string.Empty;

            path =  "Data\\";

            if (!Directory.Exists(path + "Backup\\"))
            {
                Directory.CreateDirectory(path + "Backup\\");
            }

#if DEBUG
            nameDDBB = "rycBBDD_PRE.db";
#else
            nameDDBB = "rycBBDD.db";
#endif

            if (File.Exists(path + nameDDBB))
            {
                File.Copy(path + nameDDBB, path + "Backup\\" +
                    nameDDBB + "." + DateTime.Now.Ticks.ToString() + ".bk", true);
            }
        }
    }
}
