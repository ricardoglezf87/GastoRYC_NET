using DAOLib.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAOLib.Services
{
    public class TransactionsRemindersServiceDAO
    {
        private readonly SimpleInjector.Container servicesContainer;

        public TransactionsRemindersServiceDAO(SimpleInjector.Container servicesContainer)
        {
            this.servicesContainer = servicesContainer;
        }

        #region TransactionsRemindersActions

        public List<TransactionsRemindersDAO>? getAll()
        {
            return RYCContextServiceDAO.getInstance().BBDD.transactionsReminders?.ToList();
        }

        public TransactionsRemindersDAO? getByID(int? id)
        {
            return RYCContextServiceDAO.getInstance().BBDD.transactionsReminders?.FirstOrDefault(x => id.Equals(x.id));
        }

        public void update(TransactionsRemindersDAO transactionsReminders)
        {
            servicesContainer.GetInstance<ExpirationsRemindersServiceDAO>().deleteByTransactionReminderid(transactionsReminders.id);
            RYCContextServiceDAO.getInstance().BBDD.Update(transactionsReminders);
            RYCContextServiceDAO.getInstance().BBDD.SaveChanges();
        }

        public void delete(TransactionsRemindersDAO? transactionsReminders)
        {
            if (transactionsReminders != null)
            {
                servicesContainer.GetInstance<ExpirationsRemindersServiceDAO>().deleteByTransactionReminderid(transactionsReminders.id);
                RYCContextServiceDAO.getInstance().BBDD.Remove(transactionsReminders);
                RYCContextServiceDAO.getInstance().BBDD.SaveChanges();
            }
        }

        public int getNextID()
        {
            var cmd = RYCContextServiceDAO.getInstance().BBDD.Database.
                GetDbConnection().CreateCommand();
            cmd.CommandText = "SELECT seq + 1 AS Current_Identity FROM SQLITE_SEQUENCE WHERE name = 'transactionsReminders';";

            RYCContextServiceDAO.getInstance().BBDD.Database.OpenConnection();
            var result = cmd.ExecuteReader();
            result.Read();
            int id = Convert.ToInt32(result[0]);
            result.Close();

            return id;
        }

        public void saveChanges(TransactionsRemindersDAO transactionsReminders)
        {
            if (transactionsReminders.amountIn == null)
                transactionsReminders.amountIn = 0;

            if (transactionsReminders.amountOut == null)
                transactionsReminders.amountOut = 0;

            update(transactionsReminders);
        }

        #endregion

        #region SplitsRemindersActions

        public void updateSplitsReminders(TransactionsRemindersDAO? transactionsReminders)
        {
            List<SplitsRemindersDAO>? lSplitsReminders = transactionsReminders.splits ?? servicesContainer.GetInstance<SplitsRemindersServiceDAO>().getbyTransactionid(transactionsReminders.id);

            if (lSplitsReminders != null && lSplitsReminders.Count != 0)
            {
                transactionsReminders.amountIn = 0;
                transactionsReminders.amountOut = 0;

                foreach (SplitsRemindersDAO splitsReminders in lSplitsReminders)
                {
                    transactionsReminders.amountIn += (splitsReminders.amountIn == null ? 0 : splitsReminders.amountIn);
                    transactionsReminders.amountOut += (splitsReminders.amountOut == null ? 0 : splitsReminders.amountOut);
                }

                transactionsReminders.categoryid = (int)CategoriesServiceDAO.eSpecialCategories.Split;
                transactionsReminders.category = servicesContainer.GetInstance<CategoriesServiceDAO>().getByID((int)CategoriesServiceDAO.eSpecialCategories.Split);
            }
            else if (transactionsReminders.categoryid != null
                && transactionsReminders.categoryid == (int)CategoriesServiceDAO.eSpecialCategories.Split)
            {
                transactionsReminders.categoryid = (int)CategoriesServiceDAO.eSpecialCategories.WithoutCategory;
                transactionsReminders.category = servicesContainer.GetInstance<CategoriesServiceDAO>().getByID((int)CategoriesServiceDAO.eSpecialCategories.WithoutCategory);
            }

            if (transactionsReminders.id == 0)
            {
                update(transactionsReminders);
                foreach (SplitsRemindersDAO splitsReminders in lSplitsReminders)
                {
                    splitsReminders.transactionid = transactionsReminders.id;
                    servicesContainer.GetInstance<SplitsRemindersServiceDAO>().update(splitsReminders);
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
