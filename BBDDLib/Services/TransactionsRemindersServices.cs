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
    public class TransactionsRemindersService
    {

        #region Variables

        private readonly SplitsRemindersService splitsRemindersService = new SplitsRemindersService();
        private readonly CategoriesService categoriesService = new CategoriesService();

        #endregion

        #region TransactionsRemindersActions

        public List<TransactionsReminders>? getAll()
        {
            return RYCContextService.getInstance().BBDD.transactionsReminders?.ToList();
        }

        public TransactionsReminders? getByID(int? id)
        {
            return RYCContextService.getInstance().BBDD.transactionsReminders?.FirstOrDefault(x => id.Equals(x.id));
        }

        public void update(TransactionsReminders transactionsReminders)
        {
            //TODO: Cuando actualizo este modelo hay que borrar los vencimientos para recalcularlos.
            RYCContextService.getInstance().BBDD.Update(transactionsReminders);
            RYCContextService.getInstance().BBDD.SaveChanges();
        }

        public void delete(TransactionsReminders? transactionsReminders)
        {
            if (transactionsReminders != null)
            {
                //TODO: Cuando actualizo este modelo hay que borrar los vencimientos.
                RYCContextService.getInstance().BBDD.Remove(transactionsReminders);
                RYCContextService.getInstance().BBDD.SaveChanges();
            }
        }


        public int getNextID()
        {
            var cmd = RYCContextService.getInstance().BBDD.Database.
                GetDbConnection().CreateCommand();
            cmd.CommandText = "SELECT seq + 1 AS Current_Identity FROM SQLITE_SEQUENCE WHERE name = 'transactionsReminders';";
            
            RYCContextService.getInstance().BBDD.Database.OpenConnection();
            var result = cmd.ExecuteReader();
            result.Read();
            int id = Convert.ToInt32(result[0]);
            result.Close();

            return id;
        }

        public void saveChanges(TransactionsReminders transactionsReminders)
        {
            if (transactionsReminders.amountIn == null)
                transactionsReminders.amountIn = 0;

            if (transactionsReminders.amountOut == null)
                transactionsReminders.amountOut = 0;

            updateTranfer(transactionsReminders);
            updateTranferSplit(transactionsReminders);
            update(transactionsReminders);
        }

        public void updateTranfer(TransactionsReminders transactionsReminders)
        {
            if (transactionsReminders.tranferid != null &&
                transactionsReminders.category.categoriesTypesid != (int)CategoriesTypesService.eCategoriesTypes.Transfers)
            {
                TransactionsReminders? tContraria = getByID(transactionsReminders.tranferid);
                if (tContraria != null)
                {
                    delete(tContraria);
                }
                transactionsReminders.tranferid = null;
            }
            else if (transactionsReminders.tranferid == null &&
                transactionsReminders.category.categoriesTypesid == (int)CategoriesTypesService.eCategoriesTypes.Transfers)
            {
                transactionsReminders.tranferid = getNextID();

                TransactionsReminders? tContraria = new TransactionsReminders();
                tContraria.date = transactionsReminders.date;
                tContraria.accountid = transactionsReminders.category.accounts.id;
                tContraria.personid = transactionsReminders.personid;
                tContraria.categoryid = transactionsReminders.account.categoryid;
                tContraria.memo = transactionsReminders.memo;
                tContraria.tagid = transactionsReminders.tagid;
                tContraria.amountIn = transactionsReminders.amountOut;
                tContraria.amountOut = transactionsReminders.amountIn;

                if (transactionsReminders.id != 0)
                    tContraria.tranferid = transactionsReminders.id;
                else
                    tContraria.tranferid = getNextID() + 1;

                tContraria.transactionStatusid = transactionsReminders.transactionStatusid;

                update(tContraria);

            }
            else if (transactionsReminders.tranferid != null &&
                transactionsReminders.category.categoriesTypesid == (int)CategoriesTypesService.eCategoriesTypes.Transfers)
            {
                TransactionsReminders? tContraria = getByID(transactionsReminders.tranferid);
                if (tContraria != null)
                {
                    tContraria.date = transactionsReminders.date;
                    tContraria.accountid = transactionsReminders.category.accounts.id;
                    tContraria.personid = transactionsReminders.personid;
                    tContraria.categoryid = transactionsReminders.account.categoryid;
                    tContraria.memo = transactionsReminders.memo;
                    tContraria.tagid = transactionsReminders.tagid;
                    tContraria.amountIn = transactionsReminders.amountOut;
                    tContraria.amountOut = transactionsReminders.amountIn;
                    tContraria.transactionStatusid = transactionsReminders.transactionStatusid;
                    update(tContraria);
                }
            }
        }

        #endregion

        #region SplitsRemindersActions

        public void updateTranferSplit(TransactionsReminders transactionsReminders)
        {
            if (transactionsReminders.tranferSplitid != null &&
                transactionsReminders.category.categoriesTypesid == (int)CategoriesTypesService.eCategoriesTypes.Transfers)
            {
                SplitsReminders? tContraria = splitsRemindersService.getByID(transactionsReminders.tranferSplitid);
                if (tContraria != null)
                {
                    tContraria.transaction.date = transactionsReminders.date;
                    tContraria.transaction.personid = transactionsReminders.personid;
                    tContraria.categoryid = transactionsReminders.account.categoryid;
                    tContraria.memo = transactionsReminders.memo;
                    tContraria.tagid = transactionsReminders.tagid;
                    tContraria.amountIn = transactionsReminders.amountOut;
                    tContraria.amountOut = transactionsReminders.amountIn;
                    tContraria.transaction.transactionStatusid = transactionsReminders.transactionStatusid;
                    splitsRemindersService.update(tContraria);
                }
            }
        }

        public void updateSplitsReminders(TransactionsReminders? transactionsReminders)
        {
            List<SplitsReminders>? lSplitsReminders = transactionsReminders.splits ?? splitsRemindersService.getbyTransactionid(transactionsReminders.id);

            if (lSplitsReminders != null && lSplitsReminders.Count != 0)
            {
                transactionsReminders.amountIn = 0;
                transactionsReminders.amountOut = 0;

                foreach (SplitsReminders splitsReminders in lSplitsReminders)
                {
                    transactionsReminders.amountIn += (splitsReminders.amountIn == null ? 0 : splitsReminders.amountIn);
                    transactionsReminders.amountOut += (splitsReminders.amountOut == null ? 0 : splitsReminders.amountOut);
                }

                transactionsReminders.categoryid = (int)CategoriesService.eSpecialCategories.Split;
                transactionsReminders.category = categoriesService.getByID((int)CategoriesService.eSpecialCategories.Split);
            }
            else if (transactionsReminders.categoryid != null
                && transactionsReminders.categoryid == (int)CategoriesService.eSpecialCategories.Split)
            {
                transactionsReminders.categoryid = (int)CategoriesService.eSpecialCategories.WithoutCategory;
                transactionsReminders.category = categoriesService.getByID((int)CategoriesService.eSpecialCategories.WithoutCategory);
            }

            if (transactionsReminders.id == 0)
            {
                update(transactionsReminders);
                foreach (SplitsReminders splitsReminders in lSplitsReminders)
                {
                    splitsReminders.transactionid = transactionsReminders.id;
                    splitsRemindersService.update(splitsReminders);
                }
            }
            else
            {
                update(transactionsReminders);
            }
        }

        #endregion

    }
}
