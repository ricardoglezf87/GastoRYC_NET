using BBDDLib.Models;
using BBDDLib.Models.Charts;
using System.Collections.Generic;

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

    }
}
