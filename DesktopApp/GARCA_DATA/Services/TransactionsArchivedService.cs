using GARCA.Data.Managers;
using GARCA.Models;
using GARCA.Utils.Extensions;
using GARCA_DATA.Managers;
using static GARCA.Data.IOC.DependencyConfig;


namespace GARCA.Data.Services
{
    public class TransactionsArchivedService : ServiceBase<TransactionsArchivedManager, TransactionsArchived, Int32>
    {

        #region TransactionsArchivedActions      

        private HashSet<TransactionsArchived>? GetByInvestmentProduct(int? id)
        {
            return manager.GetByInvestmentProduct(id)?.ToHashSet();
        }

        public HashSet<TransactionsArchived>? GetByPerson(int? id)
        {
            return manager.GetByPerson(id)?.ToHashSet();
        }

        public HashSet<TransactionsArchived>? GetByPerson(Persons? person)
        {
            return GetByPerson(person?.Id);
        }

        public HashSet<TransactionsArchived>? GetByInvestmentProduct(InvestmentProducts? investment)
        {
            return GetByInvestmentProduct(investment.Id);
        }

        public TransactionsArchived? Update(TransactionsArchived? transactions)
        {
            return manager.Update(transactions);
        }
       
        public HashSet<TransactionsArchived>? GetByAccount(int? id)
        {
            return manager.GetByAccount(id)?.ToHashSet();
        }

        public HashSet<TransactionsArchived>? GetByAccount(Accounts? accounts)
        {
            return GetByAccount(accounts?.Id);
        }

        public virtual async Task ArchiveTransactions(DateTime date)
        {
            IEnumerable<Transactions?>? lTrans = await Task.Run(() => iTransactionsService.GetAll()?.Where(x => x.Date != null && x.Date <= date));
            if (lTrans != null)
            {

                foreach (var trans in lTrans)
                {
                    if (trans != null)
                    {
                        TransactionsArchived? tArchived;
                        tArchived = await Task.Run(() => Update(trans.ToArchived()));
                        HashSet<Splits>? lSplits = await Task.Run(() => iSplitsService.GetbyTransactionid(trans.Id));
                        if (lSplits != null)
                        {
                            foreach (var splits in lSplits)
                            {
                                SplitsArchived? sArchived = splits?.ToArchived();
                                sArchived.Transactionid = tArchived.Id;
                                sArchived.Transaction = tArchived;
                                await Task.Run(() => iSplitsArchivedService.Update(sArchived));
                                iSplitsService.Delete(splits);
                            }
                        }
                        iTransactionsService.Delete(trans);
                    }
                }

                HashSet<Accounts>? lAcc = await Task.Run(() => iAccountsService.GetAllOpened());

                if (lAcc != null)
                {
                    foreach (var acc in lAcc)
                    {
                        Decimal? total = await Task.Run(() => iTransactionsArchivedService.GetAll()?
                            .Where(x => x.Date != null && x.Date <= date && x.Accountid == acc.Id).Sum(x => x.Amount));

                        if (total is not null and not 0)
                        {
                            Transactions? t = new()
                            {
                                Accountid = acc.Id,
                                Date = date,
                                Categoryid = (int)CategoriesService.ESpecialCategories.Cierre,
                                AmountIn = (total > 0 ? total : 0),
                                AmountOut = (total < 0 ? -total : 0),
                                Balance = total,
                                TransactionStatusid = (int)TransactionsStatusService.ETransactionsTypes.Reconciled
                            };

                            t = iTransactionsService.Update(t);

                            TransactionsArchived? at = t?.ToArchived();
                            if (at != null)
                            {
                                at.AmountIn = (total < 0 ? -total : 0);
                                at.AmountOut = (total > 0 ? total : 0);
                                at.Balance = 0;

                                iTransactionsArchivedService.Update(at);
                            }
                        }

                    }
                }
            }

        }

        #endregion
    }
}
