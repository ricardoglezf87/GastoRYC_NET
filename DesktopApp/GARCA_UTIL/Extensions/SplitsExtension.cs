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
                Transactionid = obj.TransactionsId,
                Transaction = null,
                Categoryid = obj.CategoriesId,
                Category = null,
                AmountOut = obj.AmountOut,
                AmountIn = obj.AmountIn,
                Memo = obj.Memo,
                Tranferid = obj.TranferId,
                Tagid = obj.TagsId,
                Tag = null
            };
        }
    }
}