using BBDDLib.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GastosRYC.BBDDLib.Services
{
    public interface ISplitsService
    {

        public List<Splits>? getAll();

        public List<Splits>? getbyTransactionidNull();

        public List<Splits>? getbyTransactionid(int transactionid);

        public Splits? getByID(int? id);

        public void update(Splits splits);

        public void delete(Splits splits);

        public Decimal? getAmountTotal(Transactions transactions);

        public void saveChanges(Transactions? transactions, Splits splits);

        public int getNextID();
    }
}
