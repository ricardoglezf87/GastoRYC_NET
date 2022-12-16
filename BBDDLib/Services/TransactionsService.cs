using BBDDLib.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GastosRYC.BBDDLib.Services
{
    public class TransactionsService
    {

        private readonly SplitsService splitsService = new SplitsService();
        private readonly CategoriesService categoriesService = new CategoriesService();

        public List<Transactions>? getAll()
        {
            return RYCContextService.getInstance().BBDD.transactions?.ToList();
        }

        public Transactions? getByID(int? id)
        {
            return RYCContextService.getInstance().BBDD.transactions?.FirstOrDefault(x => id.Equals(x.id));
        }

        public void update(Transactions transactions)
        {            
            RYCContextService.getInstance().BBDD.Update(transactions);
            RYCContextService.getInstance().BBDD.SaveChanges();
        }

        public void delete(Transactions? transactions)
        {
            if (transactions != null)
            {
                RYCContextService.getInstance().BBDD.Remove(transactions);
                RYCContextService.getInstance().BBDD.SaveChanges();
            }
        }


        public int getNextID()
        {
            var cmd = RYCContextService.getInstance().BBDD.Database.
                GetDbConnection().CreateCommand();
            cmd.CommandText = "SELECT seq + 1 AS Current_Identity FROM SQLITE_SEQUENCE WHERE name = 'transactions';";
            
            RYCContextService.getInstance().BBDD.Database.OpenConnection();
            var result = cmd.ExecuteReader();
            result.Read();
            int id = Convert.ToInt32(result[0]);
            result.Close();

            return id;
        }

        public void saveChanges(Transactions transactions)
        {
            if (transactions.amountIn == null)
                transactions.amountIn = 0;

            if (transactions.amountOut == null)
                transactions.amountOut = 0;

            updateTranfer(transactions);
            updateTranferSplit(transactions);
            update(transactions);
        }

        public void updateTranfer(Transactions transactions)
        {
            if (transactions.tranferid != null &&
                transactions.category.categoriesTypesid != (int)CategoriesService.eCategoriesTypes.Transfers)
            {
                Transactions? tContraria = getByID(transactions.tranferid);
                if (tContraria != null)
                {
                    delete(tContraria);
                }
                transactions.tranferid = null;
            }
            else if (transactions.tranferid == null &&
                transactions.category.categoriesTypesid == (int)CategoriesService.eCategoriesTypes.Transfers)
            {
                transactions.tranferid = getNextID();

                Transactions? tContraria = new Transactions();
                tContraria.date = transactions.date;
                tContraria.accountid = transactions.category.accounts.id;
                tContraria.personid = transactions.personid;
                tContraria.categoryid = transactions.account.categoryid;
                tContraria.memo = transactions.memo;
                tContraria.tagid = transactions.tagid;
                tContraria.amountIn = transactions.amountOut;
                tContraria.amountOut = transactions.amountIn;

                if (transactions.id != 0)
                    tContraria.tranferid = transactions.id;
                else
                    tContraria.tranferid = getNextID() + 1;

                tContraria.transactionStatusid = transactions.transactionStatusid;

                update(tContraria);

            }
            else if (transactions.tranferid != null &&
                transactions.category.categoriesTypesid == (int)CategoriesService.eCategoriesTypes.Transfers)
            {
                Transactions? tContraria = getByID(transactions.tranferid);
                if (tContraria != null)
                {
                    tContraria.date = transactions.date;
                    tContraria.accountid = transactions.category.accounts.id;
                    tContraria.personid = transactions.personid;
                    tContraria.categoryid = transactions.account.categoryid;
                    tContraria.memo = transactions.memo;
                    tContraria.tagid = transactions.tagid;
                    tContraria.amountIn = transactions.amountOut;
                    tContraria.amountOut = transactions.amountIn;
                    tContraria.transactionStatusid = transactions.transactionStatusid;
                    update(tContraria);
                }
            }
        }

        public void updateTranferSplit(Transactions transactions)
        {
            if (transactions.tranferSplitid != null &&
                transactions.category.categoriesTypesid == (int)CategoriesService.eCategoriesTypes.Transfers)
            {
                Splits? tContraria = splitsService.getByID(transactions.tranferSplitid);
                if (tContraria != null)
                {
                    tContraria.transaction.date = transactions.date;
                    tContraria.transaction.personid = transactions.personid;
                    tContraria.categoryid = transactions.account.categoryid;
                    tContraria.memo = transactions.memo;
                    tContraria.tagid = transactions.tagid;
                    tContraria.amountIn = transactions.amountOut;
                    tContraria.amountOut = transactions.amountIn;
                    tContraria.transaction.transactionStatusid = transactions.transactionStatusid;
                    splitsService.update(tContraria);
                }
            }
        }

        public void updateSplits(Transactions? transactions)
        {
            List<Splits>? lSplits = transactions.splits ?? splitsService.getbyTransactionid(transactions.id);

            if (lSplits != null && lSplits.Count != 0)
            {
                transactions.amountIn = 0;
                transactions.amountOut = 0;

                foreach (Splits splits in lSplits)
                {
                    transactions.amountIn += (splits.amountIn == null ? 0 : splits.amountIn);
                    transactions.amountOut += (splits.amountOut == null ? 0 : splits.amountOut);
                }

                transactions.categoryid = (int)CategoriesService.eSpecialCategories.Split;
                transactions.category = categoriesService.getByID((int)CategoriesService.eSpecialCategories.Split);
            }
            else if (transactions.categoryid != null
                && transactions.categoryid == (int)CategoriesService.eSpecialCategories.Split)
            {
                transactions.categoryid = (int)CategoriesService.eSpecialCategories.WithoutCategory;
                transactions.category = categoriesService.getByID((int)CategoriesService.eSpecialCategories.WithoutCategory);
            }

            if (transactions.id == 0)
            {
                update(transactions);
                foreach (Splits splits in lSplits)
                {
                    splits.transactionid = transactions.id;
                    splitsService.update(splits);
                }
            }
            else
            {
                update(transactions);
            }
        }

    }
}
