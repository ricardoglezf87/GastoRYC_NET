using BBDDLib.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GastosRYC.BBDDLib.Services
{
    public interface ISplitsRemindersService
    {
        public List<SplitsReminders>? getAll();

        public List<SplitsReminders>? getbyTransactionidNull();

        public List<SplitsReminders>? getbyTransactionid(int transactionid);

        public SplitsReminders? getByID(int? id);

        public void update(SplitsReminders splitsReminders);

        public void delete(SplitsReminders splitsReminders);

        public Decimal? getAmountTotal(TransactionsReminders transactions);

        public int getNextID();
    }
}
