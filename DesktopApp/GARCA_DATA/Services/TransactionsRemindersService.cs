using GARCA.Data.Managers;
using GARCA.Models;
using static GARCA.Data.IOC.DependencyConfig;


namespace GARCA.Data.Services
{
    public class TransactionsRemindersService : ServiceBase<TransactionsRemindersManager, TransactionsReminders>
    {

        #region TransactionsRemindersActions        

        public override async Task<TransactionsReminders?> Save(TransactionsReminders transactionsReminders)
        {
            await iExpirationsRemindersService.DeleteByTransactionReminderid(transactionsReminders.Id);
            return await base.Save(transactionsReminders);
        }

        public override async Task<bool> Delete(TransactionsReminders transactionsReminders)
        {
            if (transactionsReminders != null)
            {
                await iExpirationsRemindersService.DeleteByTransactionReminderid(transactionsReminders.Id);
                return await manager.Delete(transactionsReminders);
            }
            return false;
        }

        public async Task SaveChanges(TransactionsReminders transactionsReminders)
        {
            transactionsReminders.AmountIn ??= 0;

            transactionsReminders.AmountOut ??= 0;

            await Save(transactionsReminders);
        }

        #endregion

        #region SplitsRemindersActions

        public async Task UpdateSplitsReminders(TransactionsReminders? transactionsReminders)
        {
            var lSplitsReminders = await iSplitsRemindersService.GetbyTransactionid(transactionsReminders.Id);

            if (lSplitsReminders != null && lSplitsReminders.Any())
            {
                transactionsReminders.AmountIn = 0;
                transactionsReminders.AmountOut = 0;

                foreach (var splitsReminders in lSplitsReminders)
                {
                    transactionsReminders.AmountIn += splitsReminders.AmountIn ?? 0;
                    transactionsReminders.AmountOut += splitsReminders.AmountOut ?? 0;
                }

                transactionsReminders.CategoriesId = (int)CategoriesService.ESpecialCategories.Split;
                transactionsReminders.Categories = await iCategoriesService.GetById((int)CategoriesService.ESpecialCategories.Split);
            }
            else if (transactionsReminders.CategoriesId is ((int)CategoriesService.ESpecialCategories.Split))
            {
                transactionsReminders.CategoriesId = (int)CategoriesService.ESpecialCategories.WithoutCategory;
                transactionsReminders.Categories = await iCategoriesService.GetById((int)CategoriesService.ESpecialCategories.WithoutCategory);
            }

            if (transactionsReminders.Id == 0)
            {
                await Save(transactionsReminders);

                if (lSplitsReminders == null)
                {
                    return;
                }

                foreach (var splitsReminders in lSplitsReminders)
                {
                    splitsReminders.TransactionsId = transactionsReminders.Id;
                    await iSplitsRemindersService.Save(splitsReminders);
                }
            }
            else
            {
                await Save(transactionsReminders);
            }
        }

        #endregion

    }
}
