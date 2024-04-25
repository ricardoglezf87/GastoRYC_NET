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

        public async Task<IEnumerable<Transactions>?> GetAllOpenned()
        {
            return (await GetAll())?.Where(x => !x.Accounts.Closed.HasValue
                || !x.Accounts.Closed.Value);
        }

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
            obj.Orden = await CreateOrden(obj);
            return await base.Save(obj);
        }

        public async Task<IEnumerable<Transactions>?> GetAllOpennedOrderByOrdenDesc()
        {
            return (await GetAll())?.OrderByDescending(x => x.Orden);
        }

        public async Task<IEnumerable<Transactions>?> GetAllOpennedOrderByOrdenDesc(int startIndex, int nPage)
        {
            return (await GetAllOpennedOrderByOrdenDesc())?.Skip(startIndex).Take(nPage);
        }

        public async Task<IEnumerable<Transactions>?> GetByAccountOrderByOrderDesc(int? id)
        {
            return (await GetAll())?.Where(x => id.Equals(x.AccountsId))
                    .OrderByDescending(x => x.Orden);
        }

        public async Task<IEnumerable<Transactions>?> GetByAccountOrderByOrderDesc(int? id, int startIndex, int nPage)
        {
            return (await GetByAccountOrderByOrderDesc(id))?.Skip(startIndex).Take(nPage);
        }

        public async Task<IEnumerable<Transactions>?> GetByAccount(int? id)
        {
            return (await manager.GetByAccount(id ?? -99));
        }

        public async Task<IEnumerable<Transactions>?> GetByAccount(Accounts? accounts)
        {
            return await GetByAccount(accounts?.Id);
        }

        public async Task<IEnumerable<Transactions>?> GetByPerson(int? id)
        {
            return (await GetAll())?.Where(x => id.Equals(x.PersonsId));
        }

        public async Task<IEnumerable<Transactions>?> GetByPerson(Persons? person)
        {
            return await GetByPerson(person?.Id);
        }

        private async Task<int> GetNextId()
        {
            return await manager.GetNextId();
        }

        private async Task<double> CreateOrden(Transactions transactions)
        {
            int id = (transactions.Id == 0 ? await GetNextId() : transactions.Id);

            return Convert.ToDouble(
                    transactions.Date?.Year.ToString("0000")
                    + transactions.Date?.Month.ToString("00")
                    + transactions.Date?.Day.ToString("00")
                    + id.ToString("000000")
                    + (transactions.AmountIn != 0 ? "1" : "0"));
        }

        public async Task<Transactions?> SaveChanges(Transactions? transactions)
        {
            transactions.AmountIn ??= 0;
            transactions.AmountOut ??= 0;

            await UpdateTranfer(transactions);
            await UpdateTranferFromSplit(transactions);
            transactions = await Save(transactions);
            await iPersonsService.SetCategoryDefault(transactions.PersonsId ?? -1);
            return transactions;
        }

        public async Task RefreshBalanceTransactions(Accounts? acc)
        {
            await manager.UpdateBalance(acc.Id);
        }

        public async Task RefreshBalanceAllTransactions()
        {
            foreach (var acc in await iAccountsService.GetAll())
            {
                await RefreshBalanceTransactions(acc);
            }
        }

        private async Task UpdateTranfer(Transactions transactions)
        {
            if (transactions.TranferId != null &&
                !await iCategoriesService.IsTranfer(transactions.CategoriesId ?? -99))
            {
                var tContraria = await GetById(transactions.TranferId ?? -99);
                if (tContraria != null)
                {
                    await Delete(tContraria);
                }
                transactions.TranferId = null;
            }
            else if (transactions.TranferId == null && await iCategoriesService.IsTranfer(transactions.CategoriesId ?? -99))
            {
                transactions.TranferId = await GetNextId();

                Transactions tContraria = new()
                {
                    Date = transactions.Date,
                    AccountsId = (await iAccountsService.GetByCategoryId(transactions.CategoriesId ?? -99))?.Id,
                    PersonsId = transactions.PersonsId,
                    CategoriesId = (await iAccountsService.GetById(transactions.AccountsId ?? -99))?.Categoryid,
                    Memo = transactions.Memo,
                    TagsId = transactions.TagsId,
                    AmountIn = transactions.AmountOut,
                    AmountOut = transactions.AmountIn
                };

                tContraria.TranferId = transactions.Id != 0 ? transactions.Id : await GetNextId() + 1;

                tContraria.TransactionsStatusId = transactions.TransactionsStatusId;

                await Save(tContraria);
            }
            else if (transactions.TranferId != null &&
                await iCategoriesService.IsTranfer(transactions.CategoriesId ?? -99))
            {
                var tContraria = await GetById(transactions.TranferId ?? -1);
                if (tContraria != null)
                {
                    tContraria.Date = transactions.Date;
                    tContraria.AccountsId = (await iAccountsService.GetByCategoryId(transactions.CategoriesId ?? -99))?.Id;
                    tContraria.PersonsId = transactions.PersonsId;
                    tContraria.CategoriesId = (await iAccountsService.GetById(transactions.AccountsId ?? -99))?.Categoryid;
                    tContraria.Memo = transactions.Memo;
                    tContraria.TagsId = transactions.TagsId;
                    tContraria.AmountIn = transactions.AmountOut;
                    tContraria.AmountOut = transactions.AmountIn;
                    tContraria.TransactionsStatusId = transactions.TransactionsStatusId;
                    await Save(tContraria);
                    await RefreshBalanceAllTransactions();
                }
            }
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

        public async Task<Splits> UpdateTranferSplits(Transactions? transactions, Splits splits)
        {
            if (splits.TranferId != null &&
                !await iCategoriesService.IsTranfer(splits.CategoriesId ?? -99))
            {
                var tContraria = await GetById(splits.TranferId ?? -99);
                if (tContraria != null)
                {
                    await Delete(tContraria);
                }
                splits.TranferId = null;
            }
            else if (splits.TranferId == null &&
                await iCategoriesService.IsTranfer(splits.CategoriesId ?? -99))
            {
                splits.TranferId = await GetNextId();

                Transactions tContraria = new()
                {
                    Date = transactions.Date,
                    AccountsId = (await iAccountsService.GetByCategoryId(splits.CategoriesId ?? -99))?.Id,
                    PersonsId = transactions.PersonsId,
                    CategoriesId = (await iAccountsService.GetById(transactions.AccountsId ?? -99)).Categoryid,
                    Memo = splits.Memo,
                    TagsId = transactions.TagsId,
                    AmountIn = splits.AmountOut,
                    AmountOut = splits.AmountIn,
                    TranferSplitId = splits.Id != 0 ? splits.Id : await GetNextId() + 1,
                    TransactionsStatusId = transactions.TransactionsStatusId
                };

                await Save(tContraria);

            }
            else if (splits.TranferId != null &&
                await iCategoriesService.IsTranfer(splits.CategoriesId ?? -99))
            {
                var tContraria = await GetById(splits.TranferId ?? -99);
                if (tContraria != null)
                {
                    tContraria.Date = transactions.Date;
                    tContraria.AccountsId = (await iAccountsService.GetByCategoryId(splits.CategoriesId ?? -99))?.Id;
                    tContraria.PersonsId = transactions.PersonsId;
                    tContraria.CategoriesId = (await iAccountsService.GetById(transactions.AccountsId ?? -99)).Categoryid;
                    tContraria.Memo = splits.Memo;
                    tContraria.TagsId = transactions.TagsId;
                    tContraria.AmountIn = splits.AmountOut ?? 0;
                    tContraria.AmountOut = splits.AmountIn ?? 0;
                    tContraria.TransactionsStatusId = transactions.TransactionsStatusId;
                    await Save(tContraria);
                }
            }

            return splits;
        }

        #endregion

    }
}
