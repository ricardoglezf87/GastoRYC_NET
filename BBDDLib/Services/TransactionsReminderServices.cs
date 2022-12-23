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
    public class TransactionsReminderService
    {

        #region Variables

        private readonly SplitsReminderService splitsReminderService = new SplitsReminderService();
        private readonly CategoriesService categoriesService = new CategoriesService();

        #endregion

        #region TransactionsReminderActions

        public List<TransactionsReminder>? getAll()
        {
            return RYCContextService.getInstance().BBDD.transactionsReminder?.ToList();
        }

        public TransactionsReminder? getByID(int? id)
        {
            return RYCContextService.getInstance().BBDD.transactionsReminder?.FirstOrDefault(x => id.Equals(x.id));
        }

        public void update(TransactionsReminder transactionsReminder)
        {            
            RYCContextService.getInstance().BBDD.Update(transactionsReminder);
            RYCContextService.getInstance().BBDD.SaveChanges();
        }

        public void delete(TransactionsReminder? transactionsReminder)
        {
            if (transactionsReminder != null)
            {
                RYCContextService.getInstance().BBDD.Remove(transactionsReminder);
                RYCContextService.getInstance().BBDD.SaveChanges();
            }
        }


        public int getNextID()
        {
            var cmd = RYCContextService.getInstance().BBDD.Database.
                GetDbConnection().CreateCommand();
            cmd.CommandText = "SELECT seq + 1 AS Current_Identity FROM SQLITE_SEQUENCE WHERE name = 'transactionsReminder';";
            
            RYCContextService.getInstance().BBDD.Database.OpenConnection();
            var result = cmd.ExecuteReader();
            result.Read();
            int id = Convert.ToInt32(result[0]);
            result.Close();

            return id;
        }

        public void saveChanges(TransactionsReminder transactionsReminder)
        {
            if (transactionsReminder.amountIn == null)
                transactionsReminder.amountIn = 0;

            if (transactionsReminder.amountOut == null)
                transactionsReminder.amountOut = 0;

            updateTranfer(transactionsReminder);
            updateTranferSplit(transactionsReminder);
            update(transactionsReminder);
        }

        public void updateTranfer(TransactionsReminder transactionsReminder)
        {
            if (transactionsReminder.tranferid != null &&
                transactionsReminder.category.categoriesTypesid != (int)CategoriesTypesService.eCategoriesTypes.Transfers)
            {
                TransactionsReminder? tContraria = getByID(transactionsReminder.tranferid);
                if (tContraria != null)
                {
                    delete(tContraria);
                }
                transactionsReminder.tranferid = null;
            }
            else if (transactionsReminder.tranferid == null &&
                transactionsReminder.category.categoriesTypesid == (int)CategoriesTypesService.eCategoriesTypes.Transfers)
            {
                transactionsReminder.tranferid = getNextID();

                TransactionsReminder? tContraria = new TransactionsReminder();
                tContraria.date = transactionsReminder.date;
                tContraria.accountid = transactionsReminder.category.accounts.id;
                tContraria.personid = transactionsReminder.personid;
                tContraria.categoryid = transactionsReminder.account.categoryid;
                tContraria.memo = transactionsReminder.memo;
                tContraria.tagid = transactionsReminder.tagid;
                tContraria.amountIn = transactionsReminder.amountOut;
                tContraria.amountOut = transactionsReminder.amountIn;

                if (transactionsReminder.id != 0)
                    tContraria.tranferid = transactionsReminder.id;
                else
                    tContraria.tranferid = getNextID() + 1;

                tContraria.transactionStatusid = transactionsReminder.transactionStatusid;

                update(tContraria);

            }
            else if (transactionsReminder.tranferid != null &&
                transactionsReminder.category.categoriesTypesid == (int)CategoriesTypesService.eCategoriesTypes.Transfers)
            {
                TransactionsReminder? tContraria = getByID(transactionsReminder.tranferid);
                if (tContraria != null)
                {
                    tContraria.date = transactionsReminder.date;
                    tContraria.accountid = transactionsReminder.category.accounts.id;
                    tContraria.personid = transactionsReminder.personid;
                    tContraria.categoryid = transactionsReminder.account.categoryid;
                    tContraria.memo = transactionsReminder.memo;
                    tContraria.tagid = transactionsReminder.tagid;
                    tContraria.amountIn = transactionsReminder.amountOut;
                    tContraria.amountOut = transactionsReminder.amountIn;
                    tContraria.transactionStatusid = transactionsReminder.transactionStatusid;
                    update(tContraria);
                }
            }
        }

        #endregion

        #region SplitsReminderActions

        public void updateTranferSplit(TransactionsReminder transactionsReminder)
        {
            if (transactionsReminder.tranferSplitid != null &&
                transactionsReminder.category.categoriesTypesid == (int)CategoriesTypesService.eCategoriesTypes.Transfers)
            {
                SplitsReminder? tContraria = splitsReminderService.getByID(transactionsReminder.tranferSplitid);
                if (tContraria != null)
                {
                    tContraria.transaction.date = transactionsReminder.date;
                    tContraria.transaction.personid = transactionsReminder.personid;
                    tContraria.categoryid = transactionsReminder.account.categoryid;
                    tContraria.memo = transactionsReminder.memo;
                    tContraria.tagid = transactionsReminder.tagid;
                    tContraria.amountIn = transactionsReminder.amountOut;
                    tContraria.amountOut = transactionsReminder.amountIn;
                    tContraria.transaction.transactionStatusid = transactionsReminder.transactionStatusid;
                    splitsReminderService.update(tContraria);
                }
            }
        }

        public void updateSplitsReminder(TransactionsReminder? transactionsReminder)
        {
            List<SplitsReminder>? lSplitsReminder = transactionsReminder.splits ?? splitsReminderService.getbyTransactionid(transactionsReminder.id);

            if (lSplitsReminder != null && lSplitsReminder.Count != 0)
            {
                transactionsReminder.amountIn = 0;
                transactionsReminder.amountOut = 0;

                foreach (SplitsReminder splitsReminder in lSplitsReminder)
                {
                    transactionsReminder.amountIn += (splitsReminder.amountIn == null ? 0 : splitsReminder.amountIn);
                    transactionsReminder.amountOut += (splitsReminder.amountOut == null ? 0 : splitsReminder.amountOut);
                }

                transactionsReminder.categoryid = (int)CategoriesService.eSpecialCategories.Split;
                transactionsReminder.category = categoriesService.getByID((int)CategoriesService.eSpecialCategories.Split);
            }
            else if (transactionsReminder.categoryid != null
                && transactionsReminder.categoryid == (int)CategoriesService.eSpecialCategories.Split)
            {
                transactionsReminder.categoryid = (int)CategoriesService.eSpecialCategories.WithoutCategory;
                transactionsReminder.category = categoriesService.getByID((int)CategoriesService.eSpecialCategories.WithoutCategory);
            }

            if (transactionsReminder.id == 0)
            {
                update(transactionsReminder);
                foreach (SplitsReminder splitsReminder in lSplitsReminder)
                {
                    splitsReminder.transactionid = transactionsReminder.id;
                    splitsReminderService.update(splitsReminder);
                }
            }
            else
            {
                update(transactionsReminder);
            }
        }

        #endregion

    }
}
