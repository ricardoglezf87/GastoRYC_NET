using GARCA.BO.Models;
using GARCA.DAO.Managers;
using GARCA.Utils.IOC;
using GARCA.Utlis.Extensions;
using System.Collections.Generic;

namespace GARCA.BO.Services
{
    public class TransactionsRemindersService
    {
        private readonly TransactionsRemindersManager transactionsRemindersManager;

        public TransactionsRemindersService()
        {
            transactionsRemindersManager = new();
        }

        #region TransactionsRemindersActions

        public HashSet<TransactionsReminders?>? GetAll()
        {
            return transactionsRemindersManager.GetAll()?.ToHashSetBo();
        }

        public TransactionsReminders? GetById(int? id)
        {
            return (TransactionsReminders?)transactionsRemindersManager.GetById(id);
        }

        public TransactionsReminders? Update(TransactionsReminders transactionsReminders)
        {
            DependencyConfig.IExpirationsRemindersService.DeleteByTransactionReminderid(transactionsReminders.Id);
            return (TransactionsReminders?)transactionsRemindersManager.Update(transactionsReminders.ToDao());
        }

        public void Delete(TransactionsReminders? transactionsReminders)
        {
            if (transactionsReminders != null)
            {
                DependencyConfig.IExpirationsRemindersService.DeleteByTransactionReminderid(transactionsReminders.Id);
                transactionsRemindersManager.Delete(transactionsReminders.ToDao());
            }
        }

        public void SaveChanges(TransactionsReminders transactionsReminders)
        {
            transactionsReminders.AmountIn ??= 0;

            transactionsReminders.AmountOut ??= 0;

            Update(transactionsReminders);
        }

        #endregion

        #region SplitsRemindersActions

        public void UpdateSplitsReminders(TransactionsReminders? transactionsReminders)
        {
            var lSplitsReminders = DependencyConfig.ISplitsRemindersService.GetbyTransactionid(transactionsReminders.Id);

            if (lSplitsReminders != null && lSplitsReminders.Count != 0)
            {
                transactionsReminders.AmountIn = 0;
                transactionsReminders.AmountOut = 0;

                foreach (var splitsReminders in lSplitsReminders)
                {
                    transactionsReminders.AmountIn += splitsReminders.AmountIn ?? 0;
                    transactionsReminders.AmountOut += splitsReminders.AmountOut ?? 0;
                }

                transactionsReminders.Categoryid = (int)CategoriesService.ESpecialCategories.Split;
                transactionsReminders.Category = DependencyConfig.ICategoriesService.GetById((int)CategoriesService.ESpecialCategories.Split);
            }
            else if (transactionsReminders.Categoryid is not null
                and ((int)CategoriesService.ESpecialCategories.Split))
            {
                transactionsReminders.Categoryid = (int)CategoriesService.ESpecialCategories.WithoutCategory;
                transactionsReminders.Category = DependencyConfig.ICategoriesService.GetById((int)CategoriesService.ESpecialCategories.WithoutCategory);
            }

            if (transactionsReminders.Id == 0)
            {
                Update(transactionsReminders);
                foreach (var splitsReminders in lSplitsReminders)
                {
                    splitsReminders.Transactionid = transactionsReminders.Id;
                    DependencyConfig.ISplitsRemindersService.Update(splitsReminders);
                }
            }
            else
            {
                Update(transactionsReminders);
            }
        }

        #endregion

    }
}
