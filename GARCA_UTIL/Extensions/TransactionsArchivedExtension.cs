using GARCA.Models;

namespace GARCA.Utils.Extensions
{
    public static class TransactionsArchivedExtension
    {
        public static void AddRange(this List<TransactionsArchived>? source, List<Transactions>? elements)
        {
            if (elements != null)
            {
                foreach (var obj in elements)
                {
                    if (obj != null)
                    {
                        source.Add(obj.ToArchived());
                    }
                }
            }
        }
    }
}
