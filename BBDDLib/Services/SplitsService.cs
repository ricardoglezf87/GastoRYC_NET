using BBDDLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GastosRYC.BBDDLib.Services
{
    public class SplitsService
    {
        public List<Splits>? getAll()
        {
            return RYCContextService.getInstance().BBDD.splits?.ToList();
        }

        public List<Splits>? getbyTransactionid(int transactionid)
        {
            return RYCContextService.getInstance().BBDD.splits?.Where(x=>x.transactionid == transactionid).ToList();
        }

        public Splits? getByID(int? id)
        {
            return RYCContextService.getInstance().BBDD.splits?.FirstOrDefault(x => id.Equals(x.id));
        }

        public void update(Splits splits)
        {
            RYCContextService.getInstance().BBDD.Update(splits);
            RYCContextService.getInstance().BBDD.SaveChanges();
        }

        public void delete(Splits splits)
        {
            RYCContextService.getInstance().BBDD.Remove(splits);
            RYCContextService.getInstance().BBDD.SaveChanges();
        }
    }
}
