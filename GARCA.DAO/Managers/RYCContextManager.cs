using GARCA.DAO.Properties;
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

        public void loadContext()
        {
            using (RYCContext context = getContext())
            {
                context.dateCalendar?.Load();
                context.categoriesTypes?.Load();
                context.categories?.Load();
                context.persons?.Load();
                context.accountsTypes?.Load();
                context.accounts?.Load();
                context.transactionsStatus?.Load();
                context.tags?.Load();
                context.investmentProductsTypes?.Load();
                context.investmentProducts?.Load();                
                context.investmentProductsPrices?.Load();
                context.splits?.Load();
                context.transactions?.Load();
                context.periodsReminders?.Load();
                context.splitsReminders?.Load();
                context.transactionsReminders?.Load();
                context.expirationsReminders?.Load();
                context.vBalancebyCategory?.Load();
            }
        }

        public void makeBackup()
        {
            string path = string.Empty;
            string nameDDBB = string.Empty;

            path = !Settings.Default.BBDDLocal
                ? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                    + "\\GARCA\\Data\\"
                : "Data\\";

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
