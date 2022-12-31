using BBDDLib.Models;
using BBDDLib.Models.Charts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GastosRYC.BBDDLib.Services
{
    public interface ITransactionsRemindersService
    {
        public List<TransactionsReminders>? getAll();

        public TransactionsReminders? getByID(int? id);

        public void update(TransactionsReminders transactionsReminders);

        public void delete(TransactionsReminders? transactionsReminders);

        public int getNextID();

        public void saveChanges(TransactionsReminders transactionsReminders);

        public void updateSplitsReminders(TransactionsReminders? transactionsReminders);

    }
}
