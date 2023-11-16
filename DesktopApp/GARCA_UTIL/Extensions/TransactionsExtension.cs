using GARCA.Models;

namespace GARCA.Utils.Extensions
{
    public static class TransactionsExtension
    {
        public static TransactionsArchived ToArchived(this Transactions obj)
        {
            return new TransactionsArchived
            {
                IdOriginal = obj.Id,
                Date = obj.Date,
                Accountid = obj.Accountid,
                Account = null,
                Personid = obj.Personid,
                Person = null,
                Categoryid = obj.Categoryid,
                Category = null,
                AmountIn = obj.AmountIn,
                AmountOut = obj.AmountOut,
                Memo = obj.Memo,
                InvestmentCategory = obj.InvestmentCategory,
                InvestmentProducts = null,
                InvestmentProductsid = obj.InvestmentProductsid,
                Tranferid = obj.Tranferid,
                TranferSplitid = obj.TranferSplitid,
                TransactionStatus = null,
                TransactionStatusid = obj.TransactionStatusid,
                NumShares = obj.NumShares,
                PricesShares = obj.PricesShares,
                Tagid = obj.Tagid,
                Tag = null,
                Balance = obj.Balance,
                Orden = obj.Orden
            };
        }
    }
}