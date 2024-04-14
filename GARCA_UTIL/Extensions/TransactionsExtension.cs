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
                AccountsId = obj.AccountsId,
                Accounts = null,
                PersonsId = obj.PersonsId,
                Persons = null,
                CategoriesId = obj.CategoriesId,
                Categories = null,
                AmountIn = obj.AmountIn,
                AmountOut = obj.AmountOut,
                Memo = obj.Memo,
                InvestmentCategory = obj.InvestmentCategory,
                InvestmentProducts = null,
                InvestmentProductsId = obj.InvestmentProductsId,
                TranferId = obj.TranferId,
                TranferSplitId = obj.TranferSplitId,
                TransactionsStatus = null,
                TransactionsStatusId = obj.TransactionsStatusId,
                NumShares = obj.NumShares,
                PricesShares = obj.PricesShares,
                TagsId = obj.TagsId,
                Tags = null,
                Balance = obj.Balance,
                Orden = obj.Orden
            };
        }
    }
}