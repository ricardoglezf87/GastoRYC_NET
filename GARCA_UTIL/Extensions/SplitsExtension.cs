using GARCA.Models;

namespace GARCA.Utils.Extensions
{
    public static class SplitsExtension
    {
        public static SplitsArchived ToArchived(this Splits obj)
        {
            return new SplitsArchived
            {
                IdOriginal = obj.Id,
                TransactionsId = obj.TransactionsId,
                Transactions = null,
                CategoriesId = obj.CategoriesId,
                Categories = null,
                AmountOut = obj.AmountOut,
                AmountIn = obj.AmountIn,
                Memo = obj.Memo,
                TranferId = obj.TranferId,
                TagsId = obj.TagsId,
                Tags = null
            };
        }
    }
}