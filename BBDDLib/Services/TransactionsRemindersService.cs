using BBDDLib.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GastosRYC.BBDDLib.Services
{
    public class TransactionsRemindersService : ITransactionsRemindersService
    {
        private readonly SimpleInjector.Container servicesContainer;

        public TransactionsRemindersService(SimpleInjector.Container servicesContainer)
        {
            this.servicesContainer = servicesContainer;
        }

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
            servicesContainer.GetInstance<IExpirationsRemindersService>().deleteByTransactionReminderid(transactionsReminders.id);
            RYCContextService.getInstance().BBDD.Update(transactionsReminders);
            RYCContextService.getInstance().BBDD.SaveChanges();
        }

        public void delete(TransactionsReminders? transactionsReminders)
        {
            if (transactionsReminders != null)
            {
                servicesContainer.GetInstance<IExpirationsRemindersService>().deleteByTransactionReminderid(transactionsReminders.id);
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

            update(transactionsReminders);
        }

        #endregion

        #region SplitsRemindersActions

        public void updateSplitsReminders(TransactionsReminders? transactionsReminders)
        {
            List<SplitsReminders>? lSplitsReminders = transactionsReminders.splits ?? servicesContainer.GetInstance<ISplitsRemindersService>().getbyTransactionid(transactionsReminders.id);

            if (lSplitsReminders != null && lSplitsReminders.Count != 0)
            {
                transactionsReminders.amountIn = 0;
                transactionsReminders.amountOut = 0;

                foreach (SplitsReminders splitsReminders in lSplitsReminders)
                {
                    transactionsReminders.amountIn += (splitsReminders.amountIn == null ? 0 : splitsReminders.amountIn);
                    transactionsReminders.amountOut += (splitsReminders.amountOut == null ? 0 : splitsReminders.amountOut);
                }

                transactionsReminders.categoryid = (int)ICategoriesService.eSpecialCategories.Split;
                transactionsReminders.category = servicesContainer.GetInstance<ICategoriesService>().getByID((int)ICategoriesService.eSpecialCategories.Split);
            }
            else if (transactionsReminders.categoryid != null
                && transactionsReminders.categoryid == (int)ICategoriesService.eSpecialCategories.Split)
            {
                transactionsReminders.categoryid = (int)ICategoriesService.eSpecialCategories.WithoutCategory;
                transactionsReminders.category = servicesContainer.GetInstance<ICategoriesService>().getByID((int)ICategoriesService.eSpecialCategories.WithoutCategory);
            }

            if (transactionsReminders.id == 0)
            {
                update(transactionsReminders);
                foreach (SplitsReminders splitsReminders in lSplitsReminders)
                {
                    splitsReminders.transactionid = transactionsReminders.id;
                    servicesContainer.GetInstance<ISplitsRemindersService>().update(splitsReminders);
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
