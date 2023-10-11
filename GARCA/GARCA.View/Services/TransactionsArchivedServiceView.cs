using GARCA.BO.Models;
using GARCA.BO.Services;
using GARCA.Utils.IOC;
using GARCA.View.Views.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GARCA.View.Services
{
    public class TransactionsArchivedServiceView: TransactionsArchivedService
    {
        
        public async override Task ArchiveTransactions(DateTime date)
        {
            IEnumerable<Transactions?>? lTrans = await Task.Run(() => DependencyConfig.TransactionsService.GetAll()?.Where(x => x.Date != null && x.Date <= date));
            if (lTrans != null)
            {

                LoadDialog loadDialog = new(lTrans.Count());
                loadDialog.Show();

                foreach (var trans in lTrans)
                {
                    if (trans != null)
                    {
                        TransactionsArchived? tArchived;
                        tArchived = await Task.Run(() => Update(trans.ToArchived()));
                        HashSet<Splits?>? lSplits = await Task.Run(() => DependencyConfig.SplitsService.GetbyTransactionid(trans.Id));
                        if (lSplits != null)
                        {
                            loadDialog.setMax(lSplits.Count);

                            foreach (var splits in lSplits)
                            {
                                SplitsArchived sArchived = splits.ToArchived();
                                sArchived.Transactionid = tArchived.Id;
                                sArchived.Transaction = tArchived;
                                await Task.Run(() => DependencyConfig.SplitsArchivedService.Update(sArchived));
                                DependencyConfig.SplitsService.Delete(splits);
                                loadDialog.PerformeStep();
                            }
                        }
                        DependencyConfig.TransactionsService.Delete(trans);
                    }
                    loadDialog.PerformeStep();
                }

                HashSet<Accounts?>? lAcc = await Task.Run(() => DependencyConfig.AccountsService.GetAllOpened());

                if (lAcc != null)
                {
                    loadDialog.setMax(lAcc.Count);
                    foreach (var acc in lAcc)
                    {
                        Decimal? total = await Task.Run(() => DependencyConfig.TransactionsArchivedService.GetAll()?
                            .Where(x => x.Date != null && x.Date <= date && x.Accountid == acc.Id).Sum(x => x.Amount));

                        if (total != null && total != 0)
                        {
                            Transactions? t = new Transactions()
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

                        loadDialog.PerformeStep();
                    }
                }

                loadDialog.Close();
            }

        }
    }
}
