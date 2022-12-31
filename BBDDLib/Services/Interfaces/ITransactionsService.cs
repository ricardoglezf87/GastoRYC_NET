using BBDDLib.Models;
using BBDDLib.Models.Charts;
using BBDDLib.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GastosRYC.BBDDLib.Services
{
    public interface ITransactionsService
    {
        public List<Transactions>? getAll();

        public Transactions? getByID(int? id);

        public void update(Transactions transactions);

        public void delete(Transactions? transactions);

        public int getNextID();

        public void saveChanges(Transactions transactions);

        public void updateTranfer(Transactions transactions);

        public void updateTranferFromSplit(Transactions transactions);

        public void updateTransactionAfterSplits(Transactions? transactions);

        public void updateTranferSplits(Transactions? transactions, Splits splits);

        public List<ExpensesChart> getExpenses();

    }
}
