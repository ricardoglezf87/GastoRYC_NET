using GARCA.Models;
using GARCA.Data.Managers;
using GARCA.Data.IOC;
using GARCA.Utils.Extensions;


namespace GARCA.Data.Services
{
    public class TransactionsArchivedService
    {
        #region Propiedades y Contructor

        private readonly TransactionsArchivedManager transactionsManager;

        public TransactionsArchivedService()
        {
            transactionsManager = new TransactionsArchivedManager();
        }

        #endregion Propiedades y Contructor

        #region TransactionsArchivedActions

        public HashSet<TransactionsArchived?>? GetAll()
        {
            return transactionsManager.GetAll()?.ToHashSet();
        }

        public TransactionsArchived? GetById(int? id)
        {
            return (TransactionsArchived)transactionsManager.GetById(id);
        }

        private HashSet<TransactionsArchived?>? GetByInvestmentProduct(int? id)
        {
            return transactionsManager.GetByInvestmentProduct(id)?.ToHashSet();
        }

        public HashSet<TransactionsArchived?>? GetByPerson(int? id)
        {
            return transactionsManager.GetByPerson(id)?.ToHashSet();
        }

        public HashSet<TransactionsArchived?>? GetByPerson(Persons? person)
        {
            return GetByPerson(person?.Id);
        }

        public HashSet<TransactionsArchived?>? GetByInvestmentProduct(InvestmentProducts? investment)
        {
            return GetByInvestmentProduct(investment.Id);
        }

        public TransactionsArchived? Update(TransactionsArchived transactions)
        {
            return (TransactionsArchived)transactionsManager.Update(transactions);
        }

        public void Delete(TransactionsArchived? transactions)
        {
            transactionsManager.Delete(transactions);
        }

        public HashSet<TransactionsArchived?>? GetByAccount(int? id)
        {
            return transactionsManager.GetByAccount(id)?.ToHashSet();
        }

        private IEnumerable<TransactionsArchived?>? GetByAccountOrderByOrderDesc(int? id)
        {
            return transactionsManager.GetByAccountOrderByOrdenDesc(id);
        }

        public HashSet<TransactionsArchived?>? GetByAccount(Accounts? accounts)
        {
            return GetByAccount(accounts?.Id);
        }

        public virtual async Task ArchiveTransactions(DateTime date)
        {
            IEnumerable<Transactions?>? lTrans = await Task.Run(() => DependencyConfig.TransactionsService.GetAll()?.Where(x => x.Date != null && x.Date <= date));
            if (lTrans != null)
            {

                foreach (var trans in lTrans)
                {
                    if (trans != null)
                    {
                        TransactionsArchived? tArchived;
                        tArchived = await Task.Run(() => Update(trans.ToArchived()));
                        HashSet<Splits?>? lSplits = await Task.Run(() => DependencyConfig.SplitsService.GetbyTransactionid(trans.Id));
                        if (lSplits != null)
                        {
                            foreach (var splits in lSplits)
                            {
                                SplitsArchived sArchived = splits.ToArchived();
                                sArchived.Transactionid = tArchived.Id;
                                sArchived.Transaction = tArchived;
                                await Task.Run(() => DependencyConfig.SplitsArchivedService.Update(sArchived));
                                DependencyConfig.SplitsService.Delete(splits);
                            }
                        }
                        DependencyConfig.TransactionsService.Delete(trans);
                    }
                }

                HashSet<Accounts?>? lAcc = await Task.Run(() => DependencyConfig.AccountsService.GetAllOpened());

                if (lAcc != null)
                {
                    foreach (var acc in lAcc)
                    {
                        Decimal? total = await Task.Run(() => DependencyConfig.TransactionsArchivedService.GetAll()?
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

                            t = DependencyConfig.TransactionsService.Update(t);

                            TransactionsArchived? at = t.ToArchived();
                            at.AmountIn = (total < 0 ? -total : 0);
                            at.AmountOut = (total > 0 ? total : 0);
                            at.Balance = 0;

                            DependencyConfig.TransactionsArchivedService.Update(at);

                        }

                    }
                }
            }

        }

        #endregion
    }
}
