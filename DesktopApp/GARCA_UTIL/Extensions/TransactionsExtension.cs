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
                Accountid = obj.AccountsId,
                Account = null,
                Personid = obj.PersonsId,
                Person = null,
                Categoryid = obj.CategoriesId,
                Category = null,
                AmountIn = obj.AmountIn,
                AmountOut = obj.AmountOut,
                Memo = obj.Memo,
                InvestmentCategory = obj.InvestmentCategory,
                InvestmentProducts = null,
                InvestmentProductsid = obj.InvestmentProductsId,
                Tranferid = obj.TranferId,
                TranferSplitid = obj.TranferSplitId,
                TransactionStatus = null,
                TransactionStatusid = obj.TransactionsStatusId,
                NumShares = obj.NumShares,
                PricesShares = obj.PricesShares,
                Tagid = obj.TagsId,
                Tag = null,
                Balance = obj.Balance,
                Orden = obj.Orden
            };
        }
    }
}