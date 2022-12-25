using BBDDLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GastosRYC.BBDDLib.Services
{
    public class ExpirationsRemindersService
    {
        public List<ExpirationsReminders>? getAll()
        {
            return RYCContextService.getInstance().BBDD.expirationsReminders?.ToList();
        }

        public ExpirationsReminders? getByID(int? id)
        {
            return RYCContextService.getInstance().BBDD.expirationsReminders?.FirstOrDefault(x => id.Equals(x.id));
        }

        public void update(ExpirationsReminders expirationsReminders)
        {
            RYCContextService.getInstance().BBDD.Update(expirationsReminders);
            RYCContextService.getInstance().BBDD.SaveChanges();
        }

        public void delete(ExpirationsReminders expirationsReminders)
        {
            RYCContextService.getInstance().BBDD.Remove(expirationsReminders);
            RYCContextService.getInstance().BBDD.SaveChanges();
        }
    }
}
