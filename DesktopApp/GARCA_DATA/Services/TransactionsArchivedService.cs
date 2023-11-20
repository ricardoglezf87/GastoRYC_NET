using GARCA.Data.Managers;
using GARCA.Models;
using GARCA.Utils.Extensions;
using GARCA_DATA.Managers;
using static GARCA.Data.IOC.DependencyConfig;


namespace GARCA.Data.Services
{
    public class TransactionsArchivedService : ServiceBase<TransactionsArchivedManager, TransactionsArchived>
    {

        #region TransactionsArchivedActions      

        private async Task<IEnumerable<TransactionsArchived>?> GetByInvestmentProduct(int id)
        {
            return await manager.GetByInvestmentProduct(id);
        }

        public async Task<IEnumerable<TransactionsArchived>?> GetByPerson(int id)
        {
            return await manager.GetByPerson(id);
        }

        public async Task<IEnumerable<TransactionsArchived>?> GetByPerson(Persons? person)
        {
            return await GetByPerson(person?.Id ?? -99);
        }

        public async Task<IEnumerable<TransactionsArchived>?> GetByInvestmentProduct(InvestmentProducts? investment)
        {
            return await GetByInvestmentProduct(investment.Id);
        }
               
        public async Task<IEnumerable<TransactionsArchived>?> GetByAccount(int id)
        {
            return await manager.GetByAccount(id);
        }

        public async Task<IEnumerable<TransactionsArchived>?> GetByAccount(Accounts? accounts)
        {
            return await GetByAccount(accounts?.Id ?? -99);
        }

        public async Task ArchiveTransactions(DateTime date)
        {
            IEnumerable<Transactions?>? lTrans = (await iTransactionsService.GetAll())?.Where(x => x.Date != null && x.Date <= date);
            if (lTrans != null)
            {

                foreach (var trans in lTrans)
                {
                    if (trans != null)
                    {
                        TransactionsArchived? tArchived;
                        tArchived = await Save(trans.ToArchived());
                        var lSplits = await iSplitsService.GetbyTransactionid(trans.Id);
                        if (lSplits != null)
                        {
                            foreach (var splits in lSplits)
                            {
                                SplitsArchived? sArchived = splits?.ToArchived();
                                sArchived.TransactionsId = tArchived.Id;
                                sArchived.Transactions = tArchived;
                                await iSplitsArchivedService.Save(sArchived);
                                await iSplitsService.Delete(splits);
                            }
                        }
                        await iTransactionsService.Delete(trans);
                    }
                }

                var lAcc = await iAccountsService.GetAllOpened();

                if (lAcc != null)
                {
                    foreach (var acc in lAcc)
                    {
                        Decimal? total = (await iTransactionsArchivedService.GetAll())?
                            .Where(x => x.Date != null && x.Date <= date && x.AccountsId == acc.Id).Sum(x => x.Amount);

                        if (total is not null and not 0)
                        {
                            Transactions? t = new()
                            {
                                AccountsId = acc.Id,
                                Date = date,
                                CategoriesId = (int)CategoriesService.ESpecialCategories.Cierre,
                                AmountIn = (total > 0 ? total : 0),
                                AmountOut = (total < 0 ? -total : 0),
                                Balance = total,
                                TransactionsStatusId = (int)TransactionsStatusService.ETransactionsTypes.Reconciled
                            };

                            t = await iTransactionsService.Save(t);

                            TransactionsArchived? at = t?.ToArchived();
                            if (at != null)
                            {
                                at.AmountIn = (total < 0 ? -total : 0);
                                at.AmountOut = (total > 0 ? total : 0);
                                at.Balance = 0;

                                await iTransactionsArchivedService.Save(at);
                            }
                        }

                    }
                }
            }

        }

        #endregion
    }
}
