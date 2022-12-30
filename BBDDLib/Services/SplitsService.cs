using BBDDLib.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GastosRYC.BBDDLib.Services
{
    public class SplitsService
    {

        private readonly CategoriesService categoriesService = new CategoriesService();
        private readonly TransactionsService transactionsService = new TransactionsService();

        public List<Splits>? getAll()
        {
            return RYCContextService.getInstance().BBDD.splits?.ToList();
        }

        public List<Splits>? getbyTransactionidNull()
        {
            return RYCContextService.getInstance().BBDD.splits?.Where(x => x.transactionid == null).ToList();
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

        public Decimal? getAmountTotal(Transactions transactions)
        {
            Decimal? total = 0;
            if (transactions.splits != null && transactions.splits.Count != 0)
            {
                foreach (Splits splits in transactions.splits)
                {
                    total += (splits.amountIn == null ? 0 : splits.amountIn);
                    total -= (splits.amountOut == null ? 0 : splits.amountOut);
                }
            }

            return total;
        }

        public void saveChanges(Transactions? transactions,Splits splits)
        {
            if (splits.category == null && splits.categoryid != null)
            {
                splits.category = categoriesService.getByID(splits.categoryid);
            }

            if (splits.amountIn == null)
                splits.amountIn = 0;

            if (splits.amountOut == null)
                splits.amountOut = 0;

            updateTranfer(transactions,splits);

            update(splits);
        }

        public void updateTranfer(Transactions? transactions,Splits splits)
        {
            if (splits.tranferid != null &&
                splits.category.categoriesTypesid != (int)CategoriesTypesService.eCategoriesTypes.Transfers)
            {
                Transactions? tContraria = transactionsService.getByID(splits.tranferid);
                if (tContraria != null)
                {
                    transactionsService.delete(tContraria);
                }
                splits.tranferid = null;
            }
            else if (splits.tranferid == null &&
                splits.category.categoriesTypesid == (int)CategoriesTypesService.eCategoriesTypes.Transfers)
            {
                splits.tranferid = transactionsService.getNextID();

                Transactions? tContraria = new Transactions();
                tContraria.date = transactions.date;
                tContraria.accountid = splits.category.accounts.id;
                tContraria.personid = transactions.personid;
                tContraria.categoryid = transactions.account.categoryid;
                tContraria.memo = splits.memo;
                tContraria.tagid = transactions.tagid;
                tContraria.amountIn = splits.amountOut;
                tContraria.amountOut = splits.amountIn;

                if (splits.id != 0)
                    tContraria.tranferSplitid = splits.id;
                else
                    tContraria.tranferSplitid = getNextID() + 1;

                tContraria.transactionStatusid = transactions.transactionStatusid;

                transactionsService.update(tContraria);

            }
            else if (splits.tranferid != null &&
                splits.category.categoriesTypesid == (int)CategoriesTypesService.eCategoriesTypes.Transfers)
            {
                Transactions? tContraria = transactionsService.getByID(splits.tranferid);
                if (tContraria != null)
                {
                    tContraria.date = transactions.date;
                    tContraria.accountid = splits.category.accounts.id;
                    tContraria.personid = transactions.personid;
                    tContraria.categoryid = transactions.account.categoryid;
                    tContraria.memo = splits.memo;
                    tContraria.tagid = transactions.tagid;
                    tContraria.amountIn = splits.amountOut ?? 0;
                    tContraria.amountOut = splits.amountIn ?? 0;
                    tContraria.transactionStatusid = transactions.transactionStatusid;
                    transactionsService.update(tContraria);
                }
            }
        }

        public int getNextID()
        {
            var cmd = RYCContextService.getInstance().BBDD.Database.
                GetDbConnection().CreateCommand();
            cmd.CommandText = "SELECT seq + 1 AS Current_Identity FROM SQLITE_SEQUENCE WHERE name = 'splits';";

            RYCContextService.getInstance().BBDD.Database.OpenConnection();
            var result = cmd.ExecuteReader();
            result.Read();
            int id = Convert.ToInt32(result[0]);
            result.Close();

            return id;
        }
    }
}
