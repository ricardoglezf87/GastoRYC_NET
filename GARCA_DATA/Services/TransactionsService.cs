using GARCA.Data.Managers;
using GARCA.Models;
using GARCA.Utils.Extensions;
using static GARCA.Data.IOC.DependencyConfig;
using static GARCA.Utils.Enums.EnumCategories;

namespace GARCA.Data.Services
{
    public class TransactionsService : ServiceBase<TransactionsManager, Transactions>
    {
        #region TransactionsActions

        private async Task<IEnumerable<Transactions>?> GetByInvestmentProduct(int id)
        {
            return (await GetAll())?.Where(x => id.Equals(x.InvestmentProductsId));
        }

        public async Task<IEnumerable<Transactions>?> GetByInvestmentProduct(InvestmentProducts investment)
        {
            return await GetByInvestmentProduct(investment.Id);
        }

        public override async Task<Transactions> Save(Transactions obj)
        {
            obj.Date = obj.Date.RemoveTime();            
            return await base.Save(obj);
        }

        public async Task<IEnumerable<Transactions>?> GetAllOpennedOrderByOrdenDesc()
        {
            return (await GetAll())?.OrderByDescending(x => x.Orden);
        }

        public async Task<IEnumerable<Transactions>?> GetByAccount(int? id)
        {
            return await manager.GetByAccount(id ?? -99);
        }

        private async Task<int> GetNextId()
        {
            return await manager.GetNextId();
        }

        public async Task<Transactions?> SaveChanges(Transactions? transactions)
        {
            await UpdateTranferFromSplit(transactions);
            transactions = await Save(transactions);            
            return transactions;
        }
       
        #endregion

        #region SplitsActions

        private async Task UpdateTranferFromSplit(Transactions transactions)
        {
            if (transactions.TranferSplitId != null &&
                await iCategoriesService.IsTranfer(transactions.CategoriesId ?? -99))
            {
                var tContraria = await iSplitsService.GetById(transactions.TranferSplitId ?? -99);
                if (tContraria != null)
                {
                    tContraria.Transactions.Date = transactions.Date;
                    tContraria.Transactions.PersonsId = transactions.PersonsId;
                    tContraria.CategoriesId = transactions.Accounts.Categoryid;
                    tContraria.Memo = transactions.Memo;
                    tContraria.TagsId = transactions.TagsId;
                    tContraria.AmountIn = transactions.AmountOut;
                    tContraria.AmountOut = transactions.AmountIn;
                    tContraria.Transactions.TransactionsStatusId = transactions.TransactionsStatusId;
                    await iSplitsService.Save(tContraria);
                }
            }
        }

        public async Task UpdateTransactionAfterSplits(Transactions? transactions)
        {
            var lSplits = transactions.Splits ?? await iSplitsService.GetbyTransactionid(transactions.Id);

            if (lSplits != null && lSplits.Any())
            {
                transactions.AmountIn = 0;
                transactions.AmountOut = 0;

                foreach (var splits in lSplits)
                {
                    transactions.AmountIn += splits.AmountIn ?? 0;
                    transactions.AmountOut += splits.AmountOut ?? 0;
                }

                transactions.CategoriesId = (int)ESpecialCategories.Split;
                transactions.Categories = await iCategoriesService.GetById((int)ESpecialCategories.Split);
            }
            else if (transactions.CategoriesId is (int)ESpecialCategories.Split)
            {
                transactions.CategoriesId = (int)ESpecialCategories.WithoutCategory;
                transactions.Categories = await iCategoriesService.GetById((int)ESpecialCategories.WithoutCategory);
            }

            if (transactions.Id == 0)
            {
                await Save(transactions);

                if (lSplits == null)
                {
                    return;
                }

                foreach (var splits in lSplits)
                {
                    splits.TransactionsId = transactions.Id;
                    await iSplitsService.Save(splits);
                }
            }
            else
            {
                await Save(transactions);
            }
        }

        #endregion

    }
}
